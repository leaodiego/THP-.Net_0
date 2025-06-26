using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Paradas.Interface;
using Paradas.Models;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees;
using UnilogApi.Persistencia;

namespace Paradas.Services
{
    public class JustificativaService : IJustificativaService
    {
        #region Metodo construtor com o banco UNILOG
        private DataAccess conexao;

        public JustificativaService(string connstring, bool activeLog)
        {
            this.conexao = new DataAccess();
        }
        #endregion


        public IEnumerable<Justificativa> GetJustificativa(int paradaId)
        {
            var listJustRetro = JustificativaRetroativa(paradaId);

            if (listJustRetro == null || !listJustRetro.Any())
            {
                return JustificativaPatmotparada(paradaId);
            }

            return listJustRetro;
        }

        #region Criar Justificativa
        public Task CreateJustificativa(Justificativa justificativa)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Update Justificativa
        public Task UpdateJustificativa(Justificativa justificativa)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Deletar Justificativa
        public Task DeleteJustificativa(int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        public IEnumerable<Justificativa> JustificativaRetroativa(int paradaId)
        {
            List<Justificativa> justificativas = new List<Justificativa>();

            var parametros = new List<OracleParameter>();

            parametros.Add(conexao.GetOracleParameter("paradasId", OracleDbType.Int32, (paradaId)));

            string sql = @" SELECT me.MV_DRS_PT , ma.* 
                              FROM JUSTIF_RETROATIVA  ma
                                   LEFT JOIN EVENTO_FERROVIARIO ef ON ef.EVF_IDT_EVF = ma.EVF_IDT_EVF  
                                   LEFT JOIN MOTIVO_EVENTO mef ON mef.MV_IDT_MTV = ef.MV_IDT_MTV  
                                   LEFT JOIN MOTIVO_EVENTO me ON me.MV_IDT_MTV = ma.MV_IDT_MTV 
                             WHERE ma.PR_ID_PRT = :paradaId
                             ORDER BY ma.JRP_DAT_INC";

            DataTable dt = conexao.LeDadosParams<OracleConnection>(sql, parametros);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Justificativa justificativa = new Justificativa();

                    justificativa.JustificativaId = Int32.Parse(row["MA_IDT_PMT"].ToString());
                    justificativa.Motivo = row["MV_DRS_PT"].ToString();
                    justificativa.DataInicio = DateTime.Parse(row["JRP_DAT_INC"].ToString());
                    justificativa.DataFim = DateTime.Parse(row["JRP_DAT_FIM"].ToString());
                    justificativa.TempoMin = (int)Math.Round(Convert.ToDouble(row["JRP_TMP_DUR"]));


                    justificativas.Add(justificativa);
                }
            }
            return justificativas;
        }

        public IEnumerable<Justificativa> JustificativaPatmotparada(int paradaId)
        {
            List<Justificativa> justificativas = new List<Justificativa>();

            var parametros = new List<OracleParameter>();

            parametros.Add(conexao.GetOracleParameter("paradasId", OracleDbType.Int32, (paradaId)));

            string sql = @"SELECT nvl(mef.MV_DRS_PT ,me.MV_DRS_PT) AS Motivo, ma.* 
                             FROM PATMOTPARADA ma
                                  LEFT JOIN EVENTO_FERROVIARIO ef ON ef.EVF_IDT_EVF = ma.EVF_IDT_EVF
                                  LEFT JOIN MOTIVO_EVENTO mef ON mef.MV_IDT_MTV = ef.MV_IDT_MTV
                                  LEFT JOIN MOTIVO_EVENTO me ON me.MV_IDT_MTV = ma.MV_IDT_MTV
                            WHERE ma.PR_ID_PRT = :paradasId 
                            ORDER BY ma.MA_DAT_INC";

            DataTable dt = conexao.LeDadosParams<OracleConnection>(sql, parametros);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Justificativa justificativa = new Justificativa();

                    justificativa.JustificativaId = Int32.Parse(row["MA_IDT_PMT"].ToString());
                    justificativa.Motivo = row["Motivo"].ToString();
                    justificativa.DataInicio = DateTime.Parse(row["MA_DAT_INC"].ToString());
                    justificativa.DataFim = DateTime.Parse(row["MA_DAT_FIM"].ToString());
                    justificativa.TempoMin = (int)Math.Round(Convert.ToDouble(row["MA_TMP_DRC"]));


                    justificativas.Add(justificativa);
                }
            }
            return justificativas;
        }
    }
}
