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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  ///   This interfaces declares advanced properties and methods for data-aware web controls.
  ///   <seealso cref="Remotion.Web.UI.Controls.FormGridManager"/>
  /// </summary>
  public interface ISmartControl : IControl
  {
    /// <summary>
    ///   Specifies whether the control must be filled out by the user before submitting the form.
    /// </summary>
    bool IsRequired { get; }

    /// <summary>
    ///   Gets an instance of the <see cref="HelpInfo"/> type, which contains all information needed for rendering a help-link.
    /// </summary>
    HelpInfo? HelpInfo { get; }

    /// <summary>
    ///   Creates an appropriate validator for this control.
    /// </summary>
    IEnumerable<BaseValidator> CreateValidators ();

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its ClientID.
    /// </summary>
    /// <remarks>
    ///   For compound controls that accept user input in text boxes, lists etc., this is the control that
    ///   actually accepts user input. For all other controls, this is the control itself.
    /// </remarks>
    Control TargetControl { get; }

    /// <summary>
    ///   If UseLabel is true, it is valid to generate HTML &lt;label&gt; tags referencing <see cref="TargetControl"/>.
    /// </summary>
    /// <remarks>
    ///   This flag is usually true, except for controls that render combo boxes or other HTML tags that do not function properly
    ///   with labels. This flag has been introduced due to a bug in Microsoft Internet Explorer.
    /// </remarks>
    bool UseLabel { get; }

    /// <summary>
    /// Assigns a sequence of <paramref name="labelIDs"/> for this <see cref="ISmartControl"/>.
    /// </summary>
    void AssignLabels (IEnumerable<string> labelIDs);

    //  /// <summary>
    //  ///   If UseInputControlCSS is true, the control requires special formatting.
    //  /// </summary>
    //  /// <remarks>
    //  ///   This flag should be true for controls rendering &lt;input&gt; or &lt;textarea&gt; elements.
    //  ///   The reason for this is in excentric application of CSS-classes to these elements via
    //  ///   the definition of global styles (input {...} and textarea {...}). The most predictable result
    //  ///   is acchivied by directly assigning the class instead of using the global definitions.
    //  /// </remarks>
    //  bool UseInputControlCSS { get; }

    /// <summary> Gets the text to be written into the label for this control. </summary>
    WebString DisplayName { get; }

    /// <summary>Regsiteres stylesheet and script files with the <see cref="HtmlHeadAppender"/>.</summary>
    void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender);
  }

}
