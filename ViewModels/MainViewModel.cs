using KanbanBoardApp.Models;
using System.Collections.ObjectModel;
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
        public ICommand AddColumnCommand { get; }
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

            AddColumnCommand = new RelayCommand(AddColumnHandler);
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

        public void RemoveColumn(KanbanColumn column)
        {
            if (Columns.Contains(column))
                Columns.Remove(column);
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

        public void MoveCard(KanbanCard card, KanbanColumn targetColumn)
        {
            var sourceColumn = Columns.FirstOrDefault(col => col.Cards.Contains(card));
            if (sourceColumn == null || targetColumn == null || sourceColumn == targetColumn)
                return;

            // Remove from source, add to target
            sourceColumn.Cards.Remove(card);
            targetColumn.Cards.Add(card);

            // Update card status
            string oldStatus = card.Status;
            card.Status = targetColumn.Title;

            // Record history
            card.History.Add(new UserActivityEntry
            {
                Timestamp = DateTime.Now,
                PropertyChanged = "Status",
                OldValue = oldStatus,
                NewValue = card.Status,
                ChangedBy = Environment.UserName
            });
        }

        public void MoveColumn(KanbanColumn column, int newIndex)
        {
            int oldIndex = Columns.IndexOf(column);
            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex)
                return;

            Columns.Move(oldIndex, newIndex);
        }

        public void RecordCardHistory(KanbanCard card, KanbanCard edited)
        {
            var user = Environment.UserName;

            if (card.Title != edited.Title)
                card.History.Add(new UserActivityEntry
                {
                    Timestamp = DateTime.Now,
                    PropertyChanged = "Title",
                    OldValue = card.Title,
                    NewValue = edited.Title,
                    ChangedBy = user
                });

            if (card.Owner != edited.Owner)
                card.History.Add(new UserActivityEntry
                {
                    Timestamp = DateTime.Now,
                    PropertyChanged = "Owner",
                    OldValue = card.Owner,
                    NewValue = edited.Owner,
                    ChangedBy = user
                });

            if (card.Description != edited.Description)
                card.History.Add(new UserActivityEntry
                {
                    Timestamp = DateTime.Now,
                    PropertyChanged = "Description",
                    OldValue = card.Description,
                    NewValue = edited.Description,
                    ChangedBy = user
                });

            if (card.Urgency != edited.Urgency)
                card.History.Add(new UserActivityEntry
                {
                    Timestamp = DateTime.Now,
                    PropertyChanged = "Urgency",
                    OldValue = card.Urgency,
                    NewValue = edited.Urgency,
                    ChangedBy = user
                });

            if (card.Status != edited.Status)
                card.History.Add(new UserActivityEntry
                {
                    Timestamp = DateTime.Now,
                    PropertyChanged = "Status",
                    OldValue = card.Status,
                    NewValue = edited.Status,
                    ChangedBy = user
                });

            if (card.DueDate != edited.DueDate)
                card.History.Add(new UserActivityEntry
                {
                    Timestamp = DateTime.Now,
                    PropertyChanged = "DueDate",
                    OldValue = card.DueDate?.ToString() ?? "",
                    NewValue = edited.DueDate?.ToString() ?? "",
                    ChangedBy = user
                });

            if (card.Comment != edited.Comment)
                card.History.Add(new UserActivityEntry
                {
                    Timestamp = DateTime.Now,
                    PropertyChanged = "Description",
                    OldValue = card.Description,
                    NewValue = edited.Description,
                    ChangedBy = user
                });
        }

    }   
}
