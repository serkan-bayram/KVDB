using System.Diagnostics;
using KVDB.Data;
using KVDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            if (!String.IsNullOrEmpty(searchString))
            {
                transcripts = transcripts.Where(s => s.Text.ToUpper().Contains(searchString.ToUpper()));
            }
            else
            {
                return View();
            }

            return View(await transcripts.ToListAsync());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
