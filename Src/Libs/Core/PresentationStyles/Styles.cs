using System;
using System.Windows;
using System.Windows.Markup;

namespace MMK.PresentationStyles
{
    public sealed class Styles : MarkupExtension
    {
        private static readonly ResourceDictionary ResourceDictionary;

        static Styles()
        {
            ResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MMK.PresentationStyles;component/Styles.xaml")
            };
        }

        public Styles(string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public Styles()
        {

        }

        public string ResourceKey { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ResourceDictionary[ResourceKey];
        }

        public static TResource Get<TResource>(string resourceKey)
        {
            return (TResource)ResourceDictionary[resourceKey];            
        }
    }
}