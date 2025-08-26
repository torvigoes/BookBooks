using BookBooks.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookBooks.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Digite o nome do usuário")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Digite o login do usuário")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O login deve ter entre 3 e 50 caracteres")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Digite o email do usuário")]
        [EmailAddress(ErrorMessage = "Digite um email válido")]
        [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe o perfil do usuário")]
        public PerfilEnum? Perfil { get; set; }
    }
}