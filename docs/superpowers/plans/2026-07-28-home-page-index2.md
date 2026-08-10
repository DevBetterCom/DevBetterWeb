# Modern Home Page at /index2.html — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ship a modern, SEO- and conversion-optimized static home page at `/index2.html`, side by side with the untouched current home page at `/`.

**Architecture:** One self-contained static file, `src/DevBetterWeb.Web/wwwroot/index2.html` — served automatically by the existing `UseStaticFiles()` middleware (verified: no route matches `/index2.html`). Embedded CSS, inline SVG graphics, ~10 lines of vanilla JS. Reuses existing testimonial headshots and logo from `wwwroot/images/`; the multi-MB hero JPEGs are replaced by a CSS gradient + SVG mesh. One xUnit functional test class grows with each task.

**Tech Stack:** Plain HTML5/CSS3 (custom properties, grid, `clamp()`), vanilla JS, xUnit + `CustomWebApplicationFactory<Program>` (existing pattern in `tests/DevBetterWeb.FunctionalTests`).

**Design spec:** `docs/superpowers/specs/2026-07-28-home-page-modernization-design.md` — read it first for the rationale behind every decision below.

## Global Constraints

- **Do NOT touch** `Views/Home/Index.cshtml`, `_Layout.cshtml`, `Program.cs`, or any existing page/test. This feature is purely additive: one new HTML file + one new test file.
- All CTAs link to `/checkout?src=index2` (the query string distinguishes new-page conversions in server logs). Pricing copy is exactly **$200/mo** and **$2,000/yr (2 months free)** — matches the live Stripe links on `/checkout`.
- Testimonial quotes must be **verbatim excerpts** from the current `Index.cshtml` (full sentences may be dropped, but never reworded). Never invent quotes, member counts, or statistics. Approved facts (from `/FAQ` and `Index.cshtml`): est. 2018, weekly live Zoom sessions, 150+ hours of recorded sessions with searchable notes, private Discord, private GitHub org with katas, private Stack Overflow Teams, guest speakers, alumni go free after 24 months, cancel anytime.
- The page must make **zero external requests**: no CDNs, no web fonts, no analytics. Only same-origin images (`/images/...`).
- Canonical stays `https://devbetter.com/` while side-by-side (this page must not compete with `/` in search). Do not add it to robots.txt or any sitemap.
- Per `CLAUDE.md`: before every commit, run the Codex review via `/codex:review` and fix or explicitly rebut each finding. If the plugin is unavailable in your session, state that in the commit message process notes and proceed.
- Build/test from repo root on Windows; commands below use `dotnet` CLI (PowerShell-safe).
- Every commit message ends with: `Co-Authored-By: Claude Fable 5 <noreply@anthropic.com>`

## File Structure

- Create: `src/DevBetterWeb.Web/wwwroot/index2.html` — the page (grows across Tasks 1–4).
- Create: `tests/DevBetterWeb.FunctionalTests/StaticPages/Index2PageGet.cs` — functional tests (grows across Tasks 1–4).
- No other files change.

---

### Task 1: Functional test + page shell (head/SEO, styles, nav, hero, footer)

**Files:**
- Create: `tests/DevBetterWeb.FunctionalTests/StaticPages/Index2PageGet.cs`
- Create: `src/DevBetterWeb.Web/wwwroot/index2.html`

**Interfaces:**
- Consumes: existing `CustomWebApplicationFactory<Program>` (namespace `DevBetterWeb.FunctionalTests`), existing assets `/images/logo_273x60.png`.
- Produces: `index2.html` containing the literal marker comment `<!-- SECTIONS:END -->` inside `<main>`. Tasks 2–3 insert their sections **immediately before** that comment, in task order. The full stylesheet for ALL tasks ships here — later tasks add HTML only.

- [ ] **Step 1: Write the failing test**

Create `tests/DevBetterWeb.FunctionalTests/StaticPages/Index2PageGet.cs`:

```csharp
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DevBetterWeb.FunctionalTests.StaticPages;

public class Index2PageGet : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public Index2PageGet(CustomWebApplicationFactory<Program> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ReturnsPageWithHeroAndPrimaryCta()
  {
    HttpResponseMessage response = await _client.GetAsync("/index2.html");
    response.EnsureSuccessStatusCode();
    string stringResponse = await response.Content.ReadAsStringAsync();

    Assert.Contains("Accelerate your software career", stringResponse);
    Assert.Contains("/checkout?src=index2", stringResponse);
    Assert.Contains("rel=\"canonical\" href=\"https://devbetter.com/\"", stringResponse);
  }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DevBetterWeb.FunctionalTests --filter "FullyQualifiedName~Index2PageGet"`
