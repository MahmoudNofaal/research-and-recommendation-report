# Component Specs — Research and Recommendation Reports

Read `design-tokens.md` first — every value below refers back to those tokens. Each component includes structure, states, and the Tailwind class pattern to use. Matching example partials live in `assets/examples/`.

---

## Sidebar (icon rail)

- Fixed width: `64px` (`w-16`)
- Background: `bg-surface-sidebar`, right border `border-r border-border-subtle`
- Vertical stack of icon buttons, each `40x40` (`w-10 h-10`), `rounded-xl`, centered icon
- Inactive: icon `text-tertiary`, transparent background
- Active: icon `text-accent-primary` (or `text-primary`), background `bg-surface-active/40`
- Hover (inactive): `bg-surface-raised`
- Gap between icons: `12px` within a group, `24px` between groups (top nav icons vs. bottom-anchored profile icon)

## Search input

- Full pill: `rounded-full bg-surface-raised h-10 px-4`
- Leading icon (search, Lucide) `text-tertiary`, `18px`
- Placeholder text: `text-tertiary text-body-sm`
- Focus: `ring-2 ring-accent-primary/50 bg-surface-raised-hover`

## Breadcrumb

- Row of `text-body-sm`, separated by a chevron-right Lucide icon (`14px`, `text-tertiary`)
- Parent crumbs: `text-tertiary`, hover `text-secondary`
- Current page: `text-primary font-semibold`

## Status indicator ("Last Update…")

- 8px filled circle, `bg-accent-success`, optional soft pulse animation (`animate-pulse`, respect reduced-motion)
- Label: `text-tertiary text-body-sm`

## Page header

Layout: `flex items-center justify-between`

Left side:
- Type icon: circle outline, `28px`, stroke `accent-primary`
- Title: `text-display text-primary`
- Attachment/link icon button: small circle, `bg-surface-raised`, icon `text-tertiary`, `32px` diameter

Right side:
- Avatar stack (see below)

## Tab navigation

- Row, `gap-24px` (use `gap-6`)
- Each tab: icon (16–18px) + label, `flex items-center gap-2`
- Inactive: `text-tertiary`
- Active: `text-primary`, `2px` bottom border `border-accent-primary`, offset with `pb-3 border-b-2`
- Transition: `150ms ease-out` on color/border

## Segmented control (Kanban / Table View / Timeline View)

- Outer container: `bg-surface-raised rounded-full p-1 flex`
- Each segment: `px-4 py-2 rounded-full text-body-sm font-medium transition-colors duration-150`
- Active segment: `bg-surface-active text-primary`
- Inactive segment: `text-secondary`, hover `text-primary`
- Implement the active-state swap with **Alpine.js** (`x-data`, `x-on:click`) since it's pure UI state; if switching also changes the rendered content (Kanban vs Table vs Timeline), pair it with an **htmx** `hx-get` to swap in the corresponding partial.

## Toolbar buttons (Sort By, Filter)

- Pill: `bg-surface-raised rounded-full h-9 px-4 flex items-center gap-2`
- Icon `16px text-secondary`, label `text-body-sm text-secondary`
- Hover: `bg-surface-raised-hover`
- These typically open a dropdown (Alpine.js `x-show` panel) or trigger an htmx `hx-get` that re-renders the list with new sort/filter params

## Kanban column

Structure:
```
<column>
  <header>  [status dot/icon]  Column Name   (count)     [+ add]  [... menu]
  <body>    vertical stack of cards, gap-3 (12px)
</column>
```
- Column min-width: `340px`, `flex-1` within a horizontally scrollable row on mobile
- Header: `flex items-center justify-between py-3`
- Column title: `text-heading-sm text-secondary`
- Count: `text-tertiary text-body-sm`, shown in parentheses or a small pill `bg-surface-raised rounded-md px-1.5`
- "+" and "…" buttons: ghost icon buttons, `28px`, `text-tertiary`, hover `bg-surface-raised text-primary`
- No distinct column background — columns sit directly on `surface-base`; only the cards inside have their own `surface-card` background

## Kanban card

- Container: `bg-surface-card border border-border-subtle rounded-md p-4` (rounded-md = `radius-md`, 12px)
- Hover: `border-border-default` + very subtle `translate-y-[-1px]` (150ms, skip if reduced motion)
- Top row: `flex items-center justify-between`
  - Left: one or more badge pills (`gap-2`) — see Badge component
  - Right: item ID, `text-tertiary text-caption`
- Title: `text-body-md text-primary mt-3` (semibold)
- Description: `text-body-sm text-tertiary mt-1 line-clamp-1` (or `line-clamp-2` if you want more preview)
- Make the whole card draggable via SortableJS; on drop, `hx-post` the new column/status to the server and re-render just that column (or optimistically reorder client-side, then reconcile)

## Badge / tag pill

- `inline-flex items-center rounded-md px-2 py-0.5 text-caption font-semibold`
- Background + text color: pick one pair from the `tag-*` tokens in `design-tokens.md` — never a one-off color
- Multiple badges on a card sit inline with `gap-2`, wrap to a second line only if truly necessary (prefer truncating/prioritizing one badge + a "+2" style overflow if space is tight)

## Avatar stack

- Wrapping element: `flex items-center` with each avatar `-ml-2` except the first
- Avatar: `w-8 h-8 rounded-full border-2 border-surface-base object-cover` (border color matches whatever surface it sits on, so it "cuts into" the one behind it)
- Overflow badge (e.g. "+5"): same sizing as an avatar, `bg-surface-raised text-secondary text-caption font-semibold flex items-center justify-center rounded-full border-2 border-surface-base`

## Table view (if/when you build one)

- Row height: `48px`, `border-b border-border-subtle` between rows, no border on last row
- Header row: `text-caption text-tertiary uppercase tracking-wide`, sticky if the table scrolls
- Row hover: `bg-surface-card`
- Cell padding: `px-4`

## Empty / loading states

- Empty state: centered icon (`text-tertiary`, 32px) + `text-body-sm text-tertiary` message + a primary action button if applicable. Voice: say what happened and what to do next, don't leave it blank.
- Loading (htmx swap in-flight): skeleton blocks using `bg-surface-raised animate-pulse rounded-md`, matching the exact dimensions of the content being replaced — don't use a generic spinner for list/card content, it causes layout shift.

## Buttons (primary/secondary — not shown in reference but needed across the app)

- Primary: `bg-accent-primary text-white rounded-full px-4 py-2 text-body-sm font-semibold hover:bg-accent-primary-hover`
- Secondary/ghost: `bg-surface-raised text-secondary rounded-full px-4 py-2 hover:bg-surface-raised-hover hover:text-primary`
- Destructive: same shape, `bg-tag-rose`-style but solid — use a dedicated `accent-danger` (`#EF4444`) rather than reusing the badge token, since a button needs a fully opaque, higher-contrast fill
- All buttons: `focus-visible:ring-2 focus-visible:ring-accent-primary focus-visible:ring-offset-2 focus-visible:ring-offset-surface-base`
