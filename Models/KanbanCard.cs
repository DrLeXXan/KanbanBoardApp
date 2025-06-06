using System;
using System.ComponentModel;

namespace KanbanBoardApp.Models
{
    public class KanbanCard : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _owner = string.Empty;
        private string _description = string.Empty;
        private string _urgency = string.Empty;
        private DateTime? _dueDate;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }
        public string Owner
        {
            get => _owner;
            set { _owner = value; OnPropertyChanged(nameof(Owner)); }
        }
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }
        public string Urgency
        {
            get => _urgency;
            set { _urgency = value; OnPropertyChanged(nameof(Urgency)); }
        }
        public DateTime? DueDate
        {
            get => _dueDate;
            set { _dueDate = value; OnPropertyChanged(nameof(DueDate)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
