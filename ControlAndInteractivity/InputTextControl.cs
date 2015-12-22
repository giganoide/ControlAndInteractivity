using System.Windows;
using System.Windows.Controls;

namespace ControlAndInteractivity
{
    public class InputTextControl : Control
    {
        #region Label Dependency Property
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof (string), typeof (InputTextControl), new PropertyMetadata(default(string)));

        public string Label { get { return (string)GetValue(LabelProperty); } set { SetValue(LabelProperty, value); } }
        #endregion Label Dependency Property

        #region EditableText Dependency Property
        public static readonly DependencyProperty EditableTextProperty = DependencyProperty.Register(
            "EditableText", typeof (string), typeof (InputTextControl), new PropertyMetadata(default(string)));

        public string EditableText { get { return (string)GetValue(EditableTextProperty); } set { SetValue(EditableTextProperty, value); } }
        #endregion Label Dependency Property

        public void Clear()
        {
            EditableText = null;
        }
    }
}