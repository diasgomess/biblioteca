using System;
using Biblioteca.Data;
using Biblioteca.Models;
using Biblioteca.Services;

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
            var relatorioService = new RelatorioService(context);

            // Cadastra um usuário e um livro
            var usuario = new Usuario { Nome = "João Silva", Email = "joao@email.com", Tipo = TipoUsuario.ALUNO };
            livroService.CadastrarUsuario(usuario);

            var livro = new Livro { ISBN = "001", Titulo = "C# Básico", Autor = "Fulano", Categoria = Categoria.TECNICO };
            livroService.CadastrarLivro(livro);

            // Empréstimo
            emprestimoService.RegistrarEmprestimo("001", usuario.Id);

            // Simular devolução atrasada
            var emprestimo = context.Emprestimos.First();
            emprestimo.DataPrevistaDevolucao = DateTime.Now.AddDays(-3); // atrasado
            context.SaveChanges();

            emprestimoService.RegistrarDevolucao(emprestimo.Id);

            // Relatórios
            relatorioService.ListarLivrosMaisEmprestados();
            relatorioService.ListarUsuariosComMaisEmprestimos();
            relatorioService.ListarEmprestimosEmAtraso();
        }
    }
}
