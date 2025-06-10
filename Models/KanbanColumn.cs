using System.Collections.ObjectModel;
using System.ComponentModel;


namespace KanbanBoardApp.Models
{
    public class KanbanColumn : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private bool _isEditing;
        public ObservableCollection<KanbanCard> Cards { get; set; } = new();
        public int CardCount => Cards?.Count ?? 0;

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

        public KanbanColumn()
        {
            Cards.CollectionChanged += (s, e) => OnPropertyChanged(nameof(CardCount));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    }
}
