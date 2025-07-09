using KanbanBoardApp.Models;

namespace KanbanBoardApp.ViewModels
{
    /// <summary>
    /// ViewModel for the CardDialog, providing data and logic for creating or editing a Kanban card.
    /// </summary>
    public class CardDialogViewModel
    {
        /// <summary>
        /// Gets the Kanban card being created or edited.
        /// </summary>
        public KanbanCard Card { get; }

        /// <summary>
        /// Gets the collection of urgency levels available for selection.
        /// </summary>
        public List<string> UrgencyCollection { get; }

        /// <summary>
        /// Gets the list of available columns for the card.
        /// </summary>
        public List<KanbanColumn> Columns { get; }

        /// <summary>
        /// Gets or sets the currently selected column for the card.
        /// </summary>
        public KanbanColumn? SelectedColumn { get; set; }

        /// <summary>
        /// Gets a value indicating whether the card is an existing card (true) or a new card (false).
        /// </summary>
        public bool IsExistingCard { get; }

        /// <summary>
        /// Gets the next available card ID (for new cards) or the current card's ID (for existing cards).
        /// </summary>
        public int NextId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CardDialogViewModel"/> class.
        /// </summary>
        /// <param name="card">The card to edit, or null to create a new card.</param>
        /// <param name="columns">The list of available columns.</param>
        /// <param name="defaultColumn">The default column for the card.</param>
        public CardDialogViewModel(KanbanCard? card = null, List<KanbanColumn>? columns = null, KanbanColumn? defaultColumn = null)
        {
            if (card == null)
            {
                int nextId = KanbanCard.PeekNextId();
                Card = new KanbanCard() { Id = nextId, Urgency = "Medium" };
                NextId = nextId;
                IsExistingCard = false;
            }
            else
            {
                Card = card;
                NextId = card.Id;
                IsExistingCard = true;
            }
            UrgencyCollection = new List<string> { "Low", "Medium", "High", "Urgent" };
            Columns = columns ?? new List<KanbanColumn>();

            if (defaultColumn != null)
                SelectedColumn = Columns.FirstOrDefault(c => c.Title == defaultColumn.Title);
            else
                SelectedColumn = Columns.FirstOrDefault(c => c.Title == Card.Status);
        }
    }
}
