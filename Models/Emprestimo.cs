using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public enum StatusEmprestimo
    {
        ATIVO,
        FINALIZADO,
        ATRASADO
    }

    public class Emprestimo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string LivroISBN { get; set; } = string.Empty;

        [ForeignKey("LivroISBN")]
        public Livro? Livro { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public DateTime DataEmprestimo { get; set; } = DateTime.Now;
        public DateTime DataPrevistaDevolucao { get; set; }
        public DateTime? DataRealDevolucao { get; set; }
        public StatusEmprestimo Status { get; set; } = StatusEmprestimo.ATIVO;
    }
}
