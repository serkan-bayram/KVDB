using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KVDB.Data;
using KVDB.Models;

namespace KVDB.Controllers
{
    public class TranscriptsController : Controller
    {
        private readonly KVDBContext _context;

        public TranscriptsController(KVDBContext context)
        {
            _context = context;
        }

        // GET: Transcripts
        public async Task<IActionResult> Index()
        {
            var kVDBContext = _context.Transcript.Include(t => t.Episode);
            return View(await kVDBContext.ToListAsync());
        }

        // GET: Transcripts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transcript = await _context.Transcript
                .Include(t => t.Episode)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transcript == null)
            {
                return NotFound();
            }

            return View(transcript);
        }

        // GET: Transcripts/Create
        public IActionResult Create()
        {
            ViewData["EpisodeId"] = new SelectList(_context.Set<Episode>(), "Id", "Id");
            return View();
        }

        // POST: Transcripts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,Start,Duration,EpisodeId")] Transcript transcript)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transcript);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EpisodeId"] = new SelectList(_context.Set<Episode>(), "Id", "Id", transcript.EpisodeId);
            return View(transcript);
        }

        // GET: Transcripts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transcript = await _context.Transcript.FindAsync(id);
            if (transcript == null)
            {
                return NotFound();
            }
            ViewData["EpisodeId"] = new SelectList(_context.Set<Episode>(), "Id", "Id", transcript.EpisodeId);
            return View(transcript);
        }

        // POST: Transcripts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,Start,Duration,EpisodeId")] Transcript transcript)
        {
            if (id != transcript.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transcript);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TranscriptExists(transcript.Id))
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
            ViewData["EpisodeId"] = new SelectList(_context.Set<Episode>(), "Id", "Id", transcript.EpisodeId);
            return View(transcript);
        }

        // GET: Transcripts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transcript = await _context.Transcript
                .Include(t => t.Episode)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transcript == null)
            {
                return NotFound();
            }

            return View(transcript);
        }

        // POST: Transcripts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transcript = await _context.Transcript.FindAsync(id);
            if (transcript != null)
            {
                _context.Transcript.Remove(transcript);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TranscriptExists(int id)
        {
            return _context.Transcript.Any(e => e.Id == id);
        }
    }
}
