using BookBooks.Data;
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

        public IActionResult EditarLivro(int id)
        {
            LivroModel livro = _livroRepository.ListarPorId(id);
            return View(livro);
        }

        public IActionResult OpenDeleteModelPartial(int id)
        {
            LivroModel livro = _livroRepository.ListarPorId(id);
            return PartialView("~/Views/Shared/_DeleteBook.cshtml", livro);
        }

        [HttpPost]
        public IActionResult Criar(LivroModel livro)
        {
            if (ModelState.IsValid)
            {
                _livroRepository.Adicionar(livro);
                return RedirectToAction("Index");
            }
            else
                return View(livro);
        }

        [HttpPost]
        public IActionResult Alterar(LivroModel livro)
        {
            if (ModelState.IsValid)
            {
                _livroRepository.Atualizar(livro);
                return RedirectToAction("Index");
            }
            else
                return View("EditarLivro", livro);

        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            _livroRepository.Remover(id);
            return RedirectToAction("Index");
        }
    }
}
