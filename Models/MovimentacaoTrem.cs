namespace Paradas.Models
{
    public class MovimentacaoTrem
    {
        public int MoviId { get; set; }
        public string AreaOperMovimentacao { get; set; }
        public DateTime? InicioMovimentacao { get; set; }
        public DateTime? FimMovimentacao { get; set; }
    }
}
