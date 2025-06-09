using KanbanBoardApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace KanbanBoardApp.ViewModels
{
    public class MainViewModel
    //public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<KanbanColumn> Columns { get; set; }
        public ICommand AddColumnsCommand { get; }
        public ICommand DeleteColumnCommand { get; set; }
        public ICommand AddCardCommand { get; }
        public ICommand SaveBoardCommand { get; }

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
            SaveBoardCommand = new RelayCommand(_ => SaveBoard());
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
                var dialog = new KanbanBoardApp.View.CardDialog(null, Columns.ToList(), column);
                if (dialog.ShowDialog() == true)
                {
                    var card = dialog.GetCard();
                    if (card != null)
                        AddCardToColumn(card);
                }
            }
        }

        public void AddCardToColumn(KanbanCard card)
        {
            var targetColumn = Columns.FirstOrDefault(c => c.Title == card.Status);
            if (targetColumn != null)
                targetColumn.Cards.Add(card);
            else if (Columns.Any())
                Columns[0].Cards.Add(card);

            // Add initial history entry
            card.History.Add(new UserActivityEntry
            {
                Timestamp = DateTime.Now,
                PropertyChanged = "Created",
                OldValue = "",
                NewValue = $"Title: {card.Title}, Status: {card.Status}",
                ChangedBy = Environment.UserName
            });
        }

        public void SaveBoard(string? filePath = null)
        {
            var boardData = Columns.ToList();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string json = JsonSerializer.Serialize(boardData, options);

            if (filePath == null)
                return;

            File.WriteAllText(filePath, json);
        }

        public void LoadBoardFromJson(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            var columns = JsonSerializer.Deserialize<List<KanbanColumn>>(json, options);
            if (columns != null)
            {
                Columns.Clear();
                foreach (var col in columns)
                    Columns.Add(col);
            }
        }

    }   
}
