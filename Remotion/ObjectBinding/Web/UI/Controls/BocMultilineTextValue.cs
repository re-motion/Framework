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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> This control can be used to display or edit a list of strings. </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/Class/*' />
  [ValidationProperty ("Text")]
  [DefaultEvent ("TextChanged")]
  [ToolboxItemFilter ("System.Web.UI")]
  public class BocMultilineTextValue : BocTextValueBase, IBocMultilineTextValue
  {
    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocMultilineTextValue")]
    public enum ResourceIdentifier
    {
      /// <summary> The validation error message displayed when no text is entered but input is required. </summary>
      RequiredValidationMessage,
      /// <summary> The validation error message displayed when entered text exceeds the maximum length. </summary>
      MaxLengthValidationMessage,
      /// <summary> The validation error message displayed when the text contains invalid characters. </summary>
      InvalidCharactersErrorMessage
    }

    // static members

    private static readonly Type[] s_supportedPropertyInterfaces = new[] { typeof(IBusinessObjectStringProperty) };

    private string[]? _text = null;
    private string? _errorMessage;
    private ReadOnlyCollection<BaseValidator>? _validators;

    // construction and disposing

    public BocMultilineTextValue ()
        : base(BocTextBoxMode.MultiLine)
    {
    }

    // methods and properties

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      base.RegisterHtmlHeadContents(htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender, this.TextBoxStyle);
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (interim)
        return;

      if (Property == null)
        return;

      if (DataSource == null)
        return;

      string[]? value = null;

      if (DataSource.BusinessObject != null)
        value = (string[]?)DataSource.BusinessObject.GetProperty(Property);

      LoadValueInternal(value, false);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> The <see cref="String"/> <see cref="Array"/> to load or <see langword="null"/>. </param>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (string[] value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (string[]? value, bool interim)
    {
      if (interim)
        return;

      SetValue(value);
      IsDirty = false;
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocMultilineTextValue.xml' path='BocMultilineTextValue/SaveValue/*' />
    public override bool SaveValue (bool interim)
    {
      if (interim)
        return false;

      bool isValid = Validate();
      if (!isValid)
        return false;

      if (!IsDirty)
        return true;

      if (SaveValueToDomainModel())
      {
        IsDirty = false;
        return true;
      }
      return false;
    }

    protected override IBusinessObjectConstraintVisitor CreateBusinessObjectConstraintVisitor ()
    {
      return new BocMultilineTextValueConstraintVisitor(this);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer));
    }

    protected virtual IBocMultilineTextValueRenderer CreateRenderer ()
    {
      return ServiceLocator.GetInstance<IBocMultilineTextValueRenderer>();
    }

    protected virtual BocMultilineTextValueRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      Assertion.IsNotNull(Context, "Context must not be null.");

      return new BocMultilineTextValueRenderingContext(Context, writer, this);
    }

    /// <summary> Gets or sets the <see cref="IBusinessObjectStringProperty"/> object this control is bound to. </summary>
    /// <value> An <see cref="IBusinessObjectStringProperty"/> object. </value>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public new IBusinessObjectStringProperty? Property
    {
      get { return (IBusinessObjectStringProperty?)base.Property; }
      set { base.Property = value; }
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> The <see cref="String"/> array currently displayed or <see langword="null"/> if no text is entered. </value>
    /// <remarks> The dirty state is reset when the value is set. </remarks>
    [Browsable (false)]
    public new string[]? Value
    {
      get
      {
        return GetValue();
      }
      set
      {
        IsDirty = true;

        SetValue(value);
      }
    }

    /// <summary> Gets or sets the string representation of the current value. </summary>
    /// <remarks> Uses <c>\r\n</c> or <c>\n</c> as separation characters. The default value is an empty <see cref="String"/>. </remarks>
    [Description ("The string representation of the current value.")]
    [Category ("Data")]
    [DefaultValue ("")]
    public override sealed string Text
    {
      get { return string.Join("\r\n", _text ?? Enumerable.Empty<string>()); }
      set
      {
        IsDirty = true;
        if (value == null)
          _text = null;
        else
          _text = StringUtility.ParseNewLineSeparatedString(value).ToArray();
      }
    }

    /// <summary> Gets or sets the validation error message. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description ("Validation message displayed if there is an error.")]
    [Category ("Validator")]
    [DefaultValue ("")]
    public string? ErrorMessage
    {
      get { return _errorMessage; }
      set
      {
        _errorMessage = value;

        UpdateValidtaorErrorMessages<RequiredFieldValidator>(_errorMessage);
        UpdateValidtaorErrorMessages<LengthValidator>(_errorMessage);
      }
    }

    /// <summary>
    /// Gets the value from the backing field.
    /// </summary>
    /// <remarks>Override this member to modify the storage of the value. </remarks>
    protected virtual string[]? GetValue ()
    {
      if (_text == null)
        return null;

      var temp = new List<string>(_text.SkipWhile(string.IsNullOrWhiteSpace));

      if (temp.Count > 0)
        temp[0] = temp[0].TrimStart();

      while (temp.Count > 0 && string.IsNullOrWhiteSpace(temp[temp.Count - 1]))
        temp.RemoveAt(temp.Count - 1);

      if (temp.Count > 0)
        temp[temp.Count - 1] = temp[temp.Count - 1].TrimEnd();

      if (temp.Count > 0)
        return temp.ToArray();

      return null;
    }

    /// <summary>
    /// Sets the value from the backing field.
    /// </summary>
    /// <remarks>
    /// <para>Setting the value via this method does not affect the control's dirty state.</para>
    /// <para>Override this member to modify the storage of the value.</para>
    /// </remarks>
    protected virtual void SetValue (string[]? value)
    {
      _text = value;
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    /// <value> The value must be of type <b>string[]</b>. </value>
    protected override sealed object? ValueImplementation
    {
      get { return Value; }
      set { Value = ArgumentUtility.CheckType<string[]>("value", value); }
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocMultilineTextValue"/> contains a value. </summary>
    public override bool HasValue
    {
      get { return _text != null && _text.Any(s => !string.IsNullOrWhiteSpace(s)); }
    }

    /// <summary>
    /// Loads <see cref="Text"/> in addition to the base state.
    /// </summary>
    /// <param name="savedState">The state object created by <see cref="SaveControlState"/>.</param>
    protected override void LoadControlState (object? savedState)
    {
      object?[] values = (object?[])savedState!;

      base.LoadControlState(values[0]);
      _text = (string[]?)values[1];
    }

    /// <summary>
    /// Saves <see cref="Text"/> in addition to the base state.
    /// </summary>
    /// <returns>An object containing the state to be loaded in the control's next lifecycle.</returns>
    protected override object SaveControlState ()
    {
      object?[] values = new object?[2];

      values[0] = base.SaveControlState();
      values[1] = _text;

      return values;
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    public override IResourceManager GetResourceManager ()
    {
      return GetResourceManager(typeof(ResourceIdentifier));
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      base.LoadResources(resourceManager, globalizationService);

      //  Dispatch simple properties
      string? key = ResourceManagerUtility.GetGlobalResourceKey(ErrorMessage);
      if (! string.IsNullOrEmpty(key))
        ErrorMessage = resourceManager.GetString(key);
    }

    [Obsolete ("For DependDB only.", true)]
    private new BaseValidator[] CreateValidators ()
    {
      throw new NotImplementedException("For DependDB only.");
    }

    /// <summary>
    /// If applicable, validators for non-empty and maximum length are created.
    /// </summary>
    /// <param name="isReadOnly">
    /// This flag is initialized with the value of <see cref="BusinessObjectBoundEditableWebControl.IsReadOnly"/>. 
    /// Implemantations should consider whether they require a validator also when the control is rendered as read-only.
    /// </param>
    /// <returns>An enumeration of all applicable validators.</returns>
    /// <remarks>
    ///   <list type="bullet">
    ///     <item>
    ///       If the control requires input, a <see cref="RequiredFieldValidator"/> is generated.
    ///     </item>
    ///     <item>
    ///       If a maximum length is specified, a <see cref="LengthValidator"/> is generated.
    ///     </item>
    ///   </list>
    /// </remarks>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.CreateValidators()">BusinessObjectBoundEditableWebControl.CreateValidators()</seealso>
    protected override IEnumerable<BaseValidator> CreateValidators (bool isReadOnly)
    {
      var validatorFactory = ServiceLocator.GetInstance<IBocMultilineTextValueValidatorFactory>();
      _validators = validatorFactory.CreateValidators(this, isReadOnly).ToList().AsReadOnly();

      OverrideValidatorErrorMessages();

      return _validators;
    }

    private void OverrideValidatorErrorMessages ()
    {
      if (string.IsNullOrEmpty(_errorMessage))
        return;

      UpdateValidtaorErrorMessages<RequiredFieldValidator>(_errorMessage);
      UpdateValidtaorErrorMessages<LengthValidator>(_errorMessage);
    }

    private void UpdateValidtaorErrorMessages<T> (string? errorMessage) where T : BaseValidator
    {
      var validator = _validators.GetValidator<T>();
      if (validator != null)
        validator.ErrorMessage = errorMessage!;
    }

    /// <summary>
    ///   The <see cref="BocMultilineTextValue"/> supports properties of types <see cref="IBusinessObjectStringProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary> The <see cref="BocMultilineTextValue"/> supports only list properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="true"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return isList;
    }

    protected override string ControlType
    {
      get { return "BocMultilineTextValue"; }
    }
  }
}
