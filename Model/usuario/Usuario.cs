using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace remarsemanal.Model.usuario
{
    public class Usuario
    {
        [Key]
        public int id { get; set; }
        public string nome { get; set; }
        public string login { get; set; }
        public string ativo { get; set; }
        public string primeiroacesso { get; set; }
        [JsonIgnore]
        public byte[] senha { get; set; }
        [JsonIgnore]
        public byte[] senhahash { get; set; }
        public string usuarioidrunnit { get; set; }
    }
}