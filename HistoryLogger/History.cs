using System;

namespace HistoryLogger
{
    public class History
    {
        public Guid Id { get; set; }
        public string ItemId { get; set; }
        public string ModelName { get; set; }
        public string UserId { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public int Type { get; set; }
        public string FieldName { get; set; }
        public string OriginalValue { get; set; }
        public string ChangedValue { get; set; }
    }
}