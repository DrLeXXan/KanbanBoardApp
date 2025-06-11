
namespace KanbanBoardApp.Models
{
    /// <summary>
    /// Represents a single entry in the user activity history for a Kanban card.
    /// Tracks changes to card properties, including who made the change and when.
    /// </summary>
    public class UserActivityEntry
    {
        /// <summary>
        /// Gets or sets the timestamp when the change occurred.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the name of the property that was changed.
        /// </summary>
        public required string PropertyChanged { get; set; }

        /// <summary>
        /// Gets or sets the old value of the property before the change.
        /// </summary>
        public required string OldValue { get; set; }

        /// <summary>
        /// Gets or sets the new value of the property after the change.
        /// </summary>
        public required string NewValue { get; set; }

        /// <summary>
        /// Gets or sets the user who made the change.
        /// </summary>
        public required string ChangedBy { get; set; } // Optional for boards with multiple users
    }
}
