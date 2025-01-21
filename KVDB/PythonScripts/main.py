from youtube_transcript_api import YouTubeTranscriptApi
from pytubefix import YouTube, Playlist
import json
import os


def download_video(video_id):
    yt = YouTube(
        f'https://youtu.be/{video_id}', 'WEB')

    yt.streams.first().download(f"files/{video_id}")


def download_captions(video_id):
    transcript = YouTubeTranscriptApi.get_transcript(
        video_id, languages=['tr'])

    json_transcript = json.dumps(transcript)

    with open(f"files/{video_id}/transcript.json", "w", encoding="utf-8") as f:
        f.write(json_transcript)


def main():
    playlist = Playlist(
        "https://www.youtube.com/playlist?list=PLpwZTEbwdv2eIG-TOnJngfybSoB4QZVp5")

    if not os.path.exists('files'):
        os.mkdir("files")

    for video in playlist.videos:
        try:
            video_id = video.video_id

            if os.path.exists(f'files/{video_id}'):
                continue

            if not os.path.exists(f'files/{video_id}'):
                os.mkdir(f'files/{video_id}')

            print(f"Downloading {video_id}")

            download_captions(video_id)
            download_video(video_id)
        except:
            print("err: ", video.video_id)
            continue


main()
