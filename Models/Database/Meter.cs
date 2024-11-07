namespace It_Swarm_Test_Project.Models.Database
{
    public class Meter
    {
        public int Id { get; set; } // Первичный ключ
        public required string SerialNumber { get; set; } // Заводской номер
        public required DateTime LastVerificationDate { get; set; } // Дата последней поверки
        public required DateTime NextVerificationDate { get; set; } // Дата следующей поверки

        // Навигационные свойства
        public ICollection<MeterReading>? Readings { get; set; } // Показания счетчика
        public ICollection<MeterReplacementHistory>? ReplacementHistories { get; set; } // История замен
    }
}
