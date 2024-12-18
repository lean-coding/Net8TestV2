using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTestV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        [HttpGet("{ShortCode}")]
        public Model.M_URL Get(string ShortCode)
        {
            BlazorTestV2.Service.ShortURL sUrl = new();
            Model.M_URL url = sUrl.GetByShortCode(ShortCode);
            return url;
        }
    }
}
