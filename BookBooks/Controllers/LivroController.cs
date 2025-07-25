using Microsoft.AspNetCore.Mvc;

namespace BookBooks.Controllers
{
    public class LivroController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateBook()
        {
            return View();
        }

        public IActionResult UpdateBook()
        {
            return View();
        }

        public IActionResult DeleteBook()
        {
            return View();
        }

        public IActionResult DeleteModelPartial()
        {
            return PartialView("~/Views/Shared/_DeleteBook.cshtml");
        }
    }
}
