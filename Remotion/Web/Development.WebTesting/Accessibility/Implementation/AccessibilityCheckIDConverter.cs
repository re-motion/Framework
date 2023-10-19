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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Accessibility.Implementation
{
  /// <summary>
  /// Provides a conversion from <see cref="string"/> to <see cref="AccessibilityRequirementID"/>.
  /// </summary>
  /// <remarks>
  /// A list of all all requirements can be found at <see href="https://github.com/dequelabs/axe-core/tree/develop/lib/checks"/>.
  /// Each folder relates to a section in the dictionary.</remarks>
  public static class AccessibilityCheckIDConverter
  {
    private static readonly Dictionary<string, AccessibilityRequirementID> s_dictionary
        = new Dictionary<string, AccessibilityRequirementID>
          {
              { "", AccessibilityRequirementID.Unknown },

              // --- Aria Checks ---
              { "abstractrole", AccessibilityRequirementID.AbstractRole },
              { "aria-allowed-attr", AccessibilityRequirementID.AriaAllowedAttr },
              { "aria-allowed-role", AccessibilityRequirementID.AriaAllowedRole },
              { "aria-busy", AccessibilityRequirementID.AriaBusy },
              { "aria-conditional-attr", AccessibilityRequirementID.AriaConditionalAttr },
              { "aria-errormessage", AccessibilityRequirementID.AriaErrorMessage },
              { "aria-hidden-body", AccessibilityRequirementID.AriaHiddenBody },
              { "aria-level", AccessibilityRequirementID.AriaLevel },
              { "aria-prohibited-attr", AccessibilityRequirementID.AriaProhibitedAttr },
              { "aria-required-attr", AccessibilityRequirementID.AriaRequiredAttr },
              { "aria-required-children", AccessibilityRequirementID.AriaRequiredChildren },
              { "aria-required-parent", AccessibilityRequirementID.AriaRequiredParent },
              { "aria-roledescription", AccessibilityRequirementID.AriaRoledescription },
              { "aria-unsupported-attr", AccessibilityRequirementID.AriaUnsupportedAttr },
              { "aria-valid-attr-value", AccessibilityRequirementID.AriaValidAttrValue },
              { "aria-valid-attr", AccessibilityRequirementID.AriaValidAttr },
              { "braille-label-equivalent", AccessibilityRequirementID.BrailleLabelEquivalent },
              { "braille-roledescription-equivalent", AccessibilityRequirementID.BrailleRoledescriptionEquivalent },
              { "deprecatedrole", AccessibilityRequirementID.Deprecatedrole },
              { "fallbackrole", AccessibilityRequirementID.Fallbackrole },
              { "has-global-aria-attribute-evaluate", AccessibilityRequirementID.HasGlobalAriaAttribute },
              { "has-widget-role", AccessibilityRequirementID.HasWidgetRole },
              { "invalidrole", AccessibilityRequirementID.InvalidRole },
              { "is-element-focusable-evaluate", AccessibilityRequirementID.IsElementFocusable },
              { "no-implicit-explicit-label", AccessibilityRequirementID.NoImplicitExplicitLabel },
              { "unsupportedrole", AccessibilityRequirementID.Unsupportedrole },
              { "valid-scrollable-semantics", AccessibilityRequirementID.ValidScrollableSemantics },

              // --- Color Checks ---
              { "color-contrast-enhanced", AccessibilityRequirementID.ColorContrastEnhanced },
              { "color-contrast", AccessibilityRequirementID.ColorContrast },
              { "link-in-text-block-style", AccessibilityRequirementID.LinkInTextBlockStyle },
              { "link-in-text-block", AccessibilityRequirementID.LinkInTextBlock },

              // --- Forms Checks ---
              { "autocomplete-appropriate", AccessibilityRequirementID.AutocompleteAppropriate },
              { "autocomplete-valid", AccessibilityRequirementID.AutocompleteValid },

              // --- Generic Checks ---

              // --- Keyboard Checks ---
              { "accesskeys", AccessibilityRequirementID.AccessKeys },
              { "focusable-content", AccessibilityRequirementID.FocusableContent },
              { "focusable-disabled", AccessibilityRequirementID.FocusableDisabled },
              { "focusable-element", AccessibilityRequirementID.FocusableElement },
              { "focusable-modal-open", AccessibilityRequirementID.FocusableModalOpen },
              { "focusable-no-name", AccessibilityRequirementID.FocusableNoName },
              { "focusable-not-tabbable", AccessibilityRequirementID.FocusableNotTabbable },
              { "frame-focusable-content", AccessibilityRequirementID.FrameFocusableContent },
              { "landmark-is-top-level", AccessibilityRequirementID.LandmarkIsTopLevel },
              { "no-focusable-content", AccessibilityRequirementID.NoFocusableContent },
              { "page-has-heading-one", AccessibilityRequirementID.PageHasHeadingOne },
              { "page-has-main", AccessibilityRequirementID.PageHasMain },
              { "page-no-duplicate-banner", AccessibilityRequirementID.PageNoDuplicateBanner },
              { "page-no-duplicate-contentinfo", AccessibilityRequirementID.PageNoDuplicateContentInfo },
              { "page-no-duplicate-main", AccessibilityRequirementID.PageNoDuplicateMain },
              { "tabindex", AccessibilityRequirementID.Tabindex },

              // --- Label Checks ---
              { "alt-space-value", AccessibilityRequirementID.AltSpaceValue },
              { "duplicate-img-label", AccessibilityRequirementID.DuplicateImgLabel },
              { "explicit-label", AccessibilityRequirementID.ExplicitLabel },
              { "help-same-as-label", AccessibilityRequirementID.HelpSameAsLabel },
              { "hidden-explicit-label", AccessibilityRequirementID.HiddenExplicitLabel },
              { "implicit-label", AccessibilityRequirementID.ImplicitLabel },
              { "label-content-name-mismatch", AccessibilityRequirementID.LabelContentNameMismatch },
              { "multiple-label", AccessibilityRequirementID.MultipleLabel },
              { "title-only", AccessibilityRequirementID.TitleOnly },

              // --- Landmark Checks ---
              { "landmark-is-unique", AccessibilityRequirementID.LandmarkIsUnique },

              // --- Language Checks ---
              { "has-lang", AccessibilityRequirementID.HasLang },
              { "valid-lang", AccessibilityRequirementID.ValidLang },
              { "xml-lang-mismatch", AccessibilityRequirementID.XMLLangMismatch },

              // --- List Checks ---
              { "dlitem", AccessibilityRequirementID.DlItem },
              { "listitem", AccessibilityRequirementID.ListItem },
              { "only-dlitems", AccessibilityRequirementID.OnlyDlItems },
              { "only-listitems", AccessibilityRequirementID.OnlyListItems },
              { "structured-dlitems", AccessibilityRequirementID.StructuredDlItems },

              // --- Media Checks ---
              { "caption", AccessibilityRequirementID.Caption },
              { "frame-tested", AccessibilityRequirementID.FrameTested },
              { "no-autoplay-audio", AccessibilityRequirementID.NoAutoplayAudio },

              // --- Mobile Checks ---
              { "css-orientation-lock", AccessibilityRequirementID.CSSOrientationLock },
              { "meta-viewport-large", AccessibilityRequirementID.MetaViewportLarge },
              { "meta-viewport", AccessibilityRequirementID.MetaViewport },
              { "target-offset", AccessibilityRequirementID.TargetOffset },
              { "target-size", AccessibilityRequirementID.TargetSize },

              // --- Navigation Checks ---
              { "header-present", AccessibilityRequirementID.HeaderPresent },
              { "heading-order", AccessibilityRequirementID.HeadingOrder },
              { "identical-links-same-purpose", AccessibilityRequirementID.IdenticalLinksSamePurpose },
              { "internal-link-present", AccessibilityRequirementID.InternalLinkPresent },
              { "landmark", AccessibilityRequirementID.Landmark },
              { "meta-refresh-no-exceptions", AccessibilityRequirementID.MetaRefreshNoExceptions },
              { "meta-refresh", AccessibilityRequirementID.MetaRefresh },
              { "p-as-heading", AccessibilityRequirementID.PAsHeading },
              { "region", AccessibilityRequirementID.Region },
              { "skip-link", AccessibilityRequirementID.SkipLink },
              { "unique-frame-title", AccessibilityRequirementID.UniqueFrameTitle },

              // --- Parsing Checks ---
              { "duplicate-id-active", AccessibilityRequirementID.DuplicateIdActive },
              { "duplicate-id-aria", AccessibilityRequirementID.DuplicateIdAria },
              { "duplicate-id", AccessibilityRequirementID.DuplicateId },

              // --- Shared Checks ---
              { "aria-label", AccessibilityRequirementID.AriaLabel },
              { "aria-labelledby", AccessibilityRequirementID.AriaLabelledBy },
              { "avoid-inline-spacing", AccessibilityRequirementID.AvoidInlineSpacing },
              { "button-has-visible-text", AccessibilityRequirementID.ButtonHasVisibleText },
              { "doc-has-title", AccessibilityRequirementID.DocHasTitle },
              { "exists", AccessibilityRequirementID.Exists },
              { "has-alt", AccessibilityRequirementID.HasAlt },
              { "has-visible-text", AccessibilityRequirementID.HasVisibleText },
              { "important-letter-spacing", AccessibilityRequirementID.ImportantLetterSpacing },
              { "important-line-height", AccessibilityRequirementID.ImportantLineHeight },
              { "important-word-spacing", AccessibilityRequirementID.ImportantWordSpacing },
              { "is-on-screen", AccessibilityRequirementID.IsOnScreen },
              { "non-empty-alt", AccessibilityRequirementID.NonEmptyAlt },
              { "non-empty-if-present", AccessibilityRequirementID.NonEmptyIfPresent },
              { "non-empty-placeholder", AccessibilityRequirementID.NonEmptyPlaceholder },
              { "non-empty-title", AccessibilityRequirementID.NonEmptyTitle },
              { "non-empty-value", AccessibilityRequirementID.NonEmptyValue },
              { "presentational-role", AccessibilityRequirementID.PresentationalRole },
              { "svg-non-empty-title", AccessibilityRequirementID.SVGNonEmptyTitle },

              // --- Table Checks ---
              { "caption-faked", AccessibilityRequirementID.CaptionFaked },
              { "html5-scope", AccessibilityRequirementID.Html5Scope },
              { "same-caption-summary", AccessibilityRequirementID.SameCaptionSummary },
              { "scope-value", AccessibilityRequirementID.ScopeValue },
              { "td-has-header", AccessibilityRequirementID.TdHasHeader },
              { "td-headers-attr", AccessibilityRequirementID.TdHeadersAttr },
              { "th-has-data-cells", AccessibilityRequirementID.ThHasDataCells },

              // --- Visibility Checks ---
              { "hidden-content", AccessibilityRequirementID.HiddenContent }
          };

    /// <summary>
    /// Converts a <see cref="string"/> to its <see cref="AccessibilityRequirementID"/> representation.
    /// </summary>
    public static AccessibilityRequirementID ConvertToEnum ([NotNull] string checkIDAsString)
    {
      ArgumentUtility.CheckNotNullOrEmpty("checkIDAsString", checkIDAsString);

      if (!s_dictionary.TryGetValue(checkIDAsString, out var ruleID))
        return AccessibilityRequirementID.Unknown;

      return ruleID;
    }
  }
}
