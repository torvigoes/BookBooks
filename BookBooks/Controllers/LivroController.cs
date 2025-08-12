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
            try
            {
                if (ModelState.IsValid)
                {
                    _livroRepository.Adicionar(livro);
                    TempData["MsgSucesso"] = "Livro cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                    return View(livro);
            }
            catch (System.Exception erro)
            {
                TempData["MsgErro"] = $"Não foi possível cadastrar o livro, tente novamente! Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Alterar(LivroModel livro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _livroRepository.Atualizar(livro);
                    TempData["MsgSucesso"] = "Livro alterado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                    return View("EditarLivro", livro);
            }
            catch (System.Exception erro)
            {
                TempData["MsgErro"] = $"Não foi possível atualizar o livro, tente novamente! Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            try
            {
                bool apagado = _livroRepository.Remover(id);

                if (apagado)
                    TempData["MsgSucesso"] = "Livro removido com sucesso!";
                else
                    TempData["MsgSucesso"] = "Houve um erro ao remover o livro!";

                return RedirectToAction("Index");
            }
            catch(System.Exception erro)
            {
                TempData["MsgErro"] = $"Não foi possível excluir o livro, tente novamente! Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
