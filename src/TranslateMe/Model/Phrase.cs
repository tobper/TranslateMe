using System.Diagnostics;
using System.Windows;

namespace TranslateMe.Model
{
    [DebuggerDisplay("{Name,nq}")]
    public class Phrase : DependencyObject
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
    }
}