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

        //public void AddColumn(string title)
        //{
        //    Columns.Add(new KanbanColumn { Title = title });
        //}

        // Implement INotifyPropertyChanged...
    }   
}
