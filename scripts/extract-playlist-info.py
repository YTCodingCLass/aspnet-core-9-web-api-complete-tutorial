#!/usr/bin/env python3
"""
YouTube Playlist Video Information Extractor
Uses yt-dlp to extract metadata from all videos in a playlist

Usage:
    python3 extract-playlist-info.py <playlist-url>

Example:
    python3 extract-playlist-info.py "https://www.youtube.com/playlist?list=PL7RnrrCfV_JdYXcg1lhvEDTYuJeVXBJfA"
"""

import subprocess
import json
import sys

def get_playlist_info(playlist_url):
    """Extract information from all videos in a YouTube playlist"""

    cmd = [
        'yt-dlp',
        '--flat-playlist',
        '--print', '%(title)s|%(id)s|%(duration)s|%(url)s',
        playlist_url
    ]

    try:
        result = subprocess.run(cmd, capture_output=True, text=True, check=True)
        videos = []

        for line in result.stdout.strip().split('\n'):
            if line:
                parts = line.split('|')
                if len(parts) >= 4:
                    title, video_id, duration, url = parts[0], parts[1], parts[2], parts[3]

                    # Convert duration to minutes and seconds
                    try:
                        duration_sec = int(duration) if duration != 'NA' else 0
                        minutes = duration_sec // 60
                        seconds = duration_sec % 60
                        duration_formatted = f"{minutes}:{seconds:02d}"
                    except (ValueError, TypeError):
                        duration_formatted = "N/A"
                        duration_sec = 0

                    videos.append({
                        'title': title,
                        'video_id': video_id,
                        'duration_seconds': duration_sec,
                        'duration_formatted': duration_formatted,
                        'url': f"https://www.youtube.com/watch?v={video_id}"
                    })

        return videos

    except subprocess.CalledProcessError as e:
        print(f"Error: {e}", file=sys.stderr)
        print(f"stderr: {e.stderr}", file=sys.stderr)
        return []

def print_table(videos):
    """Print videos in a formatted table"""
    print("\n📺 YouTube Playlist Videos\n")
    print(f"{'#':<4} {'Duration':<10} {'Title':<80}")
    print("=" * 95)

    for idx, video in enumerate(videos, 1):
        title = video['title'][:77] + "..." if len(video['title']) > 80 else video['title']
        print(f"{idx:<4} {video['duration_formatted']:<10} {title}")

    print("\n" + "=" * 95)

    # Calculate total duration
    total_seconds = sum(v['duration_seconds'] for v in videos)
    total_minutes = total_seconds // 60
    total_hours = total_minutes // 60
    remaining_minutes = total_minutes % 60

    print(f"\n📊 Total Videos: {len(videos)}")
    print(f"⏱️  Total Duration: {total_hours}h {remaining_minutes}m")
    print()

def print_markdown_table(videos):
    """Print videos in Markdown table format for README"""
    print("\n## 📝 Markdown Format for README:\n")
    print("| Chapter | Topic | Duration |")
    print("|---------|-------|----------|")

    for idx, video in enumerate(videos, 1):
        # Extract just the main topic from title
        title = video['title'].split('|')[0].strip() if '|' in video['title'] else video['title']
        duration_min = video['duration_seconds'] // 60
        print(f"| **{idx:02d}** | {title} | ⏱️ ~{duration_min} min |")

    print()

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python3 extract-playlist-info.py <playlist-url>")
        print("\nExample:")
        print('  python3 extract-playlist-info.py "https://www.youtube.com/playlist?list=YOUR_PLAYLIST_ID"')
        sys.exit(1)

    playlist_url = sys.argv[1]

    print("🔍 Fetching playlist information...")
    videos = get_playlist_info(playlist_url)

    if videos:
        print_table(videos)
        print_markdown_table(videos)
    else:
        print("❌ No videos found or error occurred.")
        sys.exit(1)
