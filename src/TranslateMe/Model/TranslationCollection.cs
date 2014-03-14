using System.Globalization;
using TranslateMe.Model.Support;

namespace TranslateMe.Model
{
    public class TranslationCollection : IndexedCollection<Translation, CultureInfo>
    {
        public TranslationCollection() :
            base(translation => translation.Culture)
        {
        }
    }
}