using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using remarsemanal.Brokers;
using remarsemanal.Helpers;
using remarsemanal.Model;
using remarsemanal.Utils;

namespace remarsemanal.Controllers
{
    [Route("api/v1/tarefa")]
    [ApiController]
    [Authorize]
    public class TarefaController : ControllerBase
    {
        private readonly EmpresaDatabase _database;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        public TarefaController(EmpresaDatabase database, IMapper mapper)
        {
            _database = database;
            _mapper = mapper;
            _jwt = new JWT();
        }

        [HttpGet]
        public async Task<ActionResult<List<Tarefa>>> ListaTodasTarefas([FromQuery] string filter, [FromQuery]int pageIndex,
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? finalDate,
            [FromQuery] int pageSize, [FromQuery] bool onlyRowCount = false) {

            int rowCount = 0;
            int userID = await _jwt.RetornaIDUsuarioDoToken(HttpContext);
            List<Tarefa> tarefa;
            tarefa = await _database.Tarefa
                    .AsNoTracking()
                    .Include(t => t.Quadro)
                    .Include(t => t.Tipo)
                    .Where(t => t.usuarioid == userID)
                    .OrderByDescending(t => t.id)
                    .ToListAsync();
            
            rowCount = tarefa.Count;
            if (string.IsNullOrEmpty(filter) || filter.ToLower() == "undefined") {
                filter = null;
            }

            if (startDate == null || startDate.ToString().ToLower() == "undefined") {
                startDate = null;
            }

            if (finalDate == null || finalDate.ToString().ToLower() == "undefined") {
                finalDate = null;
            }
            
            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                tarefa = tarefa.Where(t => (t.Quadro != null && t.Quadro.nome.ToLower().RemoveAcentosECaracteresEspeciais().Contains(filter)) 
                    || (t.Tipo != null  && t.Tipo!.nome.ToLower().RemoveAcentosECaracteresEspeciais().Contains(filter))
                    || t.id.ToString().Contains(filter)
                    || t.tarefaid.ToString().Contains(filter)).ToList();
            }

            if (startDate != null && finalDate != null) {
                tarefa = tarefa.Where(t => t.data >= startDate && t.data <= finalDate).ToList();
            } else if (startDate != null && finalDate == null) {
                tarefa = tarefa.Where(t => t.data >= startDate).ToList();
            } else if (startDate == null && finalDate != null) {
                tarefa = tarefa.Where(t => t.data <= finalDate).ToList();
            }

            if (onlyRowCount)
            {
                return Ok(new { rowCount = rowCount });
            }

            tarefa = await PaginatedList<Tarefa>.CreateAsync(tarefa, pageIndex, pageSize);
            rowCount = tarefa.Count;
            return Ok(new { rowCount = rowCount, tarefa });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarefa>> ListaTarefaById(int id)
        {
            int userID = await _jwt.RetornaIDUsuarioDoToken(HttpContext);
            
            return await _database.Tarefa
                                .AsNoTracking()
                                .Where(t => t.usuarioid == userID)
                                .FirstOrDefaultAsync(t => t.id == id);
        }

        [HttpPost("novo")]
        public async Task<ActionResult> CadastraTarefa(TarefaCadastro tarefa) {
            if(ModelState.IsValid)
            {
                if(await _database.Tarefa.FirstOrDefaultAsync(t => t.nome.ToLower() == tarefa.nome.ToLower()) != null){
                    return BadRequest(new {status = false, msg =  $"O Tipo: {tarefa.nome} já esta cadastrado, por favor, utilize outro"});
                }
                
                Tarefa novaTarefa = _mapper.Map<Tarefa>(tarefa);
                novaTarefa.usuarioid = await _jwt.RetornaIDUsuarioDoToken(HttpContext);
                if (novaTarefa == null) {
                    novaTarefa.data = DateTime.Now;
                }
                await _database.Tarefa.AddAsync(novaTarefa);
                try {
                    await _database.SaveChangesAsync();
                    return Ok(new {status = true, msg = "Tarefa cadastrado com sucesso!"});
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Falha ao cadastrar Tarefa",
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
        public async Task<ActionResult> AlteraTarefa([FromRoute]int id,[FromBody] Tarefa tarefa){
            if (ModelState.IsValid) {

                if(id != tarefa.id) {
                    return NotFound();
                }

                _database.Entry(tarefa).State = EntityState.Modified;

                try {
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Tarefa alterado com sucesso!"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Falha ao alterar tarefa",
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
        public async Task<ActionResult> ExcluiTarefa(int id) {
            Tarefa tarefa = await _database.Tarefa.FindAsync(id);

            if (tarefa == null) {
                return NotFound();
            }

            _database.Entry(tarefa).State = EntityState.Deleted;
            try
            {
                await _database.SaveChangesAsync();
                return Ok(new {
                    status = true,
                    msg = "Tarefa excluída com sucesso!"
                });
            } catch(Exception e) {
                return BadRequest(new {
                    status = false,
                    msg = "Falha ao excluir tarefa",
                    erro = e.Message
                });
            }
        }
    }
}