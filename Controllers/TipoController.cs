using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using remarsemanal.Brokers;
using remarsemanal.Model;
using remarsemanal.Utils;

namespace remarsemanal.Controllers
{
    [Route("api/v1/tipo")]
    [ApiController]
    [Authorize]
    public class TipoController : ControllerBase
    {
        private readonly EmpresaDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        
        public TipoController(EmpresaDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<Tipo>>> ListaTipo()
        {
            //int userID = await _jwt.RetornaIDUsuarioDoToken(HttpContext);
            return await _database.Tipo
                            .AsNoTracking()
                            .Where(t => t.ativo == "S")
                            .OrderByDescending(t => t.id)
                            .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tipo>> ListaTipoById(int id)
        {
            int userID = await _jwt.RetornaIDUsuarioDoToken(HttpContext);
            return await _database.Tipo
                            .AsNoTracking()
                            .Where(t => t.ativo == "S" && t.usuarioid == userID)
                            .FirstOrDefaultAsync(t => t.id == id);
        }

        [HttpPost("novo")]
        public async Task<ActionResult> CadastraTipo(TipoCadastro tipo) {
            if(ModelState.IsValid)
            {
                if(await _database.Tipo.FirstOrDefaultAsync(t => t.nome.ToLower() == tipo.nome.ToLower()) != null){
                    return BadRequest(new {status = false, msg =  $"O Tipo: {tipo.nome} já esta cadastrado, por favor, utilize outro"});
                }
                
                Tipo novoTipo = _mapper.Map<Tipo>(tipo);
                novoTipo.usuarioid = await _jwt.RetornaIDUsuarioDoToken(HttpContext);
                await _database.Tipo.AddAsync(novoTipo);
                try {
                    await _database.SaveChangesAsync();
                    return Ok(new {status = true, msg = "Tipo-Tarefa cadastrado com sucesso!"});
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Falha ao cadastrar Tipo-Tarefa",
                        erro = e.Message
                    });
                }
            }
            else
            {
                return BadRequest(new {status = false, msg = "Por favor, preencha todos os campos"});
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AlteraTipo([FromRoute]int id,[FromBody] Tipo tipo){
            if (ModelState.IsValid) {

                if(id != tipo.id) {
                    return NotFound();
                }

                _database.Entry(tipo).State = EntityState.Modified;
                _database.Entry(tipo).Property(tipo => tipo.ativo).IsModified = false;

                try {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Tipo alterado com sucesso!"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Falha ao alterar tipo",
                        erro = e.Message
                    });
                }
            } else {
                return BadRequest(new {
                    status = false,
                    msg = "Por favor, preencha todos os campos obrigatórios"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> ExcluiTipo(int id){
            Tipo tipo = await _database.Tipo.FindAsync(id);

            if (tipo == null) {
                return NotFound();
            }

            tipo.ativo = "N";
            _database.Entry(tipo).State = EntityState.Deleted;
            _database.Entry(tipo).Property(tipo => tipo.nome).IsModified = false;

            try
            {
                await _database.SaveChangesAsync();
                return Ok(new {status = true, msg = "Tipo excluído com sucesso!"});
            } catch(Exception e) {
                return BadRequest(new {
                    status = false,
                    msg = "Falha ao excluir tipo",
                    erro = e.Message
                });
            }
        }
    }
}