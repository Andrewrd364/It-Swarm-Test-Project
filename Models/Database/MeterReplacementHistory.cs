namespace It_Swarm_Test_Project.Models.Database
{
    public class MeterReplacementHistory
    {
        public int Id { get; set; } // Первичный ключ
        public int ApartmentId { get; set; } // Внешний ключ на квартиру
        public int? NewMeterId { get; set; } // Внешний ключ на новый счетчик, если замена была
        public DateTime InstallationDate { get; set; } // Дата установки
        public int? PreviousMeterReadingId { get; set; } // Внешний ключ на показания старого счетчика, null если устанавливается впервые

        // Навигационные свойства
        public required Apartment Apartment { get; set; } // Ссылка на квартиру
        public Meter? NewMeter { get; set; } // Ссылка на новый счетчик
        public MeterReading? PreviousMeterReading { get; set; } // Ссылка на показания старого счетчика
    }
}
