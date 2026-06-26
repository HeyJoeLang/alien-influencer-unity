using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityMCP.Editor.Welcome
{
    /// <summary>
    /// Built-in editor icons, tinted to the warm palette. Built-in symbols
    /// survive Unity 2022.3 (no emoji font fallback); bundled PNGs (ab-*) are
    /// shipped next to this window under Icons/ and resolved by asset name.
    /// </summary>
    internal static class UnityMcpWelcomeIcon
    {
        public const string STAR = "Favorite";
        public const string STORE = "Asset Store";
        public const string INSTALLED = "Installed";
        public const string DOWNLOAD = "Download-Available";
        public const string WEB = "BuildSettings.Web.Small";

        public const string LIGHTNING = "ab-lightning";
        public const string HEART = "ab-heart";

        public const string CUSTOM_PREFIX = "guid:";

        public static Image Make(string iconName, int size, float marginRight = 6f)
        {
            Texture tex = Load(iconName);
            if (tex == null) return null;

            Image image = new Image { image = tex, scaleMode = ScaleMode.ScaleToFit, tintColor = Tint(iconName) };
            image.style.width = size;
            image.style.height = size;
            image.style.flexShrink = 0;
            image.style.marginRight = marginRight;
            return image;
        }

        private static Texture Load(string iconName)
        {
            if (string.IsNullOrEmpty(iconName)) return null;

            if (iconName.StartsWith(CUSTOM_PREFIX, System.StringComparison.Ordinal))
            {
                string path = AssetDatabase.GUIDToAssetPath(iconName.Substring(CUSTOM_PREFIX.Length));
                return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }

            if (iconName.StartsWith("ab-", System.StringComparison.Ordinal))
            {
                foreach (string guid in AssetDatabase.FindAssets(iconName + " t:Texture2D"))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (System.IO.Path.GetFileNameWithoutExtension(path) == iconName)
                        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
                return null;
            }

            try { GUIContent c = EditorGUIUtility.IconContent(iconName); return c != null ? c.image : null; }
            catch { return null; }
        }

        private static Color Tint(string iconName)
        {
            switch (iconName)
            {
                case STAR:      return new Color(0.96f, 0.63f, 0.28f);
                case STORE:     return new Color(0.85f, 0.48f, 0.16f);
                case INSTALLED: return new Color(0.55f, 0.85f, 0.45f);
                case DOWNLOAD:  return new Color(0.95f, 0.75f, 0.35f);
                case WEB:       return new Color(0.85f, 0.48f, 0.16f);
                case LIGHTNING: return new Color(0.96f, 0.78f, 0.30f);
                case HEART:     return new Color(0.95f, 0.40f, 0.45f);
                default:        return Color.white;
            }
        }
    }
}
