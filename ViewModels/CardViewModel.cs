using KanbanBoardApp.Models;
using System.Collections.Generic;

namespace KanbanBoardApp.ViewModels
{
    public class CardDialogViewModel
    {
        public KanbanCard Card { get; }
        public List<string> UrgencyCollection { get; }
        public List<KanbanColumn> Columns { get; } // Add this property
        public KanbanColumn? SelectedColumn { get; set; } // For binding

        public bool IsExistingCard { get; }
        public int NextId { get; }

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
            // Set SelectedColumn to defaultColumn if provided, otherwise use Card.Status
            if (defaultColumn != null)
                SelectedColumn = Columns.FirstOrDefault(c => c.Title == defaultColumn.Title);
            else
                SelectedColumn = Columns.FirstOrDefault(c => c.Title == Card.Status);
        }
    }
}
