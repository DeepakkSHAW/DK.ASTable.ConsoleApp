using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DK.AzureFn.TableStorageAPI
{
    public class FnHeartBit
    {
        [FunctionName("FnHeartBit")]
        [Disable]
        public void HeartBit([TimerTrigger("%Schedule:HeartBit:CronExpression%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Azure Function Heart Bit Checked at: {DateTime.Now}");
            log.LogWarning($"Azure Storage Clean up process will begine");
        }
    }
}
