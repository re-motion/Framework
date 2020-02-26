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
  public enum AccessibilityRequirementID
  {
    Unknown,
    AbstractRole,
    AriaAllowedAttr,
    AriaHiddenBody,
    AriaErrorMessage,
    HasWidgetRole,
    ImplicitRoleFallback,
    InvalidRole,
    AriaRequiredAttr,
    AriaRequiredChildren,
    AriaRequiredParent,
    AriaValidAttrValue,
    AriaValidAttr,
    ValidScrollableSemantics,
    ColorContrast,
    LinkInTextBlock,
    Fieldset,
    GroupLabeledBy,
    AccessKeys,
    FocusableNoName,
    LandmarkIsTopLevel,
    PageHasHeadingOne,
    PageHasMain,
    PageNoDuplicateBanner,
    PageNoDuplicateContentInfo,
    PageNoDuplicateMain,
    Tabindex,
    DuplicateImgLabel,
    ExplicitLabel,
    HelpSameAsLabel,
    ImplicitLabel,
    MultipleLabel,
    TitleOnly,
    HasLang,
    ValidLang,
    DlItem,
    ListItem,
    OnlyDlItems,
    OnlyListItems,
    StructuredDlItems,
    Caption,
    Description,
    FrameTested,
    MetaViewportLarge,
    MetaViewport,
    HeaderPresent,
    HeadingOrder,
    InternalLinkPresent,
    Landmark,
    MetaRefresh,
    PAsHeading,
    Region,
    SkipLink,
    UniqueFrameTitle,
    AriaLabel,
    AriaLabeledBy,
    ButtonHasVisibleText,
    DocHasTitle,
    DuplicateId,
    Exists,
    HasAlt,
    HasVisibleText,
    IsOnScreen,
    NonEmptyAlt,
    NonEmptyIfPresent,
    NonEmptyTitle,
    NonEmptyValue,
    RoleNone,
    RolePresentation,
    CaptionFaked,
    HasCaption,
    HasSummary,
    HasTh,
    Html5Scope,
    SameCaptionSummary,
    ScopeValue,
    TdHasHeader,
    TdHeadersAttr,
    ThHasDataCells,
    HiddenContent
  }
}