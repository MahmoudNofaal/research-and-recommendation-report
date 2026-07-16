---
name: research-reports-design-system
description: Design system and UI component spec for the "Research and Recommendation Reports" app — a dark SaaS dashboard built with ASP.NET Core MVC + Razor + Tailwind CSS + htmx + Alpine.js. MUST be consulted before writing, editing, or reviewing ANY Razor view, layout, partial view, tag helper, Tailwind class, or CSS file in this project. Applies to every page and component: report list/kanban boards, report detail pages, dashboards, tables, forms, sidebars, tabs, buttons, cards, badges, avatars, filters. Trigger this whenever the user asks to build a page, add a component, style something, restyle something, or make a page "match the rest of the app" — even if they don't say "design system" explicitly. Do NOT improvise colors, spacing, radii, fonts, or component structure from general knowledge — always pull exact values from references/design-tokens.md and references/components.md, and pattern-match new components after assets/examples/*.cshtml.
---

# Research and Recommendation Reports — Design System

This app uses a dark, floating-panel SaaS dashboard aesthetic: a bright blue gradient background with a dot-grid pattern, holding a near-black rounded panel that contains an icon-only sidebar and the main content area (breadcrumbs, page header, tabs, toolbar, and card/table/kanban content).

**Read this file first for every visual task. Then read the reference file that matches what you're building, before writing any markup.**

## Stack (do not deviate without asking the user)

- **ASP.NET Core MVC** — Razor views/partials/tag helpers, no client framework (no React/Vue/Angular)
- **Tailwind CSS** — utility classes only, using the **semantic tokens** defined in `assets/tailwind.config.js` (never raw `gray-800`, `blue-500`, etc. — always the named tokens like `bg-surface-card`, `text-text-secondary`, `border-border-subtle`)
- **htmx** — for view switching (Kanban/Table/Timeline), filtering, sorting, pagination, and any partial-page update. Controller actions return `PartialView()`.
- **Alpine.js** — for pure client-side UI state: dropdown menus, tab active-state, mobile sidebar toggle. Nothing that needs server data.
- **SortableJS** — for kanban drag-and-drop; on drop, fire an htmx `hx-post` to persist the new status/column.
- **Lucide icons** — outline style, 1.5px stroke, 18–20px. Never mix in a different icon set.
- **Font** — Inter, self-hosted (see `assets/globals.css` for `@font-face`). Never fall back to a system sans silently — if Inter isn't loaded, that's a bug, flag it.

## Files in this skill

| File | When to read it |
|---|---|
| `references/design-tokens.md` | Before writing ANY new markup — colors, type scale, spacing, radius, shadows, motion |
| `references/components.md` | Before building a specific component (sidebar, card, badge, tabs, toolbar, kanban column, avatar stack, etc.) |
| `assets/tailwind.config.js` | Drop into the project root as-is. This IS the source of truth for token → class names |
| `assets/globals.css` | Base layer: gradient background, dot-grid pattern, font-face, focus rings |
| `assets/examples/*.cshtml` | Reference Razor partials already matched to the tokens — copy the pattern, don't reinvent it |
| `assets/palette-preview.html` | Open in a browser to visually sanity-check the color tokens against the real screenshot |

## Non-negotiable rules

1. **Never hardcode a hex value in a `.cshtml` or component file.** Every color comes from a Tailwind token defined in `assets/tailwind.config.js`. If a token you need doesn't exist yet, add it to the config (and note it in `design-tokens.md`) rather than inlining a color.
2. **Reuse before you rebuild.** Check `Views/Shared/Partials/` (see structure below) for an existing partial before writing new markup for a badge, card, avatar stack, etc.
3. **Server-render by default.** Only reach for Alpine.js state or htmx swaps when the interaction genuinely needs it — don't build a client-side SPA pattern inside an MVC view.
4. **Match spacing and radius scales exactly** (see design-tokens.md) — this UI reads as "designed" specifically because spacing is consistent across every card, pill, and panel. Sloppy spacing is the fastest way to break the look.
5. **Every interactive element needs a visible focus state** (`focus-visible:ring-2 ring-accent-primary`) — this is a real product, not a mockup.
6. **Respect `prefers-reduced-motion`** for any hover/transition animation.

## Suggested Razor project structure

```
Views/
├── Shared/
│   ├── _Layout.cshtml              (gradient bg wrapper + dark panel shell + sidebar)
│   └── Partials/
│       ├── _Sidebar.cshtml
│       ├── _Breadcrumb.cshtml
│       ├── _PageHeader.cshtml
│       ├── _TabNav.cshtml
│       ├── _SegmentedControl.cshtml
│       ├── _Toolbar.cshtml          (sort/filter pill buttons)
│       ├── _Badge.cshtml
│       ├── _AvatarStack.cshtml
│       ├── _KanbanColumn.cshtml
│       └── _KanbanCard.cshtml
├── Reports/
│   ├── Index.cshtml                 (kanban/table/timeline switch view)
│   ├── _KanbanView.cshtml           (htmx partial)
│   ├── _TableView.cshtml            (htmx partial)
│   └── Details.cshtml
wwwroot/
├── css/
│   └── globals.css
├── fonts/ (Inter woff2 files)
tailwind.config.js
```

## Quick-reference cheat sheet

(Full detail in `references/design-tokens.md` — this is just the 90%-of-the-time values)

- Panel background: `bg-surface-base` · Card background: `bg-surface-card` · Pills/toolbar: `bg-surface-raised`
- Body text: `text-text-primary` · Secondary/labels: `text-text-secondary` · Faint/counts: `text-text-tertiary`
- Borders: `border-border-subtle` (default), 1px
- Accent (links, active tab, active icon): `text-accent-primary` / `bg-accent-primary`
- Card radius: `rounded-xl` (12px) · Outer panel radius: `rounded-3xl` (24px) · Pills: `rounded-full`
- Spacing scale: 4 / 8 / 12 / 16 / 20 / 24 / 32 / 40px — always from this scale, never arbitrary px
- Badge pill: `rounded-md px-2 py-0.5 text-xs font-semibold` + one of the tag-color pairs in design-tokens.md

If anything in a new request conflicts with this system, flag it to the user instead of silently improvising — this file is meant to keep every page looking like it came from the same designer.
