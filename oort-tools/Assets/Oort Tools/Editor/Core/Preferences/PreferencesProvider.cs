#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

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
