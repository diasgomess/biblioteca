using System;
using Biblioteca.Data;
using Biblioteca.Models;

namespace Biblioteca
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando sistema de biblioteca...");

            using (var context = new BibliotecaContext())
            {
                context.Database.EnsureCreated(); // cria o banco se não existir

                var livro = new Livro
                {
                    ISBN = "1234567890",
                    Titulo = "Programação em C#",
                    Autor = "Fulano",
                    Categoria = Categoria.TECNICO
                };

                context.Livros.Add(livro);
                context.SaveChanges();

                Console.WriteLine($"Livro '{livro.Titulo}' cadastrado com sucesso!");
            }
        }
    }
}
