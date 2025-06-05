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
        }

        private void btnAdd_Card(object sender, RoutedEventArgs e)
        {
            CardDialog cardDialog = new CardDialog();
            cardDialog.ShowDialog();
        }
    }
}