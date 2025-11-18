#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;

namespace OortTools.Core.Preferences
{
    internal class PreferencesProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new PreferencesProviderView();
        }
    }
}
#endif
