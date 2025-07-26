using BookBooks.Data;
using BookBooks.Models;

namespace BookBooks.Repositories
{
    public class LivroRepository : ILivroRepository
    {
        private readonly BancoContext _bancoContext;

        public LivroRepository(BancoContext bancoContext) 
        {
            _bancoContext = bancoContext;
        }

        public List<LivroModel> ObterTodos()
        {
            return _bancoContext.Livros.ToList();
        }

        public LivroModel Adicionar(LivroModel livro)
        {
            _bancoContext.Livros.Add(livro);
            _bancoContext.SaveChanges();
            return livro;
        }
    }
}
