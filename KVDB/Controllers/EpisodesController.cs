using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KVDB.Data;
using KVDB.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KVDB.Controllers
{
    public static class JsonFileReader
    {
        public static T Read<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<T>(text, options);
        }
    }

    public class TranscriptLine
    {
        public required string Text { get; set; }
        public decimal Start { get; set; }
        public decimal Duration { get; set; }
    }

    public class EpisodesController : Controller
    {
        private readonly KVDBContext _context;

        public EpisodesController(KVDBContext context)
        {
            _context = context;
        }

        // GET: Episodes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Episode.ToListAsync());
        }

        // GET: Episodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episode
                .FirstOrDefaultAsync(m => m.Id == id);
            if (episode == null)
            {
                return NotFound();
            }

            return View(episode);
        }

        // GET: Episodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Episodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,YoutubeId")] Episode episode)
        {
            if (ModelState.IsValid)
            {
                _context.Add(episode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(episode);
        }



        // Files are episodes and transcripts
        // This api saves transcripts and episodes to the database
        public async Task<string> HandleFiles()
        {
            var filesPath = "C:\\Users\\Serkan\\Programming\\KVDB\\KVDB\\PythonScripts\\files\\";

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
                string? episodePath = null;

                foreach (var file in files)
                {

                    if (file.Contains("transcript"))
                    {
                        transcriptPath = file;
                    }
                    else
                    {
                        // It's the episode title
                        episodePath = file;
                    }

                }

                if (transcriptPath == null || episodePath == null)
                {
                    return "Transcript or episode file not found";
                }

                var youtubeId = episode.Split("\\").Last();
                var title = episodePath.Split("\\").Last();

                var isAlreadySaved = _context.Episode.Any(Episode => Episode.YoutubeId == youtubeId);

                if(isAlreadySaved)
                {
                    return "Episode already saved";
                }

                var newEpisode = new Episode { Title = title, YoutubeId = youtubeId };

                _context.Episode.Add(newEpisode);
                await _context.SaveChangesAsync();

                Console.WriteLine(transcriptPath);

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

        // GET: Episodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episode.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }
            return View(episode);
        }

        // POST: Episodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,YoutubeId")] Episode episode)
        {
            if (id != episode.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(episode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpisodeExists(episode.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(episode);
        }

        // GET: Episodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episode
                .FirstOrDefaultAsync(m => m.Id == id);
            if (episode == null)
            {
                return NotFound();
            }

            return View(episode);
        }

        // POST: Episodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var episode = await _context.Episode.FindAsync(id);
            if (episode != null)
            {
                _context.Episode.Remove(episode);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EpisodeExists(int id)
        {
            return _context.Episode.Any(e => e.Id == id);
        }
    }
}
