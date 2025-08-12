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

        public LivroModel ListarPorId(int id)
        {
            return _bancoContext.Livros.FirstOrDefault(x => x.Id == id);
        }

        public LivroModel Adicionar(LivroModel livro)
        {
            _bancoContext.Livros.Add(livro);
            _bancoContext.SaveChanges();
            return livro;
        }

        public Boolean Remover(int id)
        {
            try
            {
                var livro = new LivroModel { Id = id };
                _bancoContext.Livros.Attach(livro);
                _bancoContext.Livros.Remove(livro);
                _bancoContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public LivroModel Atualizar(LivroModel livro)
        {
            LivroModel livroDB = ListarPorId(livro.Id);

            if (livroDB == null)
                throw new Exception("Houve um erro na atualização do Livro");
            else
            {
                livroDB.Titulo = livro.Titulo;
                livroDB.Autor = livro.Autor;
                livroDB.AnoPublicacao = livro.AnoPublicacao;
            }

            _bancoContext.Update(livroDB);
            _bancoContext.SaveChanges();

            return livroDB;
        }
    }
}
