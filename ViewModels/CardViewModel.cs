using KanbanBoardApp.Models;
using System.Collections.Generic;

namespace KanbanBoardApp.ViewModels
{
    public class CardDialogViewModel
    {
        public KanbanCard Card { get; }
        public List<string> UrgencyCollection { get; }

        public CardDialogViewModel(KanbanCard? card = null)
        {
            Card = card ?? new KanbanCard();
            UrgencyCollection = new List<string> { "Low", "Medium", "High", "Urgent" };
        }
    }
}
