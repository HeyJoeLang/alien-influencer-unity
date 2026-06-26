using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityMCP.Editor.Welcome
{
    /// <summary>
    /// Shared building blocks (boxes, headings, buttons) and button actions
    /// (open URL / EditorWindow / examples / docs / import) reused by both tabs.
    /// </summary>
    public sealed partial class UnityMcpWelcomeWindow
    {
        // --- Building blocks --------------------------------------------------

        private static void AddHeading(VisualElement parent, string text)
        {
            Label h = new Label(text);
            h.AddToClassList("ab-section-heading");
            parent.Add(h);
        }

        private static void AddBody(VisualElement parent, string text)
        {
            Label b = new Label(text) { enableRichText = true };
            b.style.whiteSpace = WhiteSpace.Normal;
            b.style.color = new Color(0.91f, 0.88f, 0.85f);
            b.style.marginBottom = 6;
            parent.Add(b);
        }

        private static VisualElement AddBox(VisualElement parent, string heading, string iconName)
        {
            VisualElement box = new VisualElement();
            box.AddToClassList("ab-box");
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            Image icon = string.IsNullOrEmpty(iconName) ? null : UnityMcpWelcomeIcon.Make(iconName, 16);
            if (icon != null) row.Add(icon);
            Label h = new Label(heading);
            h.AddToClassList("ab-section-heading");
            row.Add(h);
            box.Add(row);
            parent.Add(box);
            return box;
        }

        private static void AddBoxBody(VisualElement box, string text)
        {
            Label b = new Label(text) { enableRichText = true };
            b.style.whiteSpace = WhiteSpace.Normal;
            b.style.color = new Color(0.91f, 0.88f, 0.85f);
            b.style.marginBottom = 6;
            box.Add(b);
        }

        private static Button MakeAccentButton(string label, string iconName, Action onClick = null)
        {
            Button btn = new Button { focusable = false };
            btn.AddToClassList("ab-accent-button");
            btn.style.alignSelf = Align.Center;
            btn.style.minWidth = 200;
            btn.style.flexDirection = FlexDirection.Row;
            btn.style.alignItems = Align.Center;
            btn.style.justifyContent = Justify.Center;
            Image icon = string.IsNullOrEmpty(iconName) ? null : UnityMcpWelcomeIcon.Make(iconName, 16);
            if (icon != null) btn.Add(icon);
            btn.Add(new Label(label));
            if (onClick != null) btn.clicked += onClick;
            return btn;
        }

        private static Button MakeLinkButton(string label, string url)
        {
            Button btn = new Button { focusable = false };
            btn.style.flexGrow = 1;
            btn.style.flexBasis = 0;
            btn.style.marginLeft = 2;
            btn.style.marginRight = 2;
            btn.style.flexDirection = FlexDirection.Row;
            btn.style.alignItems = Align.Center;
            btn.style.justifyContent = Justify.Center;
            Image icon = UnityMcpWelcomeIcon.Make(UnityMcpWelcomeIcon.WEB, 13);
            if (icon != null) btn.Add(icon);
            btn.Add(new Label(label));
            btn.clicked += () => OpenUrl(url);
            return btn;
        }

        // --- Actions ----------------------------------------------------------

        private static void OpenUrl(string url)
        {
            if (string.IsNullOrEmpty(url) || url.StartsWith("TBD-URL-", StringComparison.Ordinal))
            {
                EditorUtility.DisplayDialog("Link not set", "This link has not been configured yet.", "OK");
                return;
            }
            Application.OpenURL(url);
        }

        private void OpenExamples()
        {
            if (!string.IsNullOrEmpty(_config.ExamplesWindowType)) { OpenWindowType(_config.ExamplesWindowType); return; }
            if (!string.IsNullOrEmpty(_config.ExamplesFolder))
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_config.ExamplesFolder);
                if (obj != null) { Selection.activeObject = obj; EditorGUIUtility.PingObject(obj); }
                else EditorUtility.DisplayDialog("Examples", "Examples folder not found:\n" + _config.ExamplesFolder, "OK");
            }
        }

        private void OpenDocumentation()
        {
            if (!string.IsNullOrEmpty(_config.DocumentationWindowType)) { OpenWindowType(_config.DocumentationWindowType); return; }
            OpenFirstPdf(_config.DocumentationPath);
        }

        private static void OpenFirstPdf(string docFolder)
        {
            foreach (string guid in AssetDatabase.FindAssets("t:DefaultAsset"))
            {
                string p = AssetDatabase.GUIDToAssetPath(guid);
                if (!p.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.IsNullOrEmpty(docFolder) && p.IndexOf(docFolder, StringComparison.OrdinalIgnoreCase) < 0) continue;
                string abs = System.IO.Path.GetFullPath(p);
                if (System.IO.File.Exists(abs))
                {
                    try { System.Diagnostics.Process.Start(abs); }
                    catch { Application.OpenURL("file://" + abs); }
                    return;
                }
            }
            EditorUtility.DisplayDialog("Documentation", "No PDF found under: " + docFolder, "OK");
        }

        private static void OpenWindowType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return;
            Type t = Type.GetType(typeName);
            if (t == null)
            {
                foreach (System.Reflection.Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = asm.GetType(typeName);
                    if (t != null) break;
                }
            }
            if (t != null && typeof(UnityEditor.EditorWindow).IsAssignableFrom(t)) GetWindow(t);
            else Debug.LogWarning("[Welcome] EditorWindow type not found: " + typeName);
        }

        private static void ImportOwned(string cacheHints)
        {
            string pkg = UnityMcpWelcomeDetection.ResolveCachedPackagePath(cacheHints);
            if (!string.IsNullOrEmpty(pkg)) AssetDatabase.ImportPackage(pkg, true);
        }
    }
}
