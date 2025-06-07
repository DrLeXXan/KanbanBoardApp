using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KanbanBoardApp.Helpers
{
    public class DateToBrushConverter : IValueConverter
    {
        public Brush DefaultBrush { get; set; } = Brushes.Black;
        public Brush TodayBrush { get; set; } = Brushes.Red;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                if (date.Date <= DateTime.Today)
                    return TodayBrush;
            }
            return DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

