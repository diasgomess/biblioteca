using System;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public enum TipoUsuario
    {
        ALUNO,
        PROFESSOR,
        FUNCIONARIO
    }

    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public TipoUsuario Tipo { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}
