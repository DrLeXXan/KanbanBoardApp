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
        }

        private void btnAdd_Card(object sender, RoutedEventArgs e)
        {
            CardDialog cardDialog = new CardDialog();
            cardDialog.ShowDialog();
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
    }
}