/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./Pages/**/*.cshtml",
    "./wwwroot/**/*.html",
    "./wwwroot/js/**/*.js",
  ],
  theme: {
    extend: {
      colors: {
        // Page background gradient stops (used via arbitrary gradient in globals.css,
        // exposed here too in case a component needs to reference them directly)
        "bg-gradient-from": "#2F62E8",
        "bg-gradient-via": "#1D3F91",
        "bg-gradient-to": "#0B1220",

        // Surfaces
        surface: {
          base: "#12141C",
          sidebar: "#14161E",
          card: "#171922",
          raised: "#2A2D36",
          "raised-hover": "#34373F",
          active: "#4B4E57",
        },

        // Borders
        border: {
          subtle: "#23252E",
          DEFAULT: "#2C2F39",
          strong: "#3A3D47",
        },

        // Text
        text: {
          primary: "#F5F6F8",
          secondary: "#9599A6",
          tertiary: "#5B5E6B",
          disabled: "#3A3D47",
        },

        // Accent
        accent: {
          primary: "#3B82F6",
          "primary-hover": "#2563EB",
          success: "#22C55E",
          danger: "#EF4444",
        },

        // Tag / badge pairs — use tag-x for background (already includes opacity via /14 suffix
        // in markup, e.g. bg-tag-rose/14) and tag-x-text for the solid text tint
        tag: {
          rose: "#F43F5E",
          "rose-text": "#FB7185",
          amber: "#F59E0B",
          "amber-text": "#FBBF24",
          sky: "#38BDF8",
          "sky-text": "#38BDF8",
          violet: "#A78BFA",
          "violet-text": "#A78BFA",
          gold: "#EAB308",
          "gold-text": "#EAB308",
          emerald: "#34D399",
          "emerald-text": "#34D399",
          neutral: "#94A3B8",
          "neutral-text": "#94A3B8",
        },
      },
      fontFamily: {
        sans: ["Inter", "ui-sans-serif", "system-ui", "sans-serif"],
      },
      fontSize: {
        display: ["28px", { lineHeight: "34px", fontWeight: "600" }],
        "heading-md": ["20px", { lineHeight: "28px", fontWeight: "600" }],
        "heading-sm": ["15px", { lineHeight: "20px", fontWeight: "600" }],
        "body-md": ["14px", { lineHeight: "20px", fontWeight: "600" }],
        "body-sm": ["13px", { lineHeight: "18px", fontWeight: "400" }],
        caption: ["12px", { lineHeight: "16px", fontWeight: "600" }],
        "label-xs": ["11px", { lineHeight: "14px", fontWeight: "600", letterSpacing: "0.04em" }],
      },
      borderRadius: {
        sm: "8px",
        md: "12px",
        lg: "16px",
        xl: "24px",
      },
      boxShadow: {
        panel: "0 20px 60px rgba(0,0,0,0.45)",
        dropdown: "0 8px 24px rgba(0,0,0,0.35)",
      },
      spacing: {
        // Explicit named steps so the scale reads intentionally in class names
        // (Tailwind's default 4px-based scale already covers 4/8/12/16/20/24/32/40/48/64,
        // this block is just documentation — no overrides needed)
      },
      transitionDuration: {
        DEFAULT: "150ms",
      },
    },
  },
  plugins: [],
};
