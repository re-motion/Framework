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
  /// Rule IDs of aXe Core.
  /// </summary>
  /// <remarks>
  /// A list of all all rules can be found at <see href="https://github.com/dequelabs/axe-core/blob/master/doc/rule-descriptions.md"/>.
  /// </remarks>

  public enum AccessibilityRuleID
  {
    Unknown,

    /// <summary>
    /// Ensures every accesskey attribute value is unique.
    /// </summary>
    AccessKeys,

    /// <summary>
    /// Ensures &lt;area&gt; elements of image maps have alternate text.
    /// </summary>
    AreaAlt,

    /// <summary>
    /// Ensures ARIA attributes are allowed for an element's role.
    /// </summary>
    AriaAllowedAttr,

    /// <summary>
    /// Ensures role attribute has an appropriate value for the element.
    /// </summary>
    AriaAllowedRole,

    /// <summary>
    /// Ensure aria-braillelabel and aria-brailleroledescription have a non-braille equivalent.
    /// </summary>
    AriaBrailleEquivalent,

    /// <summary>
    /// Ensures every ARIA button, link and menuitem has an accessible name.
    /// </summary>
    AriaCommandName,

    /// <summary>
    /// Ensures ARIA attributes are used as described in the specification of the element's role.
    /// </summary>
    AriaConditionalAttr,

    /// <summary>
    /// Ensures elements do not use deprecated roles
    /// </summary>
    AriaDeprecatedRole,

    /// <summary>
    /// Ensures every ARIA dialog and alertdialog node has an accessible name.
    /// </summary>
    AriaDialogName,

    /// <summary>
    /// Ensures aria-hidden='true' is not present on the document body.
    /// </summary>
    AriaHiddenBody,

    /// <summary>
    /// Ensures aria-hidden elements do not contain focusable elements.
    /// </summary>
    AriaHiddenFocus,

    /// <summary>
    /// Ensures every ARIA input field has an accessible name.
    /// </summary>
    AriaInputFieldName,

    /// <summary>
    /// Ensures every ARIA meter node has an accessible name.
    /// </summary>
    AriaMeterName,

    /// <summary>
    /// Ensures every ARIA progressbar node has an accessible name.
    /// </summary>
    AriaProgressbarName,

    /// <summary>
    /// Ensures ARIA attributes are not prohibited for an element's role.
    /// </summary>
    AriaProhibitedAttr,

    /// <summary>
    /// Ensures elements with ARIA roles have all required ARIA attributes.
    /// </summary>
    AriaRequiredAttr,

    /// <summary>
    /// Ensures elements with an ARIA role that require child roles contain them.
    /// </summary>
    AriaRequiredChildren,

    /// <summary>
    /// Ensures elements with an ARIA role that require parent roles are contained by them.
    /// </summary>
    AriaRequiredParent,

    /// <summary>
    /// Ensures all elements with a role attribute use a valid value.
    /// </summary>
    AriaRoles,

    /// <summary>
    /// Ensures role="text" is used on elements with no focusable descendants.
    /// </summary>
    AriaText,

    /// <summary>
    /// Ensures every ARIA toggle field has an accessible name.
    /// </summary>
    AriaToggleFieldName,

    /// <summary>
    /// Ensures every ARIA tooltip node has an accessible name.
    /// </summary>
    AriaTooltipName,

    /// <summary>
    /// Ensures every ARIA treeitem node has an accessible name.
    /// </summary>
    AriaTreeitemName,

    /// <summary>
    /// Ensures all ARIA attributes have valid values.
    /// </summary>
    AriaValidAttrValue,

    /// <summary>
    ///  	Ensures attributes that begin with aria- are valid ARIA attributes.
    /// </summary>
    AriaValidAttr,

    /// <summary>
    /// Ensure the autocomplete attribute is correct and suitable for the form field.
    /// </summary>
    AutocompleteValid,

    /// <summary>
    /// Ensure that text spacing set through style attributes can be adjusted with custom stylesheets.
    /// </summary>
    AvoidInlineSpacing,

    /// <summary>
    /// Ensures &lt;blink&gt; elements are not used.
    /// </summary>
    Blink,

    /// <summary>
    /// Ensures buttons have discernible text.
    /// </summary>
    ButtonName,

    /// <summary>
    /// Ensures each page has at least one mechanism for a user to bypass navigation and jump straight to the content.
    /// </summary>
    Bypass,

    /// <summary>
    /// Ensures the contrast between foreground and background colors meets WCAG 2 AA contrast ratio thresholds.
    /// </summary>
    ColorContrast,

    /// <summary>
    /// Ensures the contrast between foreground and background colors meets WCAG 2 AAA enhanced contrast ratio thresholds.
    /// </summary>
    ColorContrastEnhanced,

    /// <summary>
    /// Ensures content is not locked to any specific display orientation, and the content is operable in all display orientations.
    /// </summary>
    CssOrientationLock,

    /// <summary>
    /// Ensures &lt;dl&gt; elements are structured correctly.
    /// </summary>
    DefinitionList,

    /// <summary>
    /// Ensures &lt;dt&gt; and &lt;dd&gt; elements are contained by a &lt;dl&gt;.
    /// </summary>
    Dlitem,

    /// <summary>
    /// Ensures each HTML document contains a non-empty &lt;title&gt; element.
    /// </summary>
    DocumentTitle,

    /// <summary>
    /// Ensures every id attribute value used in ARIA and in labels is unique.
    /// </summary>
    DuplicateIdAria,

    /// <summary>
    /// Ensures headings have discernible text.
    /// </summary>
    EmptyHeading,

    /// <summary>
    /// Ensures table headers have discernible text.
    /// </summary>
    EmptyTableHeader,

    /// <summary>
    /// Ensures elements in the focus order have an appropriate role.
    /// </summary>
    FocusOrderSemantics,

    /// <summary>
    /// Ensures form field does not have multiple label elements.
    /// </summary>
    FormFieldMultipleLabels,

    /// <summary>
    /// Ensures &lt;frame&gt; and &lt;iframe&gt; elements with focusable content do not have tabindex=-1.
    /// </summary>
    FrameFocusableContent,

    /// <summary>
    /// Ensures &lt;iframe&gt; and &lt;frame&gt; elements contain the axe-core script.
    /// </summary>
    FrameTested,

    /// <summary>
    /// Ensures &lt;iframe&gt; and &lt;frame&gt; elements contain a unique title attribute.
    /// </summary>
    FrameTitleUnique,

    /// <summary>
    /// Ensures &lt;iframe&gt; and &lt;frame&gt; elements contain a non-empty title attribute.
    /// </summary>
    FrameTitle,

    /// <summary>
    /// Ensures the order of headings is semantically correct.
    /// </summary>
    HeadingOrder,

    /// <summary>
    /// Informs users about hidden content.
    /// </summary>
    HiddenContent,

    /// <summary>
    /// Ensures every HTML document has a lang attribute.
    /// </summary>
    HtmlHasLang,

    /// <summary>
    /// Ensures the lang attribute of the &lt;html&gt; element has a valid value.
    /// </summary>
    HtmlLangValid,

    /// <summary>
    /// Ensure that HTML elements with both valid lang and xml:lang attributes agree on the base language of the page.
    /// </summary>
    HtmlXmlLangMismatch,

    /// <summary>
    /// Ensure that links with the same accessible name serve a similar purpose.
    /// </summary>
    IdenticalLinksSamePurpose,

    /// <summary>
    /// Ensures &lt;img&gt; elements have alternate text or a role of none or presentation.
    /// </summary>
    ImageAlt,

    /// <summary>
    /// Ensures image alternative is not repeated as text.
    /// </summary>
    ImageRedundantAlt,

    /// <summary>
    /// Ensures input buttons have discernible text.
    /// </summary>
    InputButtonName,

    /// <summary>
    /// Ensures &lt;input type="image"&gt; elements have alternate text.
    /// </summary>
    InputImageAlt,

    /// <summary>
    /// Ensures that elements labelled through their content must have their visible text as part of their accessible name.
    /// </summary>
    LabelContentNameMismatch,

    /// <summary>
    /// Ensures that every form element is not solely labeled using the title or aria-describedby attributes.
    /// </summary>
    LabelTitleOnly,

    /// <summary>
    /// Ensures every form element has a label.
    /// </summary>
    Label,

    /// <summary>
    /// Ensures the banner landmark is at top level.
    /// </summary>
    LandmarkBannerIsTopLevel,

    /// <summary>
    /// Ensures the complementary landmark or aside is at top level.
    /// </summary>
    LandmarkComplementaryIsTopLevel,

    /// <summary>
    /// Ensures the contentinfo landmark is at top level.
    /// </summary>
    LandmarkContentInfoIsTopLevel,

    /// <summary>
    /// Ensures the main landmark is at top level.
    /// </summary>
    LandmarkMainIsTopLevel,

    /// <summary>
    /// Ensures the document has at most one banner landmark.
    /// </summary>
    LandmarkNoDuplicateBanner,

    /// <summary>
    /// Ensures the document has at most one contentinfo landmark.
    /// </summary>
    LandmarkNoDuplicateContentInfo,

    /// <summary>
    /// Ensures the document has at most one main landmark.
    /// </summary>
    LandmarkNoDuplicateMain,

    /// <summary>
    /// Ensures the document has only one main landmark and each iframe in the page has at most one main landmark.
    /// </summary>
    LandmarkOneMain,

    /// <summary>
    /// Landmarks must have a unique role or role/label/title (i.e. accessible name) combination.
    /// </summary>
    LandmarkUnique,

    /// <summary>
    /// Links can be distinguished without relying on color.
    /// </summary>
    LinkInTextBlock,

    /// <summary>
    /// Ensures links have discernible text.
    /// </summary>
    LinkName,

    /// <summary>
    /// Ensures that lists are structured correctly.
    /// </summary>
    List,

    /// <summary>
    /// Ensures &lt;li&gt; elements are used semantically.
    /// </summary>
    ListItem,

    /// <summary>
    /// Ensures &lt;marquee&gt; elements are not used.
    /// </summary>
    Marquee,

    /// <summary>
    /// Ensures &lt;meta http-equiv="refresh"&gt; is not used.
    /// </summary>
    MetaRefresh,

    /// <summary>
    /// Ensures &lt;meta http-equiv="refresh"&gt; is not used for delayed refresh.
    /// </summary>
    MetaRefreshNoExceptions,

    /// <summary>
    /// Ensures &lt;meta name="viewport"&gt; can scale a significant amount.
    /// </summary>
    MetaViewportLarge,

    /// <summary>
    /// Ensures &lt;meta name="viewport"&gt; does not disable text scaling and zooming.
    /// </summary>
    MetaViewport,

    /// <summary>
    /// Ensures interactive controls are not nested as they are not always announced by screen readers or can cause focus problems for assistive technologies.
    /// </summary>
    NestedInteractive,

    /// <summary>
    /// Ensures &lt;video&gt; or &lt;audio&gt; elements do not autoplay audio for more than three seconds without a control mechanism to stop or mute the audio.
    /// </summary>
    NoAutoplayAudio,

    /// <summary>
    /// Ensures &lt;object&gt; elements have alternate text.
    /// </summary>
    ObjectAlt,

    /// <summary>
    /// Ensure p elements are not used to style headings.
    /// </summary>
    PAsHeading,

    /// <summary>
    /// Ensure that the page, or at least one of its frames contains a level-one heading.
    /// </summary>
    PageHasHeadingOne,

    /// <summary>
    /// Elements marked as presentational should not have global ARIA or tabindex to ensure all screen readers ignore them.
    /// </summary>
    PresentationRoleConflict,

    /// <summary>
    /// Ensures all page content is contained by landmarks.
    /// </summary>
    Region,

    /// <summary>
    /// Ensures [role='img'] elements have alternate text.
    /// </summary>
    RoleImgAlt,

    /// <summary>
    /// Ensures the scope attribute is used correctly on tables.
    /// </summary>
    ScopeAttrValid,

    /// <summary>
    /// Elements that have scrollable content should be accessible by keyboard.
    /// </summary>
    ScrollableRegionFocusable,

    /// <summary>
    /// Ensures select element has an accessible name.
    /// </summary>
    SelectName,

    /// <summary>
    /// Ensures that server-side image maps are not used.
    /// </summary>
    ServerSideImageMap,

    /// <summary>
    /// Ensure all skip links have a focusable target.
    /// </summary>
    SkipLink,

    /// <summary>
    /// Ensures SVG elements with an &lt;img&gt;, graphics-document or graphics-symbol role have an accessible text alternative.
    /// </summary>
    SvgImageAlt,

    /// <summary>
    /// Ensures tabindex attribute values are not greater than 0.
    /// </summary>
    Tabindex,

    /// <summary>
    /// Ensure that tables do not have the same summary and caption.
    /// </summary>
    TableDuplicateName,

    /// <summary>
    /// Ensure that tables with a caption use the &lt;caption&gt; element.
    /// </summary>
    TableFakeCaption,

    /// <summary>
    /// Ensure touch target have sufficient size and space.
    /// </summary>
    /// <remarks>
    /// This rule is a wcag2.2 rule, which as of aXe core 4.8.1 (remotion 6.0.0-alpha.3) are not enabled by default.
    /// </remarks>
    TargetSize,

    /// <summary>
    /// Ensure that each non-empty data cell in a large table has one or more table headers.
    /// </summary>
    TdHasHeader,

    /// <summary>
    /// Ensure that each cell in a table using the headers refers to another cell in that table.
    /// </summary>
    TdHeadersAttr,

    /// <summary>
    /// Ensure that each table header in a data table refers to data cells.
    /// </summary>
    ThHasDataCells,

    /// <summary>
    /// Ensures lang attributes have valid values.
    /// </summary>
    ValidLang,

    /// <summary>
    /// Ensures &lt;video&gt; elements have captions.
    /// </summary>
    VideoCaption
  }
}
