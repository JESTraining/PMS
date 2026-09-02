# UX Review

## Overall assessment

The app has a recognizable clinical admin shell and a focused login screen, but the interaction model is currently inconsistent with a production healthcare workflow. The largest UX risks are not visual polish: dead controls, unrestricted module visibility, weak feedback, and unclear navigation states can cause users to make wrong assumptions about what happened.

## Findings

| Priority | Area | Observation | Recommendation |
|---|---|---|---|
| High | Login | Forgot Password, Sign up, and Google/Facebook/Apple buttons call `noopClick`; role presets fill usernames that may not be real accounts | Remove unsupported controls or provide honest disabled states; implement reset/invite flow before advertising it |
| High | Navigation | `menu-config.ts` contains `#` routes, while the layout shows all clinical, billing, reporting, and user modules | Replace placeholders with real routes and role-aware navigation derived from claims |
| High | Feedback | Login uses `alert()` and logs the raw response; service errors are not presented as inline, accessible form errors | Use a consistent alert/toast region, preserve focus, and show actionable field/form messages |
| Medium | Session | Expiry/401 behavior is not visible to users because the guard only checks token presence | Redirect with a clear session-expired message and return the user to the intended route after login |
| Medium | Responsive layout | Login styling has a two-column intent but the reviewed mobile rules and dense clinical screens need a device pass | Test 320px, 768px, and desktop widths; ensure tables have readable overflow and controls do not collide |
| Medium | Accessibility | Password toggle has an accessible label, but the login form lacks explicit invalid/error/status patterns and icon-only social buttons are dead ends | Add `aria-invalid`, linked error text, live status messaging, keyboard focus order, and remove unavailable actions |
| Medium | Information architecture | Sidebar presents a long flat list mixing daily work, administration, and historical reports | Group by workflow: Today, Patients, Clinical, Billing, Reports; preserve active context and breadcrumbs where needed |
| Low | Visual system | Login uses a Roboto/system fallback and the overall shell has a polished visual direction, but tokens should be shared across pages | Define shared typography, spacing, color, focus, loading, empty, and error states rather than styling page-by-page |

## Suggested acceptance pass

1. A receptionist can sign in, see only receptionist work, search a patient, and understand loading, empty, success, and error states.
2. A doctor cannot see billing/user administration and can reach the patient's encounter history in two predictable steps.
3. A billing user can distinguish pending, overdue, paid, and failed payment states without relying on color alone.
4. Every interactive element works with keyboard navigation and has visible focus.
5. Mobile users can complete login, patient search, and the primary update workflow without horizontal page scrolling.

## Design direction

Preserve the existing calm clinical palette and sidebar shell, but make the product more operational: stronger status hierarchy, fewer decorative controls, clearer role context, and dense but breathable data views. UX changes should follow the authorization model so the interface does not promise access the API will deny.

The actionable backlog for this reset is in [ux-github-issues-import.csv](ux-github-issues-import.csv). Start with `UX-01` to agree on the reduced MVP surface, then execute the navigation and login simplification before polishing individual clinical workflows.
