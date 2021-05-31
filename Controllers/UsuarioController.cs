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
using remarsemanal.Model.usuario;
using remarsemanal.Utils;

namespace remarsemanal.Controllers
{
    [Route("api/v1/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly EmpresaDatabase _database;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        public UsuarioController(EmpresaDatabase database, JwtSettings jwtSettings, IMapper mapper)
        {
           _database = database;
           _jwtSettings = jwtSettings;
           _mapper = mapper;
           _jwt = new JWT();
        }

        [HttpGet("auth/token")]
        [Authorize]
        public void TokenValido()
        {
            return;
        }

        [HttpGet]
        [Authorize]
        public async Task<List<Usuario>> ListaUsuarios()
        {
            int userID = await _jwt.RetornaIDUsuarioDoToken(HttpContext);
            return await _database.Usuario
                                .AsNoTracking()
                                .Where(t => t.ativo == "S" && t.id == userID)
                                .OrderByDescending(t => t.id)
                                .ToListAsync();
        }

        [HttpPost("auth/login")]
        public async Task<ActionResult<Usuario>> LoginUsuario([FromBody]UsuarioAuth autenticacao) {
            if(ModelState.IsValid) {

                Usuario usuario;

                try{
                    usuario = await _database.Usuario.FirstOrDefaultAsync(u => u.login.ToLower().Equals(autenticacao.login.ToLower()));
                } catch (Exception e) {
                    return BadRequest(new {msg = e.ToString()});
                }
                
                if(usuario == null){
                    ///Usuário ou senha não encontrados
                    return NotFound(new {status = false, msg = "Usuário não encontrado"});
                } else if (usuario.ativo.Equals("N")) {
                    //Usuário desativado
                    return Unauthorized(new {status = false, msg = "Usuário desativado"});
                } else {
                    //Verifica se a senha esta correta
                    Hash hash = new Hash();
                    if (hash.VerificaSenhaHash(usuario, autenticacao.senha)) {
                        var token = await new JWT().CriaTokenJWT(usuario, _jwtSettings);
                        return Ok(new {status = true, token = token});
                    } else {
                        return Ok(new {status = false, msg = "Senha incorreta"});
                    }
                }
            } else {
                //Request errado
                return BadRequest();
            }
        }

        [HttpPost("novo")]
        [Authorize]
        public async Task<ActionResult> CadastraNovoUsuario(UsuarioCadastro usuario){
            if(ModelState.IsValid)
            {
                // Se encontrar um outro usuário com o mesmo login, irá retornar um aviso informando que esse login já esta cadastrado
                if (await _database.Usuario.FirstOrDefaultAsync(u => u.login.ToLower() == usuario.login.ToLower()) != null) {
                    return BadRequest(new {status = false, msg =  $"O Login {usuario.login} já esta cadastrado, por favor, utilize outro"});
                }
                
                Hash hash = new Hash();
                hash.HasheiaSenha(usuario);
                Usuario novoUsuario = _mapper.Map<Usuario>(usuario);
                novoUsuario.senha = usuario.senhacadastro;
                novoUsuario.senhahash = usuario.senhahash;
                novoUsuario.ativo = "S";
                await _database.Usuario.AddAsync(novoUsuario);
                try{
                    await _database.SaveChangesAsync();
                    return Ok(new {
                        status = true,
                        msg = "Usuário cadastrado com sucesso!"
                    });
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Falha ao cadastrar usuário",
                        erro = e.Message
                    });
                }
            } else {
                return BadRequest(new {status = false, msg = "Por favor, preencha todos os campos"});
            }
        }

        [HttpPut("altera/{id}")]
        [Authorize]
        public async Task<ActionResult> AlteraUsuario(int id, Usuario usuario){
            if(ModelState.IsValid)
            {
                if (id != usuario.id) {
                    return NotFound();
                }

                if (await _database.Usuario.Where(u => u.login == usuario.login && u.id != id).FirstOrDefaultAsync() != null) {
                    return BadRequest(new {
                        status = false,
                        msg =  $"O Usuário {usuario.login} já esta cadastrado, por favor, utilize outro login"
                    });
                }

                _database.Entry(usuario).State = EntityState.Modified;
                _database.Entry(usuario).Property(u => u.senha).IsModified = false;
                _database.Entry(usuario).Property(u => u.senhahash).IsModified = false;
                _database.Entry(usuario).Property(u => u.primeiroacesso).IsModified = false;

                try
                {
                    await _database.SaveChangesAsync();
                    return Ok(new {status = true, msg = "Usuário alterado com sucesso!"});
                } catch (Exception e) {
                    return BadRequest(new {
                        status = false,
                        msg = "Falha ao alterar usuário",
                        erro = e.Message
                    });
                }
            }
            else
            {
                return BadRequest(new {status = false, msg = "Por favor, preencha todos os campos"});
            }
        }

        [HttpPatch("alterasenha")]
        public async Task<ActionResult> AlteraSenhaUsuario(UsuarioAlteraSenha usuarioAlteraSenha) {
            Usuario usuario = await _database.Usuario.FindAsync(usuarioAlteraSenha.id);
            
            if (usuario == null) {
                return NotFound();
            }

            Hash hash = new Hash();
            hash.HasheiaSenha(usuarioAlteraSenha);
            usuario.senha = usuarioAlteraSenha.senhanova;
            usuario.senhahash = usuarioAlteraSenha.senhahash;
            usuario.primeiroacesso = usuarioAlteraSenha.primeiroacesso;
            _database.Entry(usuario).State = EntityState.Modified;
            try {
                await _database.SaveChangesAsync();
                return Ok(new {status = true, msg = "Senha alterada com sucesso!"});
            } catch (Exception e) {
                return BadRequest(new {
                    status = false,
                    msg = "Falha ao alterar senha",
                    erro = e.Message
                });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DesativaTecnico(int id){
            Usuario usuario = await _database.Usuario.FindAsync(id);
            
            if (usuario == null) {
                return NotFound();
            }

            usuario.ativo = "N";
            _database.Entry(usuario).State = EntityState.Modified;
            _database.Entry(usuario).Property(usuario => usuario.senha).IsModified = false;
            _database.Entry(usuario).Property(usuario => usuario.senhahash).IsModified = false;
            _database.Entry(usuario).Property(usuario => usuario.primeiroacesso).IsModified = false;

            try
            {
                await _database.SaveChangesAsync();
                return Ok(new {status = true, msg = "Usuário desativado com sucesso!"});
            } catch (Exception e) {
                return BadRequest(new {
                    status = false,
                    msg = "Falha ao desativar usuário",
                    erro = e.Message
                });
            }
        }
    }
}