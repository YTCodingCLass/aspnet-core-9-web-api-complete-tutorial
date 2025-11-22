#!/usr/bin/env python3
"""
YouTube Video Chapter Generator
Extracts transcript and suggests chapter timestamps based on topic changes

Usage:
    python3 generate-chapters.py <youtube-url>

Example:
    python3 generate-chapters.py "https://www.youtube.com/watch?v=BEG49WICGEo"
"""

import subprocess
import json
import sys
import re
from collections import defaultdict

def get_transcript(video_url):
    """Download video transcript/subtitles"""
    print("🔍 Downloading transcript...")

    cmd = [
        'yt-dlp',
        '--write-auto-sub',
        '--sub-lang', 'en,ar',
        '--skip-download',
        '--sub-format', 'json3',
        '--output', '/tmp/%(id)s',
        video_url
    ]

    try:
        subprocess.run(cmd, capture_output=True, text=True, check=True)

        # Get video ID
        id_cmd = ['yt-dlp', '--print', '%(id)s', video_url]
        result = subprocess.run(id_cmd, capture_output=True, text=True, check=True)
        video_id = result.stdout.strip()

        # Try to read subtitle file
        subtitle_files = [
            f'/tmp/{video_id}.en.json3',
            f'/tmp/{video_id}.ar.json3',
        ]

        for sub_file in subtitle_files:
            try:
                with open(sub_file, 'r', encoding='utf-8') as f:
                    return json.load(f), video_id
            except FileNotFoundError:
                continue

        return None, video_id

    except subprocess.CalledProcessError as e:
        print(f"❌ Error downloading transcript: {e}")
        return None, None

def get_video_description(video_url):
    """Get video description which may contain existing chapters"""
    cmd = [
        'yt-dlp',
        '--print', '%(description)s',
        video_url
    ]

    try:
        result = subprocess.run(cmd, capture_output=True, text=True, check=True)
        return result.stdout
    except subprocess.CalledProcessError:
        return None

def extract_chapters_from_description(description):
    """Extract existing chapters from video description"""
    if not description:
        return []

    # Common chapter patterns: 0:00, 00:00, [0:00]
    chapter_pattern = r'(\[?\d{1,2}:\d{2}\]?)\s+(.+?)(?:\n|$)'
    matches = re.findall(chapter_pattern, description)

    chapters = []
    for timestamp, title in matches:
        # Clean up timestamp
        timestamp = timestamp.strip('[]')
        chapters.append({
            'timestamp': timestamp,
            'title': title.strip()
        })

    return chapters

def format_timestamp(seconds):
    """Convert seconds to MM:SS format"""
    minutes = int(seconds) // 60
    secs = int(seconds) % 60
    return f"{minutes}:{secs:02d}"

def analyze_transcript_for_chapters(transcript_data, num_segments=5):
    """Analyze transcript to suggest chapter breakpoints"""
    if not transcript_data or 'events' not in transcript_data:
        return []

    events = transcript_data['events']
    total_duration = events[-1].get('tStartMs', 0) / 1000 if events else 0

    # Collect all text segments with timestamps
    segments = []
    for event in events:
        if 'segs' in event:
            start_time = event.get('tStartMs', 0) / 1000
            text = ''.join([seg.get('utf8', '') for seg in event['segs']])
            if text.strip():
                segments.append({
                    'time': start_time,
                    'text': text.strip()
                })

    if not segments:
        return []

    # Divide into roughly equal segments
    segment_size = len(segments) // num_segments
    suggested_chapters = []

    for i in range(0, len(segments), segment_size):
        if i < len(segments):
            seg = segments[i]
            # Take first few words as chapter hint
            words = seg['text'].split()[:8]
            hint = ' '.join(words)
            suggested_chapters.append({
                'timestamp': format_timestamp(seg['time']),
                'time_seconds': seg['time'],
                'text_hint': hint + '...'
            })

    return suggested_chapters

def get_video_title(video_url):
    """Get video title"""
    cmd = ['yt-dlp', '--print', '%(title)s', video_url]
    try:
        result = subprocess.run(cmd, capture_output=True, text=True, check=True)
        return result.stdout.strip()
    except:
        return "Unknown"

def main(video_url):
    print("📹 YouTube Chapter Generator\n")

    # Get video title
    title = get_video_title(video_url)
    print(f"Video: {title}\n")

    # Check for existing chapters in description
    print("🔍 Checking for existing chapters in description...")
    description = get_video_description(video_url)
    existing_chapters = extract_chapters_from_description(description)

    if existing_chapters:
        print(f"✅ Found {len(existing_chapters)} existing chapters:\n")
        for chapter in existing_chapters:
            print(f"  {chapter['timestamp']} - {chapter['title']}")
        print("\n" + "="*70)
        return

    print("❌ No chapters found in description.\n")

    # Try to get transcript
    print("🔍 Attempting to download transcript...")
    transcript_data, video_id = get_transcript(video_url)

    if not transcript_data:
        print("❌ No transcript available for this video.")
        print("\n💡 Suggestions:")
        print("  1. Enable auto-generated subtitles on YouTube")
        print("  2. Manually watch the video and note major topic changes")
        print("  3. Use the video's natural sections (intro, main content, outro)")
        return

    print("✅ Transcript downloaded!\n")

    # Analyze transcript for chapter suggestions
    print("🤖 Analyzing transcript for chapter suggestions...\n")
    suggestions = analyze_transcript_for_chapters(transcript_data, num_segments=5)

    if suggestions:
        print("📝 Suggested Chapter Points (based on transcript segments):\n")
        print("Timestamp | Nearby Content")
        print("-" * 70)
        for suggestion in suggestions:
            print(f"{suggestion['timestamp']:<10} | {suggestion['text_hint']}")

        print("\n" + "="*70)
        print("\n💡 How to create chapters:")
        print("  1. Review the timestamps and nearby content above")
        print("  2. Watch those sections of the video")
        print("  3. Create descriptive chapter titles")
        print("  4. Add to video description in format: 0:00 Chapter Title")
        print("\n📋 Template:")
        for suggestion in suggestions:
            print(f"  {suggestion['timestamp']} [Your Chapter Title Here]")
    else:
        print("❌ Could not generate chapter suggestions from transcript.")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python3 generate-chapters.py <youtube-url>")
        print("\nExample:")
        print('  python3 generate-chapters.py "https://www.youtube.com/watch?v=BEG49WICGEo"')
        sys.exit(1)

    main(sys.argv[1])
