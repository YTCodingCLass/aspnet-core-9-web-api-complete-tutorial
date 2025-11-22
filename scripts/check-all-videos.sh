#!/bin/bash

# Check all videos for chapters
echo "📊 Checking All Videos for Chapters"
echo "===================================="
echo ""

videos=(
    "https://www.youtube.com/watch?v=BEG49WICGEo|Chapter 01"
    "https://www.youtube.com/watch?v=kCVpIWl_nUk|Chapter 02-03"
    "https://www.youtube.com/watch?v=4BQniKV0zjg|Chapter 03"
    "https://www.youtube.com/watch?v=3pkXQIpd-Tk|Chapter 04"
    "https://www.youtube.com/watch?v=zwp3Qvxxzgc|Chapter 05"
    "https://www.youtube.com/watch?v=JlJGgamL5Iw|Chapter 06"
    "https://www.youtube.com/watch?v=8Q0cU9Gq8Co|Chapter 07"
    "https://www.youtube.com/watch?v=ZoW5RnvgeSo|Chapter 08"
    "https://www.youtube.com/watch?v=Efmvo90NuiA|Chapter 09"
)

has_chapters=()
no_chapters=()

for video in "${videos[@]}"; do
    IFS='|' read -r url chapter <<< "$video"
    echo "Checking $chapter..."

    # Get description and check for timestamps
    desc=$(yt-dlp --print "%(description)s" "$url" 2>/dev/null)

    if echo "$desc" | grep -qE '^\[?[0-9]{1,2}:[0-9]{2}\]?'; then
        has_chapters+=("$chapter")
        echo "  ✅ Has chapters"
    else
        no_chapters+=("$chapter")
        echo "  ❌ No chapters"
    fi
    echo ""
done

echo "===================================="
echo ""
echo "📊 Summary:"
echo ""
echo "✅ Videos WITH chapters (${#has_chapters[@]}):"
for ch in "${has_chapters[@]}"; do
    echo "  - $ch"
done

echo ""
echo "❌ Videos WITHOUT chapters (${#no_chapters[@]}):"
for ch in "${no_chapters[@]}"; do
    echo "  - $ch"
done
