using System.ComponentModel.DataAnnotations;

namespace remarsemanal.Model.usuario
{
    public class UsuarioDTO
    {
        public string nome { get; set; }
        public string login { get; set; }
        public string ativo { get; set; }
        public string primeiroacesso { get; set; }
        public string usuarioidrunnit { get; set; }
    }
}