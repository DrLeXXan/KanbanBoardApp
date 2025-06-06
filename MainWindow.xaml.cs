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

        private void btnAdd_Card(object sender, RoutedEventArgs e)
        {
            // Assume you have a way to get the selected column, e.g., the first column for demo
            var vm = DataContext as MainViewModel;
            var column = vm?.Columns.FirstOrDefault();
            if (column == null) return;

            var dialog = new CardDialog();
            if (dialog.ShowDialog() == true)
            {
                column.Cards.Add(dialog.GetCard());
            }
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
}