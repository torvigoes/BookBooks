using BookBooks.Data;
using BookBooks.Models;

namespace BookBooks.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly BancoContext _bancoContext;

        public UsuarioRepository(BancoContext bancoContext) 
        {
            _bancoContext = bancoContext;
        }

        public List<UsuarioModel> ObterTodos()
        {
            return _bancoContext.Usuarios.ToList();
        }

        public UsuarioModel ListarPorId(int id)
        {
            if (_bancoContext.Usuarios.Any()) 
            {
                return _bancoContext.Usuarios.FirstOrDefault(x => x.Id == id);
            }
            else
            {
                throw new Exception("O usuário não existe.");
            }
        }

        public UsuarioModel Adicionar(UsuarioModel usuario)
        {
            usuario.DataCadastro = DateTime.Now;
            _bancoContext.Usuarios.Add(usuario);
            _bancoContext.SaveChanges();
            return usuario;
        }

        public Boolean Remover(int id)
        {
            try
            {
                var usuario = new UsuarioModel { Id = id };
                _bancoContext.Usuarios.Attach(usuario);
                _bancoContext.Usuarios.Remove(usuario);
                _bancoContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public UsuarioModel Atualizar(UsuarioModel usuario)
        {
            UsuarioModel usuarioDB = ListarPorId(usuario.Id);

            if (usuarioDB == null)
                throw new Exception("Houve um erro na atualização do Usuário.");
            else
            {
                usuarioDB.Perfil = usuario.Perfil;
                usuarioDB.Nome = usuario.Nome;
                usuarioDB.Login = usuario.Login;
                usuarioDB.Email = usuario.Email;
                usuarioDB.DataAlteracao = DateTime.Now;
            }

            _bancoContext.Update(usuarioDB);
            _bancoContext.SaveChanges();

            return usuarioDB;
        }
    }
}
