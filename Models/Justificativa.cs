namespace Paradas.Models
{
    public class Justificativa
    {
        public int JustificativaId { get; set; }
        public string Motivo { get; set; }
        public DateTime DataInicio { get; set;}
        public DateTime DataFim { get; set;}
        public int TempoMin { get; set;}
    }
}
