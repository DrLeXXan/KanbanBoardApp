using KanbanBoardApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Diagnostics;

namespace KanbanBoardApp.ViewModels
{
    /// <summary>
    /// The main ViewModel for the Kanban board application.
    /// Manages columns, cards, and board persistence.
    /// </summary>
    public class MainViewModel

    {
        /// <summary>
        /// Gets or sets the collection of columns in the Kanban board.
        /// </summary>
        public ObservableCollection<KanbanColumn> Columns { get; set; }

        /// <summary>
        /// Gets the command to add a new column.
        /// </summary>
        public ICommand AddColumnCommand { get; }

        /// <summary>
        /// Gets or sets the command to delete a column.
        /// </summary>
        public ICommand DeleteColumnCommand { get; set; }

        /// <summary>
        /// Gets the command to add a new card.
        /// </summary>
        public ICommand AddCardCommand { get; }

        /// <summary>
        /// Gets the command to save the board.
        /// </summary>
        public ICommand SaveBoardCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
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
                var stopwatch = Stopwatch.StartNew();

                var dialog = new KanbanBoardApp.View.CardDialog(null, Columns.ToList(), column);
                bool? result = dialog.ShowDialog();

                stopwatch.Stop();
                long elapsedMs = stopwatch.ElapsedMilliseconds;

                // Define your log file path (e.g., in the user's Documents folder)
                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "KanbanBoardApp_CardDialogLog.txt");

                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - CardDialog open duration: {elapsedMs} ms{Environment.NewLine}";

                // Append the log entry to the file
                File.AppendAllText(logPath, logEntry);

                if (result == true)
                {
                    var card = dialog.GetCard();
                    if (card != null)
                        AddCardToColumn(card);
                }
            }
        }

        /// <summary>
        /// Adds a new card to the specified column, or the first column if not specified.
        /// Also records the creation in the card's history.
        /// </summary>
        /// <param name="card">The card to add.</param>
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

        /// <summary>
        /// Removes the specified column from the board.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        public void RemoveColumn(KanbanColumn column)
        {
            if (Columns.Contains(column))
                Columns.Remove(column);
        }

        /// <summary>
        /// Saves the current board state to a JSON file.
        /// </summary>
        /// <param name="filePath">The file path to save to. If null, the method does nothing.</param>
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

        /// <summary>
        /// Loads the board state from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string representing the board.</param>
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
                int maxId = 0;
                foreach (var col in columns)
                {
                    col.EnsureCardCountBinding();
                    Columns.Add(col);
                    foreach (var card in col.Cards)
                    {
                        if (card.Id > maxId)
                            maxId = card.Id;
                    }
                }
                // Set the next ID to one higher than the highest found
                if (maxId >= 1)
                    typeof(KanbanCard).GetField("_nextId", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.SetValue(null, maxId + 1);
            }
        }

        /// <summary>
        /// Moves a card to a different column and records the status change in history.
        /// </summary>
        /// <param name="card">The card to move.</param>
        /// <param name="targetColumn">The column to move the card to.</param>
        public void MoveCard(KanbanCard card, KanbanColumn targetColumn)
        {
            var sourceColumn = Columns.FirstOrDefault(col => col.Cards.Contains(card));
            if (sourceColumn == null || targetColumn == null || sourceColumn == targetColumn)
                return;

            // Remove from source, add to target
            sourceColumn.Cards.Remove(card);
            targetColumn.Cards.Add(card);

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

        /// <summary>
        /// Moves a column to a new index in the collection.
        /// </summary>
        /// <param name="column">The column to move.</param>
        /// <param name="newIndex">The new index for the column.</param>
        public void MoveColumn(KanbanColumn column, int newIndex)
        {
            int oldIndex = Columns.IndexOf(column);
            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex)
                return;

            Columns.Move(oldIndex, newIndex);
        }

        /// <summary>
        /// Records changes made to a card by comparing it to an edited version.
        /// Adds history entries for each changed property.
        /// </summary>
        /// <param name="card">The original card.</param>
        /// <param name="edited">The edited card.</param>
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
                    OldValue = card.DueDate?.ToString("yyyy-MM-dd") ?? "",
                    NewValue = edited.DueDate?.ToString("yyyy-MM-dd") ?? "",
                    ChangedBy = user
                });

            if (card.Comment != edited.Comment)
                card.History.Add(new UserActivityEntry
                {
                    Timestamp = DateTime.Now,
                    PropertyChanged = "Comment",
                    OldValue = card.Comment,
                    NewValue = edited.Comment,
                    ChangedBy = user
                });
        }

    }   
}
