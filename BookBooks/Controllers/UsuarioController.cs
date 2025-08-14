using BookBooks.Models;
using BookBooks.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookBooks.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public IActionResult Index()
        {
            List<UsuarioModel> usuarios = _usuarioRepository.ObterTodos();
            return View(usuarios);
        }

        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(UsuarioModel usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _usuarioRepository.Adicionar(usuario);
                    TempData["MsgSucesso"] = "Usuário cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                    return View(usuario);
            }
            catch (System.Exception erro)
            {
                TempData["MsgErro"] = $"Não foi possível cadastrar o Usuário, tente novamente! Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
