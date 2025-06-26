using Oracle.ManagedDataAccess.Client;
using Paradas.Interface;
using Paradas.Models;
using System.Data;
using UnilogApi.Persistencia;

namespace Paradas.Services
{
    public class TremService : ITremService
    {
        #region Metodo construtor com o banco UNILOG
        private DataAccess conexao;

        public TremService(string connstring, bool activeLog)
        {
            this.conexao = new DataAccess();
        }
        #endregion

        #region Metodos GET 

        #region GetParadas
        public Trem GetTrem(int NumOS)
        {
            Trem trem = new Trem();

            try
            {
                var parametros = new List<OracleParameter>
                {
                    conexao.GetOracleParameter("NumOs", OracleDbType.Int32, (NumOS))
                };

                String sql = String.Format(@"SELECT x1.X1_ID_OS as Id_Os
                                                   ,x1.X1_NRO_OS as Os
                                                   ,tr.TR_PFX_TRM as Prefixo
                                                   ,ep.EP_SIG_EMP || ' - ' || ep.EP_DSC_RSM as Ferrovia
                                                   ,aoo.AO_COD_AOP as Origem
                                                   ,aod.AO_COD_AOP as Destino
                                                   ,tr.TR_DTR_PRT as Partida
                                                   ,tr.TR_DTR_CHG as Chegada
                                               FROM T2_OS x1
                                                    JOIN TREM tr ON tr.OF_ID_OSV = x1.X1_ID_OS
                                                    JOIN AREA_OPERACIONAL aoo ON aoo.AO_ID_AO = x1.X1_IDT_EST_ORI
                                                    JOIN AREA_OPERACIONAL aod ON aod.AO_ID_AO = X1.X1_IDT_EST_DES
                                                    JOIN EMPRESA ep ON ep.EP_ID_EMP = x1.x1_IDT_EMP
                                              WHERE x1.X1_NRO_OS = :NumOS");

                DataTable dt = conexao.LeDadosParams<OracleConnection>(sql, parametros);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    trem.OsId = Convert.ToInt32(row["Id_Os"]);
                    trem.NumOs = Convert.ToInt32(row["Os"]);
                    trem.Prefixo = row["Prefixo"]?.ToString() ?? string.Empty;
                    trem.Ferrovia = row["Ferrovia"]?.ToString() ?? string.Empty;
                    trem.Origem = row["Origem"]?.ToString() ?? string.Empty;
                    trem.Destino = row["Destino"]?.ToString() ?? string.Empty;

                    DateTime.TryParse(row["Partida"]?.ToString(), out DateTime partida);
                    DateTime.TryParse(row["Chegada"]?.ToString(), out DateTime chegada);
                    trem.DataRealSaida = partida;
                    trem.DataRealchegada = chegada;

                }

            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível recuperar a os  " + NumOS + ": " + ex.ToString());
            }
            return trem;
        }
        #endregion

        #endregion
    }

}
