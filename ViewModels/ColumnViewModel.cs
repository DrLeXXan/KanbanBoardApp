using KanbanBoardApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KanbanBoardApp.ViewModels
{
    public class MainViewModel
    //public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<KanbanColumn> Columns { get; set; }
        public ICommand AddColumnsCommand { get; }
        public ICommand DeleteColumnCommand { get; set; }
        public ICommand AddCardCommand { get; }

        public MainViewModel()
        {

            Columns = new ObservableCollection<KanbanColumn>
            {
                new KanbanColumn { Title = "To Do" },
                new KanbanColumn { Title = "In Progress" },
                new KanbanColumn { Title = "Review" },
                new KanbanColumn { Title = "Done" }
            };

            AddColumnsCommand = new RelayCommand(AddColumnHandler);
            DeleteColumnCommand = new RelayCommand(DeleteColumnHandler);
            AddCardCommand = new RelayCommand(AddCardHandler);
        }

        private void AddColumnHandler(object? parameter)
        {
            var currentColumn = parameter as KanbanColumn;
            if (currentColumn != null)
            {
                int index = Columns.IndexOf(currentColumn);
                if (index >= 0)
                {
                    Columns.Insert(index + 1, new KanbanColumn { Title = "New Column" });
                    return;
                }
            }
            // Fallback: add at end if parameter is null or not found
            Columns.Add(new KanbanColumn { Title = "New Column" });
        }

        private void DeleteColumnHandler(object? parameter)
        {
            // This will be handled in the code-behind for confirmation
        }

        private void AddCardHandler(object? parameter)
        {
            if (parameter is KanbanColumn column)
            {
                var dialog = new KanbanBoardApp.View.CardDialog();
                if (dialog.ShowDialog() == true)
                {
                    var card = dialog.GetCard();
                    if (card != null)
                        column.Cards.Add(card);
                }
            }
        }

        // Implement INotifyPropertyChanged...
    }   
}
