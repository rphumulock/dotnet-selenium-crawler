# HAI-Selenium — Claims Portal Crawler

## Why this project exists

Someone I know was spending hours a month hand-entering the same claims into a
behavioral-health payer portal: open the form, type six service dates, submit,
repeat, and start over from the top whenever the session died halfway through.
That chore is the reason this exists.

It was also my excuse to learn .NET properly. I wanted hands-on time with:

- **C# / .NET 8** — generic host, DI, `async` all the way down, primary constructors.
- **EF Core + Postgres** — migrations, `DbContext`, and using a table as a
  *checkpoint* rather than as a place to file finished results.
- **The GoF patterns everyone name-drops** — Strategy, Template Method, Factory,
  and a chain-of-steps pipeline — applied to something with real failure modes
  instead of a textbook exercise.

Browser automation is a good forcing function for the last one. A crawler against
a JavaScript-heavy portal fails constantly and in ways you don't control: a stale
element, a modal that opens a beat late, a session that expires mid-batch. Most of
the design here is about **what happens after a step fails**, not about the happy
path.

> [!NOTE]
> This is an archived snapshot from 2024, kept as a project to read rather than
> one to run. The target portal, its selectors, and its credentials aren't
> public, so nobody else can point this at anything. The interesting part is the
> resume mechanism in [Failure and Resume](#failure-and-resume).

## Stack

| Layer | Tool |
| --- | --- |
| Runtime | [.NET 8](https://dotnet.microsoft.com/) console app |
| Browser | [Selenium WebDriver](https://www.selenium.dev/) 4 + Chrome, driver pinned by [WebDriverManager](https://github.com/rosolko/WebDriverManager.Net) |
| Persistence | [EF Core 8](https://learn.microsoft.com/ef/core/) + [Npgsql](https://www.npgsql.org/) → PostgreSQL |
| Mutual exclusion | [DistributedLock.Postgres](https://github.com/madelson/DistributedLock) advisory lock |
| Logging | [Serilog](https://serilog.net/) → console + daily rolling file |
| Config | [dotenv.net](https://github.com/bolorundurowb/dotenv.net) |

## How it works

One run does one job. `ACTION` picks which:

```shell
ACTION=Create dotnet run    # file a month of claims
ACTION=Status dotnet run    # scrape the status of claims already filed
```

`Program.cs` loads `.env`, takes a Postgres advisory lock so two runs can't drive
the same portal session at once, starts Chrome, hands the driver to a workflow,
and quits the browser in a `finally` no matter how the run ends.

`WorkflowFactory` maps the `ACTION` string to an `IWorkflowStrategy` and supplies
its input — for `Create`, the `IInvoiceRequestService` plus a JSON request; for
`Status`, just the JSON.

### The pipeline

Four pieces, each doing one thing:

| Piece | Role |
| --- | --- |
| [`IWorkflowStrategy`](./Workflow/Interfaces/IWorkflowStrategy.cs) | **Strategy** — one implementation per `ACTION` |
| [`InvoiceWorkflowTemplate`](./Workflow/Classes/InvoiceWorkflowTemplate.cs) | **Template Method** — fixes the order `InitializeData` → `InitializeDataAsync` → `ProcessDataAsync`, subclasses fill in the steps |
| [`WorkflowChain`](./Workflow/Classes/WorkflowChain.cs) | An ordered list of `IWorkflowStep`s, executed in sequence, logging each by type name |
| [`WorkflowContext`](./Workflow/Classes/WorkflowContext.cs) | A string-keyed blackboard the steps read and write, so no step needs a reference to any other |

A step is a small class over one portal interaction —
[`LoginAction`](./Workflow/Steps/Shared/LoginAction.cs),
`FindPatientAction`, `AddClaimAction`, `ProcessClaimFormHeaderAction`. Chains are
assembled at the call site, so the shape of a run is readable in one place:

```csharp
var compileFormDataChain = new WorkflowChain()
    .AddStep(new NavigateToSiteAction(_context))
    .AddStep(new LoginAction(_context))
    .AddStep(new NavigateToMembershipSearchAction(_context))
    .AddStep(new FindPatientAction(_context))
    .AddStep(new SelectPatientAction(_context))
    ...
```

### Batching

The portal's claim form holds **six service-date rows**. So
[`SetServiceDatesFormData`](./Workflow/Steps/CreateRequest/SetServiceDatesFormData.cs)
splits a month of dates into groups of six, and the Create workflow runs the
form-filling chain once per batch — one claim per batch, which is how the portal
wants it anyway.

Before that, [`ValidateCreateRequestAction`](./Workflow/Steps/CreateRequest/ValidateCreateRequestAction.cs)
rejects the run if the service dates straddle a month boundary or include today,
because the portal silently misfiles both.

## Failure and Resume

This is the part worth reading.

**Every step retries itself.** [`WorkflowStepBase`](./Workflow/Classes/WorkflowStepBase.cs)
wraps `PerformStepAsync` in three attempts with exponential backoff (2s, 4s, 8s),
which absorbs the ordinary Selenium flake — a modal still animating, an element
that went stale between the wait and the click.

**A step that exhausts its retries carries the pipeline out with it.** Instead of
rethrowing the `NoSuchElementException`, it wraps it in an `HAIException` that
holds the whole `WorkflowContext`:

```csharp
throw new HAIException(ex.Message, Context, ex);
```

That means the top-level handler doesn't just get "a click failed" — it gets the
run's entire state, including which batch was in flight and which batches hadn't
been touched yet.

**The database is a checkpoint, not a results table.**
[`ErrorHandlerUtils`](./Utilities/ErrorHandlerUtils.cs) pulls
`CurrentBatchServiceDateRequests` and `RemainingBatchesServiceDateRequests` out of
that context, and writes exactly the *unfinished* work to `ServiceDateRequests`,
keyed by invoice id — deleting any rows from a previous run that have since been
completed. On the next run,
[`SetupInvoiceData`](./Workflow/Steps/CreateRequest/SetupInvoiceData.cs) looks for
rows under that invoice id: if it finds them it processes only those, and if it
finds none it starts fresh from the JSON. When a run finishes cleanly, the
workflow deletes the invoice's rows — an empty table means nothing is outstanding.

So the table's contents are the answer to "what's left to do", and the crawler is
restartable by construction: run it again and it picks up where the browser died.

Errors are also triaged before being recorded —
network/timeout and most Selenium faults are classified `Recoverable`, while
`ElementClickInterceptedException` and `UnhandledAlertException` (an unexpected
modal, a portal-side error dialog) are `NonRecoverable`, since retrying those just
clicks the wrong thing repeatedly.

`InvoiceRequestWorkflow` keeps an unused `IntroduceError` helper that plants a
malformed date in the first row of a batch — fault injection for testing that the
resume path actually resumes.

## Layout

```
Program.cs                  Host setup, env, advisory lock, driver lifecycle
Workflow/
  Interfaces/               IWorkflowStrategy, IWorkflowStep
  Classes/                  Template, chain, context, factory, retrying step base
  Workflows/                InvoiceRequestWorkflow (Create), InvoiceStatusWorkflow (Status)
  Steps/
    Shared/                 Navigate, login, menu traversal
    CreateRequest/          Patient lookup, form fill, batching
    StatusRequest/          Claim lookup, header + line-item scraping
InternalClasses/            Request/response shapes and the payment lookup table
Database/                   DbContext, ServiceDateRequest model, advisory-lock manager
Services/                   InvoiceRequestService — the checkpoint read/write API
Utilities/                  Driver setup, env, JSON loading, error triage, exceptions
  mockData/                 JSON inputs (see Inputs)
Migrations/                 EF Core migrations
```

## Inputs

There's no API in front of this — a run reads JSON from
[`Utilities/mockData/`](./Utilities/mockData):

| File | Feeds | Holds |
| --- | --- | --- |
| `InvoiceCreateClaimsRequest.json` | `Create` | Patient, policy number, diagnosis codes, and the month's service dates |
| `InvoiceStatusRequest.json` | `Status` | Invoice id and the claim ids to look up |
| `PaymentBreakdown.json` | `Create` | Visit-count → dollar-amount table, split by `Intensive` / `General` treatment |

All three are copied to the output directory by the `.csproj`. The service-date
form values themselves (place of service `15`, CPT `H2016`, `$1.00`) are hardcoded
placeholders in `SetServiceDatesFormData` — real values were never wired in,
which fits the dry-run posture below.

## Configuration

Everything comes from the environment; `dotenv.net` probes upward for a `.env`.
There is no `.env.example` in the repo — the variables are:

| Variable | Purpose |
| --- | --- |
| `ACTION` | `Create` or `Status` — selects the workflow |
| `URL` | Portal login page |
| `USERNAME` / `PASSWORD` | Portal credentials (`USERNAME` is also logged as the run's user) |
| `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD` | Postgres connection |
| `CHROME_USER_DATA_DIR` | Chrome profile root to reuse |
| `CHROME_PROFILE_DIR` | Profile within it |

Chrome is launched against a **real user profile** rather than a throwaway one, so
the portal sees a browser it has already met — MFA and device trust survive
between runs. It also means the profile must not be open in another Chrome window
when the crawler starts.

## Running

```shell
git clone https://github.com/rphumulock/dotnet-selenium-crawler.git
cd dotnet-selenium-crawler

dotnet restore
dotnet ef database update     # requires dotnet-ef
dotnet run
```

Logs go to the console and to `logs/myapp.txt`, rolled daily.

## Known rough edges

Kept as-is; this is where the snapshot was left.

- **The Create workflow cancels instead of submitting.** Every batch chain ends in
  [`CancelClaimAction`](./Workflow/Steps/CreateRequest/CancelClaimAction.cs) —
  it fills the entire claim form and then clicks Cancel. That was deliberate for
  testing against the live portal, and it was never flipped to submit.
- **The advisory lock key isn't stable.** `DbLockManager` derives it from
  `"HAI_Selenium_DistributedLock".GetHashCode()`, and .NET randomizes string hash
  codes per process — so two concurrent runs compute different keys and don't
  actually exclude each other. It needs a fixed constant.
- **`Program.cs` never exits.** It runs the workflow, then builds a *second* host
  and awaits `host.RunAsync()`, so the process hangs after the work is done.
- **Checkpoint rows are matched by service date, not id.** The mock JSON carries no
  ids, so every restored row is an insert; reconciliation compares parsed
  `ServiceDate` values. Two entries on the same date would collide.
- **Batch resume is coarser than it should be.** A mid-run failure writes back the
  current batch plus everything after it. Work inside the failed batch that had
  already landed in the portal isn't tracked, so a resume can re-enter it —
  flagged in the commit history and never resolved.
- **`IWebElement` handles live in the context.** `CaptureButtonsAction` stashes the
  Add and Cancel buttons; if the page re-renders between capture and click the
  reference goes stale, and `WorkflowStepBase`'s retry re-runs only the failing
  step, not the capture.
- **`Status` output goes nowhere.** `InvoiceStatusWorkflow` scrapes claim headers
  and line items into `ClaimsStatusWithLineItems` and then `Console.WriteLine`s
  them. Persisting that side was next on the list.
- **NRules came and went.** A rules-engine pass over the validation logic was tried
  and reverted (`8520eaa` → `28286d9`); the dependency is still in the `.csproj`
  with nothing using it.
