using System;
using System.Reflection;

using log4net;

using BackgroundService.Properties;
using System.Linq;

namespace BackgroundService
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            var apiClient = new FuelApiClient(Settings.Default.ApiUrl);
            using(var context = new FuelDbContext())
            {
                var editService = new FuelPriceEditService(context);
                using(var service = new FuelBackgroundService(Settings.Default.DaysCount, Settings.Default.TaskExecutionDelay, apiClient, editService, log))
                {
                    service.Start();
                    PressAnyKeyForContinue("Press any key for stop.");
                    service.Stop();

                    var prices = context.Prices.ToList();
                    var message = String.Join(Environment.NewLine, prices);
                    Console.WriteLine($"{Environment.NewLine}Fuel prices:");
                    Console.WriteLine(message);
                    PressAnyKeyForContinue("Press any key for exit...");
                }
            }
        }

        private static void PressAnyKeyForContinue(string message)
        {
            Console.WriteLine(message);
            Console.ReadKey();
        }
    }
}
