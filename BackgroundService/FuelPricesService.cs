using System;
using System.Collections.Generic;
using System.Linq;

namespace BackgroundService
{
    public sealed class FuelPriceEditService
    {
        private static readonly object _writerLock = new object();

        public FuelPriceEditService(FuelDbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private FuelDbContext Context { get; }

        public int Add(ICollection<FuelPriceDto> prices)
        {
            if(prices == null)
            {
                throw new ArgumentNullException(nameof(prices));
            }

            if(prices.Count == 0)
            {
                return 0;
            }

            lock(_writerLock)
            {
                var newDates = prices.Select(item => item.Date).ToList();
                var excludeDates = Context.Prices.Where(item => newDates.Contains(item.Date)).Select(item => item.Date);
                var hashSet = new HashSet<DateTime>(excludeDates);
                var addEntities = prices.Where(item => !hashSet.Contains(item.Date)).Select(item => new FuelPrice() { Date = item.Date, Price = item.Price }).ToList();
                if(addEntities.Count > 0)
                {
                    Context.Prices.AddRange(addEntities);
                    return Context.SaveChanges();
                }
            }

            return 0;
        }
    }
}
