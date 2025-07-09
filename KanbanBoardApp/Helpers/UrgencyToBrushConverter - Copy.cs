using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace KanbanBoardApp.Helpers
{
    /// <summary>
    /// Converts a string representing urgency ("Low", "Medium", "High", "Urgent") to a corresponding <see cref="Brush"/> for UI display.
    /// </summary>
    public class UrgencyToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts an urgency string to a <see cref="Brush"/>.
        /// </summary>
        /// <param name="value">The urgency value as a string.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A <see cref="Brush"/> corresponding to the urgency level:
        /// Green for "Low", Goldenrod for "Medium", OrangeRed for "High", Red for "Urgent", Transparent otherwise.
        /// </returns>
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

