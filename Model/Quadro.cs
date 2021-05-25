using System.ComponentModel.DataAnnotations;

namespace remarsemanal.Model
{
    public class Quadro
    {
        [Key]
        public int id { get; set; }
        public string nome { get; set; }
        public string ativo { get; set; }
        public int usuarioid { get; set; }
    }
}