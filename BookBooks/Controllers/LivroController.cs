using BookBooks.Models;
using BookBooks.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookBooks.Controllers
{
    public class LivroController : Controller
    {
        private readonly ILivroRepository _livroRepository;
        public LivroController(ILivroRepository livroRepository)
        {
            _livroRepository = livroRepository;
        }

        public IActionResult Index()
        {
            List<LivroModel> livros = _livroRepository.ObterTodos();
            return View(livros);
        }

        public IActionResult AdicionarLivro()
        {
            return View();
        }

        public IActionResult EditarLivro()
        {
            return View();
        }

        public IActionResult ExcluirLivro()
        {
            return View();
        }

        public IActionResult OpenDeleteModelPartial()
        {
            return PartialView("~/Views/Shared/_DeleteBook.cshtml");
        }

        [HttpPost]
        public IActionResult Criar(LivroModel livro)
        {
            _livroRepository.Adicionar(livro);
            return RedirectToAction("Index");
        }
    }
}
