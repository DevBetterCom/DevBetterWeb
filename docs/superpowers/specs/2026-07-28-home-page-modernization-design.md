# devBetter.com Home Page Modernization — Design Spec

**Date:** 2026-07-28
**Status:** Approved for planning (side-by-side page; existing home page untouched)

## Problem

The current home page ([src/DevBetterWeb.Web/Views/Home/Index.cshtml](../../../src/DevBetterWeb.Web/Views/Home/Index.cshtml)) is a 2019-era Start Bootstrap "Landing Page" theme that has been frozen since ~2023:

- **Conversion:** No CTA button anywhere above the fold. The only live CTA is an inline text link to `/checkout` buried in prose that still says "Click the button" (the buttons were removed in Aug 2025). No pricing cards, no anchor navigation, no sign-up link in the navbar.
- **SEO:** No meta description, no canonical tag, no structured data, no `lang` attribute, two conflicting viewport tags, one global hardcoded OpenGraph block, dead Universal Analytics (`UA-470225-35` — UA was shut down in 2023 and collects nothing).
- **Performance:** ~3.7 MB of hero background JPEGs (`bg.jpg` 1.59 MB, `expert.jpg` 2.12 MB), Bootstrap 4 + jQuery + Popper + DataTables + jQuery UI + Vimeo + LaunchPass JS loaded on the public page, no responsive images, no lazy loading.
- **Content:** Community/networking benefits (the strongest differentiator — daily Discord, alumni program, 150+ hours of archives, career-long peer network) are buried in one paragraph and the FAQ.

## Goal

A modern, fast, conversion-oriented public home page that clearly explains what devBetter is and leads with the community/networking benefits — deployed **side by side** with the current page at **`/index2.html`** so it can be viewed online without replacing anything.

## Non-Goals

- Changing `/` (the current home page), `_Layout.cshtml`, or any Razor view.
- New checkout flow — CTAs link to the existing `/checkout` page.
- Site-wide SEO fixes (sitemap.xml, GA4, layout meta tags) — listed as follow-ups below.
- A/B testing infrastructure or analytics wiring (no GA4 property exists yet).

## Approaches Considered

1. **Self-contained static page at `wwwroot/index2.html`** — zero-dependency HTML with embedded modern CSS, inline SVG graphics, tiny vanilla JS. **← Chosen.** Verified: `UseStaticFiles()` is enabled and no route matches `/index2.html`, so the file serves verbatim with zero code changes. Zero risk to the live site; trivially promotable later (its body becomes the new `Index.cshtml`, or `UseDefaultFiles` could be considered).
2. New Razor action (e.g. `/home2`) sharing `_Layout` — rejected: the layout drags in the entire Bootstrap 4/jQuery stack and the dated chrome we're replacing, and the user explicitly asked for `/index2.html`.
3. Rebuild in place — rejected: user explicitly wants side-by-side deployment.

## Design

### Technology

- One file: `src/DevBetterWeb.Web/wwwroot/index2.html`. Embedded `<style>` (no external CSS request, no render-blocking), ~10 lines of inline JS for the mobile nav toggle. No Bootstrap, no jQuery, no web fonts (system font stack), no CDN requests.
- Modern CSS: custom properties, CSS grid/flexbox, `clamp()` fluid type, `prefers-reduced-motion` respected.

### Graphics (replacing the dated photos)

