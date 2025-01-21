using System.Diagnostics;
using KVDB.Data;
using KVDB.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FFMpegCore;
using FFMpegCore.Pipes;

namespace KVDB.Controllers
{
    public class HomeController : Controller
    {

        private readonly KVDBContext _context;

        public HomeController(KVDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            if (_context.Transcript == null)
            {
                throw new Exception("Transcript table is empty");
            }


            // Select all entries
            var transcripts = from t in _context.Transcript
                              select t;

            if (String.IsNullOrEmpty(searchString))
            {
                return View(new List<Transcript>());
            }

            transcripts = transcripts.Include(s => s.Episode).Where(s => s.Text.ToUpper().Contains(searchString.ToUpper()));

            var transcriptsList = await transcripts.ToListAsync();

            return View(transcriptsList);
        }

        [HttpPost]
        public IActionResult Download(double from, double to, string videoFile)
        {
            Console.WriteLine(from + " " + to);

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
