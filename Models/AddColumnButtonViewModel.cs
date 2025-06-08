using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanBoardApp.Models
{
    public class AddColumnButtonViewModel
    {
        public bool IsAddButton => true;
        public int InsertIndex { get; set; }
        public bool IsVisible { get; set; } = false; // For hover effects, if you want
    }
}
