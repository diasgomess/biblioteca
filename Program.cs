using System;
using Biblioteca.Data;
using Biblioteca.Models;
using Biblioteca.Services;
using Biblioteca.Exceptions;
using System.Linq;

namespace Biblioteca
{
    class Program
    {
        static void Main()
        {
            using var context = new BibliotecaContext();
            context.Database.EnsureCreated();

            var livroService = new LivroUsuarioService(context);
            var emprestimoService = new EmprestimoService(context);

            var usuario = new Usuario { Nome = "Maria", Email = "maria@email.com", Tipo = TipoUsuario.ALUNO };
            livroService.CadastrarUsuario(usuario);

            var livro = new Livro { ISBN = "123", Titulo = "Clean Code", Autor = "Robert C. Martin", Categoria = Categoria.TECNICO };
            livroService.CadastrarLivro(livro);

            try
            {
                // 1️⃣ Registrar o primeiro empréstimo (OK)
                emprestimoService.RegistrarEmprestimo("123", usuario.Id);

                // 2️⃣ Tentar emprestar o mesmo livro novamente (gera exceção)
                emprestimoService.RegistrarEmprestimo("123", usuario.Id);
            }
            catch (RegraNegocioException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Erro de regra de negócio: {ex.Message}");
                Console.ResetColor();
            }

            try
            {
                // 3️⃣ Simular devolução atrasada
                var emprestimo = context.Emprestimos.First();
                emprestimo.DataPrevistaDevolucao = DateTime.Now.AddDays(-4);
                context.SaveChanges();

                emprestimoService.RegistrarDevolucao(emprestimo.Id);
            }
            catch (RegraNegocioException ex)
            {
                Console.WriteLine($"⚠️ Erro: {ex.Message}");
            }
        }
    }
}
