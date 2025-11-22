#!/bin/bash

# YouTube Video Information Extractor using yt-dlp
# Usage: ./get-video-info.sh <youtube-url>

if [ -z "$1" ]; then
    echo "Usage: $0 <youtube-url>"
    echo "Example: $0 https://www.youtube.com/watch?v=VIDEO_ID"
    exit 1
fi

VIDEO_URL="$1"

echo "📺 Extracting video information..."
echo ""

# Get title
TITLE=$(yt-dlp --print "%(title)s" "$VIDEO_URL" 2>/dev/null)
echo "Title: $TITLE"

# Get duration in seconds
DURATION_SECONDS=$(yt-dlp --print "%(duration)s" "$VIDEO_URL" 2>/dev/null)

# Convert to minutes:seconds
MINUTES=$((DURATION_SECONDS / 60))
SECONDS=$((DURATION_SECONDS % 60))
echo "Duration: ${MINUTES}m ${SECONDS}s (${DURATION_SECONDS} seconds)"

# Get upload date
UPLOAD_DATE=$(yt-dlp --print "%(upload_date)s" "$VIDEO_URL" 2>/dev/null)
echo "Upload Date: $UPLOAD_DATE"

# Get view count
VIEWS=$(yt-dlp --print "%(view_count)s" "$VIDEO_URL" 2>/dev/null)
echo "Views: $VIEWS"

# Get description (first 200 characters)
DESCRIPTION=$(yt-dlp --print "%(description)s" "$VIDEO_URL" 2>/dev/null | head -c 200)
echo ""
echo "Description (first 200 chars):"
echo "$DESCRIPTION..."

echo ""
echo "✅ Done!"
