using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paradas.Interface;
using Paradas.Models;

namespace Paradas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JustificativaController : ControllerBase
    {
        private IConfiguration _configuration;
        private IJustificativaService _Justificativa;

        private string connstring;
        private bool activelog;

        public JustificativaController(IConfiguration iConfig, IJustificativaService iJustific)
        {
            _configuration = iConfig;
            _Justificativa = iJustific;
            connstring = _configuration.GetSection("ConnString").Value;
            activelog = (_configuration.GetSection("ActiveLog").Value == "1") ? true : false;
        }

        #region GetJustificativas
        [HttpGet(Name = "GetJustificativa")]
        public ActionResult<IAsyncEnumerable<Justificativa>> GetJustificativa(int paradaId)
        {
            try
            {
                var justificativas = _Justificativa.GetJustificativa(paradaId);
                return Ok(justificativas);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao obeter a parada");
            }
        }
        #endregion
    }
}
