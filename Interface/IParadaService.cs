using Paradas.Models;
using UnilogApi.Persistencia;

namespace Paradas.Interface
{
    public interface IParadaService
    {
        public IEnumerable<Parada> GetParada(int os);
        public IEnumerable<AO> GetAo(int os);
        public Parada GetParadaById(int id);
        Task<bool> AtualizarParadaAsync(int id, Parada dto);
        public bool InserirParada(ParadaInsert parada);
        public bool ExcluirParada(int paradaId);
    }
}
