using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using remarsemanal.Brokers;
using remarsemanal.Model;
using remarsemanal.Utils;

namespace remarsemanal.Controllers
{
    [Route("api/v1/quadro")]
    [ApiController]
    [Authorize]
    public class QuadroController : ControllerBase
    {
        private readonly EmpresaDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        public QuadroController(EmpresaDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<Quadro>>> ListaQuadro()
        {
            int userID = await _jwt.RetornaIDUsuarioDoToken(HttpContext);
            return await _database.Quadro
                            .AsNoTracking()
                            .Where(t => t.ativo == "S" && t.usuarioid == userID)
                            .OrderByDescending(t => t.id)
                            .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Quadro>> ListaQuadroById(int id)
        {
            int userID = await _jwt.RetornaIDUsuarioDoToken(HttpContext);
            return await _database.Quadro
                            .AsNoTracking()
                            .Where(t => t.ativo == "S" && t.usuarioid == userID)
                            .FirstOrDefaultAsync(t => t.id == id);
        }

        [HttpPost("novo")]
        public async Task<ActionResult> CadastraQuadro(QuadroCadastro quadro) {
            if(ModelState.IsValid)
            {
                if(await _database.Quadro.FirstOrDefaultAsync(t => t.nome.ToLower() == quadro.nome.ToLower()) != null){
                    return BadRequest(new {status = false, msg =  $"O Quadro: {quadro.nome} já esta cadastrado, por favor, utilize outro"});
                }
                
                Quadro novoQuadro = _mapper.Map<Quadro>(quadro);
                novoQuadro.ativo = "S";
                novoQuadro.usuarioid = await _jwt.RetornaIDUsuarioDoToken(HttpContext);
                await _database.Quadro.AddAsync(novoQuadro);
                try {
                    await _database.SaveChangesAsync();
                    return Ok(new {status = true, msg = "Quadro cadastrado com sucesso!"});
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Falha ao cadastrar Quadro",
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
        public async Task<ActionResult> AlteraQuadro([FromRoute]int id,[FromBody] Quadro quadro){
            if (ModelState.IsValid) {

                if(id != quadro.id) {
                    return NotFound();
                }

                _database.Entry(quadro).State = EntityState.Modified;
                _database.Entry(quadro).Property(quadro => quadro.ativo).IsModified = false;

                try {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Quadro alterado com sucesso!"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Falha ao alterar quadro",
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
        public async Task<ActionResult> ExcluiQuadro(int id) {
            Quadro quadro = await _database.Quadro.FindAsync(id);

            if (quadro == null) {
                return NotFound();
            }

            quadro.ativo = "N";
            _database.Entry(quadro).State = EntityState.Modified;
            _database.Entry(quadro).Property(quadro => quadro.nome).IsModified = false;

            try
            {
                await _database.SaveChangesAsync();
                return Ok(new {
                    status = true,
                    msg = "Quadro excluído com sucesso!"
                });
            } catch(Exception e) {
                return BadRequest(new {
                    status = false,
                    msg = "Falha ao excluir quadro",
                    erro = e.Message
                });
            }
        }
    }
}