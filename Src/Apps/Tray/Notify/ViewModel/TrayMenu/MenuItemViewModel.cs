using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MMK.Wpf;
using MMK.Wpf.ViewModel;

namespace MMK.Notify.ViewModel.TrayMenu
{
    public class MenuItemViewModel : ObservableObject
    {
        private ICommand command;
        private Action commandAction;
        private BitmapImage image;
        private string title;

        public MenuItemViewModel()
        {
        }

        public MenuItemViewModel(MenuItemViewModel menuItem)
        {
            title = menuItem.title;
            image = menuItem.image;
            command = menuItem.command;
        }

        public MenuItemViewModel(string title, Action commandAction, string imageUri = null)
        {
            Title = title;
            CommandAction += commandAction;
            if (!string.IsNullOrEmpty(imageUri))
            {
                Image = new BitmapImage(new Uri(imageUri));
            }
        }


        public string Title
        {
            get { return title; }
            set
            {
                if (value == title) return;
                title = value;
                NotifyPropertyChanged();
            }
        }

        public BitmapImage Image
        {
            get { return image; }
            set
            {
                if (value == image) return;
                image = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand Command
        {
            get { return command; }
            set
            {
                if (value == command) return;
                command = value;
                NotifyPropertyChanged();
            }
        }

        public event Action CommandAction
        {
            add
            {
                commandAction += value;
                Command = new Command(commandAction);
            }
            remove
            {
                commandAction -= value;
                Command = new Command(commandAction);
            }
        }
    }
}