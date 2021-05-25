using System;

namespace remarsemanal.Model
{
    public class TarefaCadastro
    {
        public string nome { get; set; }
        public DateTime? data { get; set; } = null;
        public int quadroid { get; set; }
        public int tipoid { get; set; }
        public string tarefaid { get; set; }
        public string cliente { get; set; }
        public int usuarioid { get; set; }
        public string urgente { get; set; } = "N";
    }
}