Expected: FAIL — `EnsureSuccessStatusCode` throws (404, file doesn't exist yet).

- [ ] **Step 3: Create the page shell**

Create `src/DevBetterWeb.Web/wwwroot/index2.html` with exactly this content (the `<style>` block is the complete stylesheet for the whole page, including classes used by Tasks 2–3):

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>devBetter: Coaching &amp; Community for Software Developers</title>
  <meta name="description" content="Level up your software career with weekly live group coaching from Steve 'ardalis' Smith, a daily-active Discord community, code reviews, and a professional network. Since 2018." />
  <link rel="canonical" href="https://devbetter.com/" />
  <meta name="theme-color" content="#0e1726" />
  <link rel="icon" type="image/png" sizes="200x200" href="/images/icon_200x200.png" />
  <meta property="og:type" content="website" />
  <meta property="og:site_name" content="devBetter" />
  <meta property="og:url" content="https://devbetter.com/index2.html" />
  <meta property="og:title" content="devBetter: Coaching &amp; Community for Software Developers" />
  <meta property="og:description" content="Weekly live group coaching, a daily-active Discord community, code reviews, and a professional network for software developers. Since 2018." />
  <meta property="og:image" content="https://devbetter.com/images/devBetter_Box_864x488.png" />
  <meta name="twitter:card" content="summary_large_image" />
  <meta name="twitter:site" content="@ardalis" />
  <meta name="twitter:title" content="devBetter: Coaching &amp; Community for Software Developers" />
  <meta name="twitter:description" content="Weekly live group coaching, a daily-active Discord community, code reviews, and a professional network for software developers. Since 2018." />
  <meta name="twitter:image" content="https://devbetter.com/images/devBetter_Box_864x488.png" />
  <style>
    :root {
      --ink: #0e1726;
      --ink-2: #16233a;
      --paper: #ffffff;
      --paper-2: #f4f6fb;
      --muted: #5b6472;
      --line: #e3e8f0;
      --accent: #2f6df6;
      --accent-2: #7c3aed;
      --on-dark-muted: #aab6cc;
      --radius: 14px;
      --shadow: 0 10px 30px rgba(14, 23, 38, .08);
    }
    * { box-sizing: border-box; margin: 0; padding: 0; }
    html { scroll-behavior: smooth; }
    @media (prefers-reduced-motion: reduce) {
      html { scroll-behavior: auto; }
      *, *::before, *::after { transition: none !important; animation: none !important; }
    }
    body {
      font-family: "Segoe UI", system-ui, -apple-system, Roboto, "Helvetica Neue", Arial, sans-serif;
      color: var(--ink);
      background: var(--paper);
      line-height: 1.6;
      -webkit-font-smoothing: antialiased;
    }
    img { max-width: 100%; display: block; }
    a { color: var(--accent); }
    .container { width: min(1120px, 92%); margin-inline: auto; }
    h1, h2, h3 { line-height: 1.15; letter-spacing: -0.02em; }
    h1 { font-size: clamp(2.1rem, 4.5vw + 1rem, 3.6rem); }
    h2 { font-size: clamp(1.6rem, 2.2vw + 1rem, 2.4rem); }
    .kicker {
      display: inline-block; font-size: .8rem; font-weight: 700; letter-spacing: .12em;
      text-transform: uppercase; color: var(--accent); margin-bottom: .75rem;
    }
    .section { padding: clamp(3.5rem, 8vw, 6rem) 0; }
    .section-head { max-width: 46rem; margin: 0 auto 2.5rem; text-align: center; }
    .section-head p { color: var(--muted); margin-top: .75rem; }
    .btn {
      display: inline-block; padding: .8rem 1.6rem; border-radius: 999px;
      font-weight: 600; text-decoration: none; border: 1px solid transparent;
      transition: transform .15s ease, box-shadow .15s ease;
    }
    .btn:hover { transform: translateY(-1px); }
    .btn:focus-visible { outline: 3px solid var(--accent-2); outline-offset: 2px; }
    .btn-primary {
      background: linear-gradient(135deg, var(--accent), var(--accent-2));
      color: #fff; box-shadow: 0 8px 20px rgba(47, 109, 246, .35);
    }
    .btn-ghost { color: #fff; border-color: rgba(255, 255, 255, .4); }
    .btn-ghost:hover { border-color: #fff; }
    /* Nav */
    .nav {
      position: sticky; top: 0; z-index: 50; background: rgba(14, 23, 38, .92);
      backdrop-filter: blur(8px); border-bottom: 1px solid rgba(255, 255, 255, .08);
    }
    .nav-inner { display: flex; align-items: center; gap: 1.25rem; padding: .7rem 0; }
    .nav-logo img { height: 36px; width: auto; }
    .nav-links { display: flex; align-items: center; gap: 1.25rem; margin-left: auto; list-style: none; }
    .nav-links a { color: #dde5f1; text-decoration: none; font-weight: 500; font-size: .95rem; }
    .nav-links a:hover { color: #fff; }
    .nav-links .btn { padding: .55rem 1.2rem; }
    .nav-toggle {
      display: none; margin-left: auto; background: none; border: 1px solid rgba(255,255,255,.3);
      border-radius: 8px; color: #fff; font-size: 1.4rem; line-height: 1; padding: .35rem .6rem; cursor: pointer;
    }
    @media (max-width: 820px) {
      .nav-toggle { display: block; }
      .nav-links {
        display: none; position: absolute; top: 100%; left: 0; right: 0;
        flex-direction: column; align-items: stretch; text-align: center; gap: 0;
        background: var(--ink); border-bottom: 1px solid rgba(255,255,255,.1); padding: .5rem 0 1rem;
      }
      .nav-links.open { display: flex; }
      .nav-links li { padding: .55rem 0; }
    }
    /* Hero */
    .hero {
      position: relative; overflow: hidden; color: #fff; text-align: center;
      background: radial-gradient(1200px 600px at 70% -10%, #24406e 0%, var(--ink) 55%) var(--ink);
      padding: clamp(4rem, 10vw, 7.5rem) 0 clamp(3.5rem, 8vw, 6rem);
    }
    .hero-mesh { position: absolute; inset: 0; width: 100%; height: 100%; opacity: .35; pointer-events: none; }
    .hero .container { position: relative; max-width: 54rem; }
    .hero-badge {
      display: inline-block; font-size: .85rem; font-weight: 600; color: #cdd9ee;
      border: 1px solid rgba(255, 255, 255, .25); border-radius: 999px;
      padding: .35rem 1rem; margin-bottom: 1.5rem; background: rgba(255, 255, 255, .06);
    }
    .hero p.lead { color: var(--on-dark-muted); font-size: clamp(1.05rem, 1vw + .8rem, 1.25rem); margin: 1.25rem auto 2rem; max-width: 44rem; }
    .hero-ctas { display: flex; gap: 1rem; justify-content: center; flex-wrap: wrap; }
    .hero-stats {
      display: flex; flex-wrap: wrap; justify-content: center; gap: .75rem 2.5rem;
      margin-top: 3rem; padding-top: 1.75rem; border-top: 1px solid rgba(255, 255, 255, .15);
      color: #cdd9ee; font-size: .95rem; list-style: none;
    }
    .hero-stats strong { color: #fff; }
    /* Empathy strip (Task 2) */
    .empathy { background: var(--paper-2); }
    .empathy-grid { display: grid; gap: 1.5rem; grid-template-columns: repeat(auto-fit, minmax(260px, 1fr)); }
    .empathy-grid p { color: var(--muted); }
    .empathy-grid h3 { font-size: 1.1rem; margin-bottom: .4rem; }
    /* Benefits cards (Task 2) */
    .cards { display: grid; gap: 1.5rem; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); }
    .card {
      background: var(--paper); border: 1px solid var(--line); border-radius: var(--radius);
      padding: 1.75rem; box-shadow: var(--shadow);
    }
    .card-icon {
      display: inline-flex; align-items: center; justify-content: center; width: 48px; height: 48px;
      border-radius: 12px; color: var(--accent); background: rgba(47, 109, 246, .1); margin-bottom: 1rem;
    }
    .card h3 { font-size: 1.15rem; margin-bottom: .5rem; }
    .card p { color: var(--muted); font-size: .97rem; }
    /* Community spotlight (Task 2) */
    .community { background: linear-gradient(160deg, var(--ink) 0%, var(--ink-2) 100%); color: #fff; }
    .community .section-head p { color: var(--on-dark-muted); }
    .community-grid { display: grid; gap: 2rem; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); align-items: start; }
    .community-list { list-style: none; display: grid; gap: 1rem; }
    .community-list li { padding-left: 1.75rem; position: relative; color: #dde5f1; }
    .community-list li::before { content: "✓"; position: absolute; left: 0; color: var(--accent); font-weight: 700; }
    .pull-quote {
      background: rgba(255, 255, 255, .06); border: 1px solid rgba(255, 255, 255, .12);
      border-radius: var(--radius); padding: 1.5rem; margin-bottom: 1.25rem;
    }
    .pull-quote p { color: #dde5f1; font-style: italic; }
    .pull-quote footer { margin-top: .75rem; color: var(--on-dark-muted); font-size: .9rem; }
    /* Steps (Task 3) */
    .steps { display: grid; gap: 1.5rem; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); counter-reset: step; }
    .step { text-align: center; padding: 1.5rem; }
    .step-num {
      display: inline-flex; align-items: center; justify-content: center; width: 44px; height: 44px;
      border-radius: 50%; font-weight: 700; color: #fff; margin-bottom: 1rem;
      background: linear-gradient(135deg, var(--accent), var(--accent-2));
    }
    .step p { color: var(--muted); }
    /* Testimonials (Task 3) */
    .testimonials { background: var(--paper-2); }
    .t-grid { display: grid; gap: 1.5rem; grid-template-columns: repeat(auto-fit, minmax(320px, 1fr)); }
    .t-card { background: var(--paper); border: 1px solid var(--line); border-radius: var(--radius); padding: 1.75rem; box-shadow: var(--shadow); }
    .t-card blockquote p { color: var(--ink-2); font-size: .97rem; }
    .t-card blockquote p::before { content: "\201C"; }
    .t-card blockquote p::after { content: "\201D"; }
    .t-who { display: flex; align-items: center; gap: .85rem; margin-top: 1.25rem; }
    .t-who img { width: 48px; height: 48px; border-radius: 50%; object-fit: cover; }
    .t-who strong { display: block; font-size: .95rem; }
    /* Pricing (Task 3) */
    .pricing-grid { display: grid; gap: 1.5rem; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); max-width: 52rem; margin-inline: auto; }
    .price-card {
      position: relative; background: var(--paper); border: 1px solid var(--line);
      border-radius: var(--radius); padding: 2rem; box-shadow: var(--shadow); text-align: center;
    }
    .price-card.featured { border: 2px solid var(--accent); }
    .price-tag {
      position: absolute; top: -0.85rem; left: 50%; transform: translateX(-50%);
      background: linear-gradient(135deg, var(--accent), var(--accent-2)); color: #fff;
      font-size: .75rem; font-weight: 700; letter-spacing: .06em; text-transform: uppercase;
      border-radius: 999px; padding: .25rem .9rem; white-space: nowrap;
    }
    .price { font-size: 2.6rem; font-weight: 800; letter-spacing: -0.03em; }
    .price small { font-size: 1rem; font-weight: 500; color: var(--muted); }
    .price-card ul { list-style: none; margin: 1.25rem 0 1.75rem; display: grid; gap: .6rem; text-align: left; }
    .price-card ul li { padding-left: 1.6rem; position: relative; color: var(--muted); font-size: .95rem; }
    .price-card ul li::before { content: "✓"; position: absolute; left: 0; color: var(--accent); font-weight: 700; }
    .pricing-note { text-align: center; color: var(--muted); font-size: .95rem; max-width: 40rem; margin: 2rem auto 0; }
    /* Final CTA (Task 3) */
    .final-cta { background: linear-gradient(160deg, var(--ink) 0%, var(--ink-2) 100%); color: #fff; text-align: center; }
    .final-cta p { color: var(--on-dark-muted); max-width: 40rem; margin: 1rem auto 2rem; }
    /* Footer */
    .footer { background: var(--ink); color: var(--on-dark-muted); padding: 2.5rem 0; font-size: .9rem; }
    .footer-inner { display: flex; flex-wrap: wrap; gap: 1rem 2rem; align-items: center; justify-content: space-between; }
    .footer a { color: #cdd9ee; text-decoration: none; margin-right: 1.25rem; }
    .footer a:hover { color: #fff; }
  </style>
</head>
<body>
  <nav class="nav" aria-label="Main">
    <div class="container nav-inner">
      <a class="nav-logo" href="/index2.html" aria-label="devBetter home">
        <img src="/images/logo_273x60.png" alt="devBetter" width="164" height="36" />
      </a>
      <button class="nav-toggle" type="button" aria-expanded="false" aria-controls="nav-links" aria-label="Toggle menu">&#9776;</button>
      <ul class="nav-links" id="nav-links">
        <li><a href="#benefits">Benefits</a></li>
        <li><a href="#community">Community</a></li>
        <li><a href="#testimonials">Results</a></li>
        <li><a href="#pricing">Pricing</a></li>
        <li><a href="/FAQ">FAQ</a></li>
        <li><a href="/Identity/Account/Login">Log in</a></li>
        <li><a class="btn btn-primary" href="/checkout?src=index2">Join devBetter</a></li>
      </ul>
    </div>
  </nav>

  <main>
    <header class="hero">
      <svg class="hero-mesh" viewBox="0 0 1200 600" preserveAspectRatio="xMidYMid slice" aria-hidden="true" focusable="false">
        <g stroke="#4d6ea8" stroke-width="1" fill="none">
          <path d="M120 140 L340 90 L560 180 L790 110 L1030 170" />
          <path d="M200 420 L340 90 M200 420 L560 180 M560 180 L680 470 M790 110 L680 470 M1030 170 L900 430 M680 470 L900 430 M120 140 L200 420" />
        </g>
        <g fill="#7ea2e0">
          <circle cx="120" cy="140" r="6" /><circle cx="340" cy="90" r="8" />
          <circle cx="560" cy="180" r="6" /><circle cx="790" cy="110" r="7" />
          <circle cx="1030" cy="170" r="6" /><circle cx="200" cy="420" r="7" />
          <circle cx="680" cy="470" r="8" /><circle cx="900" cy="430" r="6" />
        </g>
      </svg>
      <div class="container">
        <span class="hero-badge">Live group coaching for software developers &middot; Est. 2018</span>
        <h1>Accelerate your software career &mdash; with an expert coach and a community in your corner.</h1>
        <p class="lead">
          devBetter is a live group coaching program led by Steve &ldquo;ardalis&rdquo; Smith.
          Weekly video sessions, a private Discord that&rsquo;s active every day, code reviews,
          and a network of professional developers invested in your growth.
        </p>
        <div class="hero-ctas">
          <a class="btn btn-primary" href="/checkout?src=index2">Join devBetter</a>
          <a class="btn btn-ghost" href="#testimonials">See member results</a>
        </div>
        <ul class="hero-stats">
          <li><strong>Est. 2018</strong></li>
          <li><strong>Weekly</strong> live sessions</li>
          <li><strong>150+ hours</strong> of recorded coaching</li>
          <li><strong>Alumni go free</strong> after 24 months</li>
        </ul>
      </div>
    </header>

    <!-- SECTIONS:END -->
  </main>

  <footer class="footer">
    <div class="container footer-inner">
      <img src="/images/logo_273x60.png" alt="devBetter" width="137" height="30" loading="lazy" />
      <nav aria-label="Footer">
        <a href="/FAQ">FAQ</a>
        <a href="/CodeOfConduct">Code of Conduct</a>
        <a href="/Identity/Account/Login">Log in</a>
        <a href="https://github.com/DevBetterCom/DevBetterWeb">GitHub</a>
      </nav>
      <span>&copy; devBetter. All rights reserved.</span>
    </div>
  </footer>

  <script>
    (function () {
      var toggle = document.querySelector(".nav-toggle");
      var links = document.getElementById("nav-links");
      toggle.addEventListener("click", function () {
        var open = links.classList.toggle("open");
        toggle.setAttribute("aria-expanded", open ? "true" : "false");
      });
      links.addEventListener("click", function (e) {
        if (e.target.tagName === "A") {
          links.classList.remove("open");
          toggle.setAttribute("aria-expanded", "false");
        }
      });
    })();
  </script>
</body>
</html>
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/DevBetterWeb.FunctionalTests --filter "FullyQualifiedName~Index2PageGet"`
Expected: PASS (1 test).

- [ ] **Step 5: Codex review, then commit**

Run `/codex:review`; address or rebut findings. Then:

```bash
git add src/DevBetterWeb.Web/wwwroot/index2.html tests/DevBetterWeb.FunctionalTests/StaticPages/Index2PageGet.cs
git commit -m "feat: add modern static home page shell at /index2.html

Co-Authored-By: Claude Fable 5 <noreply@anthropic.com>"
```

---

### Task 2: Empathy strip, benefits grid, community spotlight

**Files:**
- Modify: `src/DevBetterWeb.Web/wwwroot/index2.html` (insert before `<!-- SECTIONS:END -->`)
- Modify: `tests/DevBetterWeb.FunctionalTests/StaticPages/Index2PageGet.cs` (add one test)

**Interfaces:**
- Consumes: `<!-- SECTIONS:END -->` marker and all CSS classes from Task 1 (`.empathy*`, `.cards`, `.card*`, `.community*`, `.pull-quote`, `.section`, `.section-head`, `.kicker`).
- Produces: sections with ids `#benefits` and `#community` (nav anchors from Task 1 now resolve).

- [ ] **Step 1: Add the failing test**

Add to `Index2PageGet` (same class, new fact):

```csharp
  [Fact]
  public async Task ReturnsPageWithBenefitsAndCommunitySections()
  {
    HttpResponseMessage response = await _client.GetAsync("/index2.html");
    response.EnsureSuccessStatusCode();
    string stringResponse = await response.Content.ReadAsStringAsync();

    Assert.Contains("id=\"benefits\"", stringResponse);
    Assert.Contains("Weekly live coaching", stringResponse);
    Assert.Contains("You stop doing it alone", stringResponse);
  }
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DevBetterWeb.FunctionalTests --filter "FullyQualifiedName~Index2PageGet"`
Expected: 1 PASS (Task 1), 1 FAIL — `id="benefits"` not found.

- [ ] **Step 3: Insert the three sections**

Insert immediately **before** `<!-- SECTIONS:END -->`:

```html
    <section class="section empathy" aria-label="The problem">
      <div class="container">
        <div class="section-head">
          <span class="kicker">Sound familiar?</span>
          <h2>Doing what you&rsquo;ve been doing hasn&rsquo;t gotten you where you want to be.</h2>
        </div>
        <div class="empathy-grid">
          <div>
            <h3>You&rsquo;re overwhelmed</h3>
            <p>There are a hundred things you could be learning, and no clear signal on which one actually moves your career forward.</p>
          </div>
          <div>
            <h3>You&rsquo;re on your own</h3>
            <p>Podcasts, articles, and courses are one-directional. Nobody is holding you accountable or tailoring advice to your situation.</p>
          </div>
          <div>
            <h3>Coaching feels out of reach</h3>
            <p>A dedicated one-on-one career coach would help &mdash; but the time and money required makes it a non-starter.</p>
          </div>
        </div>
      </div>
    </section>

    <section class="section" id="benefits" aria-label="What you get">
      <div class="container">
        <div class="section-head">
          <span class="kicker">What you get</span>
          <h2>Everything a career coach offers &mdash; plus a community that has your back.</h2>
        </div>
        <div class="cards">
          <div class="card">
            <span class="card-icon" aria-hidden="true">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h12v12H3z" /><path d="M15 10l6-3v10l-6-3" /></svg>
            </span>
            <h3>Weekly live coaching</h3>
            <p>Open Q&amp;A over Zoom with Steve and occasional guest speakers. Ask live, send questions in advance, or just listen and learn.</p>
          </div>
          <div class="card">
            <span class="card-icon" aria-hidden="true">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M4 5h16v11H8l-4 4z" /></svg>
            </span>
            <h3>A daily-active Discord</h3>
            <p>The private community Discord is where members collaborate every day: breakout channels, pair programming, and career threads.</p>
          </div>
          <div class="card">
            <span class="card-icon" aria-hidden="true">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="M9 6L4 12l5 6" /><path d="M15 6l5 6-5 6" /></svg>
            </span>
            <h3>Code reviews &amp; katas</h3>
            <p>A private GitHub organization with coding exercises and katas &mdash; get real feedback on real code from Steve and the group.</p>
          </div>
          <div class="card">
            <span class="card-icon" aria-hidden="true">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="9" /><path d="M10 8l6 4-6 4z" /></svg>
            </span>
            <h3>150+ hours of archives</h3>
            <p>Every coaching session since 2018, recorded with searchable notes. Your first week includes years of answers.</p>
          </div>
          <div class="card">
            <span class="card-icon" aria-hidden="true">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="8" width="18" height="11" rx="2" /><path d="M9 8V6a3 3 0 0 1 6 0v2" /></svg>
            </span>
            <h3>Career &amp; business coaching</h3>
            <p>Resumes, job hunting, negotiation, compensation, going independent, marketing, and personal branding &mdash; not just tech skills.</p>
          </div>
          <div class="card">
            <span class="card-icon" aria-hidden="true">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><circle cx="8" cy="9" r="3" /><circle cx="16.5" cy="10" r="2.5" /><path d="M2.5 19c0-3 2.5-5 5.5-5s5.5 2 5.5 5" /><path d="M14.5 14.5c2.5 0 5 1.5 5 4.5" /></svg>
            </span>
            <h3>Your network, upgraded</h3>
            <p>Members and alumni promote your content, share referrals, and become the professional peers you&rsquo;ll lean on for years.</p>
          </div>
        </div>
      </div>
    </section>

    <section class="section community" id="community" aria-label="Community">
      <div class="container">
        <div class="section-head">
          <span class="kicker">The devBetter difference</span>
          <h2>The real benefit? You stop doing it alone.</h2>
          <p>Advice is everywhere. A group of motivated professionals who know your goals, review your code, and cheer your wins is not.</p>
        </div>
        <div class="community-grid">
          <ul class="community-list">
            <li><strong>Accountability that compounds.</strong> Personalized goals between sessions, and a group that expects your progress report.</li>
            <li><strong>A network with real range.</strong> Juniors to architects, employees to founders &mdash; members answer each other&rsquo;s questions daily.</li>
            <li><strong>Alumni stay in the room.</strong> After 24 months, members graduate to free alumni membership &mdash; so the network keeps growing, and experienced voices stick around.</li>
            <li><strong>Visibility for your work.</strong> Steve and the members promote your content and celebrate your milestones.</li>
          </ul>
          <div>
            <figure class="pull-quote">
              <blockquote>
                <p>Through devBetter, I was introduced to a great group of individuals whom, although they differ in professional experience and background, all seek to better their skills and help each other do the same.</p>
              </blockquote>
              <footer>Phil V. &mdash; devBetter member</footer>
            </figure>
            <figure class="pull-quote">
              <blockquote>
                <p>One of the first things Steve did was help me negotiate my salary, which has more than paid for the entire cost of my membership already.</p>
              </blockquote>
              <footer>Pierre Gadea &mdash; devBetter member</footer>
            </figure>
          </div>
        </div>
      </div>
    </section>

```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/DevBetterWeb.FunctionalTests --filter "FullyQualifiedName~Index2PageGet"`
Expected: 2 PASS.

- [ ] **Step 5: Codex review, then commit**

Run `/codex:review`; address or rebut findings. Then:

```bash
git add src/DevBetterWeb.Web/wwwroot/index2.html tests/DevBetterWeb.FunctionalTests/StaticPages/Index2PageGet.cs
git commit -m "feat: add benefits and community sections to index2 page

Co-Authored-By: Claude Fable 5 <noreply@anthropic.com>"
```

---

### Task 3: How it works, testimonials, pricing, final CTA

**Files:**
- Modify: `src/DevBetterWeb.Web/wwwroot/index2.html` (insert before `<!-- SECTIONS:END -->`, i.e. after Task 2's community section)
- Modify: `tests/DevBetterWeb.FunctionalTests/StaticPages/Index2PageGet.cs` (add one test)

**Interfaces:**
- Consumes: `<!-- SECTIONS:END -->` marker; CSS classes `.steps`, `.step*`, `.testimonials`, `.t-grid`, `.t-card`, `.t-who`, `.pricing-grid`, `.price-card*`, `.price*`, `.pricing-note`, `.final-cta` from Task 1; headshot files in `src/DevBetterWeb.Web/wwwroot/images/people/` (exact filenames used below — verify they exist before writing HTML).
- Produces: sections `#testimonials` and `#pricing` (completes all Task 1 nav anchors).

- [ ] **Step 1: Add the failing test**

Add to `Index2PageGet`:

```csharp
  [Fact]
  public async Task ReturnsPageWithTestimonialsAndPricing()
  {
    HttpResponseMessage response = await _client.GetAsync("/index2.html");
    response.EnsureSuccessStatusCode();
    string stringResponse = await response.Content.ReadAsStringAsync();

    Assert.Contains("doubling my salary", stringResponse);
    Assert.Contains("$2,000", stringResponse);
    Assert.Contains("2 months free", stringResponse);
  }
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DevBetterWeb.FunctionalTests --filter "FullyQualifiedName~Index2PageGet"`
Expected: 2 PASS, 1 FAIL — `doubling my salary` not found.

- [ ] **Step 3: Insert the four sections**

Testimonial quotes below are verbatim excerpts from `Views/Home/Index.cshtml` — do not reword them. Insert immediately **before** `<!-- SECTIONS:END -->`:

```html
    <section class="section" aria-label="How it works">
      <div class="container">
        <div class="section-head">
          <span class="kicker">How it works</span>
          <h2>Three steps. No long-term obligation.</h2>
        </div>
        <div class="steps">
          <div class="step">
            <span class="step-num" aria-hidden="true">1</span>
            <h3>Join in minutes</h3>
            <p>Subscribe monthly or annually via Stripe. You get immediate access to the Discord, the full session archive, and the schedule of upcoming live sessions.</p>
          </div>
          <div class="step">
            <span class="step-num" aria-hidden="true">2</span>
            <h3>Show up</h3>
            <p>Meet for an hour each week over Zoom &mdash; ask questions live or in advance &mdash; and collaborate in Discord between sessions.</p>
          </div>
          <div class="step">
            <span class="step-num" aria-hidden="true">3</span>
            <h3>Level up</h3>
            <p>Work personalized goals between sessions, get feedback and code reviews, and report your progress to people who care that you make it.</p>
          </div>
        </div>
      </div>
    </section>

    <section class="section testimonials" id="testimonials" aria-label="Member results">
      <div class="container">
        <div class="section-head">
          <span class="kicker">Member results</span>
          <h2>Don&rsquo;t take our word for it.</h2>
          <p>Real members, real careers, in their own words.</p>
        </div>
        <div class="t-grid">
          <article class="t-card">
            <blockquote>
              <p>With Steve&rsquo;s guidance I&rsquo;ve gone from a developer suffering from impostor syndrome to leading a team of 8 engineers with success, and <strong>doubling my salary in the process</strong>.</p>
            </blockquote>
            <div class="t-who">
              <img src="/images/people/eric_fleming.jpg" alt="" width="48" height="48" loading="lazy" />
              <div><strong>Eric F.</strong><span>devBetter member</span></div>
            </div>
          </article>
          <article class="t-card">
            <blockquote>
              <p>I listed devBetter with Steve Smith on my resume. An interviewer once asked me about my time at devBetter. &hellip; That company ended up offering me a job resulting in a significant pay increase.</p>
            </blockquote>
            <div class="t-who">
              <img src="/images/people/ryan_wemmer_600x600.jpg" alt="" width="48" height="48" loading="lazy" />
              <div><strong>Ryan Wemmer</strong><span>devBetter member</span></div>
            </div>
          </article>
          <article class="t-card">
            <blockquote>
              <p>Becoming a member of devBetter is one of the best investments I&rsquo;ve ever made. &hellip; my career and future outlook have grown exponentially.</p>
            </blockquote>
            <div class="t-who">
              <img src="/images/people/pierre-gadea.jpg" alt="" width="48" height="48" loading="lazy" />
              <div><strong>Pierre Gadea</strong><span>devBetter member</span></div>
            </div>
          </article>
          <article class="t-card">
            <blockquote>
              <p>Signing up was one of the best decisions I have made to advance my software developer career. Steve has been a great coach and a source of precious practical advice and encouragement.</p>
            </blockquote>
            <div class="t-who">
              <img src="/images/people/philippe-vaillancourt.jpg" alt="" width="48" height="48" loading="lazy" />
              <div><strong>Phil V.</strong><span>devBetter member</span></div>
            </div>
          </article>
          <article class="t-card">
            <blockquote>
              <p>The value I received from Steve&rsquo;s coaching was immediate. &hellip; Most important to me is having that accountability and challenge to do the work which is tailored to my goals.</p>
            </blockquote>
            <div class="t-who">
              <img src="/images/people/chris_hood.jpg" alt="" width="48" height="48" loading="lazy" />
              <div><strong>Chris H.</strong><span>devBetter member</span></div>
            </div>
          </article>
          <article class="t-card">
            <blockquote>
              <p>Joining devBetter as a junior developer has been my best career step to date. &hellip; devBetter has quite literally moved my career years forward!</p>
            </blockquote>
            <div class="t-who">
              <img src="/images/people/shmuel-winegarten-320x358.jpg" alt="" width="48" height="48" loading="lazy" />
              <div><strong>Shmuel Winegarten</strong><span>devBetter member</span></div>
            </div>
          </article>
        </div>
      </div>
    </section>

    <section class="section" id="pricing" aria-label="Pricing">
      <div class="container">
        <div class="section-head">
          <span class="kicker">Pricing</span>
          <h2>Simple pricing. Cancel anytime.</h2>
          <p>A fraction of the cost of one-on-one coaching &mdash; with a whole community included.</p>
        </div>
        <div class="pricing-grid">
          <div class="price-card">
            <h3>Monthly</h3>
            <div class="price">$200<small>/month</small></div>
            <ul>
              <li>Weekly live group coaching sessions</li>
              <li>Private Discord community</li>
              <li>150+ hours of recorded sessions</li>
              <li>Code reviews, katas &amp; career coaching</li>
              <li>No long-term obligation &mdash; cancel anytime</li>
            </ul>
            <a class="btn btn-primary" href="/checkout?src=index2">Join Monthly</a>
          </div>
          <div class="price-card featured">
            <span class="price-tag">Best value &middot; 2 months free</span>
            <h3>Annual</h3>
            <div class="price">$2,000<small>/year</small></div>
            <ul>
              <li>Everything in Monthly</li>
              <li>2 months free vs. paying monthly</li>
              <li>One payment, zero interruptions</li>
              <li>Fastest path to free alumni status</li>
            </ul>
            <a class="btn btn-primary" href="/checkout?src=index2">Join Annually</a>
          </div>
        </div>
        <p class="pricing-note">
          Members in good standing graduate to <strong>Alumni after 24 months</strong> &mdash; keeping full access to the
          community and archives, free. Questions first? <a href="/FAQ">Read the FAQ</a>.
        </p>
      </div>
    </section>

    <section class="section final-cta" aria-label="Join">
      <div class="container">
        <h2>Stop guessing. Start growing.</h2>
        <p>Join a small group of like-minded professionals with an expert coach in your corner &mdash; and a community that shows up for you every single day.</p>
        <a class="btn btn-primary" href="/checkout?src=index2">Join devBetter today</a>
      </div>
    </section>

```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/DevBetterWeb.FunctionalTests --filter "FullyQualifiedName~Index2PageGet"`
Expected: 3 PASS.

- [ ] **Step 5: Codex review, then commit**

Run `/codex:review`; address or rebut findings. Then:

```bash
git add src/DevBetterWeb.Web/wwwroot/index2.html tests/DevBetterWeb.FunctionalTests/StaticPages/Index2PageGet.cs
git commit -m "feat: add testimonials, pricing, and CTA sections to index2 page

Co-Authored-By: Claude Fable 5 <noreply@anthropic.com>"
```

---

### Task 4: Structured data (JSON-LD) + full verification

**Files:**
- Modify: `src/DevBetterWeb.Web/wwwroot/index2.html` (add one `<script type="application/ld+json">` in `<head>`, after the last `<meta>`)
- Modify: `tests/DevBetterWeb.FunctionalTests/StaticPages/Index2PageGet.cs` (add one test)

**Interfaces:**
- Consumes: the `<head>` from Task 1.
- Produces: the finished page; nothing depends on this downstream.

- [ ] **Step 1: Add the failing test**

Add to `Index2PageGet`:

```csharp
  [Fact]
  public async Task ReturnsPageWithStructuredData()
  {
    HttpResponseMessage response = await _client.GetAsync("/index2.html");
    response.EnsureSuccessStatusCode();
    string stringResponse = await response.Content.ReadAsStringAsync();

    Assert.Contains("application/ld+json", stringResponse);
    Assert.Contains("\"@type\": \"Product\"", stringResponse);
  }
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/DevBetterWeb.FunctionalTests --filter "FullyQualifiedName~Index2PageGet"`
Expected: 3 PASS, 1 FAIL.

- [ ] **Step 3: Add the JSON-LD block**

Insert in `<head>`, immediately before `<style>`:

```html
  <script type="application/ld+json">
  {
    "@context": "https://schema.org",
    "@graph": [
      {
        "@type": "Organization",
        "@id": "https://devbetter.com/#org",
        "name": "devBetter",
        "url": "https://devbetter.com/",
        "logo": "https://devbetter.com/images/icon_200x200.png",
        "founder": { "@type": "Person", "name": "Steve Smith", "url": "https://ardalis.com" },
        "sameAs": [ "https://twitter.com/ardalis", "https://github.com/DevBetterCom/DevBetterWeb" ]
      },
      {
        "@type": "Product",
        "name": "devBetter Membership",
        "description": "Live group coaching program for software developers: weekly Zoom sessions with Steve 'ardalis' Smith, a private Discord community, code reviews, and 150+ hours of recorded sessions.",
        "brand": { "@id": "https://devbetter.com/#org" },
        "url": "https://devbetter.com/",
        "image": "https://devbetter.com/images/devBetter_Box_864x488.png",
        "offers": [
          {
            "@type": "Offer",
            "name": "Monthly membership",
            "price": "200",
            "priceCurrency": "USD",
            "url": "https://devbetter.com/checkout",
            "availability": "https://schema.org/InStock"
          },
          {
            "@type": "Offer",
            "name": "Annual membership (2 months free)",
            "price": "2000",
            "priceCurrency": "USD",
            "url": "https://devbetter.com/checkout",
            "availability": "https://schema.org/InStock"
          }
        ]
      }
    ]
  }
  </script>
```

Note: no `aggregateRating`/`review` — self-serving reviews violate Google's structured-data guidelines.

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/DevBetterWeb.FunctionalTests --filter "FullyQualifiedName~Index2PageGet"`
Expected: 4 PASS.

- [ ] **Step 5: Run the FULL functional + unit test suites (regression check)**

Run: `dotnet test tests/DevBetterWeb.FunctionalTests` and `dotnet test tests/DevBetterWeb.Tests`
Expected: all tests pass, including the untouched `HomePageGet` (proves `/` is unaffected).

- [ ] **Step 6: Manual verification in a browser**

1. Run the site: `dotnet run --project src/DevBetterWeb.Web` and open `https://localhost:<port>/index2.html`.
2. Verify: sticky nav works; mobile menu toggles at narrow width; all four anchor links scroll; all CTAs land on `/checkout?src=index2`; headshots load; no console errors; no external network requests in DevTools (only same-origin).
3. Run Lighthouse (DevTools) on the page: targets Performance ≥ 95, SEO ≥ 95, Accessibility ≥ 95, Best Practices ≥ 95. Fix anything below target before committing (likely candidates: image dimensions, contrast).
4. Validate the JSON-LD by pasting the page URL or source into https://validator.schema.org/ (or paste the JSON block alone).

- [ ] **Step 7: Codex review, then commit**

Run `/codex:review`; address or rebut findings. Then:

```bash
git add src/DevBetterWeb.Web/wwwroot/index2.html tests/DevBetterWeb.FunctionalTests/StaticPages/Index2PageGet.cs
git commit -m "feat: add structured data and finish index2 landing page

Co-Authored-By: Claude Fable 5 <noreply@anthropic.com>"
```

---

## Post-Deployment Checklist (informational — not tasks)

- The page ships **canonicalized to `/`** and unlisted (no sitemap) — safe to share the `/index2.html` URL for feedback without SEO side effects.
- When promoting it to be the real home page: flip the canonical to self (or port the markup into `Views/Home/Index.cshtml`), keep Phil V.'s "Steve has been a great coach and a source of precious practical" sentence somewhere on `/` (the `HomePageGet` functional test asserts it), add sitemap.xml + robots `Sitemap:` line, and wire up GA4 (needs a property ID) with a conversion event on the checkout CTAs.
- Follow-ups recorded in the design spec: GA4 replacement for the dead UA tag, `_Layout.cshtml` meta fixes, `/checkout` nested-`<html>` bug, deleting `index_NOT_USED.html`.
