using Microsoft.AspNetCore.Mvc;
using Paradas.Interface;
using Paradas.Models;

namespace Paradas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimentacaoTremController : Controller
    {
        private IConfiguration _configuration;
        private IMovimentacaoTremService _movimentacaoTrem;

        private string connstring;
        private bool activelog;

        public MovimentacaoTremController(IConfiguration iConfig, IMovimentacaoTremService movimentacaoTrem)
        {
            _configuration = iConfig;
            _movimentacaoTrem = movimentacaoTrem;
            connstring = _configuration.GetSection("ConnString").Value;
            activelog = (_configuration.GetSection("ActiveLog").Value == "1");
        }

        #region GetMovimentacaoTrem
        [HttpGet("Areaoperacional")]
        public ActionResult<IAsyncEnumerable<MovimentacaoTrem>> GetMovimentacaoTrem(int os, string ao)
        {
            try
            {
                var movimentacao = _movimentacaoTrem.GetMovimentacao(os, ao);
                return Ok(movimentacao);
            }
            catch (Exception error)
            {
                if (activelog)
                {
                    // Faça o log como preferir (ex: log em arquivo, banco de dados, etc)
                    Console.WriteLine(error); // ou seu logger
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Erro ao buscar movimentação do trem",
                    error = error.Message,
                    stackTrace = error.StackTrace
                });
            }

        }
        #endregion
    }
}
