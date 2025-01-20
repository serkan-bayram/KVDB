from youtube_transcript_api import YouTubeTranscriptApi
from pytubefix import YouTube
import json
import os


def download_video(VIDEO_ID):
    YouTube(
        f'https://youtu.be/{VIDEO_ID}').streams.first().download(f"files/{VIDEO_ID}")


def download_captions(VIDEO_ID):
    transcript = YouTubeTranscriptApi.get_transcript(
        VIDEO_ID, languages=['tr'])

    json_transcript = json.dumps(transcript)

    with open(f"files/{VIDEO_ID}/transcript.json", "w") as f:
        f.write(json_transcript)


def main():
    VIDEO_ID = "DkAHxvoA2Q8"

    if not os.path.exists('files'):
        os.mkdir("files")

    if not os.path.exists(f'files/{VIDEO_ID}'):
        os.mkdir(f'files/{VIDEO_ID}')

    download_captions(VIDEO_ID)
    download_video(VIDEO_ID)


main()
