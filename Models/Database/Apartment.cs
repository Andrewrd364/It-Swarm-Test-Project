using System.Diagnostics.Metrics;

namespace It_Swarm_Test_Project.Models.Database
{
    public class Apartment
    {
        public int Id { get; set; } // Первичный ключ
        public required string XPathName { get; set; } // Например, "<Улица>/<Дом>/<Номер квартиры>"
        public int? CurrentMeterId { get; set; } // Внешний ключ на счетчик

        // Навигационные свойства
        public Meter? CurrentMeter { get; set; } // Текущий установленный счетчик
        public ICollection<MeterReplacementHistory>? ReplacementHistories { get; set; } // История замен

        // Вспомогательные свойства для разбора XPathName
        public string Street => XPathName.Split('/')[0].Replace("<", "").Replace(">", "");
        public string House => XPathName.Split('/')[1].Replace("<", "").Replace(">", "");
        public string ApartmentNumber => XPathName.Split('/')[2].Replace("<", "").Replace(">", "");

        public MeterReading? LatestReading => CurrentMeter?.Readings?.OrderByDescending(r => r.ReadingDate).FirstOrDefault();
    }
}
