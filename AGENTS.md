# Agent Guide

## Structure

- `Galileo.DesignSystem.slnx` contains two `net10.0` projects: the `Galileo.DesignSystem` Razor Class Library and the `Galileo.DesignSystem.Demo` ASP.NET Core MVC app. The demo references the library; the library must not reference demo code.
- Library components live under `src/Galileo.DesignSystem/Components/`, split into `Atoms`, `Molecules`, and `Organisms`; `Templates/GdsAppShell.razor` is the library shell template.
- Demo pages live under `src/Galileo.DesignSystem.Demo/Pages/`; demo-only components such as `DemoShell` and `Showcase` stay under its `Components/` directory.
- MVC is only the demo's routing and page-host layer. The four content views `Views/Home/Index.cshtml`, `Atoms.cshtml`, `Molecules.cshtml`, and `Organisms.cshtml` statically render one Razor page component; `Error.cshtml` is plain MVC HTML.

## Commands

- Build everything from the repository root with `dotnet build Galileo.DesignSystem.slnx`.
- Run the demo with `dotnet run --project src/Galileo.DesignSystem.Demo/Galileo.DesignSystem.Demo.csproj`.
- The demo launch profiles use `http://localhost:5119` and `https://localhost:7277`.
- The canonical demo routes are `/Home/Index`, `/Home/Atoms`, `/Home/Molecules`, and `/Home/Organisms`.
- There are no test projects, CI workflows, package manifests, or repository lint, formatter, or typecheck commands. Verify changes with a clean build and browser smoke checks of the four routes.

## Conventions

- Keep the library independent of demo code and MVC views; demo showcase/page components must stay in `Galileo.DesignSystem.Demo`.
- Keep component styling in colocated `*.razor.css` files. Library tokens and box sizing are in `src/Galileo.DesignSystem/wwwroot/css/design-system.css`; demo reset, layout, and showcase rules are in `src/Galileo.DesignSystem.Demo/wwwroot/css/site.css`.
- Razor CSS isolation and static CSS/JS assets are the styling and behavior source of truth; do not reintroduce Vite or npm.
- Components are Razor-first and should preserve native HTML behavior. Use `AdditionalAttributes` for `aria-*`, `data-*`, classes, and other consumer attributes; do not add string-based pseudo-events such as `Onclick`.
- `GdsIcon` uses a local inline SVG path map, so adding an icon means extending that map rather than adding a package dependency.
- Dialog behavior depends on the `data-gds-dialog-*` attributes in `GdsDialog.razor` and `wwwroot/js/design-system.js`.
- `bin/`, `obj/`, and `node_modules/` are generated/ignored; delete stale generated output if old project or asset names appear after a rename, then rebuild.
