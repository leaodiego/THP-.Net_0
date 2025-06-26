using Paradas.Models;

namespace Paradas.Interface
{
    public interface IJustificativaService
    {
        public IEnumerable<Justificativa> GetJustificativa(int paradaId);
        Task CreateJustificativa(Justificativa justificativa);
        Task UpdateJustificativa(Justificativa justificativa);
        Task DeleteJustificativa(int id);

    }
}
