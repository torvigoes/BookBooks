using System.ComponentModel.DataAnnotations;

namespace BookBooks.Models
{
    public class LivroModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Digite o título do livro")]
        public string? Titulo { get; set; }
        [Required(ErrorMessage = "Digite o autor do livro")]
        public string? Autor { get; set; }
        [Required(ErrorMessage = "Digite o ano de publicação do livro")]
        [Range(1450, 2025, ErrorMessage = "O ano de publicação deve ser válido.")]
        public int AnoPublicacao { get; set; }
    }
}
