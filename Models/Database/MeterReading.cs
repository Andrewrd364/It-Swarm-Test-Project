namespace It_Swarm_Test_Project.Models.Database
{
    public class MeterReading
    {
        public int Id { get; set; } // Первичный ключ
        public int MeterId { get; set; } // Внешний ключ на счетчик
        public required decimal Value { get; set; } // Значение показания
        public required DateTime ReadingDate { get; set; } // Дата снятия показания

        // Навигационное свойство
        public required Meter Meter { get; set; } // Ссылка на счетчик
    }
}
