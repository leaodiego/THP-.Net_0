using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Paradas.Interface;
using Paradas.Models;
using System.Configuration;

namespace Paradas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParadasController : ControllerBase
    {
        private IConfiguration _configuration;
        private IParadaService _paradaService;

        private string connstring;
        private bool activelog;

        public ParadasController(IConfiguration iConfig, IParadaService paradaService)
        {
            _configuration = iConfig;
            _paradaService = paradaService;
            connstring = _configuration.GetSection("ConnString").Value;
            activelog = (_configuration.GetSection("ActiveLog").Value == "1");
        }

        #region GetParadas
        [HttpGet("todas")]
        public ActionResult<IAsyncEnumerable<Parada>> GetParadas(int os)
        {
            try
            {
                var paradas = _paradaService.GetParada(os);
                return Ok(paradas);
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, error);
            }
        }
        #endregion

        #region GetParadaById
        [HttpGet("{id}", Name = "GetParadaById")]
        public ActionResult<Parada> GetParadaById(int id)
        {
            try
            {
                var parada = _paradaService.GetParadaById(id);

                if (parada == null)
                    return NotFound($"Parada com ID {id} não encontrada.");

                return Ok(parada);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter a parada: {ex.Message}");
            }
        }
        #endregion

        #region GetAO
        [HttpGet("Area_Operacional")]
        public ActionResult<IAsyncEnumerable<AO>> GetAo(int os)
        {
            try
            {
                var paradas = _paradaService.GetAo(os);
                return Ok(paradas);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao obter a parada");
            }
        }
        #endregion

        #region Update Parada
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarParada(int id, [FromBody] Parada dto)
        {
            if (dto == null)
                return BadRequest("Dados da parada são obrigatórios.");

            try
            {
                var sucesso = await _paradaService.AtualizarParadaAsync(id, dto);

                if (!sucesso)
                    return NotFound($"Parada com ID {id} não encontrada.");

                return Ok($"Parada {id} atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar a parada: {ex.Message}");
            }
        }
        #endregion

        #region InserirParada
        [HttpPost("inserir")]
        public ActionResult InserirParada([FromBody] ParadaInsert parada)
        {
            try
            {
                if (parada == null)
                    return BadRequest("Dados da parada inválidos.");

                var sucesso = _paradaService.InserirParada(parada);

                if (sucesso)
                    return Ok("Parada inserida com sucesso.");
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao inserir a parada.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao inserir a parada: {ex.Message}");
            }
        }
        #endregion

        #region ExcluirParada
        [HttpDelete("{id}")]
        public IActionResult ExcluirParada(int id)
        {
            if (id <= 0)
                return BadRequest("ID da parada inválido.");

            try
            {
                var sucesso = _paradaService.ExcluirParada(id);

                if (!sucesso)
                    return NotFound($"Parada com ID {id} não encontrada ou não pôde ser excluída.");

                return Ok($"Parada {id} excluída com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao excluir a parada: {ex.Message}");
            }
        }
        #endregion

    }
}
