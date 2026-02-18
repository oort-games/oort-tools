#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;

namespace OortTools
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
