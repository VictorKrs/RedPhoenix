using System.Collections.ObjectModel;

namespace RedPhoenix.Common.Handlers.Localization.Models
{
    /// <summary>
    /// Model for UI (settings)
    /// </summary>
    public class LocalizationSettings : LocalizationInfoBase
    {
        #region Private fields
        private ObservableCollection<string> _availableLanguages = new ObservableCollection<string>(PhoenixTranslator.AvailableLanguages);
        private string _languageDir = PhoenixTranslator.LanguageDir;
        #endregion

        #region Properties
        public override ObservableCollection<string> AvailableLanguages { get { return _availableLanguages; } }
        public string LanguageDir
        {
            get { return _languageDir; }
            set { SetProperty(ref _languageDir, value, "LanguageDir"); }
        }
        #endregion
    }
}
