using System;
using System.Linq;
using Biblioteca.Data;
using Biblioteca.Models;
using Biblioteca.Exceptions;

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

        // 📥 Registrar empréstimo com validações
        public void RegistrarEmprestimo(string isbn, int usuarioId)
        {
            var livro = _context.Livros.FirstOrDefault(l => l.ISBN == isbn);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);

            if (livro == null)
                throw new RegraNegocioException("Livro não encontrado.");

            if (usuario == null)
                throw new RegraNegocioException("Usuário não encontrado.");

            if (livro.Status != StatusLivro.DISPONIVEL)
                throw new RegraNegocioException("Este livro já está emprestado ou reservado.");

            if (_livroUsuarioService.UsuarioAtingiuLimiteEmprestimos(usuarioId))
                throw new RegraNegocioException("Usuário atingiu o limite de 3 empréstimos ativos.");

            bool possuiMultaPendente = _context.Multas
                .Any(m => m.Emprestimo != null &&
                          m.Emprestimo.UsuarioId == usuarioId &&
                          m.Status == StatusMulta.PENDENTE);


            if (possuiMultaPendente)
                throw new RegraNegocioException("Usuário possui multa pendente. Regularize antes de novo empréstimo.");

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

            Console.WriteLine($"✅ Empréstimo registrado com sucesso para o livro '{livro.Titulo}'.");
        }

        // 📤 Registrar devolução com validações e multa
        public void RegistrarDevolucao(int emprestimoId)
        {
            var emprestimo = _context.Emprestimos.FirstOrDefault(e => e.Id == emprestimoId);
            if (emprestimo == null)
                throw new RegraNegocioException("Empréstimo não encontrado.");

            if (emprestimo.Status != StatusEmprestimo.ATIVO)
                throw new RegraNegocioException("Este empréstimo já foi finalizado ou não está ativo.");

            emprestimo.DataRealDevolucao = DateTime.Now;

            var livro = _context.Livros.FirstOrDefault(l => l.ISBN == emprestimo.LivroISBN);
            if (livro == null)
                throw new RegraNegocioException("Livro vinculado ao empréstimo não encontrado.");

            // Cálculo de multa automática
            if (emprestimo.DataRealDevolucao > emprestimo.DataPrevistaDevolucao)
            {
                int diasAtraso = (emprestimo.DataRealDevolucao.Value - emprestimo.DataPrevistaDevolucao).Days;
                decimal valorMulta = diasAtraso * 1.0m;

                var multa = new Multa
                {
                    EmprestimoId = emprestimo.Id,
                    Valor = valorMulta,
                    Status = StatusMulta.PENDENTE
                };

                _context.Multas.Add(multa);
                Console.WriteLine($"⚠️ Multa gerada automaticamente: R${valorMulta}");
            }

            // Atualizações finais
            emprestimo.Status = StatusEmprestimo.FINALIZADO;
            livro.Status = StatusLivro.DISPONIVEL;

            _context.SaveChanges();
            Console.WriteLine($"📚 Devolução registrada com sucesso para o empréstimo #{emprestimo.Id}");
        }
    }
}
