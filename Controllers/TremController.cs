using Microsoft.AspNetCore.Mvc;
using Paradas.Interface;
using Paradas.Models;

namespace Paradas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TremController : ControllerBase
    {
        private IConfiguration _configuration;
        private ITremService _trem;

        private string connstring;
        private bool activelog;

        public TremController(IConfiguration iConfig, ITremService itrem)
        {
            _configuration = iConfig;
            _trem = itrem;
            connstring = _configuration.GetSection("ConnString").Value;
            activelog = (_configuration.GetSection("ActiveLog").Value == "1") ? true : false;
        }

        #region GetJustificativas
        [HttpGet(Name = "GetTrem")]
        public Trem GetTrem(int NumOs)
        {
            try
            {
                var trem = _trem.GetTrem(NumOs);
                return trem;
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível recuperar a os  " + NumOs + ": " + ex.ToString());
            }
        }
        #endregion
    }
}