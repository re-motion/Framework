// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Accessibility.Implementation
{
  /// <summary>
  /// Provides conversions from <see cref="AccessibilityRuleID"/> to <see cref="string"/> and vice versa.
  /// </summary>
  public static class AccessibilityRuleIDConverter
  {
    private static readonly IReadOnlyDictionary<string, AccessibilityRuleID> s_stringToEnum;
    private static readonly IReadOnlyDictionary<AccessibilityRuleID, string> s_enumToString;

    static AccessibilityRuleIDConverter ()
    {
      s_stringToEnum
          = new Dictionary<string, AccessibilityRuleID>
            {
                { "accesskeys", AccessibilityRuleID.AccessKeys },
                { "area-alt", AccessibilityRuleID.AreaAlt },
                { "aria-allowed-attr", AccessibilityRuleID.AriaAllowedAttr },
                { "aria-allowed-role", AccessibilityRuleID.AriaAllowedRole },
#pragma warning disable CS0618
                { "aria-dpub-role-fallback", AccessibilityRuleID.AriaDpubRoleFallback },
#pragma warning restore CS0618
                { "aria-hidden-body", AccessibilityRuleID.AriaHiddenBody },
                { "aria-hidden-focus", AccessibilityRuleID.AriaHiddenFocus },
                { "aria-input-field-name", AccessibilityRuleID.AriaInputFieldName },
                { "aria-required-attr", AccessibilityRuleID.AriaRequiredAttr },
                { "aria-required-children", AccessibilityRuleID.AriaRequiredChildren },
                { "aria-required-parent", AccessibilityRuleID.AriaRequiredParent },
                { "aria-roledescription", AccessibilityRuleID.AriaRoleDescription},
                { "aria-roles", AccessibilityRuleID.AriaRoles },
                { "aria-toggle-field-name", AccessibilityRuleID.AriaToggleFieldName },
                { "aria-valid-attr-value", AccessibilityRuleID.AriaValidAttrValue },
                { "aria-valid-attr", AccessibilityRuleID.AriaValidAttr },
                { "audio-caption", AccessibilityRuleID.AudioCaption },
                { "autocomplete-valid", AccessibilityRuleID.AutocompleteValid },
                { "avoid-inline-spacing", AccessibilityRuleID.AvoidInlineSpacing },
                { "blink", AccessibilityRuleID.Blink },
                { "button-name", AccessibilityRuleID.ButtonName },
                { "bypass", AccessibilityRuleID.Bypass },
#pragma warning disable CS0618
                { "checkboxgroup", AccessibilityRuleID.CheckboxGroup },
#pragma warning restore CS0618
                { "color-contrast", AccessibilityRuleID.ColorContrast },
                { "css-orientation-lock", AccessibilityRuleID.CssOrientationLock },
                { "definition-list", AccessibilityRuleID.DefinitionList },
                { "dlitem", AccessibilityRuleID.Dlitem },
                { "document-title", AccessibilityRuleID.DocumentTitle },
                { "duplicate-id-active", AccessibilityRuleID.DuplicateIdActive },
                { "duplicate-id-aria", AccessibilityRuleID.DuplicateIdAria },
                { "duplicate-id", AccessibilityRuleID.DuplicateId },
                { "empty-heading", AccessibilityRuleID.EmptyHeading },
                { "focus-order-semantics", AccessibilityRuleID.FocusOrderSemantics },
                { "form-field-multiple-labels", AccessibilityRuleID.FormFieldMultipleLabels },
                { "frame-tested", AccessibilityRuleID.FrameTested },
                { "frame-title-unique", AccessibilityRuleID.FrameTitleUnique },
                { "frame-title", AccessibilityRuleID.FrameTitle },
                { "heading-order", AccessibilityRuleID.HeadingOrder },
                { "hidden-content", AccessibilityRuleID.HiddenContent },
                { "html-has-lang", AccessibilityRuleID.HtmlHasLang },
                { "html-lang-valid", AccessibilityRuleID.HtmlLangValid },
                { "html-xml-lang-mismatch", AccessibilityRuleID.HtmlXmlLangMismatch },
                { "identical-links-same-purpose", AccessibilityRuleID.IdenticalLinksSamePurpose},
                { "image-alt", AccessibilityRuleID.ImageAlt },
                { "image-redundant-alt", AccessibilityRuleID.ImageRedundantAlt },
                { "input-button-name", AccessibilityRuleID.InputButtonName },
                { "input-image-alt", AccessibilityRuleID.InputImageAlt },
                { "label-content-name-mismatch", AccessibilityRuleID.LabelContentNameMismatch },
                { "label-title-only", AccessibilityRuleID.LabelTitleOnly },
                { "label", AccessibilityRuleID.Label },
                { "landmark-banner-is-top-level", AccessibilityRuleID.LandmarkBannerIsTopLevel },
                { "landmark-complementary-is-top-level", AccessibilityRuleID.LandmarkComplementaryIsTopLevel },
                { "landmark-contentinfo-is-top-level", AccessibilityRuleID.LandmarkContentInfoIsTopLevel },
                { "landmark-main-is-top-level", AccessibilityRuleID.LandmarkMainIsTopLevel },
                { "landmark-no-duplicate-banner", AccessibilityRuleID.LandmarkNoDuplicateBanner },
                { "landmark-no-duplicate-contentinfo", AccessibilityRuleID.LandmarkNoDuplicateContentInfo },
                { "landmark-no-duplicate-main", AccessibilityRuleID.LandmarkNoDuplicateMain },
                { "landmark-one-main", AccessibilityRuleID.LandmarkOneMain },
                { "landmark-unique", AccessibilityRuleID.LandmarkUnique },
#pragma warning disable CS0618
                { "layout-table", AccessibilityRuleID.LayoutTable },
#pragma warning restore CS0618
                { "link-in-text-block", AccessibilityRuleID.LinkInTextBlock },
                { "link-name", AccessibilityRuleID.LinkName },
                { "list", AccessibilityRuleID.List },
                { "listitem", AccessibilityRuleID.ListItem },
                { "marquee", AccessibilityRuleID.Marquee },
                { "meta-refresh", AccessibilityRuleID.MetaRefresh },
                { "meta-viewport-large", AccessibilityRuleID.MetaViewportLarge },
                { "meta-viewport", AccessibilityRuleID.MetaViewport },
#pragma warning disable CS0618
                { "no-autoplay-audio", AccessibilityRuleID.NoAutoplayAudio },
#pragma warning restore CS0618
                { "object-alt", AccessibilityRuleID.ObjectAlt },
                { "p-as-heading", AccessibilityRuleID.PAsHeading },
                { "page-has-heading-one", AccessibilityRuleID.PageHasHeadingOne },
#pragma warning disable CS0618
                { "radiogroup", AccessibilityRuleID.RadioGroup },
#pragma warning restore CS0618
                { "region", AccessibilityRuleID.Region },
                { "role-img-alt", AccessibilityRuleID.RoleImgAlt },
                { "scope-attr-valid", AccessibilityRuleID.ScopeAttrValid },
                { "scrollable-region-focusable", AccessibilityRuleID.ScrollableRegionFocusable },
                { "server-side-image-map", AccessibilityRuleID.ServerSideImageMap },
                { "skip-link", AccessibilityRuleID.SkipLink },
                { "svg-img-alt", AccessibilityRuleID.SvgImageAlt},
                { "tabindex", AccessibilityRuleID.Tabindex },
                { "table-duplicate-name", AccessibilityRuleID.TableDuplicateName },
                { "table-fake-caption", AccessibilityRuleID.TableFakeCaption },
                { "td-has-header", AccessibilityRuleID.TdHasHeader },
                { "td-headers-attr", AccessibilityRuleID.TdHeadersAttr },
                { "th-has-data-cells", AccessibilityRuleID.ThHasDataCells },
                { "valid-lang", AccessibilityRuleID.ValidLang },
                { "video-caption", AccessibilityRuleID.VideoCaption },
#pragma warning disable CS0618
                { "video-description", AccessibilityRuleID.VideoDescription }
#pragma warning restore CS0618
            };

      s_enumToString = s_stringToEnum.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
    }

    /// <summary>
    /// Converts a <see cref="string"/> to its <see cref="AccessibilityRuleID"/> representation.
    /// </summary>
    public static AccessibilityRuleID ConvertToEnum ([NotNull] string ruleIDAsString)
    {
      ArgumentUtility.CheckNotNullOrEmpty("ruleIDAsString", ruleIDAsString);

      if (!s_stringToEnum.TryGetValue(ruleIDAsString, out var ruleID))
        return AccessibilityRuleID.Unknown;

      return ruleID;
    }

    /// <summary>
    /// Converts <see cref="AccessibilityRuleID"/> to its <see cref="string"/> representation.
    /// </summary>
    public static string ConvertToString (AccessibilityRuleID ruleID)
    {
      if (ruleID == AccessibilityRuleID.Unknown)
        throw new ArgumentException($"{ruleID} has no string representation.");

      return s_enumToString[ruleID];
    }
  }
}
