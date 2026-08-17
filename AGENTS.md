# Agent Guide

## Architecture

- `Galileo.DesignSystem.slnx` contains two `net10.0` projects: `Galileo.DesignSystem` is the Razor Class Library; `Galileo.DesignSystem.Demo` is the ASP.NET Core MVC host and references the library. The library must not reference demo code or MVC views.
- Reusable components live in `src/Galileo.DesignSystem/Components/Atoms`, `Molecules`, and `Organisms`; `Templates/GdsAppShell.razor` is the reusable shell template.
- Demo pages live in `src/Galileo.DesignSystem.Demo/Pages`. `DemoShell` and `Showcase` are demo-only documentation components; `Showcase` may compose library components such as `GdsCard` and `GdsText`, but stays out of the library.
- MVC only routes and hosts the static Razor component pages. `Views/Home/Index.cshtml`, `Atoms.cshtml`, `Molecules.cshtml`, and `Organisms.cshtml` use `Layout = null` and render one static Razor page component; `Error.cshtml` is separate plain MVC HTML.
- `DemoShell.razor` renders the full HTML document, so keep its stylesheet and script links synchronized. Demo CSS isolation is loaded through `/Galileo.DesignSystem.Demo.styles.css`; library tokens/assets are linked from `_content/Galileo.DesignSystem`.

## Commands

- Build from the repository root: `dotnet build Galileo.DesignSystem.slnx`.
- Run the demo: `dotnet run --project src/Galileo.DesignSystem.Demo/Galileo.DesignSystem.Demo.csproj`.
- Launch profiles use `http://localhost:5119` and `https://localhost:7277`.
- Smoke-check `/Home/Index`, `/Home/Atoms`, `/Home/Molecules`, and `/Home/Organisms`; `/Home/Molecules?page=2` exercises the demo pagination model.
- There are no test projects, CI workflows, package manifests, or lint/formatter/typecheck commands. Use a successful solution build plus browser/HTTP smoke checks for verification.

## Conventions

- Keep component styling in colocated `*.razor.css` files. Use `wwwroot/css/design-system.css` only for library-wide tokens and box sizing; use demo `wwwroot/css/site.css` for reset and demo layout; keep `Showcase.razor.css` for Showcase-specific styling.
- `GdsAppShell` owns shell sizing and its internal content scroll. Do not add card/showcase spacing to the library shell; apply sibling spacing with parent `gap` or demo Showcase margins.
- Razor CSS isolation and the static CSS/JS assets are the source of truth; do not reintroduce Vite, npm, or another frontend build pipeline.
- Preserve native HTML behavior. Use `AdditionalAttributes` for consumer `aria-*`, `data-*`, classes, and other attributes; do not add string pseudo-events such as `Onclick`.
- `GdsIcon` uses a local inline SVG path map; add icons by extending that map rather than adding a dependency.
- Dialog behavior depends on the `data-gds-dialog-*` attributes in `GdsDialog.razor` and `wwwroot/js/design-system.js`.
- `bin/`, `obj/`, and `node_modules/` are generated/ignored. If stale generated output contains old project or asset names after a rename, remove it and rebuild.
