# Design Tokens — Research and Recommendation Reports

Values below were sampled directly from the reference screenshot (pixel-averaged at multiple points per surface) and then squared up into a coherent, accessible scale. Treat these as the source of truth — implemented 1:1 in `assets/tailwind.config.js`.

## 1. Color

### 1.1 Page background (outside the main panel)
A diagonal/radial blue gradient, brighter near the top, deepening toward the edges, with a faint white dot-grid overlay.

| Token | Hex | Notes |
|---|---|---|
| `bg-gradient-from` | `#2F62E8` | brightest point, upper-center |
| `bg-gradient-via` | `#1D3F91` | mid transition |
| `bg-gradient-to` | `#0B1220` | deep edges/corners, near-black navy |
| `bg-dot-pattern` | `rgba(255,255,255,0.08)` | 1–2px dots, ~24px grid |

CSS: `background: radial-gradient(120% 100% at 50% 0%, var(--bg-gradient-from) 0%, var(--bg-gradient-via) 45%, var(--bg-gradient-to) 100%);` plus a repeating dot-grid layer on top (see `globals.css`).

### 1.2 Surfaces (the dark panel and everything inside it)

| Token | Hex | Used for |
|---|---|---|
| `surface-base` | `#12141C` | main panel background |
| `surface-sidebar` | `#14161E` | icon rail sidebar (near-identical to base, separated by a border) |
| `surface-card` | `#171922` | kanban cards, table rows |
| `surface-raised` | `#2A2D36` | search input, sort/filter pills, inactive segmented-control background |
| `surface-raised-hover` | `#34373F` | hover state for the above |
| `surface-active` | `#4B4E57` | active segment in Kanban/Table/Timeline toggle, active sidebar icon |

### 1.3 Borders

| Token | Hex | Used for |
|---|---|---|
| `border-subtle` | `#23252E` | default 1px card/panel borders |
| `border-default` | `#2C2F39` | hover state border |
| `border-strong` | `#3A3D47` | dividers that need more separation (rare) |

### 1.4 Text

| Token | Hex | Used for |
|---|---|---|
| `text-primary` | `#F5F6F8` | titles, card titles, primary labels |
| `text-secondary` | `#9599A6` | breadcrumb current, tab labels, toolbar labels |
| `text-tertiary` | `#5B5E6B` | descriptions, counts, timestamps, placeholders |
| `text-disabled` | `#3A3D47` | disabled controls |

### 1.5 Accent

| Token | Hex | Used for |
|---|---|---|
| `accent-primary` | `#3B82F6` | active tab underline, links, active icon strokes, project-type icon ring |
| `accent-primary-hover` | `#2563EB` | hover/pressed |
| `accent-success` | `#22C55E` | "last update" live status dot |

### 1.6 Tag / badge colors

Each tag token is a **background + text** pair. Background is the solid color at ~14% opacity; text is the solid tint at full opacity for contrast on the dark card.

| Token | Background | Text | Sample usage in reference |
|---|---|---|---|
| `tag-rose` | `rgba(244,63,94,0.14)` | `#FB7185` | Priority: High |
| `tag-amber` | `rgba(245,158,11,0.14)` | `#FBBF24` | Priority: Medium |
| `tag-sky` | `rgba(56,189,248,0.14)` | `#38BDF8` | Category: Wireframe |
| `tag-violet` | `rgba(167,139,250,0.14)` | `#A78BFA` | Category: Marketing Copy |
| `tag-gold` | `rgba(234,179,8,0.14)` | `#EAB308` | Category: UX Design |
| `tag-emerald` | `rgba(52,211,153,0.14)` | `#34D399` | Priority: Low / Available (reserve for future states) |
| `tag-neutral` | `rgba(148,163,184,0.14)` | `#94A3B8` | default/uncategorized tag |

**Rule for your app's domain:** map your own report metadata onto these — e.g. `tag-rose`/`tag-amber`/`tag-emerald` for priority or confidence level (High/Medium/Low), and rotate `tag-sky`/`tag-violet`/`tag-gold`/`tag-neutral` for report categories (Market Research, Competitive Analysis, Recommendation, etc.). Don't invent new hues per page — reuse this fixed set so every page reads as one system.

## 2. Typography

Font family: **Inter** (self-hosted), fallback `ui-sans-serif, system-ui, sans-serif`.

| Token | Size / Line height | Weight | Used for |
|---|---|---|---|
| `text-display` | 28px / 34px | 600 | Page title ("CodeSphere - Design Project" equivalent) |
| `text-heading-md` | 20px / 28px | 600 | Section headings |
| `text-heading-sm` | 15px / 20px | 600 | Kanban column titles |
| `text-body-md` | 14px / 20px | 600 | Card titles |
| `text-body-sm` | 13px / 18px | 400 | Card descriptions, breadcrumb |
| `text-caption` | 12px / 16px | 600 | Badges, tags, counts, IDs |
| `text-label-xs` | 11px / 14px | 600, uppercase, tracking-wide | Optional — rarely used in reference |

## 3. Spacing scale

Base unit 4px. **Only use values from this scale** — arbitrary padding is what makes AI-generated UI look slightly "off":

`4, 8, 12, 16, 20, 24, 32, 40, 48, 64`

Typical usage: card padding `16px`, column gap `24px`, toolbar item gap `8px`, section gap `32px`.

## 4. Radius scale

| Token | Value | Used for |
|---|---|---|
| `radius-sm` | 8px | badges/tags (if not fully pill) |
| `radius-md` | 12px | cards, inputs |
| `radius-lg` | 16px | kanban columns, modals |
| `radius-xl` | 24px | the outer floating panel |
| `radius-full` | 9999px | search bar, segmented control, toolbar pills, avatars |

## 5. Shadows

| Token | Value | Used for |
|---|---|---|
| `shadow-panel` | `0 20px 60px rgba(0,0,0,0.45)` | the main panel floating over the gradient background |
| `shadow-dropdown` | `0 8px 24px rgba(0,0,0,0.35)` | menus, popovers |
| `shadow-card` | none | cards are separated by border only, not shadow — keep this flat |

## 6. Motion

- Standard transition: `150ms ease-out` for hover/active state (buttons, tabs, segmented control)
- Panel/dropdown reveal: `200ms ease-out`, combined with a slight opacity + 4px translate-y
- Always wrap in `@media (prefers-reduced-motion: reduce) { transition: none; }`

## 7. Icons

- Library: **Lucide** only
- Size: 18px in toolbars/tabs, 20px in sidebar rail, 16px inline in badges (rare)
- Stroke width: 1.5
- Default color: `text-tertiary`; on hover/active: `text-primary` or `accent-primary` depending on context

## 8. Avatars

- Size: 32px circle in page header stacks, 24px in compact contexts (table rows)
- Stack overlap: `-8px` margin-left on each subsequent avatar
- Each avatar gets a 2px border in `surface-base` (creates the cutout-ring look against neighboring avatars)
- Overflow badge (`+5` equivalent): same size as avatars, `bg-surface-raised`, `text-secondary`, `text-caption` weight 600
