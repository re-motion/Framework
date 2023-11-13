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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Globalization;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> This control can be used to display or edit enumeration values. </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/Class/*' />
  [ValidationProperty("Value")]
  [DefaultEvent("SelectionChanged")]
  [ToolboxItemFilter("System.Web.UI")]
  public class BocEnumValue : BusinessObjectBoundEditableWebControl, IBocEnumValue, IPostBackDataHandler, IFocusableControl
  {
    // constants

    private const string c_nullIdentifier = "==null==";
    private const string c_listControlIDPostfix = "_Value";

    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.ObjectBinding.Web.Globalization.BocEnumValue")]
    public enum ResourceIdentifier
    {
      /// <summary> The text rendered for the null item in the list. </summary>
      UndefinedItemText,
      /// <summary> The validation error message displayed when the null item is selected. </summary>
      NullItemValidationMessage
    }

    // static members

    private static readonly Type[] s_supportedPropertyInterfaces = new[] { typeof(IBusinessObjectEnumerationProperty) };

    private static readonly object s_selectionChangedEvent = new object();

    // member fields

    private object? _value;
    private string? _internalValue;
    private IEnumerationValueInfo? _enumerationValueInfo;

    private readonly Style _commonStyle;
    private readonly ListControlStyle _listControlStyle;
    private readonly Style _labelStyle;

    private PlainTextString _undefinedItemText = PlainTextString.Empty;

    private PlainTextString _errorMessage;
    private ReadOnlyCollection<BaseValidator>? _validators;

    // construction and disposing

    public BocEnumValue ()
    {
      _commonStyle = new Style();
      _listControlStyle = new ListControlStyle();
      _labelStyle = new Style();
    }

    // methods and properties

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender);
    }

    protected override IBusinessObjectConstraintVisitor CreateBusinessObjectConstraintVisitor ()
    {
      return new BocEnumValueConstraintVisitor(this);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer));
    }

    protected virtual IBocEnumValueRenderer CreateRenderer ()
    {
      return ServiceLocator.GetInstance<IBocEnumValueRenderer>();
    }

    protected virtual BocEnumValueRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      Assertion.IsNotNull(Context, "Context must not be null.");

      return new BocEnumValueRenderingContext(Context, writer, this);
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/LoadValue/*' />
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
    /// <param name="value"> The enumeration value or <see langword="null"/>. </param>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/LoadUnboundValue/*' />
    public void LoadUnboundValue<TEnum> (TEnum? value, bool interim)
        where TEnum: struct
    {
      ArgumentUtility.CheckType<Enum>("value", value);
      LoadValueInternal(value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> The enumeration value. </param>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/LoadUnboundValue/*' />
    public void LoadUnboundValue<TEnum> (TEnum value, bool interim)
        where TEnum: struct
    {
      ArgumentUtility.CheckType<Enum>("value", value);
      LoadValueInternal(value, interim);
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/SaveValue/*' />
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

    [Obsolete("For DependDB only.", true)]
    private new BaseValidator[] CreateValidators ()
    {
      throw new NotImplementedException("For DependDB only.");
    }

    /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
    /// <param name="isReadOnly">
    /// This flag is initialized with the value of <see cref="BusinessObjectBoundEditableWebControl.IsReadOnly"/>. 
    /// Implemantations should consider whether they require a validator also when the control is rendered as read-only.
    /// </param>
    /// <remarks>
    ///   Generates a <see cref="CompareValidator"/> checking that the selected item is not the null-item if the 
    ///   control is in edit mode and input is required.
    /// </remarks>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.CreateValidators()">BusinessObjectBoundEditableWebControl.CreateValidators()</seealso>
    protected override IEnumerable<BaseValidator> CreateValidators (bool isReadOnly)
    {
      var validatorFactory = ServiceLocator.GetInstance<IBocEnumValueValidatorFactory>();
      _validators = validatorFactory.CreateValidators(this,isReadOnly).ToList().AsReadOnly();

      OverrideValidatorErrorMessages();

      return _validators;
    }

    private void OverrideValidatorErrorMessages ()
    {
      if (!_errorMessage.IsEmpty)
        UpdateValidatorErrorMessages<RequiredFieldValidator>(_errorMessage);
    }

    private void UpdateValidatorErrorMessages<T> (PlainTextString errorMessage) where T : BaseValidator
    {
      var validator = _validators.GetValidator<T>();
      if (validator != null)
        validator.ErrorMessage = errorMessage.GetValue();
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
    ///   interface.
    /// </summary>
    /// <returns> 
    ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
    ///   <see cref="ListControl"/> (or the radio buttons that make up the list), if the control is in edit mode, 
    ///   or an empty array if the control is read-only.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      if (IsReadOnly)
        return new string[0];
      else if (ListControlStyle.ControlType == ListControlType.DropDownList
               || ListControlStyle.ControlType == ListControlType.ListBox)
        return new[] { GetValueName() };
      else if (ListControlStyle.ControlType == ListControlType.RadioButtonList)
      {
        string[] clientIDs = new string[GetEnabledValues().Length + (IsRequired ? 0 : 1)];
        for (int i = 0; i < clientIDs.Length; i++)
          clientIDs[i] = GetValueName() + "_" + i.ToString(NumberFormatInfo.InvariantInfo);
        return clientIDs;
      }
      else
        return new string[0];
    }

    string IBocEnumValue.GetValueName ()
    {
      return GetValueName();
    }

    protected string GetValueName ()
    {
      return ClientID + c_listControlIDPostfix;
    }

    /// <summary> This event is fired when the selection is changed between postbacks. </summary>
    [Category("Action")]
    [Description("Fires when the value of the control has changed.")]
    public event EventHandler SelectionChanged
    {
      add { Events.AddHandler(s_selectionChangedEvent, value); }
      remove { Events.RemoveHandler(s_selectionChangedEvent, value); }
    }

    /// <summary> Gets or sets the <see cref="IBusinessObjectEnumerationProperty"/> object this control is bound to. </summary>
    /// <value> An <see cref="IBusinessObjectEnumerationProperty"/> object. </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new IBusinessObjectEnumerationProperty? Property
    {
      get { return (IBusinessObjectEnumerationProperty?)base.Property; }
      set { base.Property = ArgumentUtility.CheckType<IBusinessObjectEnumerationProperty>("value", value); }
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/Value/*' />
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

    /// <summary>
    /// Gets the value from the backing field.
    /// </summary>
    protected object? GetValue ()
    {
      EnsureValue();
      return _value;
    }

    /// <summary>
    /// Sets the value from the backing field.
    /// </summary>
    /// <remarks>
    /// <para>Setting the value via this method does not affect the control's dirty state.</para>
    /// </remarks>
    protected void SetValue (object? value)
    {
      _value = value;

      if (Property != null && _value != null)
      {
        _enumerationValueInfo = Property.GetValueInfoByValue(_value, GetBusinessObject());
      }
      else
      {
        _enumerationValueInfo = null;
      }

      if (_enumerationValueInfo != null)
        InternalValue = _enumerationValueInfo.Identifier;
      else
        InternalValue = null;
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    protected override sealed object? ValueImplementation
    {
      get { return Value; }
      set { Value = value; }
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocEnumValue"/> contains a value. </summary>
    public override bool HasValue
    {
      get { return InternalValue != null; }
    }

    /// <summary> Gets or sets the current value. </summary>
    /// <value> 
    ///   The <see cref="IEnumerationValueInfo.Identifier"/> object
    ///   or <see langword="null"/> if no item / the null item is selected.
    /// </value>
    /// <remarks> Used to identify the currently selected item. </remarks>
    protected virtual string? InternalValue
    {
      get
      {
        if (_internalValue == null && EnumerationValueInfo != null)
        {
          var identifier = EnumerationValueInfo.Identifier;
          Assertion.IsFalse(string.IsNullOrEmpty(identifier), "EnumerationValueInfo.Identifier must not be null or empty.");
          _internalValue = identifier;
        }

        return _internalValue;
      }
      set
      {
        if (value == string.Empty)
          throw new ArgumentException("Value must not be an empty string.", "value");

        if (_internalValue == value)
          return;

        _internalValue = value;

        EnsureValue();
      }
    }

    /// <summary> Ensures that the <see cref="Value"/> is set to the enum-value of the <see cref="InternalValue"/>. </summary>
    protected void EnsureValue ()
    {
      Assertion.DebugAssert(_internalValue != string.Empty, "InternalValue must not be empty.");

      if (_enumerationValueInfo != null
          && _enumerationValueInfo.Identifier == _internalValue)
      {
        //  Still chached in _enumerationValueInfo
        _value = _enumerationValueInfo.Value;
      }
      else if (_internalValue != null && Property != null)
      {
        //  Can get a new EnumerationValueInfo
        _enumerationValueInfo = Property.GetValueInfoByIdentifier(_internalValue, GetBusinessObject());
        Assertion.IsNotNull(_enumerationValueInfo, "_enumerationValueInfo is null for identifier '{0}'.", _internalValue);
        _value = _enumerationValueInfo.Value;
      }
      else if (_internalValue == null)
      {
        _value = null;
        _enumerationValueInfo = null;
      }
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="TargetControl"/>.
    /// </summary>
    /// <value> Always <see langword="true"/>. </value>
    public override bool UseLabel
    {
      get { return true; }
    }

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
    ///   <see cref="Control.ClientID"/>.
    /// </summary>
    /// <value> The  control itself. </value>
    public override Control TargetControl
    {
      get { return this; }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the list control if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public string? FocusID
    {
      get { return IsReadOnly ? null : GetValueName(); }
    }

    /// <summary>
    ///   Gets the style that you want to apply to the <see cref="ListControl"/> (edit mode) 
    ///   and the <see cref="Label"/> (read-only mode).
    /// </summary>
    /// <remarks>
    ///   Use the <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/> to assign individual 
    ///   style settings for the respective modes. Note that if you set one of the <b>Font</b> 
    ///   attributes (Bold, Italic etc.) to <see langword="true"/>, this cannot be overridden using 
    ///   <see cref="TextBoxStyle"/> and <see cref="LabelStyle"/>  properties.
    /// </remarks>
    [Category("Style")]
    [Description("The style that you want to apply to the ListControl (edit mode) and the Label (read-only mode).")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public Style CommonStyle
    {
      get { return _commonStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="ListControl"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category("Style")]
    [Description("The style that you want to apply to the ListControl (edit mode) only.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ListControlStyle ListControlStyle
    {
      get { return _listControlStyle; }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="Label"/> (read-only mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="CommonStyle"/>. </remarks>
    [Category("Style")]
    [Description("The style that you want to apply to the Label (read-only mode) only.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public Style LabelStyle
    {
      get { return _labelStyle; }
    }

    /// <summary> Gets or sets the text displayed for the undefined item. </summary>
    /// <value> 
    ///   The text displayed for <see langword="null"/>. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("The description displayed for the undefined item.")]
    [Category("Appearance")]
    [DefaultValue(typeof(PlainTextString), "")]
    public PlainTextString UndefinedItemText
    {
      get { return _undefinedItemText; }
      set { _undefinedItemText = value; }
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
      }
    }

    [Browsable(false)]
    public string NullIdentifier
    {
      get { return c_nullIdentifier; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      Page!.RegisterRequiresPostBack(this);
    }

    protected override void LoadControlState (object? savedState)
    {
      object?[] values = (object?[])savedState!;

      base.LoadControlState(values[0]);
      _value = values[1];
      if (values[2] != null)
        _internalValue = (string?)values[2];
    }

    protected override object SaveControlState ()
    {
      object?[] values = new object?[5];

      values[0] = base.SaveControlState();
      values[1] = _value;
      values[2] = _internalValue;

      return values;
    }

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed
    ///   between postbacks.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocEnumValue.xml' path='BocEnumValue/LoadPostData/*' />
    protected virtual bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      string? newValue = StringUtility.EmptyToNull(PageUtility.GetPostBackCollectionItem(Page!, GetValueName()));
      bool isDataChanged = false;
      if (newValue != null)
      {
        if (_internalValue == null && newValue != c_nullIdentifier)
          isDataChanged = true;
        else if (_internalValue != null && newValue != _internalValue)
          isDataChanged = true;
      }

      if (isDataChanged)
      {
        if (newValue == c_nullIdentifier)
          InternalValue = null;
        else
          InternalValue = newValue;
        IsDirty = true;
      }
      return isDataChanged;
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected virtual void RaisePostDataChangedEvent ()
    {
      if (!IsReadOnly && Enabled)
        OnSelectionChanged();
    }

    /// <summary> Fires the <see cref="SelectionChanged"/> event. </summary>
    protected virtual void OnSelectionChanged ()
    {
      EventHandler? eventHandler = (EventHandler?)Events[s_selectionChangedEvent];
      if (eventHandler != null)
        eventHandler(this, EventArgs.Empty);
    }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender(e);

      LoadResources(GetResourceManager(), GlobalizationService);

    }

    /// <summary> Gets a <see cref="HtmlTextWriterTag.Div"/> as the <see cref="WebControl.TagKey"/>. </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Div; }
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="O:Remotion.ObjectBinding.Web.UI.Controls.BocEnumValue.LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (object? value, bool interim)
    {
      if (interim)
        return;

      SetValue(value);
      IsDirty = false;
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    public IResourceManager GetResourceManager ()
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

      key = ResourceManagerUtility.GetGlobalResourceKey(UndefinedItemText.GetValue());
      if (! string.IsNullOrEmpty(key))
        UndefinedItemText = resourceManager.GetText(key);
    }

    /// <summary> Gets the current value. </summary>
    /// <value> 
    ///   The <see cref="EnumerationValueInfo"/> object
    ///   or <see langword="null"/> if no item / the null item is selected 
    ///   or the <see cref="Property"/> is <see langword="null"/>.
    /// </value>
    /// <remarks> Only used to simplify access to the <see cref="IEnumerationValueInfo"/>. </remarks>
    protected IEnumerationValueInfo? EnumerationValueInfo
    {
      get
      {
        if (_enumerationValueInfo == null && Property != null && _value != null)
          _enumerationValueInfo = Property.GetValueInfoByValue(_value, GetBusinessObject());

        return _enumerationValueInfo;
      }
    }

    /// <summary> The <see cref="BocEnumValue"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return !isList;
    }

    /// <summary>
    ///   The <see cref="BocEnumValue"/> supports properties of types <see cref="IBusinessObjectEnumerationProperty"/>
    ///   and <see cref="IBusinessObjectBooleanProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    private IEnumerationValueInfo[] GetEnabledValues ()
    {
      if (Property == null)
        return new IEnumerationValueInfo[0];

      return Property.GetEnabledValues(GetBusinessObject());
    }

    private IBusinessObject? GetBusinessObject ()
    {
      return (Property != null && DataSource != null) ? DataSource.BusinessObject : null;
    }

    private PlainTextString GetNullItemText ()
    {
      var nullDisplayName = _undefinedItemText;
      if (nullDisplayName.IsEmpty)
        nullDisplayName = GetResourceManager().GetText(ResourceIdentifier.UndefinedItemText);
      return nullDisplayName;
    }

    /// <summary> Invokes the <see cref="LoadPostData"/> method. </summary>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (IsLoadPostDataRequired())
        return LoadPostData(postDataKey, postCollection);
      else
        return false;
    }


    /// <summary> Invokes the <see cref="RaisePostDataChangedEvent"/> method. </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
      RaisePostDataChangedEvent();
    }

    IEnumerable<string> IControlWithLabel.GetLabelIDs ()
    {
      return GetLabelIDs();
    }

    IEnumerationValueInfo[] IBocEnumValue.GetEnabledValues ()
    {
      return GetEnabledValues();
    }

    IEnumerationValueInfo? IBocEnumValue.EnumerationValueInfo
    {
      get { return EnumerationValueInfo; }
    }

    PlainTextString IBocEnumValue.GetNullItemText ()
    {
      return GetNullItemText();
    }

    IEnumerable<PlainTextString> IBocEnumValue.GetValidationErrors ()
    {
      return GetRegisteredValidators()
          .Where(v => !v.IsValid)
          .Select(v => v.ErrorMessage)
          .Select(PlainTextString.CreateFromText)
          .Distinct();
    }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return "BocEnumValue"; }
    }
  }
}
