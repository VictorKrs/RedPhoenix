using System.Collections.ObjectModel;

namespace RedPhoenix.Common.Handlers.Localization.Models
{
    public class LocalizationInfo : LocalizationInfoBase
    {
        #region Properties
        public override ObservableCollection<string> AvailableLanguages { get { return PhoenixTranslator.AvailableLanguages; } }
        #endregion
    }
}
