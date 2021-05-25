using System;
using System.ComponentModel.DataAnnotations;

namespace remarsemanal.Model
{
    public class Empresa
    {
        [Key]
        public int CodEmpresa{get;set;}
        public string Nome {get;set;}
        public int? CodGrupoEmpresa{get;set;}
        public string StringConexao {get;set;}
        public Guid UUID {get;set;}
        public DateTime? CriadoEm{get;set;}
        public DateTime? AtualizadoEm {get;set;}
        public DateTime? DesativadoEm{get;set;}
        public int? DesativadoPor{get;set;}
        public string Ativo {get;set;} = "S";
        public string Sincronizado {get;set;} = "N";
        public string PastaAWS {get;set;}
    }
}