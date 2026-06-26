using UnityEditor;
using UnityEngine.UIElements;

namespace UnityMCP.Editor.Welcome
{
    /// <summary>
    /// Applies the warm-brown + molten-orange editor theme to the window root.
    /// Call once from <c>CreateGUI</c>. Idempotent. The style sheet ships next to
    /// this window and is resolved by asset name (no hardcoded path).
    /// </summary>
    internal static class UnityMcpWelcomeTheme
    {
        public const string CLASS_WINDOW = "ab-window";

        private const string USS_FILTER = "UnityMcpWelcomeTheme t:StyleSheet";

        private static StyleSheet s_sheet;

        public static void Apply(VisualElement root)
        {
            if (root == null) return;

            if (!root.ClassListContains(CLASS_WINDOW))
                root.AddToClassList(CLASS_WINDOW);

            StyleSheet sheet = LoadSheet();
            if (sheet != null && !root.styleSheets.Contains(sheet))
                root.styleSheets.Add(sheet);
        }

        private static StyleSheet LoadSheet()
        {
            if (s_sheet != null) return s_sheet;

            string[] guids = AssetDatabase.FindAssets(USS_FILTER);
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                s_sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            }
            return s_sheet;
        }
    }
}
