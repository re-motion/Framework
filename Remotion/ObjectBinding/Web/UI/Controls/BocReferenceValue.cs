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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Globalization;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
// ReSharper disable once RedundantUsingDirective
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Globalization;
using Remotion.Web.Services;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> This control can be used to display or select references as the value of a property using a drop-down list. </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/Class/*' />
  // TODO: see "Doc\Bugs and ToDos.txt"
  [ValidationProperty("ValidationValue")]
  [DefaultEvent("SelectionChanged")]
  [ToolboxItemFilter("System.Web.UI")]
  public class BocReferenceValue
      :
          BocReferenceValueBase<IBocReferenceValueWebService>,
          IBocReferenceValue,
          IFocusableControl
  {
    // constants

    private const string c_dropDownListIDPostfix = "_Value";

    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.ObjectBinding.Web.Globalization.BocReferenceValue")]
    public enum ResourceIdentifier
    {
      /// <summary> The text rendered for the null item in the list. </summary>
      NullItemText,

      /// <summary> Label displayed in the OptionsMenu. </summary>
      OptionsTitle,

      /// <summary> The validation error message displayed when the null item is selected. </summary>
      NullItemErrorMessage,
    }


    // static members

    // member fields

    private bool _isBusinessObjectListPopulated;

    private readonly DropDownListStyle _dropDownListStyle;

    /// <summary> 
    ///   The object returned by <see cref="BocReferenceValue"/>. 
    ///   Does not require <see cref="System.Runtime.Serialization.ISerializable"/>. 
    /// </summary>
    private IBusinessObjectWithIdentity? _value;

    private string? _displayName;
    private readonly ListItemCollection _listItems;

    private PlainTextString _nullItemText = PlainTextString.Empty;
    private string? _select = String.Empty;
    private bool? _enableSelectStatement;
    private PlainTextString _nullItemErrorMessage;
    private ReadOnlyCollection<BaseValidator>? _validators;

    // construction and disposing

    public BocReferenceValue ()
        : this(SafeServiceLocator.Current.GetInstance<IWebServiceFactory>())
    {
    }

    protected BocReferenceValue ([NotNull] IWebServiceFactory webServiceFactory)
        : base(webServiceFactory)
    {
      _listItems = new ListItemCollection();
      _dropDownListStyle = new DropDownListStyle();
    }

    // methods and properties

    /// <summary> Gets or sets the text displayed for the undefined item. </summary>
    /// <value> 
    ///   The text displayed for <see langword="null"/>. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("The description displayed for the undefined item.")]
    [Category("Appearance")]
    [DefaultValue(typeof(PlainTextString), "")]
    public PlainTextString NullItemText
    {
      get { return _nullItemText; }
      set { _nullItemText = value; }
    }

    /// <summary> Gets or sets the validation error message displayed when the value is not set but the control is required. </summary>
    /// <value> 
    ///   The error message displayed when validation fails. The default value is an empty <see cref="String"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("Validation message displayed if the value is not set but the control is required.")]
    [Category("Validator")]
    [DefaultValue(typeof(PlainTextString), "")]
    public PlainTextString NullItemErrorMessage
    {
      get { return _nullItemErrorMessage; }
      set
      {
        _nullItemErrorMessage = value;
        UpdateValidatorErrorMessages<RequiredFieldValidator>(_nullItemErrorMessage);
      }
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents(htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender);
    }

    protected virtual IBocReferenceValueRenderer CreateRenderer ()
    {
      return ServiceLocator.GetInstance<IBocReferenceValueRenderer>();
    }

    protected virtual BocReferenceValueRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      Assertion.IsNotNull(Context, "Context must not be null.");

      return new BocReferenceValueRenderingContext(Context, writer, this, CreateBusinessObjectWebServiceContext());
    }

    /// <remarks>
    ///   If the <see cref="DropDownList"/> could not be created from <see cref="DropDownListStyle"/>,
    ///   the control is set to read-only.
    /// </remarks>
    protected override void CreateChildControls ()
    {
      base.CreateChildControls();
      ((IStateManager)_listItems).TrackViewState();
    }

    protected override string ValueContainingControlID
    {
      get { return GetValueName(); }
    }

    string IBocReferenceValue.GetValueName ()
    {
      return GetValueName();
    }

    protected string GetValueName ()
    {
      return ClientID + c_dropDownListIDPostfix;
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected override void RaisePostDataChangedEvent ()
    {
      var internalValue = InternalValue;
      if (internalValue == null)
      {
        _displayName = null;
      }
      else
      {
        ListItem selectedItem = _listItems.FindByValue(internalValue);
        if (selectedItem == null)
        {
          // GetObject(...) can load any value that is provided by ID. This behavior is symmetrical with BocAutoCompleteReferenceValue.
          var businessObject = GetBusinessObjectClass()?.GetObject(internalValue);
          if (businessObject == null)
            throw new InvalidOperationException(string.Format("The key '{0}' does not correspond to a known element.", internalValue));
          else
            _displayName = GetDisplayName(businessObject);
        }
        else
        {
          _displayName = selectedItem.Text;
        }
      }

      if (!IsReadOnly && Enabled)
        OnSelectionChanged();
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      base.LoadResources(resourceManager, globalizationService);

      var key = ResourceManagerUtility.GetGlobalResourceKey(NullItemText.GetValue());
      if (! string.IsNullOrEmpty(key))
        NullItemText = resourceManager.GetText(key);

      key = ResourceManagerUtility.GetGlobalResourceKey(NullItemErrorMessage.GetValue());
      if (!string.IsNullOrEmpty(key))
        NullItemErrorMessage = resourceManager.GetText(key);

      key = ResourceManagerUtility.GetGlobalResourceKey(Select);
      if (!string.IsNullOrEmpty(key))
        Select = resourceManager.GetString(key);
    }

    /// <summary>
    /// A validator for the display name is created.
    /// </summary>
    /// <param name="isReadOnly">
    /// This flag is initialized with the value of <see cref="BusinessObjectBoundEditableWebControl.IsReadOnly"/>. 
    /// Implemantations should consider whether they require a validator also when the control is rendered as read-only.
    /// </param>
    /// <returns>An enumeration of all applicable validators.</returns>
    /// <remarks>
    ///   Generates a <see cref="BocAutoCompleteReferenceValueInvalidDisplayNameValidator"/> checking that the display name can be resolved 
    /// to a valid object reference.
    /// </remarks>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.CreateValidators()">BusinessObjectBoundEditableWebControl.CreateValidators()</seealso>
    protected override IEnumerable<BaseValidator> CreateValidators (bool isReadOnly)
    {
      var validatorFactory = ServiceLocator.GetInstance<IBocReferenceValueValidatorFactory>();
      _validators = validatorFactory.CreateValidators(this, isReadOnly).ToList().AsReadOnly();

      OverrideValidatorErrorMessages();

      return _validators;
    }

    private void OverrideValidatorErrorMessages ()
    {
      if (NullItemErrorMessage.IsEmpty)
        return;

      UpdateValidatorErrorMessages<RequiredFieldValidator>(NullItemErrorMessage);
    }

    private void UpdateValidatorErrorMessages<T> (PlainTextString errorMessage) where T : BaseValidator
    {
      var validator = _validators.GetValidator<T>();
      if (validator != null)
        validator.ErrorMessage = errorMessage.GetValue();
    }

    protected override IBusinessObjectConstraintVisitor CreateBusinessObjectConstraintVisitor ()
    {
      return new BocReferenceValueConstraintVisitor(this);
    }

    protected override void OnPreRender (EventArgs e)
    {
      // Required to provide the list before SaveControlState is performed
      EnsureBusinessObjectListPopulated();

      base.OnPreRender(e);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer));
    }

    protected override void LoadControlState (object? savedState)
    {
      object?[] values = (object?[])savedState!;

      base.LoadControlState(values[0]);
      if (values[1] != null)
        InternalValue = (string?)values[1];
      _displayName = (string?)values[2];
      _isBusinessObjectListPopulated = (bool)values[3]!;
      if (_isBusinessObjectListPopulated)
        ((IStateManager)_listItems).LoadViewState(values[4]);
    }

    protected override object SaveControlState ()
    {
      object?[] values = new object?[5];

      values[0] = base.SaveControlState();
      values[1] = InternalValue;
      values[2] = _displayName;
      values[3] = _isBusinessObjectListPopulated;
      if (_isBusinessObjectListPopulated)
        values[4] =  ((IStateManager)_listItems).SaveViewState();

      return values;
    }


    /// <summary> Loads the <see cref="BocReferenceValueBase.Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (interim)
        return;

      if (Property == null)
        return;

      if (DataSource == null)
        return;

      IBusinessObjectWithIdentity? value = null;

      if (DataSource.BusinessObject != null)
        value = (IBusinessObjectWithIdentity?)DataSource.BusinessObject.GetProperty(Property);

      LoadValueInternal(value, false);
    }

    /// <summary> Populates the <see cref="BocReferenceValueBase.Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (IBusinessObjectWithIdentity? value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (IBusinessObjectWithIdentity? value, bool interim)
    {
      if (interim)
        return;

      SetValue(value);
      IsDirty = false;
    }

    /// <summary> Saves the <see cref="BocReferenceValueBase.Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocReferenceValue.xml' path='BocReferenceValue/SaveValue/*' />
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

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    protected override IResourceManager GetResourceManager ()
    {
      return GetResourceManager(typeof(ResourceIdentifier));
    }

    protected override sealed WebString GetOptionsMenuTitle ()
    {
      return GetResourceManager().GetText(ResourceIdentifier.OptionsTitle);
    }

    protected override sealed string GetSelectionCountScript ()
    {
      return "function() { return BocReferenceValue.GetSelectionCount ('" + GetValueName() + "', '" + c_nullIdentifier + "'); }";
    }

    /// <summary> Sets the <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
    /// <remarks>
    ///   Use this method to set the listed items, e.g. from the parent control, if no <see cref="Select"/>
    ///   statement was provided.
    /// </remarks>
    /// <param name="businessObjects">
    ///   An array of <see cref="IBusinessObjectWithIdentity"/> objects to be used as list of possible values.
    ///   Must not be <see langword="null"/>.
    /// </param>
    public void SetBusinessObjectList (IBusinessObjectWithIdentity[] businessObjects)
    {
      ArgumentUtility.CheckNotNull("businessObjects", businessObjects);
      RefreshBusinessObjectList(businessObjects);
    }

    /// <summary> Sets the <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
    /// <remarks>
    ///   Use this method to set the listed items, e.g. from the parent control, if no <see cref="Select"/>
    ///   statement was provided.
    /// </remarks>
    /// <param name="businessObjects">
    ///   An <see cref="IList"/> of <see cref="IBusinessObjectWithIdentity"/> objects to be used as list of possible 
    ///   values. Must not be <see langword="null"/>.
    /// </param>
    public void SetBusinessObjectList (IList businessObjects)
    {
      ArgumentUtility.CheckNotNull("businessObjects", businessObjects);
      ArgumentUtility.CheckItemsNotNullAndType("businessObjects", businessObjects, typeof(IBusinessObjectWithIdentity));
      RefreshBusinessObjectList(businessObjects);
    }

    /// <summary> Clears the list of <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
    /// <remarks> If the value is not required, the null item will displayed anyway. </remarks>
    [Obsolete(
        "Use SetBusinessObjectList(Array.Empty<IBusinessObjectWithIdentity>()) to set an empty list or ResetBusinessObjectList() to trigger an update of the list. (Version: 3.0.0)")]
    public void ClearBusinessObjectList ()
    {
      SetBusinessObjectList(Array.Empty<IBusinessObjectWithIdentity>());
    }

    /// <summary> Resets the list of <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode. </summary>
    /// <remarks> Clears the list of values and reloads the values during the next rendering phase. </remarks>
    public void ResetBusinessObjectList ()
    {
      _isBusinessObjectListPopulated = false;
      _listItems.Clear();
    }

    /// <summary> Calls <see cref="PopulateBusinessObjectList"/> if the list has not yet been populated. </summary>
    protected void EnsureBusinessObjectListPopulated ()
    {
      if (_isBusinessObjectListPopulated)
        return;
      PopulateBusinessObjectList();
    }


    /// <summary>
    ///   Queries <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> for the
    ///   <see cref="IBusinessObjectWithIdentity"/> objects to be displayed in edit mode and sets the list with the
    ///   objects returned by the query.
    /// </summary>
    /// <remarks> 
    ///   <para>
    ///     Uses the <see cref="Select"/> statement to query the <see cref="BocReferenceValueBase.Property"/>'s 
    ///     <see cref="IBusinessObjectReferenceProperty.SearchAvailableObjects"/> method for the list contents.
    ///   </para><para>
    ///     Only populates the list if <see cref="EnableSelectStatement"/> is not <see langword="false"/>.
    ///     Otherwise the list will be left empty.
    ///   </para>  
    /// </remarks>
    protected void PopulateBusinessObjectList ()
    {
      if (!IsSelectStatementEnabled)
        return;

      if (Property == null)
        return;

      //  Get all matching business objects
      var businessObjects = Property.SearchAvailableObjects(DataSource?.BusinessObject, new DefaultSearchArguments(_select));

      RefreshBusinessObjectList(businessObjects);
    }

    /// <summary> Populates the <see cref="DropDownList"/> with the items passed in <paramref name="businessObjects"/>. </summary>
    /// <param name="businessObjects">
    ///   The <see cref="IList"/> of <see cref="IBusinessObjectWithIdentity"/> objects to populate the <see cref="DropDownList"/>.
    ///   Must not be <see langword="null" />.
    /// </param>
    /// <remarks> This method controls the actual refilling of the <see cref="DropDownList"/>. </remarks>
    protected virtual void RefreshBusinessObjectList (IList businessObjects)
    {
      ArgumentUtility.CheckNotNull("businessObjects", businessObjects);

      _isBusinessObjectListPopulated = true;
      _listItems.Clear();

      foreach (IBusinessObjectWithIdentity businessObject in businessObjects)
      {
        ListItem item = new ListItem(GetDisplayName(businessObject), businessObject.UniqueIdentifier);
        _listItems.Add(item);
      }
    }

    protected override string? GetLabelText ()
    {
      string? text;
      if (InternalValue != null)
        text = _displayName;
      else
        text = String.Empty;
      return text;
    }

    protected override sealed IBusinessObjectWithIdentity? GetValue ()
    {
      if (InternalValue == null)
        _value = null;
      //  Only reload if value is outdated
      else if (_value == null || _value.UniqueIdentifier != InternalValue)
      {
        var businessObjectClass = GetBusinessObjectClass();
        if (businessObjectClass != null)
          _value = businessObjectClass.GetObject(InternalValue);
      }

      return _value;
    }

    protected override sealed void SetValue (IBusinessObjectWithIdentity? value)
    {
      _value = value;

      if (value != null)
      {
        InternalValue = value.UniqueIdentifier;
        _displayName = GetDisplayName(value);
      }
      else
      {
        InternalValue = null;
        _displayName = null;
      }
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocReferenceValue"/> contains a value. </summary>
    public override bool HasValue
    {
      get { return InternalValue != null; }
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user interface.
    /// </summary>
    /// <returns> 
    ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
    ///   <see cref="DropDownList"/> if the control is in edit mode, or an empty array if the control is read-only.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      return IsReadOnly ? new string[0] : new[] { GetValueName() };
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the <see cref="DropDownList"/> if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public string? FocusID
    {
      get { return IsReadOnly ? null : GetValueName(); }
    }

    /// <summary> Gets the style that you want to apply to the <see cref="DropDownList"/> (edit mode) only. </summary>
    /// <remarks> These style settings override the styles defined in <see cref="BocReferenceValueBase.CommonStyle"/>. </remarks>
    [Category("Style")]
    [Description("The style that you want to apply to the DropDownList (edit mode) only.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public DropDownListStyle DropDownListStyle
    {
      get { return _dropDownListStyle; }
    }

    /// <summary> The search expression used to populate the selection list in edit mode. </summary>
    /// <value> A <see cref="String"/> with a valid search expression. The default value is an empty <see cref="String"/>. </value>
    /// <remarks> A valid <see cref="BocReferenceValueBase.Property"/> is required in order to populate the list using the search statement. </remarks>
    [Category("Data")]
    [Description("Set the search expression for populating the selection list.")]
    [DefaultValue("")]
    public string? Select
    {
      get { return _select; }
      set
      {
        if (value == null)
          _select = null;
        else
          _select = value.Trim();
      }
    }

    /// <summary> Gets or sets the flag that determines whether to evaluate the <see cref="Select"/> statement. </summary>
    /// <value> 
    ///   <see langword="true"/> to evaluate the select statement. 
    ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
    /// </value>
    /// <remarks>
    ///   Use <see cref="IsSelectStatementEnabled"/> to evaluate this property.
    /// </remarks>
    [Description("The flag that determines whether to evaluate the Select statement. Undefined is interpreted as true.")]
    [Category("Behavior")]
    [DefaultValue(typeof(bool?), "")]
    public bool? EnableSelectStatement
    {
      get { return _enableSelectStatement; }
      set { _enableSelectStatement = value; }
    }

    public override string? ValidationValue
    {
      get { return InternalValue; }
    }

    /// <summary> Gets the evaluated value for the <see cref="EnableSelectStatement"/> property. </summary>
    /// <value>
    ///   <see langowrd="false"/> if <see cref="EnableSelectStatement"/> is <see langword="false"/>. 
    /// </value>
    protected bool IsSelectStatementEnabled
    {
      get { return _enableSelectStatement != false; }
    }

    void IBocReferenceValue.PopulateDropDownList (DropDownList dropDownList)
    {
      // PopulateDropDownList should be moved to the renderer, the BocReferenceValue should only provide a list of items, see also BocEnumValue.

      ArgumentUtility.CheckNotNull("dropDownList", dropDownList);

      EnsureBusinessObjectListPopulated();

      dropDownList.Items.Clear();

      bool isNullItem = (InternalValue == null);

      if (isNullItem || !IsRequired)
        dropDownList.Items.Add(CreateNullItem());

      foreach (ListItem listItem in _listItems)
        dropDownList.Items.Add(new ListItem(listItem.Text, listItem.Value));

      //  Check if null item is to be selected
      if (isNullItem)
        dropDownList.SelectedValue = c_nullIdentifier;
      else
      {
        if (dropDownList.Items.FindByValue(InternalValue!) != null)
          dropDownList.SelectedValue = InternalValue!;
        else if (Value != null)
        {
          //  Item not yet in the list but is a valid item.
          var businessObject = Value;

          var item = new ListItem(GetDisplayName(businessObject), businessObject.UniqueIdentifier);
          dropDownList.Items.Add(item);

          dropDownList.SelectedValue = InternalValue!;
        }
      }
    }

    /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
    /// <returns> A <see cref="ListItem"/>. </returns>
    private ListItem CreateNullItem ()
    {
      var nullItem = new ListItem(GetNullItemText().GetValue(), c_nullIdentifier);
      if (!DropDownListStyle.NullValueTextVisible)
      {
        nullItem.Attributes[HtmlTextWriterAttribute2.AriaLabel] = nullItem.Text;
        // By setting the label to a single whitespace, we can convince the HTML validator that the element is valid,
        // while preventing text from being displayed in the UI.
        nullItem.Attributes[HtmlTextWriterAttribute2.Label] = " ";
        nullItem.Text = string.Empty;
      }

      return nullItem;
    }

    private PlainTextString GetNullItemText ()
    {
      var nullDisplayName = _nullItemText;
      if (nullDisplayName.IsEmpty)
        nullDisplayName = GetResourceManager().GetText(ResourceIdentifier.NullItemText);
      return nullDisplayName;
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="BocReferenceValueBase.TargetControl"/>.
    /// </summary>
    /// <value> Always <see langword="true"/>. </value>
    public override bool UseLabel
    {
      get { return true; }
    }

    protected override string ControlType
    {
      get { return "BocReferenceValue"; }
    }
  }
}
