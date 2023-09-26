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

namespace Remotion.Web.Development.WebTesting.Accessibility
{
  /// <summary>
  /// IDs of the accessibility checks.
  /// </summary>
  /// <remarks>
  /// A list of all all requirements can be found at <see href="https://github.com/dequelabs/axe-core/tree/develop/lib/checks"/>.
  /// Each folder relates to a section in this list.
  /// The easiest way to quickly find which checks changed was to look at the diff of these files between the releases.
  /// </remarks>
  public enum AccessibilityRequirementID
  {
    Unknown,

    // --- Aria Checks ---
    AbstractRole,
    AriaAllowedAttr,
    AriaAllowedRole,
    AriaBusy,
    AriaConditionalAttr,
    AriaErrorMessage,
    AriaHiddenBody,
    AriaLevel,
    AriaProhibitedAttr,
    AriaRequiredAttr,
    AriaRequiredChildren,
    AriaRequiredParent,
    AriaRoledescription,
    AriaUnsupportedAttr,
    AriaValidAttrValue,
    AriaValidAttr,
    BrailleLabelEquivalent,
    BrailleRoledescriptionEquivalent,
    Deprecatedrole,
    Fallbackrole,
    HasGlobalAriaAttribute,
    HasWidgetRole,
    InvalidRole,
    IsElementFocusable,
    NoImplicitExplicitLabel,
    Unsupportedrole,
    ValidScrollableSemantics,

    // --- Color Checks ---
    ColorContrastEnhanced,
    ColorContrast,
    LinkInTextBlockStyle,
    LinkInTextBlock,

    // --- Forms Checks ---
    AutocompleteAppropriate,
    AutocompleteValid,

    // --- Generic Checks ---

    // --- Keyboard Checks ---
    AccessKeys,
    FocusableContent,
    FocusableDisabled,
    FocusableElement,
    FocusableModalOpen,
    FocusableNoName,
    FocusableNotTabbable,
    FrameFocusableContent,
    LandmarkIsTopLevel,
    NoFocusableContent,
    PageHasHeadingOne,
    PageHasMain,
    PageNoDuplicateBanner,
    PageNoDuplicateContentInfo,
    PageNoDuplicateMain,
    Tabindex,

    // --- Label Checks ---
    AltSpaceValue,
    DuplicateImgLabel,
    ExplicitLabel,
    HelpSameAsLabel,
    HiddenExplicitLabel,
    ImplicitLabel,
    LabelContentNameMismatch,
    MultipleLabel,
    TitleOnly,

    // --- Landmark Checks ---
    LandmarkIsUnique,

    // --- Language Checks ---
    HasLang,
    ValidLang,
    XMLLangMismatch,

    // --- List Checks ---
    DlItem,
    ListItem,
    OnlyDlItems,
    OnlyListItems,
    StructuredDlItems,

    // --- Media Checks ---
    Caption,
    FrameTested,
    NoAutoplayAudio,

    // --- Mobile Checks ---
    CSSOrientationLock,
    MetaViewportLarge,
    MetaViewport,
    TargetOffset,
    TargetSize,

    // --- Navigation Checks ---
    HeaderPresent,
    HeadingOrder,
    IdenticalLinksSamePurpose,
    InternalLinkPresent,
    Landmark,
    MetaRefreshNoExceptions,
    MetaRefresh,
    PAsHeading,
    Region,
    SkipLink,
    UniqueFrameTitle,

    // --- Parsing Checks
    DuplicateIdActive,
    DuplicateIdAria,
    DuplicateId,

    // --- Shared Checks
    AriaLabel,
    AriaLabelledBy,
    AvoidInlineSpacing,
    ButtonHasVisibleText,
    DocHasTitle,
    Exists,
    HasAlt,
    HasVisibleText,
    ImportantLetterSpacing,
    ImportantLineHeight,
    ImportantWordSpacing,
    IsOnScreen,
    NonEmptyAlt,
    NonEmptyIfPresent,
    NonEmptyPlaceholder,
    NonEmptyTitle,
    NonEmptyValue,
    PresentationalRole,
    SVGNonEmptyTitle,

    // --- Table Checks ---
    CaptionFaked,
    Html5Scope,
    SameCaptionSummary,
    ScopeValue,
    TdHasHeader,
    TdHeadersAttr,
    ThHasDataCells,

    // --- Visibility Checks ---
    HiddenContent
  }
}
