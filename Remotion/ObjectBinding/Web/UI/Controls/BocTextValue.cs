// this file is part of the re-motion core framework (www.re-motion.org)
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
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Globalization;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> This control can be used to display or edit values that can be edited in a text box. </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/Class/*' />
  public class BocTextValue : BocTextValueBase, IBocTextValue
  {
    //  statics

    private static readonly Type[] s_supportedPropertyInterfaces = new[]
                                                                   {
                                                                       typeof(IBusinessObjectNumericProperty),
                                                                       typeof(IBusinessObjectStringProperty),
                                                                       typeof(IBusinessObjectDateTimeProperty)
                                                                   };

    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.ObjectBinding.Web.Globalization.BocTextValue")]
    public enum ResourceIdentifier
    {
      /// <summary> The validation error message displayed when no text is entered but input is required. </summary>
      RequiredErrorMessage,
      /// <summary> The validation error message displayed when the text exceeds the maximum length. </summary>
      MaxLengthValidationMessage,
      /// <summary> The validation error message displayed when the text is no valid date/time value. </summary>
      InvalidDateAndTimeErrorMessage,
      /// <summary> The validation error message displayed when the text is no valid date value. </summary>
      InvalidDateErrorMessage,
      /// <summary> The validation error message displayed when the text is no valid integer. </summary>
      InvalidIntegerErrorMessage,
      /// <summary> The validation error message displayed when the text is no valid integer. </summary>
      InvalidDoubleErrorMessage,
      /// <summary> The validation error message displayed when the singleline text contains invalid characters. </summary>
      InvalidCharactersForSingleLineErrorMessage,
      /// <summary> The validation error message displayed when the multiline text contains invalid characters. </summary>
      InvalidCharactersForMultiLineErrorMessage
    }

    // fields

    private BocTextValueType _valueType = BocTextValueType.Undefined;
    private BocTextValueType _actualValueType = BocTextValueType.Undefined;

    private string? _format;
    private string? _text = string.Empty;
    private PlainTextString _errorMessage;
    private ReadOnlyCollection<BaseValidator>? _validators;

    public BocTextValue ()
    {
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (interim)
        return;

      if (Property == null)
        return;

      if (DataSource == null)
        return;

      object? value = null;

      if (DataSource.BusinessObject != null)
        value = DataSource.BusinessObject.GetProperty(Property);

      LoadValueInternal(value, false);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> A <see cref="String"/> to load or <see langword="null"/>. </param>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (string? value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> The <see cref="Int32"/> value to load or <see langword="null"/>. </param>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (int? value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> The <see cref="Double"/> value to load or <see langword="null"/>. </param>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (double? value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> The <see cref="DateTime"/> value to load or <see langword="null"/>. </param>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (DateTime? value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="O:Remotion.ObjectBinding.Web.UI.Controls.BocTextValue.LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (object? value, bool interim)
    {
      if (interim)
        return;

      SetValue(value);
      IsDirty = false;
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocTextValue.xml' path='BocTextValue/SaveValue/*' />
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
      return new BocTextValueConstraintVisitor(this);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer));
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> 
    ///   <para>
    ///     The value has the type specified in the <see cref="ValueType"/> property (<see cref="String"/>, 
    ///     <see cref="Int32"/>, <see cref="Double"/> or <see cref="DateTime"/>). If <see cref="ValueType"/> is not
    ///     set, the type is determined by the bound <see cref="BusinessObjectBoundWebControl.Property"/>.
    ///   </para><para>
    ///     Returns <see langword="null"/> if <see cref="Text"/> is an empty <see cref="String"/>.
    ///   </para>
    /// </value>
    /// <remarks> The dirty state is reset when the value is set. </remarks>
    /// <exception cref="FormatException"> 
    ///   The value of the <see cref="Text"/> property cannot be converted to the specified <see cref="ValueType"/>.
    /// </exception>
    [Description("Gets or sets the current value.")]
    [Browsable(false)]
    public new object? Value
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
    /// <value> 
    ///   An empty <see cref="String"/> if the control's value is <see langword="null"/> or empty. 
    ///   The default value is an empty <see cref="String"/>. 
    /// </value>
    [Description("Gets or sets the string representation of the current value.")]
    [Category("Data")]
    [DefaultValue("")]
    public override sealed string Text
    {
      get { return _text ?? string.Empty; }
      set
      {
        IsDirty = true;
        _text = value;
      }
    }

    /// <summary>
    /// Gets the value from the backing field.
    /// </summary>
    /// <remarks>Override this member to modify the storage of the value. </remarks>
    protected virtual object? GetValue ()
    {
      string? text = _text;
      if (text != null)
        text = text.Trim();

      if (string.IsNullOrEmpty(text))
        return null;

      var valueType = ActualValueType;
      switch (valueType)
      {
        case BocTextValueType.String:
          return text;

        case BocTextValueType.Byte:
          return byte.Parse(text, GetNumberStyle(valueType));

        case BocTextValueType.Int16:
          return short.Parse(text, GetNumberStyle(valueType));

        case BocTextValueType.Int32:
          return int.Parse(text, GetNumberStyle(valueType));

        case BocTextValueType.Int64:
          return long.Parse(text, GetNumberStyle(valueType));

        case BocTextValueType.Date:
          return DateTime.Parse(text).Date;

        case BocTextValueType.DateTime:
          return DateTime.Parse(text);

        case BocTextValueType.Decimal:
          return decimal.Parse(text, GetNumberStyle(valueType));

        case BocTextValueType.Double:
          return double.Parse(text, GetNumberStyle(valueType));

        case BocTextValueType.Single:
          return float.Parse(text, GetNumberStyle(valueType));
      }
      return text;
    }

    /// <summary>
    /// Sets the value from the backing field.
    /// </summary>
    /// <remarks>
    /// <para>Setting the value via this method does not affect the control's dirty state.</para>
    /// <para>Override this member to modify the storage of the value.</para>
    /// </remarks>
    protected virtual void SetValue (object? value)
    {
      if (value == null)
      {
        _text = null;
        return;
      }

      IFormattable? formattable = value as IFormattable;
      if (formattable != null)
      {
        string? format = Format;
        if (format == null)
        {
          if (ActualValueType == BocTextValueType.Date)
            format = "d";
          else if (ActualValueType == BocTextValueType.DateTime)
            format = "g";
        }
        _text = formattable.ToString(format, null);
      }
      else
        _text = value.ToString();
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    protected override sealed object? ValueImplementation
    {
      get { return Value; }
      set { Value = value; }
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocTextValue"/> contains a value. </summary>
    public override bool HasValue
    {
      get { return !string.IsNullOrWhiteSpace(_text); }
    }

    /// <summary>
    ///   Gets a flag describing whether it is save (i.e. accessing <see cref="Value"/> does not throw a 
    ///   <see cref="FormatException"/> or <see cref="OverflowException"/>) to read the contents of <see cref="Value"/>.
    /// </summary>
    /// <remarks> Valid values include <see langword="null"/>. </remarks>
    [Browsable(false)]
    public bool IsValidValue
    {
      get
      {
        try
        {
          //  Force the evaluation of Value
          if (GetValue() != null)
            return true;
        }
        catch (FormatException)
        {
          return false;
        }
        catch (OverflowException)
        {
          return false;
        }

        return true;
      }
    }

    /// <summary> Gets or sets the <see cref="BocTextValueType"/> assigned from an external source. </summary>
    /// <value> 
    ///   The externally set <see cref="BocTextValueType"/>. The default value is 
    ///   <see cref="BocTextValueType.Undefined"/>. 
    /// </value>
    [Description("Gets or sets a fixed value type.")]
    [Category("Data")]
    [DefaultValue(BocTextValueType.Undefined)]
    public BocTextValueType ValueType
    {
      get { return _valueType; }
      set
      {
        if (_valueType != value)
        {
          _valueType = value;
          _actualValueType = value;
          if (_valueType != BocTextValueType.Undefined)
            _text = null;
        }
      }
    }

    /// <summary>
    ///   Gets the controls fixed <see cref="ValueType"/> or, if <see cref="BocTextValueType.Undefined"/>, 
    ///   the <see cref="BusinessObjectBoundWebControl.Property"/>'s value type.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public BocTextValueType ActualValueType
    {
      get
      {
        if (_valueType == BocTextValueType.Undefined && Property != null)
          _actualValueType = GetBocTextValueType(Property);
        return _actualValueType;
      }
    }

    /// <summary> Gets or sets the format string used to create the string value.  </summary>
    /// <value> 
    ///   A string passed to the <b>ToString</b> method of the object returned by <see cref="Value"/>.
    ///   The default value is an empty <see cref="String"/>. 
    /// </value>
    /// <remarks>
    ///   <see cref="IFormattable"/> is used to format the value using this string. The default is "d" for date-only
    ///   values and "g" for date/time values (use "G" to display seconds too). 
    /// </remarks>
    [Description("Gets or sets the format string used to create the string value. " +
                  "Format must be parsable by the value's type if the control is in edit mode.")]
    [Category("Style")]
    [DefaultValue("")]
    public string? Format
    {
      get { return StringUtility.EmptyToNull(_format); }
      set { _format = value; }
    }

    /// <summary> Gets or sets the validation error message. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("Validation message displayed if there is an error.")]
    [Category("Validator")]
    [DefaultValue(typeof(PlainTextString), "")]
    public PlainTextString ErrorMessage
    {
      get { return _errorMessage; }
      set
      {
        _errorMessage = value;

        UpdateValidatorErrorMessages<RequiredFieldValidator>(_errorMessage);
        UpdateValidatorErrorMessages<LengthValidator>(_errorMessage);

        UpdateValidatorErrorMessages<DateTimeValidator>(_errorMessage);
        UpdateValidatorErrorMessages<CompareValidator>(_errorMessage);
        UpdateValidatorErrorMessages<NumericValidator>(_errorMessage);
      }
    }

    /// <summary>
    /// Adds a listener to the <see cref="BusinessObjectBinding.BindingChanged"/> event of the <see cref="BusinessObjectBoundWebControl.Binding"/>. 
    /// </summary>
    /// <param name="e">ignored</param>
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      Binding.BindingChanged += Binding_BindingChanged;
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      base.RegisterHtmlHeadContents(htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender, TextBoxStyle);
    }

    protected virtual IBocTextValueRenderer CreateRenderer ()
    {
      return ServiceLocator.GetInstance<IBocTextValueRenderer>();
    }

    protected virtual BocTextValueRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      Assertion.IsNotNull(Context, "Context must not be null.");

      return new BocTextValueRenderingContext(Context, writer, this);
    }

    /// <summary>
    /// Loads <see cref="Text"/>, <see cref="ValueType"/> and <see cref="ActualValueType"/> in addition to the base state.
    /// </summary>
    /// <param name="savedState">The control state object created by <see cref="SaveControlState"/>.</param>
    protected override void LoadControlState (object? savedState)
    {
      object?[] values = (object?[])savedState!;
      base.LoadControlState(values[0]);
      _text = (string)values[1]!;
      _valueType = (BocTextValueType)values[2]!;
      _actualValueType = (BocTextValueType)values[3]!;
    }

    /// <summary>
    /// Saves <see cref="Text"/>, <see cref="ValueType"/> and <see cref="ActualValueType"/> in addition to the base state.
    /// </summary>
    /// <returns>An object containing the state to be loaded in the next lifecycle.</returns>
    protected override object SaveControlState ()
    {
      object?[] values = new object?[4];
      values[0] = base.SaveControlState();
      values[1] = _text;
      values[2] = _valueType;
      values[3] = _actualValueType;
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
      string? key = ResourceManagerUtility.GetGlobalResourceKey(ErrorMessage.GetValue());
      if (! string.IsNullOrEmpty(key))
        ErrorMessage = resourceManager.GetText(key);
    }

    [Obsolete("For DependDB only.", true)]
    private new BaseValidator[] CreateValidators ()
    {
      throw new NotImplementedException("For DependDB only.");
    }

    /// <summary>
    /// If applicable, validators for non-empty, maximum length and input format are created.
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
    ///     <item>
    ///       If the <see cref="ActualValueType"/> is set to <see cref="BocTextValueType.DateTime"/>,
    ///       a <see cref="DateTimeValidator"/> is generated.
    ///     </item>
    ///     <item>
    ///       If the <see cref="ActualValueType"/> is set to <see cref="BocTextValueType.Date"/>
    ///       a <see cref="CompareValidator"/> is generated.
    ///     </item>
    ///     <item>
    ///       If the <see cref="ActualValueType"/> is set to one of the numeric types (such as <see cref="BocTextValueType.Int32"/> or
    ///       <see cref="BocTextValueType.Double"/>) a <see cref="NumericValidator"/> is generated.
    ///     </item>
    ///   </list>
    /// </remarks>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.CreateValidators()">BusinessObjectBoundEditableWebControl.CreateValidators()</seealso>
    protected override IEnumerable<BaseValidator> CreateValidators (bool isReadOnly)
    {
      var validatorFactory = ServiceLocator.GetInstance<IBocTextValueValidatorFactory>();
      _validators = validatorFactory.CreateValidators(this,isReadOnly).ToList().AsReadOnly();

      OverrideValidatorErrorMessages();

      return _validators;
    }

    private void OverrideValidatorErrorMessages ()
    {

      if (_errorMessage.IsEmpty)
        return;

      UpdateValidatorErrorMessages<RequiredFieldValidator>(_errorMessage);
      UpdateValidatorErrorMessages<LengthValidator>(_errorMessage);

      UpdateValidatorErrorMessages<DateTimeValidator>(_errorMessage);
      UpdateValidatorErrorMessages<CompareValidator>(_errorMessage);
      UpdateValidatorErrorMessages<NumericValidator>(_errorMessage);
    }

    private void UpdateValidatorErrorMessages<T> (PlainTextString errorMessage) where T : BaseValidator
    {
      var validator = _validators.GetValidator<T>();
      if (validator != null)
        validator.ErrorMessage = errorMessage.GetValue();
    }

    /// <summary> The <see cref="BocTextValue"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return ! isList;
    }

    /// <summary>
    ///   The <see cref="BocTextValue"/> supports properties of types <see cref="IBusinessObjectStringProperty"/>,
    ///   <see cref="IBusinessObjectDateTimeProperty"/>, and <see cref="IBusinessObjectNumericProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    private NumberStyles GetNumberStyle (BocTextValueType valueType)
    {
      switch (valueType)
      {
        case BocTextValueType.Byte:
        case BocTextValueType.Int16:
        case BocTextValueType.Int32:
        case BocTextValueType.Int64:
          return NumberStyles.Number & ~NumberStyles.AllowDecimalPoint;

        case BocTextValueType.Decimal:
          return NumberStyles.Number;

        case BocTextValueType.Double:
        case BocTextValueType.Single:
          return NumberStyles.Number | NumberStyles.AllowExponent;

        default:
          throw new ArgumentOutOfRangeException("valueType", valueType, "Only numeric value types are supported.");
      }
    }

    /// <summary> Handles refreshing the bound control. </summary>
    /// <param name="sender"> The source of the event. </param>
    /// <param name="e"> An <see cref="EventArgs"/> object that contains the event data. </param>
    private void Binding_BindingChanged (object? sender, EventArgs e)
    {
      RefreshPropertiesFromObjectModel();
    }

    /// <summary>
    ///   Refreshes all properties of <see cref="BocTextValue"/> that depend on the current value of 
    ///   <see cref="BusinessObjectBoundWebControl.Property"/>.
    /// </summary>
    private void RefreshPropertiesFromObjectModel ()
    {
      if (Property != null)
      {
        if (_valueType == BocTextValueType.Undefined)
          _actualValueType = GetBocTextValueType(Property);

        IBusinessObjectStringProperty? stringProperty = Property as IBusinessObjectStringProperty;
        if (stringProperty != null)
          TextBoxStyle.MaxLengthFromDomainModel = stringProperty.MaxLength;
      }
    }

    /// <summary> Returns the proper <see cref="BocTextValueType"/> for the passed <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to analyze. </param>
    /// <exception cref="NotSupportedException"> The specialized type of the <paremref name="property"/> is not supported. </exception>
    private BocTextValueType GetBocTextValueType (IBusinessObjectProperty property)
    {
      if (property is IBusinessObjectStringProperty)
        return BocTextValueType.String;
      else if (property is IBusinessObjectNumericProperty)
      {
        IBusinessObjectNumericProperty numericProperty = (IBusinessObjectNumericProperty)property;
        if (numericProperty.Type == typeof(byte))
          return BocTextValueType.Byte;
        else if (numericProperty.Type == typeof(decimal))
          return BocTextValueType.Decimal;
        else if (numericProperty.Type == typeof(double))
          return BocTextValueType.Double;
        else if (numericProperty.Type == typeof(short))
          return BocTextValueType.Int16;
        else if (numericProperty.Type == typeof(int))
          return BocTextValueType.Int32;
        else if (numericProperty.Type == typeof(long))
          return BocTextValueType.Int64;
        else if (numericProperty.Type == typeof(float))
          return BocTextValueType.Single;
        else
          throw new NotSupportedException("BocTextValue does not support property type " + property.GetType());
      }
      else if (property is IBusinessObjectDateTimeProperty)
      {
        IBusinessObjectDateTimeProperty dateTimeProperty = (IBusinessObjectDateTimeProperty)property;
        if (dateTimeProperty.Type == DateTimeType.Date)
          return BocTextValueType.Date;
        else
          return BocTextValueType.DateTime;
      }
      else
        throw new NotSupportedException("BocTextValue does not support property type " + property.GetType());
    }

    protected override string ControlType
    {
      get { return "BocTextValue"; }
    }
  }
}
