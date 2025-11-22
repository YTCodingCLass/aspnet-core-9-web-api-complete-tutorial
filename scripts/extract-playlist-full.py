#!/usr/bin/env python3
"""
YouTube Playlist Video Information Extractor (Full Details)
Uses yt-dlp to extract complete metadata from all videos in a playlist

Usage:
    python3 extract-playlist-full.py <playlist-url>
"""

import subprocess
import json
import sys

def get_video_info(video_url):
    """Get full information for a single video"""
    cmd = [
        'yt-dlp',
        '--dump-json',
        '--skip-download',
        video_url
    ]

    try:
        result = subprocess.run(cmd, capture_output=True, text=True, check=True)
        return json.loads(result.stdout)
    except (subprocess.CalledProcessError, json.JSONDecodeError) as e:
        print(f"Error fetching {video_url}: {e}", file=sys.stderr)
        return None

def get_playlist_videos(playlist_url):
    """Get list of video URLs from playlist"""
    cmd = [
        'yt-dlp',
        '--flat-playlist',
        '--print', '%(id)s',
        playlist_url
    ]

    try:
        result = subprocess.run(cmd, capture_output=True, text=True, check=True)
        video_ids = result.stdout.strip().split('\n')
        return [f"https://www.youtube.com/watch?v={vid}" for vid in video_ids if vid]
    except subprocess.CalledProcessError as e:
        print(f"Error: {e}", file=sys.stderr)
        return []

def format_duration(seconds):
    """Convert seconds to mm:ss format"""
    if not seconds:
        return "N/A"
    minutes = int(seconds) // 60
    secs = int(seconds) % 60
    return f"{minutes}:{secs:02d}"

def main(playlist_url):
    print("🔍 Fetching playlist video list...")
    video_urls = get_playlist_videos(playlist_url)

    if not video_urls:
        print("❌ No videos found in playlist")
        return

    print(f"📹 Found {len(video_urls)} videos. Fetching details...\n")

    videos = []
    for idx, url in enumerate(video_urls, 1):
        print(f"  [{idx}/{len(video_urls)}] Fetching metadata...", end='\r')
        info = get_video_info(url)
        if info:
            videos.append({
                'title': info.get('title', 'Unknown'),
                'duration_seconds': info.get('duration', 0),
                'duration_formatted': format_duration(info.get('duration')),
                'url': url,
                'video_id': info.get('id', ''),
                'view_count': info.get('view_count', 0),
                'upload_date': info.get('upload_date', 'Unknown')
            })

    print("\n")

    # Print table
    print("📺 YouTube Playlist Videos\n")
    print(f"{'#':<4} {'Duration':<10} {'Views':<12} {'Title':<60}")
    print("=" * 90)

    for idx, video in enumerate(videos, 1):
        title = video['title'][:57] + "..." if len(video['title']) > 60 else video['title']
        views = f"{video['view_count']:,}" if video['view_count'] else "N/A"
        print(f"{idx:<4} {video['duration_formatted']:<10} {views:<12} {title}")

    print("\n" + "=" * 90)

    # Calculate totals
    total_seconds = sum(v['duration_seconds'] for v in videos)
    total_minutes = total_seconds // 60
    total_hours = total_minutes // 60
    remaining_minutes = total_minutes % 60

    print(f"\n📊 Total Videos: {len(videos)}")
    print(f"⏱️  Total Duration: {total_hours}h {remaining_minutes}m ({total_seconds} seconds)")

    # Print markdown table
    print("\n\n## 📝 Markdown Format for README:\n")
    print("| Chapter | Topic | Duration |")
    print("|---------|-------|----------|")

    for idx, video in enumerate(videos, 1):
        title = video['title'].split('|')[0].strip() if '|' in video['title'] else video['title']
        # Remove chapter numbers like "#1 [arabic]"
        title = title.split(']', 1)[-1].strip() if ']' in title else title
        duration_min = video['duration_seconds'] // 60
        print(f"| **{idx:02d}** | {title} | ⏱️ ~{duration_min} min |")

    print()

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python3 extract-playlist-full.py <playlist-url>")
        print("\nExample:")
        print('  python3 extract-playlist-full.py "https://www.youtube.com/playlist?list=YOUR_PLAYLIST_ID"')
        sys.exit(1)

    main(sys.argv[1])