- Hero backgrounds (`bg.jpg`, `expert.jpg`, showcase JPEGs) are **not reused**. Replaced by a navy gradient hero with a subtle inline-SVG "network mesh" motif (nodes + connecting lines — visual metaphor for the community/networking theme). Feature cards get simple inline SVG stroke icons.
- **Testimonial headshots are kept** (real faces convert; they're small files) — reused from `images/people/`, with explicit `width`/`height` and `loading="lazy"`.
- Existing logo `images/logo_273x60.png` reused in the nav/footer.
- Net effect: page weight drops from ~5+ MB to well under 300 KB.

### Page structure (in order)

1. **Sticky nav** — logo; anchor links (Benefits, Community, Testimonials, Pricing) + FAQ + Login; prominent **Join devBetter** button → `/checkout?src=index2` (query string makes new-page conversions distinguishable in server logs).
2. **Hero** — badge "Live group coaching for software developers · Est. 2018"; H1 "Accelerate your software career — with an expert coach and a community in your corner"; subhead naming Steve "ardalis" Smith, weekly Zoom sessions, daily Discord; primary CTA **Join devBetter** + secondary "See member results" (anchor to testimonials); stats strip: Est. 2018 · Weekly live sessions · 150+ hours of recorded coaching · Alumni go free after 24 months.
3. **Empathy strip** — condensed version of the current pain-point copy (3 short items instead of 3 screens of prose).
4. **Benefits grid** (`#benefits`) — 6 cards: weekly live coaching; daily Discord community; code reviews & katas (private GitHub org); 150+ hour session archive with searchable notes; career & business coaching (negotiation, resumes, going independent); network & promotion (members/alumni promote your content, referrals).
5. **Community spotlight** (`#community`, dark section — the emphasized networking message) — "The real benefit? You stop doing it alone." Daily Discord collaboration, pair programming, accountability, the alumni program (free after 24 months, so the network compounds), two networking-focused pull quotes (Phil V., Pierre Gadea).
6. **How it works** — 3 steps: Join in minutes → Show up weekly (Zoom + Discord) → Level up (personal goals, feedback, progress reports).
7. **Testimonials** (`#testimonials`) — 6 curated with headshots: Eric F. ("doubling my salary"), Ryan Wemmer (job offer via devBetter on resume), Pierre Gadea (negotiation paid for membership), Phil V., Chris H., Shmuel Winegarten. Verbatim excerpts from the current page — no fabricated quotes.
8. **Pricing** (`#pricing`) — two cards: **$200/mo** (cancel anytime) and **$2,000/yr** ("2 months free", tagged Best value); both list full inclusions and link to `/checkout?src=index2`; alumni note beneath.
9. **Final CTA + FAQ pointer**, then footer (FAQ, Code of Conduct, Login, GitHub, © devBetter).

### SEO (page-level)

- `<html lang="en">`, single viewport tag.
- Title: `devBetter: Coaching & Community for Software Developers` (56 chars).
- Meta description (~155 chars) leading with coaching + community + since-2018 proof.
- Per-page OpenGraph + Twitter card tags (reusing `devBetter_Box_864x488.png` as the share image until a new one exists).
- JSON-LD: `Organization` (devBetter, founder Steve Smith) and `Product` ("devBetter Membership") with two `Offer`s (200 USD/month, 2000 USD/year). **No** self-serving `aggregateRating` (against Google guidelines).
- **While side-by-side:** `<link rel="canonical" href="https://devbetter.com/">` so the trial page never competes with `/` for rankings. Not added to any sitemap. **On promotion:** canonical flips to self (see checklist below).
- Semantic HTML5 landmarks, one `<h1>`, meaningful alt text, AA contrast.

### Performance budget

- ≤ 300 KB transferred (headshots + logo are the only binary assets, all lazy except logo).
- Zero third-party requests. Lighthouse targets: Performance ≥ 95, SEO ≥ 95, Accessibility ≥ 95.

### Testing

- Functional test (xUnit, existing `CustomWebApplicationFactory<Program>` pattern from `HomePageGet.cs`): GET `/index2.html` → 200 + key content assertions per section. TDD: tests written before each section exists.
- Existing `/` tests are unaffected (page untouched).

### Promotion checklist (future, out of scope now)

1. Canonical on index2 flips from `https://devbetter.com/` to self (or content is ported into `Index.cshtml`).
2. Add page to sitemap; add `Sitemap:` line to robots.txt.
3. Redirect or retire the old content; keep Phil V.'s quote (functional test `HomePageGet` asserts it on `/`).

### Follow-ups (recorded, not in this plan)

- Replace dead UA analytics with GA4 (needs a property ID from Steve) + conversion event on checkout clicks.
- sitemap.xml + robots.txt `Sitemap:` directive.
- Fix `_Layout.cshtml`: duplicate viewport tags, missing `lang`, missing meta description, per-page OG tags.
- `/checkout` page emits a nested `<html>` document inside the layout (invalid markup) — worth fixing since it's the conversion target.
- Delete unused `index_NOT_USED.html`, template leftovers in `wwwroot/img/`, stale SCSS.
