using System;
using System.ComponentModel.DataAnnotations;

namespace remarsemanal.Model
{
    public class Tarefa
    {
        [Key]
        public int id { get; set; }
        public string nome { get; set; }
        public Quadro Quadro { get; set; }
        public int quadroid { get; set; }
        public Tipo Tipo { get; set; }
        public int tipoid { get; set; }
        public DateTime? data { get; set; }
        public string tarefaid { get; set; }
        public string cliente { get; set; }
        public int usuarioid { get; set; }
        public string urgente { get; set; }
    }
}