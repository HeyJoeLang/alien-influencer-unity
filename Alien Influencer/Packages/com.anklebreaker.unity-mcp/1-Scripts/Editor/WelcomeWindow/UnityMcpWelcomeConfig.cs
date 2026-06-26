using System;
using System.Collections.Generic;

namespace UnityMCP.Editor.Welcome
{
    /// <summary>
    /// One companion / cross-sell asset advertised on the Welcome tab. Mirrors
    /// the runtime three-state card (installed / owned / store link).
    /// </summary>
    [Serializable]
    public sealed class WelcomeCrossSellEntry
    {
        public string Name = "Companion Asset";
        public string Initials = "AB";
        public string ColorHex = "#d97a2a";
        public string Pitch = "What this companion adds.";
        public string StoreUrl = "";
        public string AssemblyPrefix = "";
        public string CacheHints = "";
    }

    /// <summary>A titled box with rich body text and an optional CTA button.</summary>
    [Serializable]
    public sealed class WelcomeCustomSection
    {
        public string Heading = "Section title";
        public string Body = "Section text.";
        public string IconName = "";
        public string ButtonLabel = "";
        public string ButtonUrl = "";
        public string Placement = PLACEMENT_AFTER;

        public const string PLACEMENT_BEFORE = "BeforeMoreFrom";
        public const string PLACEMENT_AFTER = "AfterMoreFrom";
    }

    /// <summary>A free-form call-to-action button placed at a chosen anchor.</summary>
    [Serializable]
    public sealed class WelcomeCustomButton
    {
        public string Label = "Open";
        public string ActionType = ACTION_URL;
        public string Url = "";
        public string WindowType = "";
        public string Anchor = ANCHOR_AFTER_REVIEW;
        public string AnchorSectionHeading = "";
        public string IconName = "";

        public const string ACTION_URL = "Url";
        public const string ACTION_WINDOW = "Window";

        public const string ANCHOR_BEFORE_REVIEW = "BeforeReview";
        public const string ANCHOR_AFTER_REVIEW = "AfterReview";
        public const string ANCHOR_AFTER_MOREFROM = "AfterMoreFrom";
        public const string ANCHOR_AFTER_SECTION = "AfterSection";
    }

    /// <summary>
    /// The per-asset content of this Welcome window, deserialized at runtime from
    /// the base64 config embedded in the window source. Field names must match the
    /// generator's config exactly so the JSON round-trips.
    /// </summary>
    [Serializable]
    public sealed class UnityMcpWelcomeConfig
    {
        public string PackageNamespace = "";
        public string ClassPrefix = "";
        public string MenuPath = "";

        public string Distribution = DISTRIBUTION_ASSET_STORE;
        public const string DISTRIBUTION_ASSET_STORE = "AssetStore";
        public const string DISTRIBUTION_GITHUB = "GitHub";

        public string ProductName = "My Asset";
        public string Subtitle = "by AnkleBreaker Studio";

        public string AboutHeading = "About";
        public string AboutText = "";

        public string LongHaulHeading = "Built for the long haul";
        public string LongHaulText = "";

        public string ReviewHeading = "Leave us a review!";
        public string ReviewText = "";
        public string ReviewButtonLabel = "Leave a review";
        public string ReviewUrl = "";

        public string PublisherUrl = "";

        public bool ShowMoreFrom = true;
        public List<WelcomeCrossSellEntry> CrossSell = new List<WelcomeCrossSellEntry>();
        public List<WelcomeCustomSection> CustomSections = new List<WelcomeCustomSection>();

        public bool ShowExamples = false;
        public string ExamplesFolder = "";
        public string ExamplesWindowType = "";
        public string ExamplesAnchor = WelcomeCustomButton.ANCHOR_BEFORE_REVIEW;
        public string ExamplesAnchorSectionHeading = "";

        public bool ShowDocumentation = false;
        public string DocumentationPath = "Documentation";
        public string DocumentationWindowType = "";
        public string DocumentationAnchor = WelcomeCustomButton.ANCHOR_BEFORE_REVIEW;
        public string DocumentationAnchorSectionHeading = "";

        public List<WelcomeCustomButton> CustomButtons = new List<WelcomeCustomButton>();
    }
}
