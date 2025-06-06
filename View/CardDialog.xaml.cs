using KanbanBoardApp.Models;
using System.Windows;

namespace KanbanBoardApp.View
{
    public partial class CardDialog : Window
    {
        public KanbanCard Card { get; }

        public CardDialog(KanbanCard? card = null)
        {
            InitializeComponent();
            Card = card ?? new KanbanCard();
            DataContext = Card;
        }

        public KanbanCard GetCard() => Card;

        private void btnClose_window(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
