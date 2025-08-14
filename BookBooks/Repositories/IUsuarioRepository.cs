using BookBooks.Models;

namespace BookBooks.Repositories
{
    public interface IUsuarioRepository
    {
        List<UsuarioModel> ObterTodos();
        UsuarioModel Adicionar(UsuarioModel livro);
        UsuarioModel Atualizar(UsuarioModel livro);
        Boolean Remover(int id);
        UsuarioModel ListarPorId(int id);
    }
}
