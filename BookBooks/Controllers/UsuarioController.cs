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
        public IActionResult OpenDeleteModelPartial(int id)
        {
            UsuarioModel user = _usuarioRepository.ListarPorId(id);
            return PartialView("~/Views/Shared/_DeleteUser.cshtml", user);
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

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            try
            {
                bool apagado = _usuarioRepository.Remover(id);

                if (apagado)
                    TempData["MsgSucesso"] = "Usuário removido com sucesso!";
                else
                    TempData["MsgSucesso"] = "Houve um erro ao remover o Usuário!";

                return RedirectToAction("Index");
            }
            catch (System.Exception erro)
            {
                TempData["MsgErro"] = $"Não foi possível excluir o usuário, tente novamente! Detalhes do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

    }
}
