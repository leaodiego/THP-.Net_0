using Paradas.Models;

namespace Paradas.Interface
{
    public interface IMovimentacaoTremService
    {
        public IEnumerable<MovimentacaoTrem> GetMovimentacao(int os, string ao );
    }
}
