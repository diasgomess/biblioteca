using System;

namespace Biblioteca.Exceptions
{
    public class RegraNegocioException : Exception
    {
        public RegraNegocioException(string mensagem) : base(mensagem) { }
    }
}
