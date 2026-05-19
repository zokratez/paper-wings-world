---
type: research
title: "Linear + Obsidian Integration Pattern"
status: draft
updated: 2026-05
---

# Linear + Obsidian Integration (Future Execution Layer)

**Status**: Recommended pattern for when we move from Phase 0 research into active development (post-Huh? Build 44).

## Philosophy

- **Obsidian (`/notes/`)** = deep thinking, design, research, long-form specs, aerodynamics notes, content production tracking, "why" documents.
- **Linear** = execution, priorities, sprint planning, assignee, estimates, status, shipping.

We want the thinking to live in rich Markdown with links, embeds, and canvas, while the work items live in a proper task tracker.

## Recommended Setup

1. **Linear Workspace**: Create a new Linear team for Paper Wings World (separate from Huh? / PACO).

2. **Labels & Views**:
   - Labels: `plane-folding`, `region-terrain`, `flight-physics`, `ui`, `content`, `research`, `phase-0`, `v1.0`, `tablet`
   - Views: Roadmap, Phase 0, Next Up, In Progress, Needs Research, Content Production

3. **Sync Strategy (Lightweight)**:
   - Keep detailed specs in Obsidian (`Roadmap/`, `Designs/`, `Aerodynamics/`).
   - Create Linear issues from Obsidian using one of these methods:
     - Manual: Copy the task title + link back to the Obsidian note in the Linear issue description.
     - Automation (preferred): Use Linear's GitHub sync + a small "Tasks" folder in Obsidian where each file has frontmatter:
       ```yaml
       linear_id: LIN-42
       status: in_progress
       estimate: 3
       ```
     - Or use a simple script / Make.com / n8n zap that watches a specific Obsidian folder and creates/updates Linear issues.

4. **Best Practice Observed in Similar Projects**:
   - Every Linear issue has a "Spec" link back to the Obsidian note.
   - Every Obsidian planning note has a "Linear" section at the bottom with the issue ID(s).
   - Use Linear for "what" and "when"; Obsidian for "why" and "how it should feel."

## Alternative (If We Want Zero Extra Tools)

Use GitHub Issues + Projects inside the repo itself, with the same Markdown files in `notes/Roadmap/` serving as the spec source. Simpler for a solo founder, but less powerful than Linear for prioritization.

## Decision Point

We will revisit this document and choose the exact sync method once we have 20–30 real tasks and are ready to start active implementation (likely Phase 2+).

For Phase 0, everything lives in Obsidian only. No Linear workspace is needed yet.

---

*Document created as part of initial project organization per user request.*