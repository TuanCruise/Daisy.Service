using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Daisy.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        // Service Name : aspnet_state
        static ServiceController service = new ServiceController("aspnet_state");

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int nCount = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                // Start Service
                if (nCount == 10)
                {
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        RestartService(60000);
                    }
                    else
                    {
                        StartService(60000);
                    }
                    _logger.LogInformation("Service aspnet_state is running");
                }

                await Task.Delay(1000, stoppingToken);
                nCount++;
            }
        }
        #region Service Actions
        public static void RestartService(int timeoutMilliseconds)
        {
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {

            }
        }
        public static void StopService(int timeoutMilliseconds)
        {
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch
            {
                // 
            }
        }
        public static void StartService(int timeoutMilliseconds)
        {
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // 
            }
        }
        #endregion
    }
}
