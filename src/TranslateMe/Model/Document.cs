using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace TranslateMe.Model
{
    public class Document
    {
        public Document(string directory, string name)
        {
            Directory = directory;
            Name = name;
            Cultures = new ObservableCollection<CultureInfo>();
            Cultures.CollectionChanged += CulturesOnCollectionChanged;
            Phrases = new ObservableCollection<Phrase>();
            Phrases.CollectionChanged += PhrasesOnCollectionChanged;
        }

        public string Directory { get; private set; }
        public string Name { get; private set; }
        public ObservableCollection<CultureInfo> Cultures { get; private set; }
        public ObservableCollection<Phrase> Phrases { get; private set; }

        public string this[string name, CultureInfo culture]
        {
            get
            {
                var texts = from phrase in Phrases
                            where phrase.Name == name
                            from translation in phrase.Translations
                            where translation.Culture.Equals(culture)
                            select translation.Text;

                return texts.FirstOrDefault();
            }
            set
            {
                if (!Cultures.Any(c => c.Equals(culture)))
                {
                    Cultures.Add(culture);
                }

                var phrase = Phrases.FirstOrDefault(p => p.Name == name);
                if (phrase == null)
                {
                    phrase = new Phrase
                    {
                        Name = name,
                    };

                    Phrases.Add(phrase);
                }

                phrase.Translations[culture].Text = value;
            }
        }

        private void CulturesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.NewItems != null)
            {
                foreach (CultureInfo culture in eventArgs.NewItems)
                {
                    foreach (var phrase in Phrases)
                    {
                        phrase.Translations.Add(new Translation(culture));
                    }
                }
            }
        }

        private void PhrasesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.NewItems != null)
            {
                foreach (Phrase phrase in eventArgs.NewItems)
                {
                    foreach (var culture in Cultures)
                    {
                        phrase.Translations.Add(new Translation(culture));
                    }
                }
            }
        }
    }
}