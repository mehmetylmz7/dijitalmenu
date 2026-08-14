---
name: Culinary Intelligence
colors:
  surface: '#f8f9fa'
  surface-dim: '#d9dadb'
  surface-bright: '#f8f9fa'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f3f4f5'
  surface-container: '#edeeef'
  surface-container-high: '#e7e8e9'
  surface-container-highest: '#e1e3e4'
  on-surface: '#191c1d'
  on-surface-variant: '#3c4a42'
  inverse-surface: '#2e3132'
  inverse-on-surface: '#f0f1f2'
  outline: '#6c7a71'
  outline-variant: '#bbcabf'
  surface-tint: '#006c49'
  primary: '#006c49'
  on-primary: '#ffffff'
  primary-container: '#10b981'
  on-primary-container: '#00422b'
  inverse-primary: '#4edea3'
  secondary: '#575e70'
  on-secondary: '#ffffff'
  secondary-container: '#d9dff5'
  on-secondary-container: '#5c6274'
  tertiary: '#a43a3a'
  on-tertiary: '#ffffff'
  tertiary-container: '#fc7c78'
  on-tertiary-container: '#711419'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#6ffbbe'
  primary-fixed-dim: '#4edea3'
  on-primary-fixed: '#002113'
  on-primary-fixed-variant: '#005236'
  secondary-fixed: '#dce2f7'
  secondary-fixed-dim: '#c0c6db'
  on-secondary-fixed: '#141b2b'
  on-secondary-fixed-variant: '#404758'
  tertiary-fixed: '#ffdad7'
  tertiary-fixed-dim: '#ffb3af'
  on-tertiary-fixed: '#410005'
  on-tertiary-fixed-variant: '#842225'
  background: '#f8f9fa'
  on-background: '#191c1d'
  surface-variant: '#e1e3e4'
typography:
  display-lg:
    fontFamily: Sora
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Sora
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Sora
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  headline-md:
    fontFamily: Sora
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  label-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '600'
    lineHeight: 20px
    letterSpacing: 0.01em
  label-sm:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '500'
    lineHeight: 16px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 8px
  container-max: 1280px
  gutter: 24px
  margin-mobile: 16px
  margin-desktop: 40px
  stack-sm: 8px
  stack-md: 16px
  stack-lg: 32px
---

## Brand & Style
The design system is engineered for the intersection of high-growth technology and the high-stakes hospitality industry. The brand personality is **Professional, Fresh, and Systematic**, moving away from the chaotic nature of traditional kitchen software toward a calm, "Zen" operational experience.

The design style is **Modern Corporate with Tactile Softness**. It prioritizes extreme legibility and "scannability" for users who may be moving quickly in a restaurant environment. By utilizing a "Soft Minimalist" approach, the UI avoids visual noise, relying on generous whitespace and sophisticated roundedness to create an approachable yet authoritative presence.

## Colors
This design system uses a high-utility palette rooted in "Fresh Growth" and "Stable Authority."

- **Primary (#10B981):** Represents growth, freshness, and "go" states. Used for primary actions and success indicators.
- **Deep Charcoal (#111827):** Used for primary typography and dark-mode surfaces to provide maximum contrast and a premium feel.
- **Neutral Background (#F9FAFB):** An off-white base that reduces screen glare compared to pure white, essential for long-shift usage.
- **Subtle Gray (#E5E7EB):** Used exclusively for structural borders and dividers to maintain a clean, organized grid without heavy visual separation.

## Typography
The typographic hierarchy uses a dual-font strategy. **Sora** provides a modern, geometric character for headlines that feels tech-forward. **Inter** is utilized for all functional text to ensure industry-standard legibility and performance across all resolutions.

Headlines should use tight letter-spacing to appear more cohesive. Body text must maintain a generous line height (1.5x) to ensure comfort during prolonged reading of inventory lists or menu configurations.

## Layout & Spacing
The layout follows a **Fluid-Fixed Hybrid** model. Content is housed within a 12-column grid that scales fluidly up to a 1280px maximum width, after which it centers.

Spacing is based on an **8px base unit**. For SaaS dashboards, use the `stack-md` (16px) for internal card padding and `stack-lg` (32px) for section vertical spacing. Mobile views should collapse margins to 16px to maximize horizontal real estate for data tables and order lists.

## Elevation & Depth
The design system utilizes **Tonal Layering** combined with **Ambient Shadows**. 

- **Level 0 (Background):** #F9FAFB.
- **Level 1 (Cards/Surface):** Pure White (#FFFFFF) with a 1px border of #E5E7EB.
- **Level 2 (Hover/Active):** A soft, diffused shadow: `0px 4px 20px rgba(0, 0, 0, 0.05)`.
- **Level 3 (Modals/Overlays):** `0px 20px 48px rgba(17, 24, 39, 0.1)`.

Avoid high-contrast shadows or deep blacks. The goal is to make elements appear as if they are gently resting on the surface, not floating far above it.

## Shapes
Shapes are defined by the **Rounded-XL** philosophy. 

- **Buttons & Inputs:** Use `rounded-lg` (1rem / 16px) for a modern, friendly feel.
- **Cards & Containers:** Use `rounded-xl` (1.5rem / 24px) to create soft outer boundaries.
- **Chips & Tags:** Use fully rounded (Pill) shapes to distinguish them from interactive buttons.

This consistent radius creates a "friendly-tech" aesthetic that feels more modern than traditional sharp-edged enterprise software.

## Components
- **Buttons:** Primary buttons use the Emerald Green (#10B981) with white text. Secondary buttons use a white background with #E5E7EB borders and Deep Charcoal text. No gradients.
- **Input Fields:** 16px rounded corners with a subtle gray border. On focus, the border transitions to Primary Green with a soft 2px outer glow.
- **Cards:** White background, 24px rounded corners, and a 1px #E5E7EB border. No shadow by default; shadow appears only on hover for interactive cards.
- **Status Chips:** Use low-saturation backgrounds of the status color (e.g., light green background with dark green text) to ensure they don't compete with primary actions.
- **Data Tables:** Remove vertical borders. Use horizontal dividers only (#E5E7EB). Row hover state should be a subtle tint of the primary color at 5% opacity.
- **Navigation:** Vertical sidebars use the Deep Charcoal (#111827) for a high-contrast, professional workspace anchor.