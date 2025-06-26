namespace Paradas.Models
{
    public class Trem
    {
        public int OsId { get; set; }
        public int NumOs { get; set; }
        public string Prefixo { get; set; }
        public string Ferrovia { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime DataRealSaida { get; set; }
        public DateTime DataRealchegada { get; set; }
    }
}
