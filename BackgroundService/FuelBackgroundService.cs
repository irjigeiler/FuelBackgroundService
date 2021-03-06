﻿using System;
using System.Timers;

using log4net;

namespace BackgroundService
{
    public sealed class FuelBackgroundService : IDisposable
    {
        public FuelBackgroundService(int daysCount, TimeSpan interval, FuelApiClient fuelApiClient, FuelPriceEditService priceEditService, ILog logger = null)
        {
            if(daysCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(daysCount));
            }
            else if(fuelApiClient == null)
            {
                throw new ArgumentNullException(nameof(fuelApiClient));
            }
            else if(priceEditService == null)
            {
                throw new ArgumentNullException(nameof(priceEditService));
            }

            DaysCount = daysCount;
            Interval = interval;
            FuelApiClient = fuelApiClient;
            PriceEditService = priceEditService;
            Logger = logger;
            Timer = new Timer
            {
                AutoReset = false,
                Interval = interval.TotalMilliseconds,
            };

            Timer.Elapsed += (sender, e) => DoWork();
        }

        private int DaysCount { get; }
        private TimeSpan Interval { get; }
        private Timer Timer { get; }
        private FuelApiClient FuelApiClient { get; }
        private FuelPriceEditService PriceEditService { get; }
        private ILog Logger { get; }

        public void Start()
        {
            WriteLog($"Background service is started. DaysCount - {DaysCount}. Interval - {Interval}");
            DoWork();
        }

        public void Stop()
        {
            Timer.Stop();
            WriteLog("Background service is stopped");
        }

        private void DoWork()
        {
            try
            {
                WriteLog("Getting prices...");
                var prices = FuelApiClient.GetPrices(DaysCount);
                var message = String.Join(Environment.NewLine, prices);
                WriteLog(message);
                var updated = PriceEditService.Add(prices);
                WriteLog($"Prices were updated - {updated}");
            }
            catch(Exception ex)
            {
                Logger?.Error(ex);
            }
            finally
            {
                Timer.Start();
            }
        }

        private void WriteLog(string message) => Logger?.Info(message);

        #region IDisposable

        public void Dispose()
        {
            Timer.Dispose();
        }

        #endregion
    }
}
