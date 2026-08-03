# Copilot Instructions

This is an ASP.NET Core 9 Web API tutorial repository. Each numbered chapter folder is an independent runnable project.

For README synchronization with YouTube data, use the shared workflow:

- `.ai/youtube-readme-sync/workflow.md`
- `.ai/youtube-readme-sync/chapter-map.md`

When updating README files from YouTube metadata:

- Use the existing scripts in `scripts/`.
- Do not invent missing video IDs, titles, durations, or playlist indexes.
- Preserve unrelated tutorial content.
- Normalize known YouTube links only when the exact video ID and playlist index are known.
- Ask for missing video information when playlist data is incomplete.
