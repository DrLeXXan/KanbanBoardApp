using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KanbanBoardApp.Helpers
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> value to a <see cref="Brush"/> for UI display.
    /// Returns <see cref="TodayBrush"/> if the date is today or in the past, otherwise <see cref="DefaultBrush"/>.
    /// </summary>
    public class DateToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the brush used for dates that are not today or in the past.
        /// </summary>
        public Brush DefaultBrush { get; set; } = Brushes.Black;

        /// <summary>
        /// Gets or sets the brush used for dates that are today or in the past.
        /// </summary>
        public Brush TodayBrush { get; set; } = Brushes.Red;

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to a <see cref="Brush"/>.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// <see cref="TodayBrush"/> if the date is today or in the past; otherwise, <see cref="DefaultBrush"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                if (date.Date <= DateTime.Today)
                    return TodayBrush;
            }
            return DefaultBrush;
        }

        /// <summary>
        /// Not implemented. Converts a value back to its source type.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Throws <see cref="NotImplementedException"/> in all cases.</returns>
        /// <exception cref="NotImplementedException">Always thrown.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

