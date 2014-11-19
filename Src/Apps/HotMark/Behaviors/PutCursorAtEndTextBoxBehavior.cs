using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace MMK.HotMark.Behaviors
{
    class PutCursorAtEndTextBoxBehavior : Behavior<UIElement>
    {
        private TextBox textBox;

        protected override void OnAttached()
        {
            base.OnAttached();

            textBox = AssociatedObject as TextBox;

            if (textBox == null) return;

            textBox.GotFocus += TextBoxGotFocus;
        }

        protected override void OnDetaching()
        {
            if (textBox == null) return;
            textBox.GotFocus -= TextBoxGotFocus;

            base.OnDetaching();
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            textBox.CaretIndex = textBox.Text.Length;
        }
    }
}
