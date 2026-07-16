# Research Report Generator — UI/UX Design Specification

**A complete design reference: philosophy, system, pages, flows, states, and content.**

> This document translates the Clean Architecture domain model (`ReportRequest`, `GeneratedReport`, `ReportGenerationRun`, quality scoring, AI provider abstraction, multi-format export) into a concrete, opinionated design system and page-by-page spec. Every UI decision below is traceable to a real business rule in the codebase — this is not generic dashboard filler.

---

## Table of Contents

1. [Product Context](#1-product-context)
2. [Design Philosophy & Principles](#2-design-philosophy--principles)
3. [Users & Key Moments](#3-users--key-moments)
4. [Information Architecture](#4-information-architecture)
5. [Design System](#5-design-system)
6. [App Shell & Navigation](#6-app-shell--navigation)
7. [Component Library](#7-component-library)
8. [Page-by-Page Specifications](#8-page-by-page-specifications)
9. [Core User Flows (UI Walkthroughs)](#9-core-user-flows-ui-walkthroughs)
10. [States: Empty / Loading / Error / Success](#10-states-empty--loading--error--success)
11. [AI-Specific UX Patterns](#11-ai-specific-ux-patterns)
12. [Content & Microcopy Guidelines](#12-content--microcopy-guidelines)
13. [Responsive Behavior](#13-responsive-behavior)
14. [Accessibility](#14-accessibility)
15. [Design Tokens Reference](#15-design-tokens-reference)
16. [Future Enhancements](#16-future-enhancements)

---

## 1. Product Context

The app lets an authenticated user turn a structured request into an AI-generated **research report** (one topic, explained deeply) or **comparison report** (multiple topics, scored against criteria and resolved into scenario-based recommendations). Every report is scored for structural completeness (`ReportQualityScore`, 0–100, `ReadyThreshold = 70`), tracked through one or more `ReportGenerationRun` attempts, and exportable as Markdown, HTML, PDF, or DOCX.

The UI's central job is to make three things feel effortless, in this order:

1. **Asking well** — the wizard should make it easy to give the AI enough structured signal to produce a genuinely good report, without feeling like a form marathon.
2. **Trusting the output** — quality score, warnings, citations, and generation metadata must make the report's reliability legible at a glance, not just its content.
3. **Getting the file** — export should never feel like an afterthought bolted onto a chat transcript. It should feel like publishing a document.

---

## 2. Design Philosophy & Principles

**"Feels like a document tool that happens to use AI, not an AI tool that happens to make documents."**
The generated content is the product. Chrome should recede; the report itself — in Preview, in exports — should look like something a person would be proud to hand to a stakeholder.

Five working principles:

| Principle | What it means in practice |
|---|---|
| **Progressive disclosure** | The wizard reveals only what's relevant to the chosen `ReportMode`. Optional constraints stay collapsed until asked for. Technical generation metadata (tokens, prompt version, raw run history) is always one click away, never in the primary sightline. |
| **Honest about uncertainty** | Quality score is framed as *"how structurally complete this report is,"* never *"how correct."* Blocking warnings are shown, not hidden behind a passing score. AI failures get plain-language explanations, not stack traces. |
| **Reversible, low-anxiety actions** | Soft delete is explained as such. Regeneration never destroys the current version silently — the version number always increments and is visible. Nothing commits without a confirm step that shows what will change. |
| **Consistent status language across the whole app** | The same badge shapes and colors for `ReportStatus`, `GenerationStatus`, `QualityWarningSeverity`, and AI provider health are reused everywhere — dashboard, history, details, preview — so a user learns the vocabulary once. |
| **Respect the reader, not just the user** | The exported/previewed report is designed for the *audience* the user specified (`TargetAudience`), not for the app's own aesthetic. Preview typography deliberately shifts from "app UI" to "document" register. |

---

## 3. Users & Key Moments

Single user type in v1 (no admin/role split) — but two behavioral modes:

- **The Explorer** — one topic, wants clarity on something unfamiliar. Cares about the wizard being fast and about the "Why It Matters" / "Key Concepts" sections landing well.
- **The Decider** — comparing 2–8 options for a real decision. Cares about the comparison table, decision matrix, weighted criteria, and scenario-based recommendations being *actionable*, not generic.

Key emotional moments to design for deliberately:

- **The blank wizard** (first screen after "New Report") — must not feel like a job application. Warm, example-driven copy.
- **The wait** (AI generation) — 5–30 seconds of uncertainty. Needs to feel active, not stalled.
- **The reveal** (Preview loads) — the payoff moment. Quality score and warnings should feel like a helpful second opinion, not a grade.
- **The failure** (AI error) — must never feel like data was lost or that the user did something wrong.

---

## 4. Information Architecture

```
/                          Landing (unauthenticated)
/privacy                   Privacy policy
/Identity/Account/Login
/Identity/Account/Register
/Identity/Account/Logout
/Identity/Account/AccessDenied
/Identity/Account/Manage               Profile & password

/dashboard                 Home after login

/reports                   History (search + filter)
/reports/create             Guided wizard (multi-step)
/reports/{id}                Details (request + generation history)
/reports/{id}/preview         Rendered report + quality + export
/reports/{id}/regenerate       Single-page regeneration form
/reports/{id}/delete            Confirmation

/exports/{id}/{format}       Triggers file download
/exports/download-error       Friendly failure page

/ai-providers               Provider health (surfaced as a dashboard widget,
                              not necessarily its own nav item)
```

**Primary navigation (persistent):** Dashboard · New Report · History
**Secondary/utility (user menu):** Profile · Manage Account · Sign out

Breadcrumbs appear on every page below Dashboard: `Dashboard / History / “SignalR vs gRPC” / Preview`.

---

## 5. Design System

### 5.1 Color System

Two-tier palette: a calm, trustworthy **UI palette** (indigo + slate) and a **semantic palette** that is reused identically for every status concept in the domain (report status, generation status, warning severity, provider health). This 1:1 reuse is the single most important consistency decision in the system.

**Core**

| Token | Hex | Usage |
|---|---|---|
| `--color-primary-600` | `#4F46E5` | Primary buttons, links, active nav, focus rings |
| `--color-primary-700` | `#4338CA` | Primary button hover/pressed |
| `--color-primary-50`  | `#EEF2FF` | Primary-tinted backgrounds (selected wizard cards) |
| `--color-accent-500`  | `#0EA5E9` | AI/generation-specific accents (loading states, "AI" tags) |
| `--color-ink-900`     | `#0F172A` | Primary text |
| `--color-ink-600`     | `#475569` | Secondary text |
| `--color-ink-400`     | `#94A3B8` | Placeholder / disabled text |
| `--color-surface-0`   | `#FFFFFF` | Card / page surface |
| `--color-surface-50`  | `#F8FAFC` | App background |
| `--color-surface-100` | `#F1F5F9` | Recessed panels, code blocks |
| `--color-border`      | `#E2E8F0` | Default border |

**Semantic (shared everywhere)**

| Meaning | Color | Hex | Used for |
|---|---|---|---|
| Success / Ready / Healthy | Green | `#16A34A` (bg tint `#F0FDF4`) | `ReportStatus.Ready`, `GenerationStatus.Succeeded`, provider healthy, `RecommendationStrength.StronglyRecommended` |
| Caution / Warning | Amber | `#D97706` (bg tint `#FFFBEB`) | `QualityWarningSeverity.Warning`, `ReportStatus.Draft`, provider degraded |
| Critical / Blocking / Failed | Red | `#DC2626` (bg tint `#FEF2F2`) | `QualityWarningSeverity.Blocking`, `GenerationStatus.Failed`, provider down |
| Neutral / Info / Pending | Blue-gray | `#2563EB` (bg tint `#EFF6FF`) | `QualityWarningSeverity.Info`, `GenerationStatus.Pending`/`InProgress`, provider unknown |

> **Rule:** color is never the only signal. Every semantic use pairs a color with an icon shape (check / triangle / octagon / dot) and a text label, for accessibility and for users with color vision deficiency.

### 5.2 Typography

Two type families, deliberately different registers:

- **UI / Chrome:** `Inter` (400/500/600/700) — all navigation, forms, buttons, tables, badges.
- **Report Content:** `Source Serif 4` (or `Georgia` fallback) for the rendered Markdown body in Preview and in HTML/PDF/DOCX exports — signals "this is a document," not "this is an app screen."
- **Technical/Monospace:** `JetBrains Mono` — model names, prompt version tokens, token counts, run IDs, code snippets inside generated reports.

| Style | Family | Size / Line-height | Weight |
|---|---|---|---|
| Display (landing hero) | Inter | 40px / 48px | 700 |
| H1 (page title) | Inter | 28px / 36px | 700 |
| H2 (section) | Inter | 20px / 28px | 600 |
| H3 (card title) | Inter | 16px / 24px | 600 |
| Body | Inter | 15px / 24px | 400 |
| Small / meta | Inter | 13px / 18px | 400/500 |
| Report body (Preview) | Source Serif 4 | 17px / 30px | 400 |
| Report headings (Preview) | Source Serif 4 | 22–28px | 600 |
| Monospace / metadata | JetBrains Mono | 13px / 20px | 400 |

### 5.3 Spacing & Layout Grid

8px base unit: `4, 8, 12, 16, 24, 32, 48, 64, 96`.

- App content max-width: `1280px`, with an inner `1120px` container for forms/tables.
- **Reading column** for the rendered report in Preview: `720px` max-width, centered — matches good long-form reading measure regardless of viewport.
- 12-column grid, `24px` gutters on desktop, `16px` on tablet, single column on mobile.

### 5.4 Elevation & Radius

- Radius: `8px` default controls, `12px` cards, `999px` (pill) for badges/chips/buttons-as-tags.
- Shadows (flat design, soft lift on interaction only):
  - `--shadow-sm`: `0 1px 2px rgba(15,23,42,0.06)` — resting cards
  - `--shadow-md`: `0 4px 12px rgba(15,23,42,0.08)` — hovered cards, dropdowns
  - `--shadow-lg`: `0 12px 32px rgba(15,23,42,0.14)` — modals, toasts

### 5.5 Iconography

Single icon set throughout (e.g., Lucide/Feather-style, 1.5px stroke, 20px default). Icon meaning is fixed and reused:

- ✅ check-circle → Ready / Succeeded / Strongly Recommended
- ▲ triangle-alert → Warning
- ⛔ octagon-alert → Blocking
- ℹ info-circle → Info-level warning / Pending
- ⏱ loader (animated) → InProgress
- ⬇ download → Export actions
- 🔁 refresh-cw → Regenerate
- 🗑 trash-2 → Delete
- 🔗 external-link → Citations

### 5.6 Motion & Micro-interactions

- Hover/focus transitions: `150ms ease-out`.
- Skeleton loaders (shimmering gray blocks shaped like the final content) for dashboard cards, history rows, and preview panels — never a lone spinner on a blank page.
- Generation progress: indeterminate animated bar + rotating status line (see §11).
- Toasts: slide-in from top-right, `220ms`; success/info auto-dismiss at 5s, errors persist until dismissed.
- Modals: fade + scale-from-98% `180ms`, backdrop blur `4px`.
- Wizard step transitions: horizontal slide `200ms`, respects `prefers-reduced-motion` (falls back to instant cut + fade).

---

## 6. App Shell & Navigation

**Top bar (persistent, `_Layout.cshtml`):**

```
[ Logo  ResearchReportGen ]   Dashboard   New Report   History        [provider dot]  [🔔]  [Avatar ▾]
```

- Left: wordmark/logo, links to Dashboard.
- Center-left: primary nav, active item underlined in `--color-primary-600`.
- Right: a compact **provider health cluster** (three small dots for Groq/Gemini/Fake, tooltip on hover) — this keeps AI availability visible without needing its own nav destination; notifications bell (future); avatar dropdown → Manage Account, Sign out.

**Breadcrumb bar:** thin secondary row beneath top bar on every page except Dashboard/Landing, `13px` type, `--color-ink-600`.

**Global toast/status region:** fixed top-right, stacks vertically, houses `_StatusMessage.cshtml` output (success/error/info banners from redirected actions).

**Footer:** minimal — build/version tag, Privacy link. No marketing footer inside the authenticated app.

---

## 7. Component Library

### 7.1 Buttons

| Variant | Use | Style |
|---|---|---|
| Primary | Single main action per screen ("Generate Report", "Save") | Solid `--color-primary-600`, white text, `8px` radius |
| Secondary | Supporting action ("Cancel", "Back") | White bg, `1px` border `--color-border`, ink text |
| Ghost | Low-emphasis inline action (table row actions) | No border/bg, primary-colored text, bg tint on hover |
| Danger | Destructive ("Delete Report") | Solid `--color-danger-600`, white text |
| Icon button | Export format buttons, table row icons | 36×36px hit target minimum, tooltip required |

All buttons: `44px` min height on touch devices, disabled state = 40% opacity + `cursor-not-allowed` + tooltip explaining why (never just silently disabled).

### 7.2 Form Controls

- **Text input / textarea**: label above field (never placeholder-only labels), helper text below, live character counter for length-bounded fields (`ReportTitle` 200, `TargetAudience` 300, `SupplementaryNote` 2000, etc.), error state = red border + inline message beneath.
- **Chip/tag input** (Topics, Criteria): type + Enter to add a chip; each chip shows an inline remove (×); duplicate-name attempt shakes the chip and shows "Already added" inline — mirrors the domain's case-insensitive `ComparisonKey` duplicate check.
- **Weight slider** (`CriterionWeight`, 1–10, default 5): horizontal slider with numeric badge that updates live; default state visually marked "Default" until touched.
- **Segmented control** (Technical Depth, Report Length): 3–4 button group, single-select, active segment filled.
- **Selectable cards** (Report Mode, Report Style): large clickable cards with icon, title, one-line description; selected = primary-tinted background + `2px` primary border + check icon top-right.
- **Accordion** (Optional Constraints): collapsed by default, chevron rotates on expand, badge shows count of filled fields even while collapsed (e.g., "Optional constraints (2 added)").

### 7.3 Cards

- **Report card** (dashboard/history grid view): title, mode icon, status badge, style/depth tags, quality score mini-badge, generated date, hover reveals action row (Preview / Export / Regenerate / Delete).
- **Metric card** (dashboard): large number, label, small trend/context line, subtle icon top-right.
- **Preset card** (style/criteria suggestions): name, short description, "Use this" ghost button.

### 7.4 Badges

- **ReportStatusBadge**: pill. `Ready` = filled green. `Draft` = outlined amber. Always paired with icon.
- **QualityScoreBadge**: circular ring gauge + numeric center (`82`), ring color interpolates red→amber→green across the 0–100 range with a visible tick mark at `70` (the `ReadyThreshold`). Clicking opens the Quality Warnings panel.
- **ProviderStatusBadge**: 8px dot + label + last-checked timestamp on hover tooltip.
- **CriteriaChip**: rounded chip, criterion name + small weight number in a circular sub-badge (e.g., `Latency ⑧`).
- **RecommendationStrengthBadge**: four fixed styles —
  - `StronglyRecommended` → solid green, ✓✓ icon
  - `Recommended` → solid blue, ✓ icon
  - `ConditionallyRecommended` → outlined amber, ± icon
  - `NotRecommended` → outlined gray/red, – icon

### 7.5 Tables

History table (desktop): sticky header, zebra-free (rely on `1px` row dividers), sortable columns (Title, Generated Date, Quality Score), row-hover reveals action menu (⋯). Collapses to a card list under `768px`.

### 7.6 Modals & Confirmations

Centered, `480px` max-width, backdrop blur. Delete confirmation always restates the exact report title being affected and explicitly names the action as reversible-by-support (soft delete) vs. permanent, so the user's mental model matches the actual `SoftDeletableEntity` behavior.

### 7.7 Wizard / Stepper

Horizontal step indicator on desktop (numbered circles + labels + connecting line; completed steps get a check and are clickable to jump back); becomes a compact progress bar + "Step 3 of 8" label + swipeable panel on mobile. Sticky footer bar: `Back` (secondary) ←→ `Continue` (primary), right-aligned on desktop, full-width stacked on mobile.

### 7.8 Empty States

Centered, max `360px` wide: simple line-art icon (matching the icon set, larger scale, muted color), one-line heading, one-line supporting text, single primary CTA. Never more than one CTA in an empty state.

### 7.9 Loading Skeletons

Every list/grid/panel has a matching skeleton (same card shapes, gray shimmering blocks) rather than a spinner-on-blank-page. Skeletons shown for: dashboard metrics, history rows, preview panel while first loading, provider health dots (pulse gray until first health check resolves).

---

## 8. Page-by-Page Specifications

### 8.1 `Home/Index` (Landing, unauthenticated)

- **Hero:** Headline ("Turn any research question into a decision-ready report"), subheadline, primary CTA `Get Started Free`, secondary `Log In`.
- **Feature strip (3 columns):** "Research one topic deeply" / "Compare options side-by-side" / "Export anywhere" (MD · HTML · PDF · DOCX icons).
- **Proof section:** a static, stylized preview of a sample report card (quality badge, comparison table snippet) to set expectations for output quality.
- **Footer CTA:** repeat primary action.

### 8.2 `Home/Privacy`

Simple centered document layout, serif body text (matches "this is a real document" register used in Preview), back-to-home link at top and bottom.

### 8.3 Identity Pages (`Areas/Identity/Pages/Account/*`)

- **Login:** centered `400px` card, email + password, "Remember me" checkbox, primary `Log In`, link to Register. Validation summary rendered above the form, not as a toast (user hasn't left the page).
- **Register:** same card treatment, fields Display Name / Email / Password / Confirm Password, live password-strength meter reflecting Identity's actual rule set (8+ chars, upper, lower, digit — no special-char requirement, so the meter shouldn't imply one), primary `Create Account`.
- **Logout:** brief confirmation state ("You've been signed out") with CTA back to Login/Home — avoids a jarring instant redirect.
- **AccessDenied:** centered icon + "You don't have access to this page" + button back to Dashboard.
- **Manage/Index:** simple settings form — Display Name, Email (read-only unless verified-change flow exists), Change Password section as a separate card beneath, Save button per section with its own success toast.

### 8.4 `Dashboard/Index`

```
Welcome back, {DisplayName}                              [ + New Report ]
--------------------------------------------------------------------------
[ Total Reports ]  [ Reports This Month ]  [ Avg Quality Score ]  [ Draft/Needs Review ]

Recent Reports                                              [ View all → ]
[ card ][ card ][ card ]

AI Provider Status
● Groq   ● Gemini   ● Fake (dev)
```

- Metric cards use the numeric type scale prominently; "Avg Quality Score" card itself uses the quality-score color ramp on its number.
- Recent Reports uses the **Report card** component, max 6, newest first.
- First-time (zero reports) empty state replaces the Recent Reports section entirely: illustration + "Generate your first report in under two minutes" + `New Report` CTA.

### 8.5 `Reports/Index` (History)

```
[ 🔍 Search title/topic... ]  [Status ▾] [Mode ▾] [Style ▾] [Provider ▾] [Date range ▾]   [Sort: Newest ▾]

Title              Mode         Style     Status   Quality   Generated       ⋯
SignalR vs gRPC     Comparison   Balanced  Ready    ▓▓▓▓░ 82  Jul 12, 2026    ⋯
Understanding CQRS  Research     Educ.     Draft    ▓▓░░░ 45  Jul 10, 2026    ⋯
```

- Filter chips are combinable and shown as removable pills above the table once applied ("Status: Ready ×").
- Zero-results-after-filtering state: "No reports match your filters" + `Clear filters` ghost button — distinct copy/illustration from the true zero-reports empty state.
- Mobile: table → stacked report cards, filters collapse into a single `Filters` sheet triggered by a button.

### 8.6 `Reports/Create` (Guided Wizard)

Step sequence (stepper header always visible on desktop):

1. **Goal & Title** — `ReportTitle` input (≤200 chars), example placeholders rotate ("e.g., 'Choosing a Real-Time Communication Stack'").
2. **Report Mode** — two large cards: *"Research one topic"* vs *"Compare multiple topics."* This single choice reconfigures steps 3 and 7 downstream — copy should make that consequence explicit ("You'll be able to add just one topic, or several to compare").
3. **Topics** — chip input; comparison mode enforces 2–8 with a live counter ("3 of 8 topics · minimum 2"); research mode allows 1 with optional extras framed as "context topics."
4. **Audience** — `TargetAudience` free text (≤300 chars) with example chips ("backend engineers," "non-technical stakeholders," "engineering leadership").
5. **Style** — `ReportStyle` cards (Technical / Executive / Educational / Balanced); one card carries a **"Suggested for your audience"** ribbon when `StyleSuggestionPolicy` recommends it, without blocking the other choices.
6. **Depth & Length** — two segmented controls: `TechnicalDepth` (Beginner→Expert) and `ReportLength` (Concise/Standard/Comprehensive), each option has a one-line consequence hint ("Comprehensive: longer, more exhaustive coverage").
7. **Criteria / Focus Areas** — mode-dependent:
   - *Comparison:* criteria builder (name, optional description, weight slider), minimum 2 enforced with live counter, suggestion chips sourced from `CriteriaPreset` matched to typed topics/category.
   - *Research:* same builder framed as optional "Research focus areas — guide what the report should emphasize," never blocking.
8. **Optional Constraints** — accordion of the 7 `SupplementaryNote` fields, all collapsed by default, badge shows fill-count.
9. **Review** — read-only recap grouped by step with inline `Edit` links back to that step; AI Provider preference selector (`System Default` / `Groq` / `Gemini`) shown with live `ProviderStatusBadge`s; primary CTA `Generate Report`.

On submit → full-page **Generation Loading** state (see §11) → redirect to Preview.

### 8.7 `Reports/Details`

```
"SignalR vs gRPC vs WebSockets"          [Ready ✓]   [Comparison · Balanced · Advanced]
                                          [Preview] [Export ▾] [Regenerate] [Delete]

Request Summary                          Generation History
- Topics: SignalR, gRPC, WebSockets       v1  Groq   Succeeded  4.2s  1,204 tok  Jul 10
- Criteria: Latency(8), Scalability(7)…   v2  Gemini Succeeded  6.8s  1,540 tok  Jul 12 ← current
- Audience: backend engineers
- Constraints: 2 added

Current Report                            Citations (3)              Recommendations (4)
Quality: ▓▓▓▓░ 82/100  [view warnings]     1. docs.microsoft.com ↗    Small internal tool → SignalR (Strongly Recommended)
```

- Generation History is a compact vertical timeline; failed runs show a red row with a `View error` link opening the classified, friendly error message (never the raw `RawResponse`/stack trace inline — that stays behind an explicit "technical details" toggle for power users).

### 8.8 `Reports/Preview`

Two-column desktop layout:

```
┌───────────────────────────────────────┐  ┌───────────────────────┐
│  (Serif, 720px reading column)        │  │ Quality  ◔ 82/100     │
│  # Executive Summary                  │  │ [ ▲ 2 warnings ▾ ]    │
│  ...rendered Markdown...               │  │                       │
│  ## Comparison Table                  │  │ Export                │
│  |          | Latency | Scale |       │  │ [MD][HTML][PDF][DOCX] │
│  |----------|---------|-------|       │  │                       │
│                                        │  │ Regenerate ⟳          │
│  ## Scenario Recommendations           │  │                       │
│  ...                                  │  │ Citations (3)         │
└───────────────────────────────────────┘  │ Generation details ▾  │
                                            └───────────────────────┘
```

- Quality Warnings panel expands to a list sorted **Blocking → Warning → Info**, each row = icon + section name + human explanation ("The report does not appear to include a 'Comparison Table' section").
- If `ReportStatus.Draft`, a slim amber banner sits above the rendered content: *"This report hasn't cleared the quality bar yet — you can still export it, or regenerate for a stronger version."* (Export is never blocked; the user is simply informed.)
- Generation Details accordion (collapsed): provider, model, prompt version, version number, tokens, duration — monospace type.
- Mobile: sidebar content moves beneath the report body; Export and Regenerate become a sticky bottom action bar.

### 8.9 `Reports/Regenerate`

Single-page (not a full wizard) — current settings shown as editable, pre-filled: Style, Depth, Length, Provider, plus a new free-text **"Notes for this regeneration"** field (e.g., "make the recommendations more decisive"). A small "What's changing" summary lists only the fields the user actually altered before they confirm. Primary CTA `Regenerate Report` → Generation Loading → Preview (now `v{n+1}`, old recommendations/citations cleared per domain rule, and the UI explicitly shows the version increment so nothing feels silently overwritten).

### 8.10 `Reports/Delete`

Modal confirmation: report title restated verbatim, icon, copy: *"This removes '{title}' from your report history. It won't appear in searches or your dashboard."* `Cancel` (secondary) / `Delete Report` (danger). No permanent-deletion language, since this is a soft delete.

### 8.11 `Exports/DownloadError`

Centered error state: icon, *"We couldn't generate your {Format} file just now."* Two actions: `Try again` (re-triggers the same export) and `Choose a different format` (returns to Preview's export row). A muted note: *"Your report itself is unaffected — only this download failed."*

---

## 9. Core User Flows (UI Walkthroughs)

Mapped to the architecture doc's numbered flows, described from the screen's point of view:

1. **Visitor opens app** → Landing hero → chooses Register or Login.
2. **Register** → Identity card form → auto-signed-in → redirected to Dashboard (empty state if first login).
3. **Login** → Identity card form → Dashboard.
4. **Dashboard** → metric cards + recent report cards load via skeleton → resolve.
5. **Start wizard** → `New Report` → 9-step guided flow (§8.6), presets fetched live as user types topics/audience.
6. **Get suggestions** → typing in Topics/Audience triggers debounced suggestion chip fetch (style + criteria presets) — chips appear inline beneath the field, tappable to auto-fill.
7. **Submit request** → Review step → `Generate Report` → request saved as Draft-then-Submitted instantly (no loading needed here — this is just persistence, not generation) → transitions straight into Generation Loading.
8. **Generation succeeds** → Loading screen resolves → success toast ("Report generated — quality score 82/100") → redirect to Preview.
9. **Generation fails** → Loading screen swaps to Friendly Failure card (§11) with Retry / Switch Provider actions, request itself preserved.
10. **Preview report** → two-column layout, quality/warnings/citations/export all visible without scrolling on desktop.
11. **Download report** → click a format button → browser download begins, small inline "Downloading..." micro-state on that button (~1s), no full-page interruption.
12. **Search history** → filter chips + search box, results update live (debounced), empty/no-results state handled distinctly.
13. **Regenerate** → single-page form (§8.9) → Loading → Preview shows new version badge.
14. **Delete** → confirmation modal → success toast on the page the user returns to ("Report deleted").
15. *(Developer flow — no UI surface; included here only for completeness of traceability.)*

---

## 10. States: Empty / Loading / Error / Success

| State | Pattern |
|---|---|
| **First-use empty** (0 reports ever) | Illustration + "Generate your first report" + single CTA |
| **Filtered empty** (0 results after search/filter) | Smaller inline illustration + "No reports match your filters" + `Clear filters` |
| **Loading (list/grid)** | Skeleton cards/rows matching final shape, 400–800ms minimum display to avoid flash-of-skeleton flicker on fast responses |
| **Loading (generation)** | Full takeover, animated indeterminate bar + rotating status copy (§11) |
| **Success** | Toast, top-right, green accent bar, auto-dismiss 5s, always includes the *specific* outcome ("Report generated — quality score 82/100," not just "Success") |
| **Recoverable error** (AI failure, export failure) | Full card with icon, plain-language cause, 1–2 concrete next actions |
| **Validation error** (form field) | Inline, red text directly under the field, focus auto-jumps to first invalid field on submit attempt |
| **Unexpected error** (500, infra) | Friendly full-page error via `Shared/Error.cshtml` — icon, "Something went wrong on our end," `Go to Dashboard` — never a raw stack trace to the user |

---

## 11. AI-Specific UX Patterns

### 11.1 Generation progress messaging

Since actual provider progress isn't streamed, the loading screen uses **time-boxed rotating copy** to keep the wait feeling active rather than stalled:

| Elapsed | Message |
|---|---|
| 0–3s | "Contacting {ProviderName}..." |
| 3–10s | "Structuring your {report mode}..." |
| 10–20s | "Drafting sections and recommendations..." |
| 20s+ | "Almost there — finalizing your report..." |

Progress bar itself stays indeterminate (a moving gradient sweep) — never fake-precise percentages, since real duration varies by provider/topic count.

### 11.2 Quality score literacy

An (i) icon next to every `QualityScoreBadge` opens a small popover:
> *"This score reflects how structurally complete the report is — whether it includes the sections a good report of this type should have. It does not verify factual accuracy. 70+ is considered Ready."*

This framing is repeated verbatim everywhere the score appears, so users build one accurate mental model instead of several.

### 11.3 Failure classification → friendly copy

| Underlying cause | User-facing message | Primary action offered |
|---|---|---|
| Provider timeout | "The AI provider took too long to respond." | Retry same provider |
| Empty/invalid AI output | "The AI didn't return usable content this time." | Retry / Switch provider |
| Provider unavailable/misconfigured | "{Provider} isn't available right now." | Switch provider (pre-selected to a healthy one) |
| Rate limit | "This provider is temporarily busy." | Retry in a moment / Switch provider |
| Unknown | "Something unexpected happened generating your report." | Retry / Contact support |

Every failure card also reassures: *"Your request settings were saved — nothing to redo."*

### 11.4 Provider health as ambient awareness

The three-dot cluster in the top bar (§6) means a user never has to visit a separate "status page" to know whether Groq/Gemini are currently healthy before starting a wizard — it's ambient, always-visible, low-emphasis.

### 11.5 Regeneration transparency

Every regenerated report visibly shows `v2`, `v3`, etc. next to the title in Details/Preview, and the Generation History timeline (§8.7) keeps every prior attempt (including failures) — regeneration is framed as *"trying again with the same request,"* never as silently editing history.

---

## 12. Content & Microcopy Guidelines

- **Voice:** clear, warm, competent — like a helpful colleague, not a legal document or a hype-y AI product. Matches the vision doc's own instruction that generated reports "prioritize clear explanations over excessive technical detail" — the *app's own copy* should hold itself to the same standard.
- **Buttons are verbs tied to outcomes:** `Generate Report`, not `Submit`. `Add Topic`, not `OK`. `Delete Report`, not `Confirm`.
- **Warnings are framed as "still needs," not "failed":** *"The report does not appear to include a Comparison Table section"* — descriptive, not accusatory, and never implies the user did something wrong.
- **Never blame the user for AI failures.** Failures are framed as the provider/system's issue, with the user's own input explicitly preserved.
- **Numbers get units and context**, never bare: "82/100," "1,204 tokens," "4.2s," not "82," "1204," "4.2."
- **Avoid double negatives and jargon** in validation messages: "Add at least 2 topics to compare" rather than "MinimumComparisonTopicsRequired: Topics.Count must not be less than 2."

---

## 13. Responsive Behavior

| Breakpoint | Range | Key adaptations |
|---|---|---|
| Mobile | < 640px | Single column everywhere; wizard = full-screen steps with sticky Back/Continue bar; history = card list; Preview sidebar content moves below body; Export/Regenerate become a sticky bottom bar |
| Tablet | 640–1024px | Two-column where sensible (Preview keeps sidebar but narrower); wizard stepper compresses to progress bar + "Step X of 9" |
| Desktop | > 1024px | Full multi-column layouts as specified in §8; wizard shows full horizontal stepper with clickable completed steps |

Touch targets ≥ 44px on all interactive elements at mobile/tablet widths, including slider handles and chip-remove buttons.

---

## 14. Accessibility

- **Color is never the sole signal.** Every status/severity/health indicator pairs color with an icon shape *and* a text label (§5.1, §5.4).
- **Contrast:** all text meets WCAG AA (4.5:1 body, 3:1 large text/icons).
- **Keyboard:** full wizard navigable via Tab/Shift+Tab/Enter; Esc closes modals and dropdowns; stepper steps are focusable buttons, not divs with click handlers only.
- **Focus visibility:** a persistent 2px focus ring in `--color-primary-600` on every interactive element, never suppressed.
- **ARIA:** icon-only buttons (export format icons, table row ⋯ menu) always carry `aria-label`; toasts render in an `aria-live="polite"` region; form errors are linked to their field via `aria-describedby`.
- **Screen reader flow for Preview:** the rendered report's heading hierarchy (H1→H3) is preserved from the Markdown structure so screen reader users can navigate it exactly like a real document outline.
- **Reduced motion:** all step transitions, progress animations, and toast slides respect `prefers-reduced-motion` and fall back to instant/opacity-only changes.
- **Skip link:** "Skip to main content" as the first focusable element on every page.

---

## 15. Design Tokens Reference

Single source of truth lives in `site.css` as CSS custom properties; page-specific stylesheets (`dashboard.css`, `report-form.css`, `report-preview.css`) consume tokens for layout only and never redefine color/type values.

```css
/* site.css — root tokens (excerpt) */
:root {
  /* Color */
  --color-primary-600: #4F46E5;
  --color-primary-700: #4338CA;
  --color-primary-50:  #EEF2FF;
  --color-accent-500:  #0EA5E9;

  --color-success-600: #16A34A;
  --color-success-50:  #F0FDF4;
  --color-warning-600: #D97706;
  --color-warning-50:  #FFFBEB;
  --color-danger-600:  #DC2626;
  --color-danger-50:   #FEF2F2;
  --color-info-600:    #2563EB;
  --color-info-50:     #EFF6FF;

  --color-ink-900: #0F172A;
  --color-ink-600: #475569;
  --color-ink-400: #94A3B8;
  --color-surface-0:   #FFFFFF;
  --color-surface-50:  #F8FAFC;
  --color-surface-100: #F1F5F9;
  --color-border: #E2E8F0;

  /* Type */
  --font-ui: 'Inter', system-ui, sans-serif;
  --font-report: 'Source Serif 4', Georgia, serif;
  --font-mono: 'JetBrains Mono', ui-monospace, monospace;

  /* Space (8px base) */
  --space-1: 4px;  --space-2: 8px;  --space-3: 12px; --space-4: 16px;
  --space-6: 24px; --space-8: 32px; --space-12: 48px; --space-16: 64px;

  /* Radius */
  --radius-control: 8px;
  --radius-card: 12px;
  --radius-pill: 999px;

  /* Shadow */
  --shadow-sm: 0 1px 2px rgba(15,23,42,0.06);
  --shadow-md: 0 4px 12px rgba(15,23,42,0.08);
  --shadow-lg: 0 12px 32px rgba(15,23,42,0.14);

  /* Motion */
  --ease-out: cubic-bezier(0.16, 1, 0.3, 1);
  --duration-fast: 150ms;
  --duration-base: 200ms;
}
```

---

## 16. Future Enhancements

Not required for v1, but natural next steps once the core loop is validated:

- **Dark mode** — token set is already structured to support a parallel `--color-*-dark` layer without touching component code.
- **Version diff view** — side-by-side or inline diff between `v{n-1}` and `v{n}` of a regenerated report.
- **Saved request templates / duplicate-as-new** — clone a past `ReportRequest` into a fresh draft.
- **Read-only share links** for a generated report (would need a new sharing/authorization concept — out of scope for the current domain model).
- **Print-optimized stylesheet** for the Preview page, separate from the PDF export renderer.
- **Provider cost/latency comparison** panel once more than two real providers are live.
- **PWA installability** for the mobile experience, given the app is already responsive-first.

---

*Document owner: Design. Source of truth for all Razor views, partials, and `wwwroot/css/*` styling decisions. Update this file whenever a new page, component, or status enum is added to the domain.*