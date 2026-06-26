using System;
using System.Collections.Generic;
using System.IO;

namespace UnityMCP.Editor.Welcome
{
    /// <summary>
    /// Install-state detection for a cross-sell companion: in-project (assembly
    /// loaded), owned (a matching .unitypackage sits in the Asset Store download
    /// cache), or not-owned.
    /// </summary>
    internal static class UnityMcpWelcomeDetection
    {
        public enum State { NotOwned, Owned, InProject }

        public static State Resolve(string assemblyPrefix, string cacheHintsCsv)
        {
            if (HasAssembly(assemblyPrefix)) return State.InProject;
            if (FindCachedPackage(SplitHints(cacheHintsCsv)) != null) return State.Owned;
            return State.NotOwned;
        }

        public static string ResolveCachedPackagePath(string cacheHintsCsv)
        {
            return FindCachedPackage(SplitHints(cacheHintsCsv));
        }

        public static bool HasAssembly(string namePrefix)
        {
            if (string.IsNullOrEmpty(namePrefix)) return false;
            foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                string name = a.GetName().Name;
                if (!string.IsNullOrEmpty(name) && name.StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static string FindCachedPackage(string[] hints)
        {
            if (hints == null || hints.Length == 0) return null;
            foreach (string root in CacheRoots())
            {
                if (!Directory.Exists(root)) continue;
                string[] files;
                try { files = Directory.GetFiles(root, "*.unitypackage", SearchOption.AllDirectories); }
                catch { continue; }
                foreach (string f in files)
                {
                    string fileName = Path.GetFileName(f);
                    foreach (string hint in hints)
                        if (!string.IsNullOrEmpty(hint) && fileName.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0)
                            return f;
                }
            }
            return null;
        }

        private static string[] SplitHints(string csv)
        {
            if (string.IsNullOrEmpty(csv)) return Array.Empty<string>();
            string[] parts = csv.Split(',');
            for (int i = 0; i < parts.Length; i++) parts[i] = parts[i].Trim();
            return parts;
        }

        private static IEnumerable<string> CacheRoots()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!string.IsNullOrEmpty(appData))
                yield return Path.Combine(appData, "Unity", "Asset Store-5.x");

            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrEmpty(home))
            {
                yield return Path.Combine(home, "Library", "Unity", "Asset Store-5.x");
                yield return Path.Combine(home, ".local", "share", "unity3d", "Asset Store-5.x");
            }
        }
    }
}
