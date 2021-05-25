using System.ComponentModel.DataAnnotations;

namespace remarsemanal.Model
{
    public class Tipo
    {
        [Key]
        public int id { get; set; }
        public string nome { get; set; }
        public string ativo { get; set; } = "S";
        public int usuarioid { get; set; }
    }
}