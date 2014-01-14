using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace TranslateMe.Model
{
    public class Phrase : DependencyObject
    {
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
            "Name",
            typeof (string),
            typeof (Phrase),
            new PropertyMetadata(null));

        public ObservableCollection<Translation> Translations { get; private set; }

        public Phrase(ObservableCollection<CultureInfo> cultures) :
            this(cultures, string.Empty)
        {
        }

        public Phrase(ObservableCollection<CultureInfo> cultures, string name)
        {
            Name = name;
            Translations = new ObservableCollection<Translation>(cultures.Select(c => new Translation(c)));

            cultures.CollectionChanged += CulturesOnCollectionChanged;
        }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public Translation this[CultureInfo culture]
        {
            get { return GetTranslation(culture); }
        }

        private Translation GetTranslation(CultureInfo culture)
        {
            return Translations.First(t => t.Culture.Equals(culture));
        }

        private void CulturesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.OldItems != null)
            {
                foreach (CultureInfo culture in notifyCollectionChangedEventArgs.OldItems)
                {
                    Translations.Remove(item => item.Culture.Equals(culture));
                }
            }

            foreach (CultureInfo culture in notifyCollectionChangedEventArgs.NewItems)
            {
                Translations.Add(new Translation(culture));
            }
        }
    }
}