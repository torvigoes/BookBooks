using BookBooks.Models;

namespace BookBooks.Repositories
{
    public interface ILivroRepository
    {
        List<LivroModel> ObterTodos();
        LivroModel Adicionar(LivroModel livro);
        LivroModel Atualizar(LivroModel livro);
        Boolean Remover(int id);
        LivroModel ListarPorId(int id);
    }
}
