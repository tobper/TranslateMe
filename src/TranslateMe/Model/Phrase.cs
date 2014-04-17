using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using TranslateMe.Filtering;

namespace TranslateMe.Model
{
    [DebuggerDisplay("{Name,nq}")]
    public class Phrase : DependencyObject, IFilterValueProvider
    {
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
            "Name",
            typeof (string),
            typeof (Phrase),
            new PropertyMetadata(null));

        public TranslationCollection Translations { get; private set; }

        public Phrase()
        {
            Name = string.Empty;
            Translations = new TranslationCollection();
        }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public IEnumerable<string> GetFilterValues()
        {
            yield return Name;

            foreach (var translation in Translations)
            {
                yield return translation.Text;
            }
        }
    }
}