using Oracle.ManagedDataAccess.Client;
using Paradas.Interface;
using Paradas.Models;
using System.Data;
using System.Linq.Expressions;
using UnilogApi.Persistencia;

namespace Paradas.Services
{
    public class MovimentacaoTremService : IMovimentacaoTremService
    {
        #region Metodo construtor com o banco UNILOG
        private DataAccess conexao;

        public MovimentacaoTremService(string connstring, bool activeLog)
        {
            this.conexao = new DataAccess();
        }
        #endregion

        IEnumerable<MovimentacaoTrem> IMovimentacaoTremService.GetMovimentacao(int os, string ao)
        {
            List<MovimentacaoTrem> listMovimentacao = new List<MovimentacaoTrem>();

            try
            {
                var parametros = new List<OracleParameter>
                {
                    conexao.GetOracleParameter("NumOs", OracleDbType.Int32, os),
                    conexao.GetOracleParameter("'Aoper'", OracleDbType.Varchar2, ao)
                };


                string sql = string.Format(@"SELECT mt.MT_ID_MOV
                                                   ,x1.X1_PFX_TRE 
                                                   ,aos.AO_COD_AOP
                                                   ,ar.AA_SEQ_RTA
                                                   ,mt.MT_DTC_RAL
                                                   ,mt.MT_DTS_RAL
                                               FROM T2_os x1 
                                                    JOIN TREM tr ON tr.OF_ID_OSV = x1.X1_ID_OS 
                                                    JOIN MOVIMENTACAO_TREM mt ON mt.TR_ID_TRM = tr.TR_ID_TRM
                                                    LEFT JOIN AREAOPER_ROTA ar ON ar.AA_ID_AOR = mt.AA_ID_AOR
  	                                              JOIN AREA_OPERACIONAL aof ON aof.AO_ID_AO = ar.AO_ID_AO                       
                                                    JOIN AREA_OPERACIONAL aos ON aos.AO_ID_AO = nvl(aof.AO_ID_AO_inf,aof.AO_ID_AO)
                                              WHERE x1.X1_NRO_OS = :NumOs
                                                AND aos.AO_COD_AOP = :Aoper
                                              ORDER BY ar.AA_SEQ_RTA");

                DataTable dt = conexao.LeDadosParams<OracleConnection>(sql, parametros);


                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        MovimentacaoTrem mov = new MovimentacaoTrem
                        {
                            MoviId = Convert.ToInt32(row["MT_ID_MOV"]),
                            AreaOperMovimentacao = row["AO_COD_AOP"].ToString(),
                            InicioMovimentacao = Convert.ToDateTime(row["MT_DTC_RAL"]),
                            FimMovimentacao = Convert.ToDateTime(row["MT_DTS_RAL"])
                        };

                        listMovimentacao.Add(mov);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar a movimentação da parada: {ao} : {ex.Message}", ex);
            }
            return listMovimentacao;
        }
    }
}
