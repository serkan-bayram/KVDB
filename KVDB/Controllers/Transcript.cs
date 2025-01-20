using Microsoft.AspNetCore.Mvc;

namespace KVDB.Controllers
{
    public class Transcript : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
