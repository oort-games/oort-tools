#if UNITY_EDITOR
using System.Collections.Generic;

namespace OortTools.Core.Preferences
{
    public interface IPreferencesSection
    {
        string Title { get; }
        IEnumerable<string> Keywords { get; }
        void OnGUI();
    }
}
#endif