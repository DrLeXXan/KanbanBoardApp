using KanbanBoardApp.Models;
using KanbanBoardApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KanbanBoardApp.View
{
    public partial class CardDialog : Window
    {
        public CardDialogViewModel ViewModel { get; }

        public CardDialog(KanbanCard? card = null)
        {
            InitializeComponent();
            ViewModel = new CardDialogViewModel(card);
            DataContext = ViewModel;
        }

        public KanbanCard GetCard() => ViewModel.Card;

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
