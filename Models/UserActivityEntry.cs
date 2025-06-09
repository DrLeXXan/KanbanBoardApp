using System;

namespace KanbanBoardApp.Models
{
    public class UserActivityEntry
    {
        public DateTime Timestamp { get; set; }
        public string PropertyChanged { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedBy { get; set; } // Optional for boards with multiple users
    }
}
