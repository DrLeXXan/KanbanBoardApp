using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KanbanBoardApp.Helpers
{
    /// <summary>
    /// Converts a boolean value to <see cref="Visibility"/>, returning <see cref="Visibility.Collapsed"/> for true and <see cref="Visibility.Visible"/> for false.
    /// The conversion is the inverse of the standard BooleanToVisibilityConverter.
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The boolean value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// <see cref="Visibility.Collapsed"/> if the value is true; otherwise, <see cref="Visibility.Visible"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? Visibility.Collapsed : Visibility.Visible;
            return Visibility.Visible;
        }

        /// <summary>
        /// Converts a <see cref="Visibility"/> value back to a boolean.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// false if the value is <see cref="Visibility.Visible"/>; otherwise, true.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
                return v != Visibility.Visible;
            return false;
        }
    }
}
