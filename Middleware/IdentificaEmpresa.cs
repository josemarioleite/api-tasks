using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using remarsemanal.Brokers;

namespace remarsemanal.Middleware
{
    public class IdentificaEmpresa
    {
        private readonly RequestDelegate  _next;
        public IdentificaEmpresa(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, Database dbContext){
            var empresaUUID = httpContext.Request.Headers["X-Empresa-Guid"].FirstOrDefault();
            var empresaApiKey = httpContext.Request.Query["apiKey"].FirstOrDefault();
            if (!string.IsNullOrEmpty(empresaUUID)) {
                var empresa = dbContext.Empresa.FirstOrDefault(e => e.UUID.ToString().ToLower().Equals(empresaUUID.ToLower()) && e.Ativo.Equals("S"));
                httpContext.Items["EMPRESA"] = empresa;
            }else if(!string.IsNullOrEmpty(empresaApiKey)){
                var empresa = dbContext.Empresa.FirstOrDefault(e => e.UUID.ToString().ToLower().Equals(empresaApiKey.ToLower()) && e.Ativo.Equals("S"));
                httpContext.Items["EMPRESA"] = empresa;
            }
            await _next.Invoke(httpContext);
        }
    }

    public static class IdentificadorEmpresaExtensao{
        public static IApplicationBuilder UseEmpresaIdentificador(this IApplicationBuilder app){
            app.UseMiddleware<IdentificaEmpresa>();
            return app;
        }
    }
}