using KanbanBoardApp.Models;
using KanbanBoardApp.View;
using KanbanBoardApp.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KanbanBoardApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            if (DataContext is MainViewModel vm)
            {
                vm.DeleteColumnCommand = new RelayCommand(param =>
                {
                    if (param is KanbanColumn column)
                        DeleteColumnWithConfirmation(column);
                });
            }
        }

        private void AddCard(KanbanColumn? defaultColumn = null)
        {
            var vm = DataContext as MainViewModel;
            if (vm == null) return;

            var dialog = new CardDialog(null, vm.Columns.ToList(), defaultColumn ?? vm.Columns.FirstOrDefault());
            if (dialog.ShowDialog() == true)
            {
                var card = dialog.GetCard();
                vm.AddCardToColumn(card);
            }
        }

        // Use this for both header and column add buttons:
        private void btnAdd_Card(object sender, RoutedEventArgs e)
        {
            KanbanColumn? defaultColumn = null;
            if (sender is Button btn && btn.DataContext is KanbanColumn col)
                defaultColumn = col;
            AddCard(defaultColumn);
        }

        private void ColumnTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (sender is TextBlock tb && tb.DataContext is KanbanBoardApp.Models.KanbanColumn column)
                {
                    column.IsEditing = true;
                }
            }
        }

        private void ColumnTitleTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox tb && tb.DataContext is KanbanBoardApp.Models.KanbanColumn column)
                {
                    column.IsEditing = false;
                }
            }
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // If the click is inside a TextBox, do nothing (let editing continue)
            if (e.OriginalSource is DependencyObject depObj)
            {
                var parent = depObj;
                while (parent != null)
                {
                    if (parent is TextBox)
                        return;
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }

            // Otherwise, set IsEditing = false for all columns
            if (DataContext is KanbanBoardApp.ViewModels.MainViewModel vm)
            {
                foreach (var col in vm.Columns)
                    col.IsEditing = false;
            }
        }

        public void DeleteColumnWithConfirmation(KanbanColumn column)
        {
            if (column.Cards.Count > 0)
            {
                MessageBox.Show("You can only delete a column that has no cards.", "Delete Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                var result = MessageBox.Show(
                $"Are you sure you want to delete the column \"{column.Title}\"? This action cannot be undone!",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (DataContext is MainViewModel vm && vm.Columns.Contains(column))
                    {
                        vm.Columns.Remove(column);
                    }
                }
            }
        }

        private void Open_Card_In_Dialog(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is Border border && border.DataContext is KanbanCard card)
            {
                var vm = (MainViewModel)DataContext;
                var currentColumn = vm.Columns.FirstOrDefault(col => col.Cards.Contains(card));
                if (currentColumn == null) return;

                var cardCopy = card.Clone();
                var dialog = new CardDialog(cardCopy, vm.Columns.ToList());
                if (dialog.ShowDialog() == true)
                {
                    if (dialog.IsDeleteRequested)
                    {
                        currentColumn.Cards.Remove(card);
                    }
                    else
                    {
                        var edited = dialog.GetCard();

                        // Move card to new column if status changed
                        if (edited.Status != currentColumn.Title)
                        {
                            var newColumn = vm.Columns.FirstOrDefault(col => col.Title == edited.Status);
                            if (newColumn != null)
                            {
                                // Record status change before moving and updating
                                RecordCardHistory(card, edited);

                                currentColumn.Cards.Remove(card);
                                newColumn.Cards.Add(card);
                                // Update card properties
                                card.Title = edited.Title;
                                card.Owner = edited.Owner;
                                card.Description = edited.Description;
                                card.Urgency = edited.Urgency;
                                card.Status = edited.Status;
                                card.DueDate = edited.DueDate;
                                card.Comment = edited.Comment;

                                // Sync history to the clone for dialog display
                                cardCopy.History.Clear();
                                foreach (var entry in card.History)
                                    cardCopy.History.Add(entry);
                                return;
                            }
                        }

                        RecordCardHistory(card, edited);

                        // Update card properties if not moved
                        card.Title = edited.Title;
                        card.Owner = edited.Owner;
                        card.Description = edited.Description;
                        card.Urgency = edited.Urgency;
                        card.Status = edited.Status;
                        card.DueDate = edited.DueDate;
                        card.Comment = edited.Comment;

                        // Sync history to the clone for dialog display
                        cardCopy.History.Clear();
                        foreach (var entry in card.History)
                            cardCopy.History.Add(entry);
                    }
                }
            }
        }

        private Point _dragStartPoint;
        private KanbanCard? _draggedCard;
        private KanbanColumn? _sourceColumn;

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);

            if (e.ClickCount == 2)
            {
                Open_Card_In_Dialog(sender, e);
            }
        }

        // This Function is firering even if the mouse it not clicked!!! Causing issues!!!!!!
        private void Card_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(null);
                if (Math.Abs(pos.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(pos.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (sender is Border border && border.DataContext is KanbanBoardApp.Models.KanbanCard card)
                    {
                        DragDrop.DoDragDrop(border, card, DragDropEffects.Move);
                    }
                }
            }
        }

        private void Cards_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KanbanBoardApp.Models.KanbanCard)))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void Cards_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KanbanBoardApp.Models.KanbanCard)))
            {
                var card = e.Data.GetData(typeof(KanbanBoardApp.Models.KanbanCard)) as KanbanBoardApp.Models.KanbanCard;
                if (card == null) return;

                // sender is the Border (the column)
                var border = sender as Border;
                var targetColumn = border?.DataContext as KanbanBoardApp.Models.KanbanColumn;
                if (targetColumn == null) return;

                var vm = DataContext as KanbanBoardApp.ViewModels.MainViewModel;
                var sourceColumn = vm?.Columns.FirstOrDefault(col => col.Cards.Contains(card));
                if (sourceColumn == null)
                {
                    MessageBox.Show("Source column not found!");
                    return;
                }

                if (sourceColumn != targetColumn)
                {
                    // Record history before changing status
                    string oldStatus = card.Status;
                    string newStatus = targetColumn.Title;

                    sourceColumn.Cards.Remove(card);
                    targetColumn.Cards.Add(card);

                    // Update the card's status
                    card.Status = newStatus;

                    // Record the status change in history
                    card.History.Add(new UserActivityEntry
                    {
                        Timestamp = DateTime.Now,
                        PropertyChanged = "Status",
                        OldValue = oldStatus,
                        NewValue = newStatus,
                        ChangedBy = Environment.UserName
                    });
                }
            }
        }

        private Point _columnDragStartPoint;
        private KanbanColumn? _draggedColumn;

        // Column header mouse down: start drag
        private void ColumnHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _columnDragStartPoint = e.GetPosition(null);

            if (sender is Border border && border.DataContext is KanbanColumn column)
            {
                _draggedColumn = column;
            }
        }

        // Column header mouse move: initiate drag if moved enough
        private void ColumnHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _draggedColumn != null)
            {
                var pos = e.GetPosition(null);
                if (Math.Abs(pos.X - _columnDragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(pos.Y - _columnDragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    DragDrop.DoDragDrop((DependencyObject)sender, _draggedColumn, DragDropEffects.Move);
                    _draggedColumn = null;
                }
            }
        }

        // Allow dropping on header
        private void ColumnHeader_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KanbanColumn)))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        // Handle drop: reorder columns
        private void ColumnHeader_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KanbanColumn)))
            {
                var draggedColumn = e.Data.GetData(typeof(KanbanColumn)) as KanbanColumn;
                if (sender is Border border && border.DataContext is KanbanColumn targetColumn && draggedColumn != null && draggedColumn != targetColumn)
                {
                    var vm = DataContext as KanbanBoardApp.ViewModels.MainViewModel;
                    if (vm == null) return;

                    int oldIndex = vm.Columns.IndexOf(draggedColumn);
                    int newIndex = vm.Columns.IndexOf(targetColumn);

                    if (oldIndex >= 0 && newIndex >= 0 && oldIndex != newIndex)
                    {
                        vm.Columns.Move(oldIndex, newIndex);
                    }
                }
            }
        }

        private void RecordCardHistory(KanbanCard card, KanbanCard edited)
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