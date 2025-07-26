using BookBooks.Models;

namespace BookBooks.Repositories
{
    public interface ILivroRepository
    {
        List<LivroModel> ObterTodos();
        LivroModel Adicionar(LivroModel livro);
    }
}
