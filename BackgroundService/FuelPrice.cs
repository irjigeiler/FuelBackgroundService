using System;

namespace BackgroundService
{
    public class FuelPrice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }

        public override string ToString() => $"{Date}: {Price}";
    }
}
