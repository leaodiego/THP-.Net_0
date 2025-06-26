using Oracle.ManagedDataAccess.Client;
using Paradas.Interface;
using Paradas.Models;
using System.Data;
using UnilogApi.Persistencia;

namespace Paradas.Services
{
    public class ParadaService : IParadaService
    {
        #region Metodo construtor com o banco UNILOG
        private DataAccess conexao;

        public ParadaService(string connstring, bool activeLog)
        {
            this.conexao = new DataAccess();
        }
        #endregion

        #region Metodos GET, Create, Update e DELETE 

        #region GetParadas
        public IEnumerable<Parada> GetParada(int NumOs)
        {
            List<Parada> paradas = new List<Parada>();

            try
            {
                var parametros = new List<OracleParameter>();

                parametros.Add(conexao.GetOracleParameter("NumOs", OracleDbType.Int32, (NumOs)));

                string sql = string.Format(@"SELECT os.X1_NRO_OS ,prt.AO_ID_AO, ao.AO_COD_AOP, us.US_NOM_USU,tr.TR_PFX_TRM, prt.*
                                               FROM T2_OS os
                                                    INNER JOIN trem tr ON tr.OF_ID_OSV = os.X1_ID_OS
                                                    INNER JOIN PARADAS_REALIZADAS_TREM prt ON prt.TR_ID_TRM = tr.TR_ID_TRM
                                                    INNER JOIN AREA_OPERACIONAL ao ON ao.ao_id_ao = prt.AO_ID_AO
                                                    INNER JOIN usuario us ON us.US_IDT_USU = prt.US_IDT_USU
                                              WHERE os.X1_NRO_OS = :NumOs
                                              ORDER BY prt.PR_DAT_INC ");

                DataTable dt =  conexao.LeDadosParams<OracleConnection>(sql, parametros);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Parada parada = new Parada();

                        //paradas = Int32.Parse(dt.Rows[0]["prt.PR_ID_PRT"].ToString());

                        parada.ParadaId = Int32.Parse(row["PR_ID_PRT"].ToString());
                        parada.sbId = Int32.Parse(row["AO_ID_AO"].ToString());
                        parada.Local = row["AO_COD_AOP"].ToString();
                        parada.UsuarioResponsavel = row["US_NOM_USU"].ToString();
                        parada.DataInicio = DateTime.Parse(row["PR_DAT_INC"].ToString());
                        parada.DataFim = DateTime.Parse(row["PR_DAT_FIM"].ToString());
                        parada.idTrem = Int32.Parse(row["TR_ID_TRM"].ToString());
                        parada.Tempo = (int)Math.Round(Convert.ToDouble(row["PR_TMP_REA"]));

                        paradas.Add(parada);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível recuperar a parada da OS: " + NumOs + " : " + ex.ToString());
            }
            return paradas;
        }
        #endregion

        #region GetParadaById
        public Parada GetParadaById(int id)
        {
            Parada parada = null;

            try
            {
                // Parâmetros para a consulta SQL
                var parametros = new List<OracleParameter>
                {
                    conexao.GetOracleParameter("IdParada", OracleDbType.Int32, id)
                };

                // Consulta SQL para buscar a parada por ID
                string sql = @"SELECT os.X1_NRO_OS, ao.AO_COD_AOP, us.US_NOM_USU, tr.TR_PFX_TRM, prt.*
                                 FROM PARADAS_REALIZADAS_TREM prt
                                      INNER JOIN trem tr ON tr.TR_ID_TRM = prt.TR_ID_TRM
                                      INNER JOIN AREA_OPERACIONAL ao ON ao.ao_id_ao = prt.AO_ID_AO
                                      INNER JOIN usuario us ON us.US_IDT_USU = prt.US_IDT_USU
                                      INNER JOIN T2_OS os ON os.X1_ID_OS = tr.OF_ID_OSV
                                WHERE prt.PR_ID_PRT = :IdParada";

                // Executa a consulta e obtém os dados
                DataTable dt = conexao.LeDadosParams<OracleConnection>(sql, parametros);

                // Verifica se a consulta retornou algum resultado
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];  // Como estamos buscando por ID, só deve retornar um item
                    parada = new Parada
                    {
                        ParadaId = Convert.ToInt32(row["PR_ID_PRT"]),
                        sbId = Convert.ToInt32(row["AO_ID_AO"]),
                        Local = row["AO_COD_AOP"].ToString(),
                        UsuarioResponsavel = row["US_NOM_USU"].ToString(),
                        DataInicio = Convert.ToDateTime(row["PR_DAT_INC"]),
                        DataFim = Convert.ToDateTime(row["PR_DAT_FIM"]),
                        Tempo = (int)Math.Round(Convert.ToDouble(row["PR_TMP_REA"])),
                        idTrem = Convert.ToInt32(row["TR_ID_TRM"]),
                        Programacao = row["PR_INP_SN"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar a parada com ID {id}: {ex.Message}", ex);
            }

            return parada;  // Retorna o objeto 'Parada' ou null se não encontrado
        }
        #endregion

        #region GetAreaOperacioanl
        public IEnumerable<AO> GetAo(int NumOs)
        {
            List<AO> sbs = new List<AO>();

            try
            {
                var parametros = new List<OracleParameter>();

                parametros.Add(conexao.GetOracleParameter("NumOs", OracleDbType.Int32, (NumOs)));

                string sql = string.Format(@"SELECT aos.AO_ID_AO  
                                                   ,aos.AO_COD_AOP   
                                                   ,ar.AA_SEQ_RTA  
                                                   ,tr.TR_ID_TRM 
                                                   ,tr.TR_PFX_TRM 
                                                   ,rt.RT_COD_RTA
                                               FROM T2_OS x1
                                                    JOIN TREM tr ON tr.OF_ID_OSV = x1.X1_ID_OS
                                                    JOIN ROTA rt ON rt.RT_ID_RTA = tr.RT_ID_RTA
                                                    JOIN AREAOPER_ROTA ar ON ar.RT_ID_RTA = rt.RT_ID_RTA
                                                    JOIN AREA_OPERACIONAL aof ON aof.AO_ID_AO = ar.AO_ID_AO
                                                    JOIN AREA_OPERACIONAL aos ON aos.AO_ID_AO = NVL(aof.AO_ID_AO_inf, aof.AO_ID_AO)
                                              WHERE x1.X1_NRO_OS = :NumOs
                                                AND NOT EXISTS (
                                                    SELECT 1 
                                                      FROM PARADAS_REALIZADAS_TREM prt
                                                     WHERE prt.AO_ID_AO = aos.AO_ID_AO
                                                       AND prt.TR_ID_TRM = tr.TR_ID_TRM
                                                )
                                              ORDER BY ar.AA_SEQ_RTA");

                DataTable dt = conexao.LeDadosParams<OracleConnection>(sql, parametros);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        AO sb = new AO();

                        sb.aoId = Int32.Parse(row["AO_ID_AO"].ToString());
                        sb.aoCodigo = row["AO_COD_AOP"].ToString();
                        sb.idTrem = Int32.Parse(row["TR_ID_TRM"].ToString());
                        sb.prfTrem = row["TR_PFX_TRM"].ToString();

                        sbs.Add(sb);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível recuperar as SBs: " + ex.ToString());
            }
            return sbs;
        }
        #endregion

        #region Update Parada
        public async Task<bool> AtualizarParadaAsync(int id, Parada dto)
        {
            try
            {
                var parametros = new List<OracleParameter>
                {
                    conexao.GetOracleParameter("DataInicio", OracleDbType.TimeStamp, dto.DataInicio),
                    conexao.GetOracleParameter("DataFim", OracleDbType.TimeStamp, dto.DataFim),
                    conexao.GetOracleParameter("TempoRealizado", OracleDbType.Int32, dto.Tempo),
                    conexao.GetOracleParameter("Id", OracleDbType.Int32, id)
                };

                string sql = @"UPDATE PARADAS_REALIZADAS_TREM
                                  SET PR_DAT_INC = :DataInicio,
                                      PR_DAT_FIM = :DataFim,
                                      PR_TMP_REA = :TempoRealizado,
                                      PR_TIMESTAMP = SYSDATE
                                WHERE PR_ID_PRT = :Id";


                // Chamada assíncrona para manter padrão async (se desejar pode usar Task.Run)
                int rowsAffected = await Task.Run(() =>
                    conexao.ExecuteQueryParams<OracleConnection>(sql, parametros)
                );

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar a parada com ID {id}: {ex.Message}", ex);
            }
        }

        #endregion

        #region Create Parada
        public bool InserirParada(ParadaInsert parada)
        {
            try
            {
                var parametros = new List<OracleParameter>
        {
            conexao.GetOracleParameter("sbId", OracleDbType.Int32, parada.sbId),
            conexao.GetOracleParameter("IdTrem", OracleDbType.Int32, parada.idTrem),
            conexao.GetOracleParameter("DataInicio", OracleDbType.TimeStamp, parada.DataInicio),
            conexao.GetOracleParameter("DataFim", OracleDbType.TimeStamp, parada.DataFim),
            conexao.GetOracleParameter("Tempo", OracleDbType.Int32, parada.Tempo),
            conexao.GetOracleParameter("Usuario", OracleDbType.Varchar2, parada.Usuario)
        };

                string sql = @"INSERT INTO PARADAS_REALIZADAS_TREM (PR_ID_PRT, AO_ID_AO, TR_ID_TRM,
                                                                    PR_DT_PRG, PR_DT_PRV, PR_DAT_INC,
							 		                                PR_DAT_FIM, PR_INP_SN, PR_INR_SN,
							 		                                PR_TMP_PRG, PR_TMP_REA, PR_TIMESTAMP,
							 		                                EV_ID_ELV, PR_CSD_PRD, PR_NUM_PKL,
							 		                                TP_ID_PRD, PR_TXT_OBS, US_IDT_USU,
							 		                                PR_IND_APT, PR_IND_ENC, PR_IND_ENA,
							 		                                PR_DAT_ENC, PR_IND_JTP, PR_IND_RTV
                                                                   )
                                                            VALUES (PARADAS_REALIZADAS_TREM_SEQ_ID.NEXTVAL, :sbId, :IdTrem,
							 		                                NULL, NULL, :DataInicio,
							 		                                :DataFim, 'N', 'S',
							 		                                NULL, :Tempo, SYSDATE,
							 		                                NULL, NULL, NULL,
							 		                                NULL, NULL, (SELECT US_IDT_USU FROM USUARIO WHERE US_USR_IDU = :Usuario),
							 		                                NULL, 'S', 'N',
							 		                                NULL, NULL, 'N'
							 	                                   )";

                int rowsAffected = conexao.ExecuteQueryParams<OracleConnection>(sql, parametros);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir parada: {ex.Message}", ex);
            }
        }



        #endregion

        #region Delete Parada
        public bool ExcluirParada(int paradaId)
        {
            try
            {
                ExcluirJustificativaRetroativa(paradaId);
                ExcluirPatmotparada(paradaId);

                var parametros = new List<OracleParameter>
                {
                    conexao.GetOracleParameter("paradaId", OracleDbType.Int32, paradaId)
                };

                // Agora excluir a parada principal
                string sqlDeleteParada = @"DELETE FROM PARADAS_REALIZADAS_TREM WHERE PR_ID_PRT = :paradaId";
                conexao.LeDadosParams<OracleConnection>(sqlDeleteParada, parametros);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível excluir a parada e seus relacionamentos. Parada ID: " + paradaId + " - Erro: " + ex.ToString());
            }
        }


        #endregion

        #endregion

        public bool ExcluirJustificativaRetroativa(int paradaId)
        {
            try
            {
                // Parâmetros para a consulta e exclusão
                var parametros = new List<OracleParameter>
                {
                    conexao.GetOracleParameter("paradaId", OracleDbType.Int32, paradaId)
                };

                // Agora excluir a parada principal
                string sql = @"DELETE FROM JUSTIF_RETROATIVA WHERE PR_ID_PRT = :paradaId";
                conexao.LeDadosParams<OracleConnection>(sql, parametros);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir Justificativa retroativa {paradaId}: {ex.Message}", ex);
            }
        }

        public bool ExcluirPatmotparada(int paradaId)
        {
            try
            {
                // Parâmetros para a consulta e exclusão
                var parametros = new List<OracleParameter>
                {
                    conexao.GetOracleParameter("paradaId", OracleDbType.Int32, paradaId)
                };

                // Agora excluir a parada principal
                string sqlDelete = @"DELETE FROM PATMOTPARADA WHERE PR_ID_PRT = :paradaId";
                conexao.LeDadosParams<OracleConnection>(sqlDelete, parametros);
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir Justificativa da parada {paradaId}: {ex.Message}", ex);
            }
        }

    }
}
