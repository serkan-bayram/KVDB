using System.Diagnostics;
using KVDB.Data;
using KVDB.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FFMpegCore;

namespace KVDB.Controllers
{
    public class HomeController : Controller
    {

        private readonly KVDBContext _context;

        public HomeController(KVDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int page, bool isRandom = false)
        {
            if (_context.Transcript == null)
            {
                throw new Exception("Transcript table is empty");
            }

            //ViewData["BaseUrl"] = $"kvdb.serkanbayram.dev";

            if (String.IsNullOrEmpty(searchString) && !isRandom)
            {
                return View(new Search
                {
                    ItemsFound = 0,
                    TranscriptsList = new List<Transcript>()
                });
            }

            // Select all entries
            var transcripts = from t in _context.Transcript
                              select t;

            if (isRandom)
            {
                int count = await transcripts.CountAsync(); // Tablodaki toplam satır sayısını al
                int randomIndex = new Random().Next(0, count); // Rastgele bir index seç

                var randomTranscript = await transcripts.Include(s => s.Episode)
                    .Skip(randomIndex)
                    .Take(1)
                    .ToListAsync();

                return View(new Search { ItemsFound = 1, TranscriptsList = randomTranscript, CurrentPage = 1, SearchString = "", isRandom = true });
            }
            
            var pageSize = 20;

            var baseQuery = transcripts.Include(s => s.Episode).Where(s => s.Text.ToUpper().Contains(searchString.ToUpper()));

            var pagedTranscripts = baseQuery.Skip((page - 1) * pageSize).Take(pageSize);

            var transcriptsCount = await baseQuery.CountAsync();
            var transcriptsList = await pagedTranscripts.ToListAsync();

            var viewModel = new Search 
            { ItemsFound = transcriptsCount, TranscriptsList = transcriptsList, 
                CurrentPage = page, SearchString = searchString, isRandom = isRandom };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Download(double from, double to, string videoFile)
        {
            return View();

            var videoFilePath = Path.Combine(Directory.GetCurrentDirectory(), "PythonScripts", "files", videoFile);

            var tempFilePath = Path.GetTempFileName().Split("\\");

            tempFilePath[tempFilePath.Count() - 1] = "video.mp4";

            var tempFilePathString = string.Join("\\", tempFilePath);

            FFMpeg.SubVideo(videoFilePath, tempFilePathString, TimeSpan.FromSeconds(from), TimeSpan.FromSeconds(from + to));

            var fileStream = System.IO.File.OpenRead(tempFilePathString);

            //Kullanıcıya yanıt olarak stream'i gönder
            return File(fileStream, "video/mp4", "edited-video.mp4");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
