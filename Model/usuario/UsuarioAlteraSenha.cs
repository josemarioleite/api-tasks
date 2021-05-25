using System.Text.Json.Serialization;

namespace remarsemanal.Model.usuario
{
    public class UsuarioAlteraSenha
    {
        public int id { get; set; }
        public string primeiroacesso { get; set; } = "S";
        public string senha { get; set; }
        [JsonIgnore]
        public byte[] senhanova { get; set; }
        [JsonIgnore]
        public byte[] senhahash { get; set; }
    }
}