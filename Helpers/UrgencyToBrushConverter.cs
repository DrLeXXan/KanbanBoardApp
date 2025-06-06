using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KanbanBoardApp.Helpers
{
    public class UrgencyToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string urgency = value as string ?? "";
            return urgency switch
            {
                "Low" => Brushes.Green,
                "Medium" => Brushes.Goldenrod,
                "High" => Brushes.OrangeRed,
                "Urgent" => Brushes.Red,
                _ => Brushes.Transparent
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

