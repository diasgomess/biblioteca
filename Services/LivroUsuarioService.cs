using System;
using System.Linq;
using Biblioteca.Data;
using Biblioteca.Models;

namespace Biblioteca.Services
{
    public class LivroUsuarioService
    {
        private readonly BibliotecaContext _context;

        public LivroUsuarioService(BibliotecaContext context)
        {
            _context = context;
        }

        // 📘 Cadastro de livro
        public void CadastrarLivro(Livro livro)
        {
            _context.Livros.Add(livro);
            _context.SaveChanges();
        }

        // 👤 Cadastro de usuário
        public void CadastrarUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        // 🔁 Atualização de status do livro
        public void AtualizarStatusLivro(string isbn, StatusLivro novoStatus)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.ISBN == isbn);
            if (livro == null)
                throw new Exception("Livro não encontrado.");

            livro.Status = novoStatus;
            _context.SaveChanges();
        }

        // ⚠️ Validação: usuário com mais de 3 empréstimos ativos
        public bool UsuarioAtingiuLimiteEmprestimos(int usuarioId)
        {
            int ativos = _context.Emprestimos.Count(e => e.UsuarioId == usuarioId && e.Status == StatusEmprestimo.ATIVO);
            return ativos >= 3;
        }
    }
}
