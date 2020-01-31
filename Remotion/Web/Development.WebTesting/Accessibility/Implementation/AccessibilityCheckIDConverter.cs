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
  public static class AccessibilityCheckIDConverter
  {
    private static readonly Dictionary<string, AccessibilityRequirementID> s_dictionary
        = new Dictionary<string, AccessibilityRequirementID>
          {
              { "", AccessibilityRequirementID.Unknown },
              { "abstractrole", AccessibilityRequirementID.AbstractRole },
              { "aria-allowed-attr", AccessibilityRequirementID.AriaAllowedAttr },
              { "aria-hidden-body", AccessibilityRequirementID.AriaHiddenBody },
              { "aria-errormessage", AccessibilityRequirementID.AriaErrorMessage },
              { "has-widget-role", AccessibilityRequirementID.HasWidgetRole },
              { "implicit-role-fallback", AccessibilityRequirementID.ImplicitRoleFallback },
              { "invalidrole", AccessibilityRequirementID.InvalidRole },
              { "aria-required-attr", AccessibilityRequirementID.AriaRequiredAttr },
              { "aria-required-children", AccessibilityRequirementID.AriaRequiredChildren },
              { "aria-required-parent", AccessibilityRequirementID.AriaRequiredParent },
              { "aria-valid-attr-value", AccessibilityRequirementID.AriaValidAttrValue },
              { "aria-valid-attr", AccessibilityRequirementID.AriaValidAttr },
              { "valid-scrollable-semantics", AccessibilityRequirementID.ValidScrollableSemantics },
              { "color-contrast", AccessibilityRequirementID.ColorContrast },
              { "link-in-text-block", AccessibilityRequirementID.LinkInTextBlock },
              { "fieldset", AccessibilityRequirementID.Fieldset },
              { "group-labelledby", AccessibilityRequirementID.GroupLabeledBy },
              { "accesskeys", AccessibilityRequirementID.AccessKeys },
              { "focusable-no-name", AccessibilityRequirementID.FocusableNoName },
              { "landmark-is-top-level", AccessibilityRequirementID.LandmarkIsTopLevel },
              { "page-has-heading-one", AccessibilityRequirementID.PageHasHeadingOne },
              { "page-has-main", AccessibilityRequirementID.PageHasMain },
              { "page-no-duplicate-banner", AccessibilityRequirementID.PageNoDuplicateBanner },
              { "page-no-duplicate-contentinfo", AccessibilityRequirementID.PageNoDuplicateContentInfo },
              { "page-no-duplicate-main", AccessibilityRequirementID.PageNoDuplicateMain },
              { "tabindex", AccessibilityRequirementID.Tabindex },
              { "duplicate-img-label", AccessibilityRequirementID.DuplicateImgLabel },
              { "explicit-label", AccessibilityRequirementID.ExplicitLabel },
              { "help-same-as-label", AccessibilityRequirementID.HelpSameAsLabel },
              { "implicit-label", AccessibilityRequirementID.ImplicitLabel },
              { "multiple-label", AccessibilityRequirementID.MultipleLabel },
              { "title-only", AccessibilityRequirementID.TitleOnly },
              { "has-lang", AccessibilityRequirementID.HasLang },
              { "valid-lang", AccessibilityRequirementID.ValidLang },
              { "dlitem", AccessibilityRequirementID.DlItem },
              { "listitem", AccessibilityRequirementID.ListItem },
              { "only-dlitems", AccessibilityRequirementID.OnlyDlItems },
              { "only-listitems", AccessibilityRequirementID.OnlyListItems },
              { "structured-dlitems", AccessibilityRequirementID.StructuredDlItems },
              { "caption", AccessibilityRequirementID.Caption },
              { "description", AccessibilityRequirementID.Description },
              { "frame-tested", AccessibilityRequirementID.FrameTested },
              { "meta-viewport-large", AccessibilityRequirementID.MetaViewportLarge },
              { "meta-viewport", AccessibilityRequirementID.MetaViewport },
              { "header-present", AccessibilityRequirementID.HeaderPresent },
              { "heading-order", AccessibilityRequirementID.HeadingOrder },
              { "internal-link-present", AccessibilityRequirementID.InternalLinkPresent },
              { "landmark", AccessibilityRequirementID.Landmark },
              { "meta-refresh", AccessibilityRequirementID.MetaRefresh },
              { "p-as-heading", AccessibilityRequirementID.PAsHeading },
              { "region", AccessibilityRequirementID.Region },
              { "skip-link", AccessibilityRequirementID.SkipLink },
              { "unique-frame-title", AccessibilityRequirementID.UniqueFrameTitle },
              { "aria-label", AccessibilityRequirementID.AriaLabel },
              { "aria-labelledby", AccessibilityRequirementID.AriaLabeledBy },
              { "button-has-visible-text", AccessibilityRequirementID.ButtonHasVisibleText },
              { "doc-has-title", AccessibilityRequirementID.DocHasTitle },
              { "duplicate-id", AccessibilityRequirementID.DuplicateId },
              { "exists", AccessibilityRequirementID.Exists },
              { "has-alt", AccessibilityRequirementID.HasAlt },
              { "has-visible-text", AccessibilityRequirementID.HasVisibleText },
              { "is-on-screen", AccessibilityRequirementID.IsOnScreen },
              { "non-empty-alt", AccessibilityRequirementID.NonEmptyAlt },
              { "non-empty-if-present", AccessibilityRequirementID.NonEmptyIfPresent },
              { "non-empty-title", AccessibilityRequirementID.NonEmptyTitle },
              { "non-empty-value", AccessibilityRequirementID.NonEmptyValue },
              { "role-none", AccessibilityRequirementID.RoleNone },
              { "role-presentation", AccessibilityRequirementID.RolePresentation },
              { "caption-faked", AccessibilityRequirementID.CaptionFaked },
              { "has-caption", AccessibilityRequirementID.HasCaption },
              { "has-summary", AccessibilityRequirementID.HasSummary },
              { "has-th", AccessibilityRequirementID.HasTh },
              { "html5-scope", AccessibilityRequirementID.Html5Scope },
              { "same-caption-summary", AccessibilityRequirementID.SameCaptionSummary },
              { "scope-value", AccessibilityRequirementID.ScopeValue },
              { "td-has-header", AccessibilityRequirementID.TdHasHeader },
              { "td-headers-attr", AccessibilityRequirementID.TdHeadersAttr },
              { "th-has-data-cells", AccessibilityRequirementID.ThHasDataCells },
              { "hidden-content", AccessibilityRequirementID.HiddenContent }
          };

    /// <summary>
    /// Converts a <see cref="string"/> to its <see cref="AccessibilityRequirementID"/> representation.
    /// </summary>
    public static AccessibilityRequirementID ConvertToEnum ([NotNull] string checkIDAsString)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("checkIDAsString", checkIDAsString);

      if (!s_dictionary.TryGetValue (checkIDAsString, out var ruleID))
        return AccessibilityRequirementID.Unknown;

      return ruleID;
    }
  }
}
