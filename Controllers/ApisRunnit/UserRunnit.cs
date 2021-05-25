using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.ApisRunnit
{
    [Route("api/v1/usuariorunnit")]
    [ApiController]
    [Authorize]
    public class UserRunnit : ControllerBase
    {
        private static string userToken = "TLsKMI5hONHrVOH16XuI";
        private static string appKey = "97517be61ebfba95bc7db28d0263ea20";
        private static string route = "https://runrun.it/api/v1.0/users/";

        [HttpGet("idrunnit")]
        public async Task<ActionResult<Object>> ListUserRunnit([FromRoute]string idrunnit)
        {
            TimeSpan timeOut = TimeSpan.FromSeconds(20);
            try {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resposta;
                    client.DefaultRequestHeaders.Add("App-Key", appKey);
                    client.DefaultRequestHeaders.Add("User-Token", userToken);
                    client.Timeout = timeOut;
                    resposta = await client.GetAsync(route + idrunnit);
                    var resultado = await resposta.Content.ReadAsStreamAsync();
                    return resultado;
                }
            } catch (Exception e) {
                return BadRequest(new {
                    status = false,
                    msg = "Não foi possível fazer Get do runnit",
                    erro = e.Message
                });
            }
        }
    }
}