using KanbanBoardApp.Models;
using KanbanBoardApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
using System.Xml.XPath;

namespace KanbanBoardApp.View
{
    public partial class CardDialog : Window
    {
        public CardDialogViewModel ViewModel { get; }
        public bool IsDeleteRequested { get; private set; }

        public CardDialog(KanbanCard? card = null, List<KanbanColumn>? columns = null, KanbanColumn? defaultColumn = null)
        {
            InitializeComponent();
            ViewModel = new CardDialogViewModel(card, columns, defaultColumn);
            DataContext = ViewModel;
        }

        public KanbanCard GetCard() => ViewModel.Card;

            private void btnClose_window(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close(); 
        }

        private void btnSave_Card(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ViewModel.Card.Title))
            {
                MessageBox.Show("Card Title cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                if (ViewModel.SelectedColumn != null)
                {
                    ViewModel.Card.Status = ViewModel.SelectedColumn.Title;
                }

                if (ViewModel.IsExistingCard == false)
                {
                    KanbanCard.GetNextId();
                }
                DialogResult = true;
                Close();
            }
                
        }

        private void btnDelete_Card(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete the card: {ViewModel.Card.Title}? This action cannot be undone!", 
                "Confirm Delete", 
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                IsDeleteRequested = true;
                DialogResult = true;
                Close();

            }
        }


    }
}
