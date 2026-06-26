using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityMCP.Editor.Welcome
{
    /// <summary>
    /// Pulls the latest post (title, summary, link, cover image) from the studio
    /// RSS feed so the "News &amp; Devlog" box shows live content. Cached for the
    /// session, fully async, null on failure.
    /// </summary>
    internal static class UnityMcpWelcomeDevlogFetcher
    {
        public const string FEED_URL = "https://anklebreaker-studio.com/devlog/feed.xml";

        internal sealed class Entry
        {
            public string Title;
            public string Summary;
            public string Url;
            public Texture2D Thumbnail;
        }

        private static Entry s_cached;
        private static bool s_inFlight;
        private static readonly List<Action<Entry>> s_waiters = new List<Action<Entry>>();

        public static void Fetch(Action<Entry> onReady)
        {
            if (onReady == null) return;
            if (s_cached != null) { onReady(s_cached); return; }

            s_waiters.Add(onReady);
            if (s_inFlight) return;
            s_inFlight = true;

            UnityWebRequest req = UnityWebRequest.Get(FEED_URL);
            req.SendWebRequest().completed += _ => OnFeedDone(req);
        }

        private static void OnFeedDone(UnityWebRequest req)
        {
            string xml = req.result == UnityWebRequest.Result.Success ? req.downloadHandler.text : null;
            req.Dispose();
            if (string.IsNullOrEmpty(xml)) { Complete(null); return; }

            string item = Between(xml, "<item>", "</item>");
            if (item == null) { Complete(null); return; }

            Entry entry = new Entry
            {
                Title = Decode(StripCData(Between(item, "<title>", "</title>"))),
                Summary = Decode(StripCData(Between(item, "<description>", "</description>"))),
                Url = StripCData(Between(item, "<link>", "</link>"))?.Trim(),
            };
            if (string.IsNullOrEmpty(entry.Title) || string.IsNullOrEmpty(entry.Url)) { Complete(null); return; }

            UnityWebRequest postReq = UnityWebRequest.Get(entry.Url);
            postReq.SendWebRequest().completed += _ => OnPostDone(postReq, entry);
        }

        private static void OnPostDone(UnityWebRequest req, Entry entry)
        {
            string html = req.result == UnityWebRequest.Result.Success ? req.downloadHandler.text : null;
            req.Dispose();
            string imageUrl = html == null ? null : ExtractOgImage(html);
            if (string.IsNullOrEmpty(imageUrl)) { Complete(entry); return; }

            UnityWebRequest imgReq = UnityWebRequestTexture.GetTexture(imageUrl);
            imgReq.SendWebRequest().completed += _ =>
            {
                if (imgReq.result == UnityWebRequest.Result.Success)
                    entry.Thumbnail = DownloadHandlerTexture.GetContent(imgReq);
                imgReq.Dispose();
                Complete(entry);
            };
        }

        private static void Complete(Entry entry)
        {
            s_cached = entry;
            if (entry == null) s_cached = null;
            s_inFlight = false;

            List<Action<Entry>> waiters = new List<Action<Entry>>(s_waiters);
            s_waiters.Clear();
            foreach (Action<Entry> w in waiters) w(entry);
        }

        private static string Between(string source, string startTag, string endTag)
        {
            if (string.IsNullOrEmpty(source)) return null;
            int start = source.IndexOf(startTag, StringComparison.Ordinal);
            if (start < 0) return null;
            start += startTag.Length;
            int end = source.IndexOf(endTag, start, StringComparison.Ordinal);
            return end < 0 ? null : source.Substring(start, end - start);
        }

        private static string StripCData(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            value = value.Trim();
            const string open = "<![CDATA[";
            const string close = "]]>";
            if (value.StartsWith(open, StringComparison.Ordinal) && value.EndsWith(close, StringComparison.Ordinal))
                return value.Substring(open.Length, value.Length - open.Length - close.Length).Trim();
            return value;
        }

        private static string ExtractOgImage(string html)
        {
            Match m = Regex.Match(html, "<meta[^>]+property=\"og:image\"[^>]+content=\"([^\"]+)\"", RegexOptions.IgnoreCase);
            if (!m.Success)
                m = Regex.Match(html, "<meta[^>]+content=\"([^\"]+)\"[^>]+property=\"og:image\"", RegexOptions.IgnoreCase);
            return m.Success ? m.Groups[1].Value : null;
        }

        private static string Decode(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Replace("&apos;", "'").Replace("&#39;", "'").Replace("&quot;", "\"")
                        .Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").Trim();
        }
    }
}
