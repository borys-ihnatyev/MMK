using System.Windows;
using System.Windows.Controls;

namespace MMK.Notify.ViewModel.TrayMenu
{
    public class MenuItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ToggleMenuItemDataTemplate { get; set; }
        public DataTemplate MenuItemDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ToggleMenuItemViewModel)
            {
                return ToggleMenuItemDataTemplate;
            }

            if (item is MenuItemViewModel)
            {
                return MenuItemDataTemplate;
            }

            return base.SelectTemplate(item, container);
        }

    }
}
