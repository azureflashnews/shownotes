cd working
del *.* /Q
cd ..
ffmpeg\ffmpeg -i "c:\users\mgarner\downloads\afn-090.mp4" -vn -c:a copy "working/afn-090.m4a"
ffmpeg\ffprobe -v error -show_entries stream=duration -i working\afn-090.m4a -of default=noprint_wrappers=1:nokey=1 > working\length.out