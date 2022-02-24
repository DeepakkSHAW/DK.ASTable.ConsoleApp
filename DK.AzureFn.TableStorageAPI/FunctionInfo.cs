using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace DK.AzureFn.TableStorageAPI
{
    public class FunctionInfo
    {
        private readonly ILogger<FunctionInfo> _logger;
        private readonly IConfiguration _config;
        public FunctionInfo(ILogger<FunctionInfo> log, IConfiguration config)
        {
            _logger = log;
            _config = config;
        }

        [FunctionName("FunctionInfo")]
        [OpenApiOperation(operationId: "getName", tags: new[] { "Function Info" }, Summary = "Gets All Orders", Description = "This API returns all Orders in system", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Your name", Description = "Provide your name", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]

        public async Task<IActionResult> Function_Info(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string msg = req.Query["name"];
            msg= msg ?? "Please provide the name";

            var title = _config.GetValue<string>($"ApplicationSettings:Title");
            var developer = _config.GetValue<string>($"ApplicationSettings:Developer");

            return new OkObjectResult(new { Title = title, 
                                            DesignedBy = $"Developed & Designed by {developer}",
                                            Message = msg});
        }
    }
}

