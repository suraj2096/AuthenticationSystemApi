using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Jwt authorize header using the Bearer schema \r\n\r\n" + "Enter 'Bearer'[space] & then your token in the text input below \r\n\r\n" + "Name=Authorization\r\n" + "In:header",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                 {new OpenApiSecurityScheme(){Reference = new OpenApiReference(){
                 Type = ReferenceType.SecurityScheme,
                 Id = "Bearer"
                },
                Scheme = "sauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
                },
                new List<string>()}
            });
        }
    }
}
