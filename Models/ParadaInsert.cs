using System.ComponentModel.DataAnnotations;

namespace Paradas.Models
{
    public class ParadaInsert
    {
        public int sbId { get; set; }
        public int idTrem { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int Tempo { get; set; }
        public string Usuario { get; set; }
    }
}
