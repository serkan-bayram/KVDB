using KVDB.Data;
using KVDB.Helpers;
using KVDB.Models;
using Microsoft.AspNetCore.Mvc;

namespace KVDB.Controllers
{

    public class ActionsController : Controller
    {

        private readonly KVDBContext _context;

        public ActionsController(KVDBContext context)
        {
            _context = context;
        }

        // Files are episodes and transcripts
        // This api saves transcripts and episodes to the database
        // TODO: This should be POST
        public async Task<string> HandleFiles()
        {
            var filesPath = Path.Combine(Directory.GetCurrentDirectory(), "PythonScripts", "files");

            if (!Directory.Exists(filesPath))
            {
                return "Directory does not exist";
            }

            // Files directory has directories named as youtube video ids (episodes)
            var episodes = Directory.GetDirectories(filesPath);

            foreach (var episode in episodes)
            {
                // There will be two files: transcript and episode video, but order can change that's why we check Contains in a foreach loop
                var files = Directory.GetFiles(episode);

                string? transcriptPath = null;

                foreach (var file in files)
                {
                    transcriptPath = file;
                }

                if (transcriptPath == null)
                {
                    Console.WriteLine("Transcript or episode file not found: ");
                    continue;
                }

                // !!!!!!! BURası windowsda \\ olmalı
                var youtubeId = episode.Split("/").Last();

                var title = "Video";

                var isAlreadySaved = _context.Episode.Any(Episode => Episode.YoutubeId == youtubeId);

                if (isAlreadySaved)
                {
                    continue;
                }

                var newEpisode = new Episode { Title = title, YoutubeId = youtubeId };

                _context.Episode.Add(newEpisode);
                await _context.SaveChangesAsync();

                List<TranscriptLine> transcriptLines = JsonFileReader.Read<List<TranscriptLine>>(transcriptPath);

                foreach (var transcriptLine in transcriptLines)
                {
                    var newTranscript = new Transcript { Text = transcriptLine.Text, Start = transcriptLine.Start, Duration = transcriptLine.Duration, EpisodeId = newEpisode.Id, Episode = newEpisode };

                    _context.Transcript.Add(newTranscript);
                }

                await _context.SaveChangesAsync();
            }

            return "ok";
        }

    }
}
