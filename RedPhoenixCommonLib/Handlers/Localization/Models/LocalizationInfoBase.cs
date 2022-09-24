using RedPhoenix.Common.Models;
using System.Collections.ObjectModel;

namespace RedPhoenix.Common.Handlers.Localization.Models
{
    public abstract class LocalizationInfoBase : NotifyPropertyChangedBase
    {
        #region Private fields
        private string _selectedLanguage = null;
        #endregion

        #region Properties
        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                SetProperty(ref _selectedLanguage, value, "SelectedLanguage");
                OnPropertyChanged("SelectedIndex");
            }
        }

        public int SelectedIndex
        {
            get { return AvailableLanguages.IndexOf(SelectedLanguage); }
        }

        public abstract ObservableCollection<string> AvailableLanguages { get; }
        #endregion

        #region Constructor
        public LocalizationInfoBase()
        {
            _selectedLanguage = PhoenixTranslator.GlobalLocalizaitionInfo.SelectedLanguage;
        }
        #endregion

        #region Methods
        public virtual void Clear()
        {
            SelectedLanguage = null;
        }
        #endregion
    }
}
