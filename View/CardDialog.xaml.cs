using KanbanBoardApp.Models;
using KanbanBoardApp.ViewModels;
using System.Windows;


namespace KanbanBoardApp.View
{
    /// <summary>
    /// Interaction logic for the CardDialog window.
    /// Provides UI for creating or editing a Kanban card.
    /// </summary>
    public partial class CardDialog : Window
    {
        /// <summary>
        /// Gets the view model for the dialog.
        /// </summary>
        public CardDialogViewModel ViewModel { get; }

        /// <summary>
        /// Gets a value indicating whether the user requested to delete the card.
        /// </summary>
        public bool IsDeleteRequested { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardDialog"/> class.
        /// </summary>
        /// <param name="card">The card to edit, or null to create a new card.</param>
        /// <param name="columns">The list of available columns.</param>
        /// <param name="defaultColumn">The default column for the card.</param>
        public CardDialog(KanbanCard? card = null, List<KanbanColumn>? columns = null, KanbanColumn? defaultColumn = null)
        {
            InitializeComponent();
            ViewModel = new CardDialogViewModel(card, columns, defaultColumn);
            DataContext = ViewModel;

            if (ViewModel.IsExistingCard == false)
            {
                Loaded += (s, e) => TitleTextBox.Focus();
            }
            
        }

        /// <summary>
        /// Gets the card being created or edited in the dialog.
        /// </summary>
        /// <returns>The <see cref="KanbanCard"/> instance.</returns>
        public KanbanCard GetCard() => ViewModel.Card;

        /// <summary>
        /// Handles the close button click event. Closes the dialog without saving.
        /// </summary>
        private void btnClose_window(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close(); 
        }

        /// <summary>
        /// Handles the save button click event. Validates and saves the card.
        /// </summary>
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

        /// <summary>
        /// Handles the delete button click event. Prompts for confirmation and marks the card for deletion.
        /// </summary>
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
