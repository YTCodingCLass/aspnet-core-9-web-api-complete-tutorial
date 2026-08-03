# Sync README Files With YouTube Playlist

Use this prompt when asked to update README files with YouTube titles, links, video IDs, playlist indexes, or durations.

Follow the shared workflow:

- `.ai/youtube-readme-sync/workflow.md`
- `.ai/youtube-readme-sync/chapter-map.md`

Run the existing playlist metadata script when possible:

```bash
python3 scripts/extract-playlist-info.py "https://www.youtube.com/playlist?list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA"
```

Update only README content related to YouTube metadata unless the user asks for broader documentation changes. Do not invent missing video IDs or durations.
