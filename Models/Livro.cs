using System;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public enum Categoria
    {
        FICCAO,
        TECNICO,
        DIDATICO
    }

    public enum StatusLivro
    {
        DISPONIVEL,
        EMPRESTADO,
        RESERVADO
    }

    public class Livro
    {
        [Key]
        public string ISBN { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public Categoria Categoria { get; set; }
        public StatusLivro Status { get; set; } = StatusLivro.DISPONIVEL;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}
