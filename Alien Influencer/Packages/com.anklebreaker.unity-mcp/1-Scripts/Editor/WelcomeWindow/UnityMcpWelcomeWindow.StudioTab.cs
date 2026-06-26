using UnityEngine;
using UnityEngine.UIElements;

namespace UnityMCP.Editor.Welcome
{
    /// <summary>
    /// The fixed "AnkleBreaker" studio tab (identical across every generated
    /// asset): studio intro, live News &amp; Devlog box, and the studio link boxes.
    /// </summary>
    public sealed partial class UnityMcpWelcomeWindow
    {
        private void BuildStudioTab()
        {
            AddHeading(_scroll, "Who we are");
            AddBody(_scroll,
                "AnkleBreaker Studio is a French game development team based near Paris, working "
              + "remotely and fueled by a shared passion for survival, competitive and RTS games. "
              + "We're currently building our upcoming title, <b>Mithrall</b>.");

            BuildDevlogBox(_scroll);
            AddStudioBox("Join the team", "We're a remote French studio. If you love survival, competitive and RTS games, come build with us.", "View open roles", UnityMcpWelcomeIcon.WEB, CAREERS_URL);
            AddStudioBox("Our games", "Discover the games we're crafting, starting with our upcoming title <b>Mithrall</b>.", "Discover our games", UnityMcpWelcomeIcon.WEB, GAMES_URL);
            AddStudioBox("Tools & MCPs", "We build MCP plugins that bridge AI assistants into your tools -starting with our Unity MCP (200+ tools).", "View on GitHub", UnityMcpWelcomeIcon.STAR, GITHUB_URL);
            AddStudioBox("On the Asset Store", "Explore our other Unity tools and assets on the Unity Asset Store.", "See other products", UnityMcpWelcomeIcon.STORE, _config.PublisherUrl);
        }

        private void BuildDevlogBox(VisualElement parent)
        {
            VisualElement box = AddBox(parent, "News & Devlog", UnityMcpWelcomeIcon.WEB);

            Label loading = new Label("Loading the latest devlog...");
            loading.style.whiteSpace = WhiteSpace.Normal;
            loading.style.color = new Color(0.91f, 0.88f, 0.85f);
            box.Add(loading);

            UnityMcpWelcomeDevlogFetcher.Fetch(entry =>
            {
                if (box.panel == null) return;
                while (box.childCount > 1) box.RemoveAt(box.childCount - 1);

                if (entry == null)
                {
                    AddBoxBody(box, "Behind-the-scenes progress on <b>Mithrall</b> and our tools, plus studio news.");
                    box.Add(MakeAccentButton("Read the devlog", UnityMcpWelcomeIcon.WEB, () => OpenUrl(DEVLOG_URL)));
                    return;
                }

                if (entry.Thumbnail != null)
                {
                    Image cover = new Image { image = entry.Thumbnail, scaleMode = ScaleMode.ScaleAndCrop };
                    cover.style.height = 120;
                    cover.style.marginBottom = 8;
                    cover.style.borderTopLeftRadius = 4; cover.style.borderTopRightRadius = 4;
                    cover.style.borderBottomLeftRadius = 4; cover.style.borderBottomRightRadius = 4;
                    box.Add(cover);
                }

                Label title = new Label(entry.Title);
                title.AddToClassList("ab-section-heading");
                title.style.whiteSpace = WhiteSpace.Normal;
                box.Add(title);

                AddBoxBody(box, entry.Summary);
                string postUrl = string.IsNullOrEmpty(entry.Url) ? DEVLOG_URL : entry.Url;
                box.Add(MakeAccentButton("Read this devlog", UnityMcpWelcomeIcon.WEB, () => OpenUrl(postUrl)));
            });
        }

        private void AddStudioBox(string heading, string body, string buttonLabel, string iconName, string url)
        {
            VisualElement box = AddBox(_scroll, heading, iconName);
            AddBoxBody(box, body);
            box.Add(MakeAccentButton(buttonLabel, iconName, () => OpenUrl(url)));
        }
    }
}
