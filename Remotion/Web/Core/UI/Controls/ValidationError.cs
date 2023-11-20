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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  ///   Encapsulats an validation error: the effected control, the message and the validator used.
  /// </summary>
  public class ValidationError
  {
    // constants

    // types

    // static members

    // member fields

    /// <summary> The control with an invalid state. </summary>
    private readonly Control? _validatedControl;

    private readonly ControlCollection? _labels;

    /// <summary> The message to be displayed to the user. </summary>
    private readonly PlainTextString _validationMessage;

    /// <summary> The validator used to validate the <see cref="_validatedControl"/>. </summary>
    private readonly IValidator? _validator;

    // construction and disposing

    /// <summary>
    ///   Initializes a new instance of the <see cref="ValidationError"/> class with the
    ///   <see cref="Control"/> containing invalid data and the <see cref=" IValidator"/>
    ///   used to identify the error.
    /// </summary>
    /// <overload> Overloaded. </overload>
    /// <param name="validatedControl"> The control with an invalid state. </param>
    /// <param name="validator"> The validator used to validate the <paramref name="validatedControl"/>.  Must not be <see langword="null"/>. </param>
    /// <param name="labels">The labels containing the control's headings.</param>
    public ValidationError (Control? validatedControl, IValidator validator, ControlCollection? labels)
    {
      ArgumentUtility.CheckNotNull("validator", validator);

      _validatedControl = validatedControl;
      _validationMessage = PlainTextString.Empty;
      _validator = validator;
      _labels = labels;
    }

    public ValidationError (Control validatedControl, IValidator validator)
        : this(validatedControl, validator, null)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ValidationError"/> class with the
    ///   <see cref="Control"/> containing invalid data and the message describing the error.
    /// </summary>
    /// <overload> Overloaded. </overload>
    /// <param name="validatedControl"> The control with an invalid state. </param>
    /// <param name="validationMessage"> The message to be displayed to the user. Must not be <see langword="null"/> or empty. </param>
    /// <param name="labels">The labels containing the control's headings.</param>
    public ValidationError (Control validatedControl, PlainTextString validationMessage, ControlCollection? labels)
    {
      ArgumentUtility.CheckNotNullOrEmpty("validationMessage", validationMessage.GetValue());

      _validatedControl = validatedControl;
      _validationMessage = validationMessage;
      _validator = null;
      _labels = labels;
    }

    public ValidationError (Control validatedControl, PlainTextString validationMessage)
        : this(validatedControl, validationMessage, null)
    {
    }

    // methods and properties

    /// <summary> Gets the control with an invalid state. </summary>
    /// <value> The validated <see cref="Control"/>. </value>
    public Control? ValidatedControl
    {
      get { return _validatedControl; }
    }

    public ControlCollection? Labels
    {
      get { return _labels; }
    }

    /// <summary> The message to be displayed to the user. </summary>
    /// <value> A string containing the message. </value>
    public PlainTextString ValidationMessage
    {
      get
      {
        if (_validationMessage.IsEmpty)
          return PlainTextString.CreateFromText(_validator!.ErrorMessage);
        else
          return _validationMessage;
      }
    }

    /// <summary> Gets the validator used to validate the <see cref="ValidatedControl"/>. </summary>
    /// <value> A <see cref="IValidator"/> instance or <see langname="null" />. </value>
    public IValidator? Validator
    {
      get { return _validator; }
    }

    /// <summary>
    ///   Formats the <c>ValidationError</c> as a <see cref="Label"/>
    ///   and associates the <see cref="ValidatedControl"/> with it.
    /// </summary>
    /// <remarks>
    ///   Be aware that the Internet Explorer resets DropDown controls
    ///   when jumped at through a label and does not create a postback event.
    /// </remarks>
    /// <param name="cssClass"> The name of the CSS-class used to format the label. </param>
    /// <returns> A <see cref="Label"/>. </returns>
    public HtmlControl ToLabel (string cssClass)
    {
      HtmlGenericControl label = new HtmlGenericControl("label");
      label.EnableViewState = false;

      label.Controls.Add(ToSpan());
      if (_validatedControl != null)
        label.Attributes["for"] = _validatedControl.ClientID;

      if (!string.IsNullOrEmpty(cssClass))
        label.Attributes["class"] = cssClass;

      return label;
    }

    /// <summary>
    ///   Formats the <c>ValidationError</c> as a <see cref="HyperLink"/>
    ///   and references the <see cref="ValidatedControl"/> through an in-page link.
    /// </summary>
    /// <param name="cssClass"> The name of the CSS-class used to format the hyperlink. </param>
    /// <returns> A <see cref="HyperLink"/>. </returns>
    public HyperLink ToHyperLink (string cssClass)
    {
      HyperLink hyperLink = new HyperLink();
      hyperLink.EnableViewState = false;

      hyperLink.Controls.Add(ToSpan());
      if (_validatedControl != null)
        hyperLink.Attributes.Add("href", "#" + _validatedControl.ClientID);

      if (!string.IsNullOrEmpty(cssClass))
        hyperLink.CssClass = cssClass;

      return hyperLink;
    }

    /// <summary>
    ///   Places the <c>ValidationError</c>'s message into an <see cref="HtmlGenericControl"/> with <b>div</b> tags.
    /// </summary>
    /// <param name="cssClass"> The name of the CSS-class used to format the <c>div</c>-tag. </param>
    /// <returns> A <see cref="HtmlGenericControl"/>. </returns>
    public HtmlControl ToDiv (string? cssClass = null)
    {
      return ToGenericControl(cssClass, "div");
    }

    /// <summary>
    ///   Places the <c>ValidationError</c>'s message into an <see cref="HtmlGenericControl"/> with <b>span</b> tags.
    /// </summary>
    /// <param name="cssClass"> The name of the CSS-class used to format the <c>span</c>-tag. </param>
    /// <returns> A <see cref="HtmlGenericControl"/>. </returns>
    public HtmlControl ToSpan (string? cssClass = null)
    {
      return ToGenericControl(cssClass, "span");
    }

    /// <summary>
    ///   Places the <c>ValidationError</c>'s message into a which ever HTML tag is provided
    ///   and returns this construct as a <see cref="HtmlGenericControl"/>.
    /// </summary>
    /// <param name="cssClass"> The name of the CSS-class used to format the HTML tag. </param>
    /// <param name="tag"> The HTML tag to be used. </param>
    /// <returns> A <see cref="HtmlGenericControl"/>. </returns>
    private HtmlControl ToGenericControl (string? cssClass, string tag)
    {
      HtmlControl control;
      if (_validator is ILazyEvaluatedValidator)
        control = new LazyEvaluatedValidationMessageControl(tag, _validator);
      else
        control = new HtmlGenericControl(tag) { InnerHtml = ValidationMessage.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks) };

      control.EnableViewState = false;
      if (!string.IsNullOrEmpty(cssClass))
        control.Attributes["class"] = cssClass;

      return control;
    }
  }
}
