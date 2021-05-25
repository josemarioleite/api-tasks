using remarsemanal.Model.usuario;

namespace remarsemanal.Utils
{
    public class Hash
    {
        public void HasheiaSenha(UsuarioCadastro usuario) {
            if(usuario.senha != null && !string.IsNullOrWhiteSpace(usuario.senha)){
                using (var hmac = new System.Security.Cryptography.HMACSHA512()){
                    usuario.senhacadastro = hmac.Key;
                    usuario.senhahash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(usuario.senha));
                }
            }
        }

        public void HasheiaSenha(UsuarioAlteraSenha usuario) {
            if(usuario.senha != null && !string.IsNullOrWhiteSpace(usuario.senha)) {
                using (var hmac = new System.Security.Cryptography.HMACSHA512()){
                    usuario.senhanova = hmac.Key;
                    usuario.senhahash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(usuario.senha));
                }
            }
        }

        public bool VerificaSenhaHash(Usuario usuario, string senha) {
            if (senha != null && !string.IsNullOrWhiteSpace(senha)) {
                if (usuario.senha.Length == 128 && usuario.senhahash.Length == 64) {
                    using (var hmac = new System.Security.Cryptography.HMACSHA512(usuario.senha)) {
                        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha));
                        for (int i = 0; i < computedHash.Length; i++) {
                            if (computedHash[i] != usuario.senhahash[i]) {
                                return false;
                            }
                        }
                        return true;
                    }
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }
    }
}