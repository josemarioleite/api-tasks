using AutoMapper;
using remarsemanal.Model;
using remarsemanal.Model.usuario;

namespace remarsemanal.Utils
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<TipoCadastro, Tipo>();
            CreateMap<TarefaCadastro, Tarefa>();
            CreateMap<QuadroCadastro, Quadro>();
            CreateMap<UsuarioCadastro, Usuario>().ForMember(x => x.senha, opt => opt.Ignore());
            CreateMap<Usuario, UsuarioDTO>();
        }
    }
}