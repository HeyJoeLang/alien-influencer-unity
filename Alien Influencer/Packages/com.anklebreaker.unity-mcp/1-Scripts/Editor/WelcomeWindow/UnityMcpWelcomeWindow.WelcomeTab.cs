using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityMCP.Editor.Welcome
{
    /// <summary>
    /// The configurable "Welcome" tab: about / long-haul / review / custom
    /// sections / "More from" cross-sell box / anchored CTA buttons.
    /// </summary>
    public sealed partial class UnityMcpWelcomeWindow
    {
        private void BuildWelcomeTab()
        {
            AddHeading(_scroll, _config.AboutHeading);
            AddBody(_scroll, _config.AboutText);
            AddHeading(_scroll, _config.LongHaulHeading);
            AddBody(_scroll, _config.LongHaulText);

            AddAnchoredButtons(WelcomeCustomButton.ANCHOR_BEFORE_REVIEW, "");

            VisualElement review = AddBox(_scroll, _config.ReviewHeading, UnityMcpWelcomeIcon.STAR);
            AddBoxBody(review, _config.ReviewText);
            if (!string.IsNullOrEmpty(_config.ReviewButtonLabel))
            {
                string reviewUrl = string.IsNullOrEmpty(_config.ReviewUrl) ? _config.PublisherUrl : _config.ReviewUrl;
                review.Add(MakeAccentButton(_config.ReviewButtonLabel, UnityMcpWelcomeIcon.STAR, () => OpenUrl(reviewUrl)));
            }

            AddAnchoredButtons(WelcomeCustomButton.ANCHOR_AFTER_REVIEW, "");

            BuildCustomSections(WelcomeCustomSection.PLACEMENT_BEFORE);
            if (_config.ShowMoreFrom) BuildMoreFromBox();
            AddAnchoredButtons(WelcomeCustomButton.ANCHOR_AFTER_MOREFROM, "");
            BuildCustomSections(WelcomeCustomSection.PLACEMENT_AFTER);
        }

        private void AddAnchoredButtons(string anchor, string sectionHeading)
        {
            bool MatchAnchor(string a, string s) =>
                a == anchor && (anchor != WelcomeCustomButton.ANCHOR_AFTER_SECTION || s == sectionHeading);

            List<(string label, string icon, Action onClick)> specs = new List<(string, string, Action)>();

            if (_config.ShowExamples && MatchAnchor(_config.ExamplesAnchor, _config.ExamplesAnchorSectionHeading))
                specs.Add(("Browse Examples", UnityMcpWelcomeIcon.WEB, OpenExamples));
            if (_config.ShowDocumentation && MatchAnchor(_config.DocumentationAnchor, _config.DocumentationAnchorSectionHeading))
                specs.Add(("Documentation", UnityMcpWelcomeIcon.WEB, OpenDocumentation));
            if (_config.CustomButtons != null)
            {
                foreach (WelcomeCustomButton b in _config.CustomButtons)
                {
                    if (!MatchAnchor(b.Anchor, b.AnchorSectionHeading)) continue;
                    string label = string.IsNullOrEmpty(b.Label) ? "Open" : b.Label;
                    Action act;
                    if (b.ActionType == WelcomeCustomButton.ACTION_WINDOW)
                    {
                        string wt = b.WindowType;
                        act = () => OpenWindowType(wt);
                    }
                    else
                    {
                        string u = b.Url;
                        act = () => OpenUrl(u);
                    }
                    specs.Add((label, b.IconName, act));
                }
            }

            if (specs.Count == 0) return;

            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.flexWrap = Wrap.Wrap;
            row.style.justifyContent = Justify.Center;
            row.style.marginTop = 2;
            row.style.marginBottom = 4;

            foreach ((string label, string icon, Action onClick) in specs)
            {
                Button btn = MakeAccentButton(label, icon, onClick);
                btn.style.alignSelf = Align.Auto;
                btn.style.flexGrow = 0;
                btn.style.flexShrink = 0;
                btn.style.minWidth = StyleKeyword.Auto;
                btn.style.marginTop = 2;
                btn.style.marginBottom = 2;
                btn.style.marginLeft = 3;
                btn.style.marginRight = 3;
                row.Add(btn);
            }
            _scroll.Add(row);
        }

        private void BuildMoreFromBox()
        {
            VisualElement more = AddBox(_scroll, "More from AnkleBreaker Studio", UnityMcpWelcomeIcon.STORE);
            if (_config.CrossSell != null && _config.CrossSell.Count > 0)
            {
                AddBoxBody(more, _config.ProductName + " works standalone, and integrates automatically when these companions are present:");
                VisualElement row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                foreach (WelcomeCrossSellEntry e in _config.CrossSell)
                    row.Add(BuildCrossSellCard(e));
                more.Add(row);
            }

            VisualElement actions = new VisualElement();
            actions.style.flexDirection = FlexDirection.Row;
            actions.style.marginTop = 8;
            Button browse = MakeAccentButton("Browse all our assets", UnityMcpWelcomeIcon.STORE, () => OpenUrl(_config.PublisherUrl));
            browse.style.flexGrow = 1; browse.style.minWidth = 0; browse.style.alignSelf = Align.Auto;
            actions.Add(browse);
            Button github = MakeAccentButton("GitHub", UnityMcpWelcomeIcon.WEB, () => OpenUrl(GITHUB_URL));
            github.style.flexGrow = 1; github.style.minWidth = 0; github.style.alignSelf = Align.Auto; github.style.marginLeft = 8;
            actions.Add(github);
            more.Add(actions);
        }

        private void BuildCustomSections(string placement)
        {
            if (_config.CustomSections == null) return;
            foreach (WelcomeCustomSection s in _config.CustomSections)
            {
                string p = string.IsNullOrEmpty(s.Placement) ? WelcomeCustomSection.PLACEMENT_AFTER : s.Placement;
                if (p != placement) continue;
                VisualElement box = AddBox(_scroll, s.Heading, string.IsNullOrEmpty(s.IconName) ? null : s.IconName);
                AddBoxBody(box, s.Body);
                if (!string.IsNullOrEmpty(s.ButtonLabel))
                {
                    string u = s.ButtonUrl;
                    box.Add(MakeAccentButton(s.ButtonLabel, s.IconName, () => OpenUrl(u)));
                }

                AddAnchoredButtons(WelcomeCustomButton.ANCHOR_AFTER_SECTION, s.Heading);
            }
        }

        // --- Cross-sell card (3 states) --------------------------------------

        private static VisualElement BuildCrossSellCard(WelcomeCrossSellEntry entry)
        {
            VisualElement card = new VisualElement();
            card.AddToClassList("ab-box");
            card.style.flexGrow = 1;
            card.style.flexBasis = 0;
            card.style.marginLeft = 4;
            card.style.marginRight = 4;

            VisualElement head = new VisualElement();
            head.style.flexDirection = FlexDirection.Row;
            head.style.alignItems = Align.Center;
            head.Add(BuildLogo(entry));
            Label name = new Label(entry.Name) { style = { unityFontStyleAndWeight = FontStyle.Bold } };
            head.Add(name);
            card.Add(head);

            Label pitch = new Label(entry.Pitch) { enableRichText = true };
            pitch.style.whiteSpace = WhiteSpace.Normal;
            pitch.style.fontSize = 11;
            pitch.style.color = new Color(0.91f, 0.88f, 0.85f, 0.82f);
            pitch.style.marginTop = 4;
            pitch.style.marginBottom = 4;
            card.Add(pitch);

            UnityMcpWelcomeDetection.State state = UnityMcpWelcomeDetection.Resolve(entry.AssemblyPrefix, entry.CacheHints);
            switch (state)
            {
                case UnityMcpWelcomeDetection.State.InProject:
                    card.Add(StatusRow("Installed in project", UnityMcpWelcomeIcon.INSTALLED, new Color(0.55f, 0.85f, 0.45f)));
                    card.Add(StoreLink("View on Asset Store", entry.StoreUrl));
                    break;
                case UnityMcpWelcomeDetection.State.Owned:
                    card.Add(StatusRow("Owned - not imported", UnityMcpWelcomeIcon.DOWNLOAD, new Color(0.96f, 0.7f, 0.35f)));
                    string hints = entry.CacheHints;
                    Button import = new Button(() => ImportOwned(hints)) { text = "Import to project", focusable = false };
                    import.style.marginTop = 4;
                    card.Add(import);
                    card.Add(StoreLink("View on Asset Store", entry.StoreUrl));
                    break;
                default:
                    card.Add(StoreLink("Open on Asset Store", entry.StoreUrl));
                    break;
            }
            return card;
        }

        private static VisualElement StatusRow(string text, string iconName, Color color)
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            Image icon = UnityMcpWelcomeIcon.Make(iconName, 14, 4f);
            if (icon != null) row.Add(icon);
            Label l = new Label(text) { style = { color = color, unityFontStyleAndWeight = FontStyle.Bold } };
            l.style.fontSize = 11;
            row.Add(l);
            return row;
        }

        private static VisualElement StoreLink(string text, string url)
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.marginTop = 4;
            Image icon = UnityMcpWelcomeIcon.Make(UnityMcpWelcomeIcon.STORE, 14, 4f);
            if (icon != null) row.Add(icon);
            Label l = new Label(text) { style = { color = new Color(0.96f, 0.63f, 0.28f), unityFontStyleAndWeight = FontStyle.Bold } };
            l.style.fontSize = 11;
            row.Add(l);
            row.AddManipulator(new Clickable(() => OpenUrl(url)));
            return row;
        }

        private static VisualElement BuildLogo(WelcomeCrossSellEntry entry)
        {
            Color c = Color.gray;
            if (!string.IsNullOrEmpty(entry.ColorHex)) ColorUtility.TryParseHtmlString(entry.ColorHex, out c);
            VisualElement logo = new VisualElement();
            logo.style.width = 26; logo.style.height = 26; logo.style.marginRight = 6;
            logo.style.backgroundColor = c;
            logo.style.borderTopLeftRadius = 4; logo.style.borderTopRightRadius = 4;
            logo.style.borderBottomLeftRadius = 4; logo.style.borderBottomRightRadius = 4;
            logo.style.alignItems = Align.Center; logo.style.justifyContent = Justify.Center;
            logo.Add(new Label(entry.Initials) { style = { color = Color.white } });
            return logo;
        }
    }
}
