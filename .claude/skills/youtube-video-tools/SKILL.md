---
name: youtube-video-tools
description: Use when working with YouTube video metadata, playlist titles, video links, durations, transcripts, chapters, yt-dlp scripts, or README synchronization for this tutorial repository.
---

# YouTube Video Tools

Use the shared workflow at `.ai/youtube-readme-sync/workflow.md` and the chapter mapping at `.ai/youtube-readme-sync/chapter-map.md`.

The available helper scripts are in `scripts/`:

- `scripts/extract-playlist-info.py` for playlist titles, IDs, and durations.
- `scripts/extract-playlist-full.py` for fuller playlist metadata.
- `scripts/get-video-info.sh` for one video.
- `scripts/generate-chapters.py` for transcript-based chapter suggestions.
- `scripts/check-all-videos.sh` for checking existing chapter timestamps.

Do not invent missing YouTube video IDs, titles, durations, or playlist indexes. Ask the user when the playlist data is incomplete.
