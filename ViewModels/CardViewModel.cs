using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanBoardApp.ViewModels
{
    public class CardViewModel
    {
        public List<string> UrgencyCollection { get; set; }   
        
        public CardViewModel()
        {
            UrgencyCollection = new List<string>()
            {
                "Low",
                "Medium",
                "High",
                "Urgent"
            };
        }
    }
}
