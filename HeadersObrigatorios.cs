using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace remarsemanal
{
    public class HeadersObrigatorios : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if(operation.Parameters == null){
                operation.Parameters = new List<OpenApiParameter>();
            }
            operation.Parameters.Add(new OpenApiParameter{
                Name = "X-Empresa-Guid",
                In = ParameterLocation.Header,
                Required = true,
                Description = "Esse cabecalho é obrigatório define para qual empresa(banco de dados) a api deve fazer a requisição",
            });
        }
    }
}