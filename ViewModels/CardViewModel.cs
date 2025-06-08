using KanbanBoardApp.Models;
using System.Collections.Generic;

namespace KanbanBoardApp.ViewModels
{
    public class CardDialogViewModel
    {
        public KanbanCard Card { get; }
        public List<string> UrgencyCollection { get; }
        public bool IsExistingCard { get; }
        public int NextId { get; }

        public CardDialogViewModel(KanbanCard? card = null)
        {
            if (card == null)
            {
                //int nextId = KanbanCard.GetNextId();
                int nextId = KanbanCard.PeekNextId();
                Card = new KanbanCard() { Id = nextId, Urgency = "Medium" };
                NextId = IsExistingCard ? Card.Id : KanbanCard.PeekNextId();
                IsExistingCard = false;
            }
            else
            {
                Card = card;
                NextId = card.Id;
                IsExistingCard = true;
            }
            UrgencyCollection = new List<string> { "Low", "Medium", "High", "Urgent" };
        }
    }
}
