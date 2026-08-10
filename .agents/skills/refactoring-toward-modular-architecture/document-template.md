# Recommendations Document Template

Save as `.claude/plans/YYYY-MM-DD-modular-monolith-recommendations.md` (or the project's plan folder). Sections in order:

```markdown
# <App> — Modular Monolith Restructuring Recommendations

**Date:** YYYY-MM-DD
**Status:** Proposal / recommendations (no code changes yet)
**Scope:** <projects analyzed>

## 1. Executive Summary
Two paragraphs max: what the layering is today, why it isn't holding,
and the single highest-leverage decision (usually god-aggregate ownership).
Then the candidate-module table:

| # | Module | Cohesion today | Extraction difficulty |

## 2. Current State — Key Findings
### 2.1 Structure          (projects, DbContexts, event infrastructure)
### 2.2 Coupling hotspots  (numbered list; every item cites file paths —
                            this is the evidence the layering isn't holding)
### 2.3 Hygiene issues to fix regardless of restructuring
                           (auth gaps, bugs, dead code, namespace mismatches;
                            strike through + "Fixed YYYY-MM-DD" as they land)

## 3. Identified Bounded Contexts
One subsection per module + one for the Shared Kernel. Per module:
- **Owns:** entities, services, pages, endpoints (name them)
- **Interface to other modules:** queries (DTO-returning) + integration events
- **Why it's a context** (or notes on internal sub-seams / deferred splits)

## 4. Key Design Decisions
4.1 God-aggregate breakup — table: what moves, where, as what
4.2 Data access becomes module-internal (per-module repos, schemas, contexts)
4.3 Cross-module communication — events first, queries second;
    event types live in the publisher's Contracts project
4.4 Target solution structure — Contracts + implementation project pair
    per module; the WHY (cycle example from THIS codebase); reference rules
    split into compiler-enforced vs arch-test-enforced
4.5 What stays shared deliberately (layout/shell, logging, health checks)

## 5. Phased Migration Roadmap
Phase 0 hygiene → Phase 1 composition-root discipline → smallest modules
first → ... → god aggregate → strict enforcement.
State per phase what proves the pattern. Every phase leaves app releasable.

## 6. Risks & Open Questions
Riskiest mechanical step (usually EF history surgery) + mitigation;
performance tradeoffs accepted; team-size ceremony fallback;
**open questions** with a recommendation each, and which phase needs
the answer.

## 7. Module → Current-Code Mapping (quick reference)
| Module | Entities | Key services today | Web surface today |
Makes each later extraction mechanical instead of exploratory.
```
