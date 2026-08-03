# YouTube README Sync Workflow

Use this workflow when synchronizing README files with YouTube tutorial metadata such as titles, video IDs, playlist links, durations, and chapter links.

## Goal

Keep the root `README.md` and chapter-level `README.md` files aligned with the live YouTube playlist while preserving the tutorial content and repository-specific wording.

## Prerequisites

- `yt-dlp` must be installed.
- Python 3 must be available.
- Use the scripts in `scripts/`; do not create network scraping code unless the existing scripts are insufficient.

## Source Playlist

`https://www.youtube.com/playlist?list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA`

## Recommended Commands

Fetch playlist titles, video IDs, and durations:

```bash
python3 scripts/extract-playlist-info.py "https://www.youtube.com/playlist?list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA"
```

Fetch fuller metadata if views, upload dates, or exact video metadata are needed:

```bash
python3 scripts/extract-playlist-full.py "https://www.youtube.com/playlist?list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA"
```

Inspect a single video if a README has a known video URL:

```bash
./scripts/get-video-info.sh "https://www.youtube.com/watch?v=VIDEO_ID"
```

## Files To Read First

1. `.ai/youtube-readme-sync/chapter-map.md`
2. `README.md`
3. The relevant chapter README files from the chapter map
4. `scripts/README.md` if script behavior is unclear

## Update Rules

- Update the root `README.md` tutorial table with current titles, links, and durations when the playlist data is available.
- Update each chapter README's `YouTube Video` section when the matching video is known.
- Prefer full YouTube playlist URLs in chapter READMEs:
  `https://www.youtube.com/watch?v=VIDEO_ID&list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA&index=N`
- Replace `https://youtu.be/VIDEO_ID` with the full playlist URL only when the playlist index is known or can be safely derived from the playlist output.
- Replace `YOUR_VIDEO_ID` only when the exact matching video ID is known from playlist output or user input.
- Keep existing tutorial prose, learning objectives, project structure sections, code samples, and chapter-specific explanations unless the user explicitly asks to rewrite them.
- Do not update unrelated app code when the task is README synchronization.
- Do not invent missing video IDs, titles, durations, or playlist indexes.
- If playlist output does not include a chapter, ask the user for the missing video URL or video ID.
- Preserve Markdown formatting style already used in each README.
- Do not add real secrets or private YouTube data.

## Matching Strategy

1. Use the playlist order and video titles from `extract-playlist-info.py` as the primary source.
2. Match videos to chapters using chapter number, folder name, current README title, and known topic names from `chapter-map.md`.
3. If a chapter has an existing non-placeholder video ID, verify it appears in the playlist before changing it.
4. If a chapter title in the README differs slightly from the YouTube title, prefer the repository's human-friendly chapter topic unless the user requests exact YouTube titles everywhere.
5. For duration display in the root README, use the existing style: `~8 min`, `~15 min`, or similar rounded minute values.

## Expected Output

After making changes, summarize:

- Which README files were updated.
- Which video IDs or links changed.
- Which durations changed.
- Which chapters still need user-provided video IDs, if any.
- Whether `yt-dlp` was unavailable or any command failed.

## Safety Checks

- Review diffs before finishing.
- Do not stage or commit changes unless explicitly asked.
- If generated changes touch unexpected files, mention them and do not revert unrelated user changes.
