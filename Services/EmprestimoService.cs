using System;
using System.Linq;
using Biblioteca.Data;
using Biblioteca.Models;

namespace Biblioteca.Services
{
    public class EmprestimoService
    {
        private readonly BibliotecaContext _context;
        private readonly LivroUsuarioService _livroUsuarioService;

        public EmprestimoService(BibliotecaContext context)
        {
            _context = context;
            _livroUsuarioService = new LivroUsuarioService(context);
        }

        // 📥 Registrar empréstimo
        public void RegistrarEmprestimo(string isbn, int usuarioId)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.ISBN == isbn);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);

            if (livro == null) throw new Exception("Livro não encontrado.");
            if (usuario == null) throw new Exception("Usuário não encontrado.");
            if (livro.Status != StatusLivro.DISPONIVEL)
                throw new Exception("Livro não está disponível para empréstimo.");
            if (_livroUsuarioService.UsuarioAtingiuLimiteEmprestimos(usuarioId))
                throw new Exception("Usuário já atingiu o limite de 3 empréstimos ativos.");

            int diasPrazo = usuario.Tipo == TipoUsuario.PROFESSOR ? 15 : 7;

            var emprestimo = new Emprestimo
            {
                LivroISBN = livro.ISBN,
                UsuarioId = usuario.Id,
                DataEmprestimo = DateTime.Now,
                DataPrevistaDevolucao = DateTime.Now.AddDays(diasPrazo),
                Status = StatusEmprestimo.ATIVO
            };

            livro.Status = StatusLivro.EMPRESTADO;
            _context.Emprestimos.Add(emprestimo);
            _context.SaveChanges();

            Console.WriteLine($"✅ Empréstimo registrado: {livro.Titulo} para {usuario.Nome}");
        }

        // 📤 Registrar devolução
        public void RegistrarDevolucao(int emprestimoId)
        {
            var emprestimo = _context.Emprestimos.FirstOrDefault(e => e.Id == emprestimoId);
            if (emprestimo == null) throw new Exception("Empréstimo não encontrado.");

            emprestimo.DataRealDevolucao = DateTime.Now;
            emprestimo.Status = StatusEmprestimo.FINALIZADO;

            var livro = _context.Livros.First(l => l.ISBN == emprestimo.LivroISBN);
            livro.Status = StatusLivro.DISPONIVEL;

            // Calcula multa se houver atraso
            if (emprestimo.DataRealDevolucao > emprestimo.DataPrevistaDevolucao)
            {
                int diasAtraso = (emprestimo.DataRealDevolucao.Value - emprestimo.DataPrevistaDevolucao).Days;
                decimal valorMulta = diasAtraso * 1.0m; // R$1 por dia

                var multa = new Multa
                {
                    EmprestimoId = emprestimo.Id,
                    Valor = valorMulta,
                    Status = StatusMulta.PENDENTE
                };

                _context.Multas.Add(multa);
                Console.WriteLine($"⚠️ Multa gerada: R${valorMulta}");
            }

            _context.SaveChanges();
            Console.WriteLine($"📚 Devolução registrada para o empréstimo #{emprestimo.Id}");
        }
    }
}
