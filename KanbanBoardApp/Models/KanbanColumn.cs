using System.Collections.ObjectModel;
using System.ComponentModel;


namespace KanbanBoardApp.Models
{
    /// <summary>
    /// Represents a column in the Kanban board, containing a collection of cards and a title.
    /// </summary>
    public class KanbanColumn : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private bool _isEditing;

        /// <summary>
        /// Gets or sets the collection of cards in this column.
        /// </summary>
        public ObservableCollection<KanbanCard> Cards { get; set; } = new();

        /// <summary>
        /// Gets the number of cards in this column.
        /// </summary>
        public int CardCount => Cards?.Count ?? 0;

        /// <summary>
        /// Gets or sets the title of the column.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column title is currently being edited.
        /// </summary>
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    OnPropertyChanged(nameof(IsEditing));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KanbanColumn"/> class.
        /// </summary>
        public KanbanColumn()
        {
            Cards.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CardCount));
        }

        /// <summary>
        /// Ensures that the CardCount property is updated and notifies the UI
        /// when the Cards collection changes eg after loading new board.
        /// </summary>
        public void EnsureCardCountBinding()
        {
            Cards.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CardCount));
        }


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
