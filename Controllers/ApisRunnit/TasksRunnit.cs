using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace remarsemanal.Controllers
{
    [Route("api/v1/runnittask")]
    [ApiController]
    [Authorize]
    
    public class TasksRunnit : ControllerBase
    {
        private static string userToken = "TLsKMI5hONHrVOH16XuI";
        private static string appKey = "97517be61ebfba95bc7db28d0263ea20";
        private static string route = "https://runrun.it/api/v1.0/tasks/";

        [HttpGet("{id}")]
        public async Task<ActionResult<Object>> ListaTarefaById([FromRoute]int id)
        {
            TimeSpan timeOut = TimeSpan.FromSeconds(20);
            try {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage resposta;
                    client.DefaultRequestHeaders.Add("App-Key", appKey);
                    client.DefaultRequestHeaders.Add("User-Token", userToken);
                    client.Timeout = timeOut;
                    resposta = await client.GetAsync(route + id);
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