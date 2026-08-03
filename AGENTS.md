# AGENTS.md

## Repository Shape
- This is an ASP.NET Core 9 Web API tutorial repo, not a single app: each numbered chapter folder contains an independent runnable project and `.csproj`.
- There is no root `.sln`, root build script, CI workflow, or test project. Run `dotnet` commands inside the specific chapter project directory or pass that chapter's `.csproj` explicitly.
- Ignore `.claude/worktrees/` when searching or editing; it contains duplicate worktree copies, not the primary tutorial source.
- `12-oauth 2.0 (diagram explanation only)/` is documentation/diagram content only; the code resumes in `13-azure-oauth-authorization/AzureOAuthApi`.

## Commands
- Build a chapter: `dotnet build <chapter>/<ProjectName>/<ProjectName>.csproj`.
- Restore a chapter: `dotnet restore <chapter>/<ProjectName>/<ProjectName>.csproj`.
- Run a chapter from its project directory: `dotnet run` or `dotnet watch run`.
- Swagger is available only in Development after running a project: `https://localhost:7xxx/swagger`.
- Endpoint smoke tests are via chapter-local `.http` files where present; later chapters 10, 11, and 13 currently do not include `.http` files.

## Tutorial Context
- User questions are usually for YouTube tutorial preparation: answer in a beginner-friendly, demonstrable, step-by-step way with complete examples when explaining concepts.
- Keep code changes aligned with the chapter's teaching stage. Do not backport advanced patterns into early chapters unless asked.
- Root `README.md` has the public learning path; `CLAUDE.md` has the fuller source instructions this file was condensed from.

## Architecture Progression
- Chapters 01-04 are controller/Swagger/HTTP-method foundations.
- Chapters 05-06 add DTOs, DataAnnotations validation, and AutoMapper.
- Chapter 07 demonstrates DI lifetimes with visual/loggable examples.
- Chapter 08 is the clean architecture baseline: controllers delegate to services; services contain business logic/validation; repositories abstract data access.
- Chapter 09 adds global exception handling with `IExceptionHandler`, Problem Details, and custom exceptions.
- Chapters 10-11 build on chapter 09 with custom middleware and Options Pattern configuration.
- Chapter 13 adds Azure AD/OAuth packages and authentication wiring on top of the later-chapter architecture.

## High-Value Conventions
- Later chapters use `Data/ProductsData.cs` or `Data/InMemoryDatabase.cs` static in-memory stores for tutorial simplicity; do not introduce EF Core/database plumbing unless requested.
- New endpoints should stay thin in controllers and delegate to the service layer in chapters 08+.
- In chapters 08+, add data operations to both repository interface and implementation, business rules to services, and DTO mapping to `Mappings/MappingProfile.cs` where AutoMapper is used.
- In chapter 09+, avoid controller `try`/`catch`; throw custom exceptions from services and let registered handlers format responses.
- Exception handler order matters in `Program.cs`: validation handler, business handler, then global handler; `app.UseExceptionHandler()` should be early in the pipeline.
- Middleware order matters in chapters 10+: exception handler first, custom middleware before Swagger/HTTPS/auth/routing as shown in `Program.cs`.
- AutoMapper registration is `builder.Services.AddAutoMapper(typeof(MappingProfile))`; package versions differ by chapter, so preserve local `.csproj` style.

## Configuration And Secrets
- Projects use `appsettings.json` plus `appsettings.Development.json`; Development settings override base settings.
- Chapter 13 reads `AzureAd:*` settings and calls Azure Key Vault with a placeholder vault URI. Do not add real secrets to tracked config; use user secrets or environment variables for real credentials.
- Chapter 13 authentication middleware order is intentional: `app.UseAuthentication()` before `app.UseAuthorization()`.

## Scripts
- `scripts/` is for YouTube metadata/chapter helper scripts, not app build tooling.
- Script prerequisites are external: `yt-dlp` for video metadata scripts and Python 3 for Python helpers.

## YouTube README Sync
- Shared cross-tool workflow: `.ai/youtube-readme-sync/workflow.md`.
- Chapter-to-README mapping: `.ai/youtube-readme-sync/chapter-map.md`.
- Use this workflow when asked to sync README files with YouTube titles, links, durations, playlist indexes, chapters, or placeholder video IDs.
- Prefer the existing scripts in `scripts/` for YouTube metadata; do not invent missing video IDs, titles, durations, or playlist indexes.
- Preserve unrelated README prose and tutorial content unless the user explicitly asks for a broader rewrite.
