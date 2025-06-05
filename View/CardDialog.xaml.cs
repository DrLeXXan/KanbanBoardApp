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
    /// <summary>
    /// Interaction logic for CardDialog.xaml
    /// </summary>
    /// fff
    public partial class CardDialog : Window
    {
        public CardDialog()
        {
            InitializeComponent();
        }

        private void btnClose_window(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
