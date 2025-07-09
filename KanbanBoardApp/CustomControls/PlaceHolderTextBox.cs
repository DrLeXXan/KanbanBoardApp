using System.Windows;
using System.Windows.Controls;

namespace KanbanBoardApp.CustomControls
{
    /// <summary>
    /// A custom TextBox control that supports placeholder text and corner radius styling.
    /// </summary>
    public class PlaceHolderTextBox : TextBox
    {
        /// <summary>
        /// Gets or sets the placeholder text displayed when the TextBox is empty.
        /// </summary>
        public string PlaceHolder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PlaceHolder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(PlaceHolderTextBox), new PropertyMetadata(default));

        /// <summary>
        /// Gets or sets the corner radius of the TextBox border.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PlaceHolderTextBox), new PropertyMetadata(default));

        /// <summary>
        /// Initializes static members of the <see cref="PlaceHolderTextBox"/> class.
        /// Sets the default style key for the control.
        /// </summary>
        static PlaceHolderTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceHolderTextBox), new FrameworkPropertyMetadata(typeof(PlaceHolderTextBox)));
        }
    }
}
