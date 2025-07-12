using KanbanBoardApp.Models;
using KanbanBoardApp.View;
using KanbanBoardApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Diagnostics;


namespace KanbanBoardApp
{

    /// <summary>
    /// Interaction logic for the main Kanban board window.
    /// Handles UI events, drag-and-drop, and dialog interactions.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Sets up the DataContext and command bindings.
        /// </summary>
        /// 
        private Stopwatch? _cardDragStopwatch;
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

        /// <summary>
        /// Opens the Add Card dialog for the specified column.
        /// </summary>
        /// <param name="defaultColumn">The column to add the card to, or null for the first column.</param>
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

        /// <summary>
        /// Handles the Add Card button click for both header and column buttons.
        /// </summary>
        private void btnAdd_Card(object sender, RoutedEventArgs e)
        {
            KanbanColumn? defaultColumn = null;
            if (sender is Button btn && btn.DataContext is KanbanColumn col)
                defaultColumn = col;
            AddCard(defaultColumn);
        }

        /// <summary>
        /// Handles double-click on a column title to enable editing.
        /// </summary>
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

        /// <summary>
        /// Handles Enter key press to finish editing a column title.
        /// </summary>
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

        /// <summary>
        /// Handles mouse down events to end column title editing when clicking outside a TextBox.
        /// </summary>
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // If the click is inside a TextBox, do nothing
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

        /// <summary>
        /// Prompts the user to confirm column deletion and removes the column if confirmed.
        /// </summary>
        /// <param name="column">The column to delete.</param>
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
                    if (DataContext is MainViewModel vm)
                        vm.RemoveColumn(column);
                }
            }
        }

        /// <summary>
        /// Handles double-click on a card to open the edit dialog.
        /// </summary>
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
                                vm.RecordCardHistory(card, edited);

                                currentColumn.Cards.Remove(card);
                                newColumn.Cards.Add(card);

                                // Update card properties
                                card.UpdateFrom(edited);

                                // Sync history to the clone for dialog display
                                cardCopy.History.Clear();
                                foreach (var entry in card.History)
                                    cardCopy.History.Add(entry);
                                return;
                            }
                        }

                        vm.RecordCardHistory(card, edited);

                        // Update card properties if not moved
                        card.UpdateFrom(edited);

                        // Sync history to the clone for dialog display
                        cardCopy.History.Clear();
                        foreach (var entry in card.History)
                            cardCopy.History.Add(entry);
                    }
                }
            }
        }

        private Point _dragStartPoint;

        /// <summary>
        /// Handles mouse down on a card to start drag or open the edit dialog on double-click.
        /// </summary>
        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);

            // Start timing when drag is initiated
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _cardDragStopwatch = Stopwatch.StartNew();
            }

            if (e.ClickCount == 2)
            {
                Open_Card_In_Dialog(sender, e);
            }
        }

        /// <summary>
        /// Handles mouse move to initiate card drag-and-drop.
        /// </summary>
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

        /// <summary>
        /// Handles drag-over events for cards to show valid drop effects.
        /// </summary>
        private void Cards_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KanbanBoardApp.Models.KanbanCard)))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Handles drop events to move a card to a new column.
        /// </summary>
        private void Cards_Drop(object sender, DragEventArgs e)
        {
            // Stop timing and log when drop occurs
            if (_cardDragStopwatch != null)
            {
                _cardDragStopwatch.Stop();
                long elapsedMs = _cardDragStopwatch.ElapsedMilliseconds;

                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "KanbanBoardApp_CardMoveLog2.txt");

                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Card drag-and-drop duration: {elapsedMs} ms{Environment.NewLine}";
                File.AppendAllText(logPath, logEntry);

                _cardDragStopwatch = null;
            }

            if (e.Data.GetDataPresent(typeof(KanbanCard)))
            {
                var card = e.Data.GetData(typeof(KanbanCard)) as KanbanCard;
                if (card == null) return;

                var border = sender as Border;
                var targetColumn = border?.DataContext as KanbanColumn;
                if (targetColumn == null) return;

                var vm = DataContext as MainViewModel;
                vm?.MoveCard(card, targetColumn);
            }
        }


        private Point _columnDragStartPoint;
        private KanbanColumn? _draggedColumn;

        /// <summary>
        /// Handles mouse down on a column header to start column drag-and-drop.
        /// </summary>
        private void ColumnHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _columnDragStartPoint = e.GetPosition(null);

            if (sender is Border border && border.DataContext is KanbanColumn column)
            {
                _draggedColumn = column;
            }
        }

        /// <summary>
        /// Handles mouse move to initiate column drag-and-drop.
        /// </summary>
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

        /// <summary>
        /// Handles drag-over events for column headers to show valid drop effects.
        /// </summary>
        private void ColumnHeader_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KanbanColumn)))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Handles drop events to reorder columns.
        /// </summary>
        private void ColumnHeader_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(KanbanColumn)))
            {
                var draggedColumn = e.Data.GetData(typeof(KanbanColumn)) as KanbanColumn;
                if (sender is Border border && border.DataContext is KanbanColumn targetColumn && draggedColumn != null && draggedColumn != targetColumn)
                {
                    var vm = DataContext as MainViewModel;
                    if (vm == null) return;

                    int newIndex = vm.Columns.IndexOf(targetColumn);
                    vm.MoveColumn(draggedColumn, newIndex);
                }
            }
        }

        /// <summary>
        /// Handles the Save Board button click to save the board to a JSON file.
        /// </summary>
        private void SaveBoard_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm == null) return;

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json",
                FileName = "KanbanBoard.json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    vm.SaveBoard(dialog.FileName);
                    MessageBox.Show("Board saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save board: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Handles the Load Board button click to load the board from a JSON file.
        /// </summary>
        private void LoadBoard_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var json = File.ReadAllText(dialog.FileName);
                    var vm = DataContext as MainViewModel;
                    if (vm == null)
                        throw new InvalidOperationException("ViewModel is not available.");

                    vm.LoadBoardFromJson(json);
                    MessageBox.Show("Board loaded successfully.", "Load", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load board:\n{ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}