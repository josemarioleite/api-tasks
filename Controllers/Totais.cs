using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using remarsemanal.Brokers;

namespace api.Controllers
{
    [Route("api/v1/geral")]
    [ApiController]
    // [Authorize]
    public class Totais : ControllerBase
    {
        private readonly EmpresaDatabase _database;
        public Totais(EmpresaDatabase database)
        {
            _database = database;
        }

        [HttpGet("quadro")]
        public async Task<ActionResult> ObtemQuadroGeral() 
        {
            var consulta = await _database.Consulta
                            .FromSqlRaw("Select q.id, q.nome, count(t.quadroid) as quantidade" +
                                        " from \"Tarefa\" as t" +
                                        " join \"Quadro\" as q on (t.quadroid = q.id)" +
                                        " group by q.id, q.nome order by q.id asc;")
                            .ToListAsync();
            if (consulta != null)
            {
                return Ok(consulta);
            } else {
                return Ok(new {
                    status = false,
                    msg = "Não foi possível trazer dados"
                });
            }
        }

        [HttpGet("tipo")]
        public async Task<ActionResult> ObtemTipoGeral() 
        {
            var consulta = await _database.Consulta
                            .FromSqlRaw("Select q.id, q.nome, count(t.tipoid) as quantidade" +
                                        " from \"Tarefa\" as t" +
                                        " join \"Tipo\" as q on (t.tipoid = q.id)" +
                                        " group by q.id, q.nome order by q.id asc;")
                            .ToListAsync();
            if (consulta != null)
            {
                return Ok(consulta);
            } else {
                return Ok(new {
                    status = false,
                    msg = "Não foi possível trazer dados"
                });
            }
        }

        [HttpGet("cliente")]
        public async Task<ActionResult> ObtemClienteGeral() 
        {
            var consulta = await _database.Consulta
                            .FromSqlRaw("Select row_number() over(partition by 0) as id, cliente as nome, count(cliente) as quantidade" +
                                        " from \"Tarefa\"" +
                                        " group by cliente order by id asc;")
                            .ToListAsync();
            if (consulta != null)
            {
                return Ok(consulta);
            } else {
                return Ok(new {
                    status = false,
                    msg = "Não foi possível trazer dados"
                });
            }
        }
    }
}