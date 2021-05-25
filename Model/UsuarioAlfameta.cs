using System.ComponentModel.DataAnnotations;

namespace remarsemanal.Model
{
    public class UsuarioAlfameta
    {
        [Key]
        public int CodUsuario{get;set;}
        public string Login {get;set;}
        public string PrimeiroAcesso {get;set;} = "S";
        public string Token {get;set;}
        public string Nome {get;set;}
        public string Ativo {get;set;} = "S";
        public byte[] SenhaHash {get;set;}
        public byte[] SenhaDificuldade {get;set;}
    }
}