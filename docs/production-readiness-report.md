# Production Readiness Review

Date: 2026-09-02
Scope: `backend/` and `frontend/pms/`, excluding generated `bin/` and `obj/` output.

## Decision

**No-go for production**, especially for real patient or billing data. The current build has release-blocking access-control, credential, transport, and configuration risks. The proposed two-day backlog in `github-issues-import.csv` is the minimum remediation gate, not a complete compliance program.

## Findings

| Severity | Finding | Evidence | Impact |
|---|---|---|---|
| Critical | Protected PHI, billing, and analytics routes are anonymous | `backend/WebServices/Controllers/Patients/PatientsController.cs`, `InvoicesController.cs`, `DashboardController.cs` have commented `[Authorize]` | Unauthenticated disclosure and modification of sensitive data |
| Critical | Frontend route guard is bypassed | `frontend/pms/src/app/guards/auth.guard.ts` returns `true` | Any user can open protected UI routes; this does not replace server authorization |
| High | User entities can serialize `Password` | `backend/WebServices/Controllers/Users/UsersController.cs` returns `User`; `Domain/Entities/User.cs` contains `Password` | Password hashes/credentials may be disclosed |
| High | Passwords use unsalted SHA-256 | `backend/WebServices/Repositories/UsersRepository.cs` | Offline cracking risk; unsuitable password storage |
| High | Secrets and development infrastructure are committed | `backend/WebServices/appsettings.json` | Key compromise, accidental production misconfiguration, unsafe database TLS |
| High | HTTP and hard-coded localhost API URLs | `backend/WebServices/Program.cs`, `frontend/pms/src/app/services/auth/auth.service.ts`, `patient.service.ts`, `environment.prod.ts` | Credential/session exposure and broken deployment |
| High | JWT is stored in localStorage without expiry validation | `auth.service.ts` | XSS can exfiltrate bearer tokens; expired sessions remain locally accepted |
| High | Invitation state is process-local | `PatientRegistrationController.cs` uses `IMemoryCache` | Restart/scale-out breaks links; replay and operational behavior are fragile |
| High | Exception details are returned | dashboard/controller/repository catch blocks | Sensitive implementation details can leak to clients |
| Medium | Blocking `.Result` in async controller code | `UsersController.cs` | Thread starvation and degraded reliability |
| Medium | N+1 invoice detail queries and unbounded lists | `InvoicesController.cs`, patient endpoints | Latency and resource exhaustion as data grows |
| Medium | Misleading or dead UX controls | `login.component.html` calls `noopClick`; `menu-config.ts` uses `#` | Users cannot complete advertised actions and navigation is inconsistent |

## Verification

- `dotnet test backend/PMS.slnx --no-restore`: **failed**. Four authentication tests fail during fixture setup because `User.Role` is required but omitted in `backend/Tests/Authentication/AuthenticationTests.cs`.
- Angular test/build attempt from `frontend/pms`: **not executed successfully** because dependencies are not installed (`ng: command not found`). Running from the repository root also fails because the root has no `package.json`.
- No source files were changed during the review.

## Release gate

Before deployment, require: passing backend and frontend CI; endpoint-level 401/403 and IDOR tests; DTO serialization tests; secret rotation; approved HTTPS/CORS configuration; database migration/back-up rehearsal; durable invitation tests; audit logging for PHI access; and a security review of cookie/token strategy. HIPAA or other regulatory compliance cannot be inferred from code inspection alone and needs operational/legal controls too.

## Positive signals

The repository has clear project separation (`Domain`, `WebServices`, `Tests`, Angular app), EF migrations, DTOs in some patient/invoice paths, JWT validation middleware, and a meaningful set of domain/component tests. These are useful foundations once the security defaults and deployment process are corrected.
