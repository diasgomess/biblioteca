using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Models
{
    public enum StatusMulta
    {
        PENDENTE,
        PAGA
    }

    public class Multa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmprestimoId { get; set; }

        [ForeignKey("EmprestimoId")]
        public Emprestimo? Emprestimo { get; set; }

        public decimal Valor { get; set; }
        public StatusMulta Status { get; set; } = StatusMulta.PENDENTE;
    }
}
