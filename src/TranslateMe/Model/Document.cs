using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Security.Cryptography.X509Certificates;

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
        public bool LockWriteOperation = false;

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

        public void UpdatePhrases(Document documentBackup, string searchTerm)
        {
            //reset phrases to backup pharses
            Phrases.Clear();

            documentBackup.LockWriteOperation = true;

            foreach (var phrase in documentBackup.Phrases)
            {
                Phrases.Add(phrase);
            }

            for(var i=0 ; ; i++)
            {
                if ((!Phrases.Any()) || i == Phrases.Count())
                    break;
               
                var phrase = Phrases.ElementAt(i);
                var name = phrase.Name;
                var transAry = phrase.Translations;
                if (name.ToLower().Contains(searchTerm.ToLower()) ||
                    transAry.Any(t => t.Text!=null && t.Text.ToLower().Contains(searchTerm.ToLower())))
                    continue;

                Phrases.RemoveAt(i);
                i--;
            }

            documentBackup.LockWriteOperation = false;
        }

        public void RestorePrases(Document sourceDocument)
        {
            Phrases.Clear();

            sourceDocument.LockWriteOperation = true;

            foreach (var phrase in sourceDocument.Phrases)
            {
                Phrases.Add(phrase);
            }

            sourceDocument.LockWriteOperation = false;
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
                    if (Cultures.Count != phrase.Translations.Count)
                        foreach (var culture in Cultures)
                        {
                            phrase.Translations.Add(new Translation(culture));
                        }
                }
            }
        }
    }
}