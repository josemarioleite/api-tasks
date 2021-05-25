using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace remarsemanal.Model.usuario
{
    public class UsuarioCadastro
    {
        [Required]
        public string nome { get; set; }
        [Required]
        public string login { get; set; }
        [Required]
        public string ativo { get; set; } = "S";
        public string primeiroacesso { get; set; } = "S";
        public string senha { get; set; }
        [JsonIgnore]
        public byte[] senhacadastro { get; set; }
        [JsonIgnore]
        public byte[] senhahash { get; set; }
        public string usuarioidrunnit { get; set; }

    }
}