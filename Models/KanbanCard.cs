using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace KanbanBoardApp.Models
{
    /// <summary>
    /// Represents a Kanban card with properties for title, owner, description, urgency, status, due date, comment, and history.
    /// </summary>
    public class KanbanCard : INotifyPropertyChanged
    {
        private static int _nextId = 1;
        private int _id;
        private string _title = string.Empty;
        private string _owner = string.Empty;
        private string _description = string.Empty;
        private string _urgency = string.Empty;
        private string _status = string.Empty;
        private string _comment = string.Empty;
        private DateTime? _dueDate;

        /// <summary>
        /// Gets or sets the unique identifier for the card.
        /// </summary>
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        /// <summary>
        /// Returns the next available card ID without incrementing the counter.
        /// </summary>
        /// <returns>The next available card ID.</returns>
        public static int PeekNextId() => _nextId ;

        /// <summary>
        /// Returns the next available card ID and increments the counter.
        /// </summary>
        /// <returns>The next available card ID.</returns>
        public static int GetNextId() => _nextId++;

        /// <summary>
        /// Gets or sets the title of the card.
        /// </summary>
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        /// <summary>
        /// Gets or sets the owner of the card.
        /// </summary>
        public string Owner
        {
            get => _owner;
            set { _owner = value; OnPropertyChanged(nameof(Owner)); }
        }

        /// <summary>
        /// Gets or sets the description of the card.
        /// </summary>
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        /// <summary>
        /// Gets or sets the urgency level of the card.
        /// </summary>
        public string Urgency
        {
            get => _urgency;
            set { _urgency = value; OnPropertyChanged(nameof(Urgency)); }
        }

        /// <summary>
        /// Gets or sets the status (column title) of the card.
        /// </summary>
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        /// <summary>
        /// Gets or sets the due date of the card.
        /// </summary>
        public DateTime? DueDate
        {
            get => _dueDate;
            set { _dueDate = value; OnPropertyChanged(nameof(DueDate)); }
        }

        /// <summary>
        /// Gets or sets the comment associated with the card.
        /// </summary>
        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(nameof(Comment)); }
        }

        /// <summary>
        /// Creates a deep copy of this card, including its history.
        /// </summary>
        /// <returns>A new <see cref="KanbanCard"/> instance with the same values.</returns>
        public KanbanCard Clone()
        {
            var clone = new KanbanCard
            {
                Id = this.Id,
                Title = this.Title,
                Owner = this.Owner,
                Description = this.Description,
                Urgency = this.Urgency,
                Status = this.Status,
                DueDate = this.DueDate,
                Comment = this.Comment,
                History = new ObservableCollection<UserActivityEntry>(this.History)
            };
            return clone;
        }

        /// <summary>
        /// Updates this card's properties from another card instance.
        /// </summary>
        /// <param name="other">The card to copy values from.</param>
        public void UpdateFrom(KanbanCard other)
        {
            Title = other.Title;
            Owner = other.Owner;
            Description = other.Description;
            Urgency = other.Urgency;
            Status = other.Status;
            DueDate = other.DueDate;
            Comment = other.Comment;
        }

        /// <summary>
        /// Gets or sets the history of user activity entries for this card.
        /// </summary>
        public ObservableCollection<UserActivityEntry> History { get; set; } = new();

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
