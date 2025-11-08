using System;
using System.Linq;
using Biblioteca.Data;
using Biblioteca.Models;

namespace Biblioteca.Services
{
    public class RelatorioService
    {
        private readonly BibliotecaContext _context;

        public RelatorioService(BibliotecaContext context)
        {
            _context = context;
        }

        public void ListarLivrosMaisEmprestados()
        {
            var ranking = _context.Emprestimos
                .GroupBy(e => e.LivroISBN)
                .Select(g => new { ISBN = g.Key, Total = g.Count() })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToList();

            Console.WriteLine("\n📘 Livros mais emprestados:");
            foreach (var item in ranking)
            {
                var livro = _context.Livros.FirstOrDefault(l => l.ISBN == item.ISBN);
                Console.WriteLine($"{livro?.Titulo} - {item.Total} empréstimos");
            }
        }

        public void ListarUsuariosComMaisEmprestimos()
        {
            var ranking = _context.Emprestimos
                .GroupBy(e => e.UsuarioId)
                .Select(g => new { UsuarioId = g.Key, Total = g.Count() })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToList();

            Console.WriteLine("\n👥 Usuários com mais empréstimos:");
            foreach (var item in ranking)
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == item.UsuarioId);
                Console.WriteLine($"{usuario?.Nome} - {item.Total} empréstimos");
            }
        }

        public void ListarEmprestimosEmAtraso()
        {
            var atrasados = _context.Emprestimos
                .Where(e => e.Status == StatusEmprestimo.ATIVO && e.DataPrevistaDevolucao < DateTime.Now)
                .ToList();

            Console.WriteLine("\n⏰ Empréstimos em atraso:");
            foreach (var e in atrasados)
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == e.UsuarioId);
                var livro = _context.Livros.FirstOrDefault(l => l.ISBN == e.LivroISBN);
                Console.WriteLine($"Livro: {livro?.Titulo} | Usuário: {usuario?.Nome} | Previsto: {e.DataPrevistaDevolucao:d}");
            }
        }
    }
}
