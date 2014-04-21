using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace TranslateMe.UI.Controls
{
    public partial class ExportControl
    {
        class ExportViewModel : IDisposable
        {
            public bool ScopeSelection { get; set; }
            public bool ScopeAll { get; set; }
            public ObservableCollection<LanguageViewModel> Languages { get; private set; }

            public ExportViewModel(bool hasSelection, ObservableCollection<CultureInfo> cultures)
            {
                ScopeSelection = hasSelection;
                ScopeAll = !hasSelection;
                Languages = cultures.ToDependantCollection(
                    (culture, language) => language.Culture.Equals(culture),
                    culture => new LanguageViewModel(culture));
            }

            public void Dispose()
            {
                var disposableLanguages = (IDisposable)Languages;
                disposableLanguages.Dispose();
            }
        }

        class LanguageViewModel
        {
            public CultureInfo Culture { get; private set; }
            public string Name { get; private set; }
            public bool IsSelected { get; set; }

            public LanguageViewModel(CultureInfo culture)
            {
                Culture = culture;
                Name = culture.GetName();
            }
        }
    }
}