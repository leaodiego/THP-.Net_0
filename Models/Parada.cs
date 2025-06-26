using System.ComponentModel.DataAnnotations;

namespace Paradas.Models
{
    public class Parada
    {
        public int ParadaId { get; set; }
        public int sbId { get; set; }
        public string Local { get; set; }
        public string UsuarioResponsavel { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int idTrem {  get; set; }
        public int Tempo { get; set; }
        public string Programacao { get; set; }// para saber se é manual ou não
    }
}
