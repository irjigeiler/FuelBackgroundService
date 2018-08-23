using System;

namespace BackgroundService
{
    public sealed class FuelPriceDto
    {
        public FuelPriceDto(DateTime date, double price) => (Date, Price) = (date, price);

        public DateTime Date { get; set; }
        public double Price { get; set; }

        public override string ToString() => $"{Date}: {Price}";
    }
}
