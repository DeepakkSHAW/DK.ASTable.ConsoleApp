using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using DK.AzureTableStorage.Operations.Models;
using DK.AzureTableStorage.Operations.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace DK.AzureFn.TableStorageAPI
{
    public class FunctionOrder
    {
        private readonly ILogger<FunctionOrder> _logger;

        private readonly IAzureTableHandler _azTblOperation;
        //public FunctionOrder(ILogger<FunctionOrder> log)
        //{
        //    _logger = log;
        //}

        public FunctionOrder(IAzureTableHandler azTblOperation, ILogger<FunctionOrder> log)
        {
            _azTblOperation = azTblOperation;
            _logger = log;
        }

        [FunctionName("GetOrders")]
        [OpenApiOperation(operationId: "Get Orders", tags: new[] { "NTT Demo >> Orders API" }, Summary = "Gets All Orders", Description = "This API returns all Orders in system", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(List<OrderItemEntity>), Summary = "The response - Order List", Description = "This returns Entire Order List")]
        public async Task<IActionResult> Run_GetOrders(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Requested for entire order list from Azure storage table");

            var azOpt = await _azTblOperation.GetOrdersAsync();
            //var v = azOpt.Count;
            return new OkObjectResult(azOpt);
        }

        [FunctionName("GetCustomersOrder")]
        [OpenApiOperation(operationId: "Get All Orders for a customer", tags: new[] { "NTT Demo >> Orders API" }, Summary = "Gets Orders by customer", Description = "This API returns all Orders for a Customer", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "CUST_ID", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Customer name e.g. CUST_001** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(List<OrderItemEntity>), Summary = "The response - Order List for a customer", Description = "This API returns Order List for a customer")]
        public async Task<IActionResult> GetCustomers_Order(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Fetching Orders for a customer");

            string cust_name = req.Query["CUST_ID"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            cust_name = cust_name ?? data?.CUST_ID;

            var azOpt = await _azTblOperation.GetOrdersAsync(cust_name);
            //if (azOpt.Count == 0) 
            //    return NotFound(); 
            //throw new HttpResponseException(HttpStatusCode.NotFound);
            return new OkObjectResult(azOpt);
        }

        [FunctionName("GetCustomersOrderOnDate")]
        [OpenApiOperation(operationId: "Get All Orders for a customer on particular date", tags: new[] { "NTT Demo >> Orders API" }, Summary = "Gets Orders by customer on date", Description = "This API returns all Orders for a Customer on a particular date", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "CUST_ID", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Customer name e.g. CUST_001** parameter")]
        [OpenApiParameter(name: "Order_Date", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Description = "The **Customer name e.g. CUST_001** and Date as parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(List<OrderItemEntity>), Summary = "The response - Order List for a customer", Description = "This API returns Order List for a customer")]
        public async Task<IActionResult> GetCustomersOrder_OnDate(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Fetching Orders for a customer on particular date");

            string cust_name = req.Query["CUST_ID"];
            DateTime? onDate = DateTime.Parse(req.Query["Order_Date"]);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            cust_name = cust_name ?? data?.CUST_ID;
            onDate = onDate ?? DateTime.Parse(data?.Order_Date);

            var azOpt = await _azTblOperation.GetOrdersAsync(cust_name, onDate);
            //if (azOpt.Count == 0) 
            //    return NotFound(); 
            //throw new HttpResponseException(HttpStatusCode.NotFound);
            return new OkObjectResult(azOpt);
        }
    }
}

