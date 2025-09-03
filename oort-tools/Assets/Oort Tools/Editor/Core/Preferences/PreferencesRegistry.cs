#if UNITY_EDITOR
using System.Collections.Generic;

namespace OortTools.Core.Preferences
{
    public static class PreferencesRegistry 
    {
        static readonly List<IPreferencesSection> _sections = new();
        public static void Register(IPreferencesSection section)
        {
            if (section != null && !_sections.Contains(section)) _sections.Add(section);
        }
        public static IReadOnlyList<IPreferencesSection> Sections => _sections;
    }
}
#endif