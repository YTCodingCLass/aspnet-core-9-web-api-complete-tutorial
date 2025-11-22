# YouTube Video Information Scripts

This folder contains utility scripts for extracting metadata from YouTube videos using `yt-dlp`.

## Prerequisites

Make sure `yt-dlp` is installed:

```bash
brew install yt-dlp
```

## Scripts

### 1. `generate-chapters.py` - Video Chapter Generator 🆕

Analyzes YouTube videos to check for existing chapters or suggests where to add them based on transcripts.

**Usage:**
```bash
python3 generate-chapters.py "https://www.youtube.com/watch?v=VIDEO_ID"
```

**Features:**
- Checks if video already has chapters in description
- Downloads and analyzes video transcripts (if available)
- Suggests chapter breakpoints based on content
- Provides template for adding chapters

**Example:**
```bash
python3 generate-chapters.py "https://www.youtube.com/watch?v=BEG49WICGEo"
```

**See also:** `SUGGESTED_CHAPTERS.md` for pre-made chapter suggestions for your tutorial series.

### 2. `get-video-info.sh` - Single Video Information

Extracts detailed information from a single YouTube video.

**Usage:**
```bash
./get-video-info.sh "https://www.youtube.com/watch?v=VIDEO_ID"
```

**Output:**
- Video title
- Duration (minutes:seconds)
- Upload date
- View count
- Description preview

**Example:**
```bash
./get-video-info.sh "https://www.youtube.com/watch?v=BEG49WICGEo"
```

### 2. `extract-playlist-info.py` - Playlist Batch Extractor

Extracts information from all videos in a YouTube playlist and formats it for README files.

**Usage:**
```bash
python3 extract-playlist-info.py "https://www.youtube.com/playlist?list=PLAYLIST_ID"
```

**Output:**
- Formatted table of all videos
- Total video count
- Total duration
- Markdown table format ready for README

**Example:**
```bash
python3 extract-playlist-info.py "https://www.youtube.com/playlist?list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA"
```

**Sample Output:**
```
📺 YouTube Playlist Videos

#    Duration   Title
========================================================================================
1    8:10       ASP.NET Core 9 Web API – Intro and Setup
2    15:22      ASP.NET Core 9 Web API – First Controller
...

📊 Total Videos: 9
⏱️  Total Duration: 2h 30m

## 📝 Markdown Format for README:

| Chapter | Topic | Duration |
|---------|-------|----------|
| **01** | ASP.NET Core 9 Web API – Intro and Setup | ⏱️ ~8 min |
| **02** | ASP.NET Core 9 Web API – First Controller | ⏱️ ~15 min |
...
```

## Direct Command Line Usage

If you prefer not to use scripts, you can run `yt-dlp` directly:

**Get video title and duration:**
```bash
yt-dlp --print "%(title)s | %(duration)s" "VIDEO_URL"
```

**Get playlist information:**
```bash
yt-dlp --flat-playlist --print "%(title)s | %(duration)s" "PLAYLIST_URL"
```

**Get full JSON metadata:**
```bash
yt-dlp --dump-json --skip-download "VIDEO_URL"
```

## Integration with Claude Code

These scripts enable the youtube-video-analyzer functionality locally. When you need to extract YouTube video information, simply run the appropriate script instead of relying on the agent.

## Troubleshooting

**Issue: Permission denied**
```bash
chmod +x get-video-info.sh
chmod +x extract-playlist-info.py
```

**Issue: yt-dlp not found**
```bash
brew install yt-dlp
# or
pip install yt-dlp
```

**Issue: Python script errors**
```bash
# Make sure you're using Python 3
python3 --version

# Install required Python (if needed)
brew install python@3
```
