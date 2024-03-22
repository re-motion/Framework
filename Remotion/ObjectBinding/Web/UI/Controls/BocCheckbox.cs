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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Globalization;
using Remotion.Web.UI;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> This control can be used to display or edit a boolean value (true or false). </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocCheckBox.xml' path='BocCheckBox/Class/*' />
  [ValidationProperty("ValidationValue")]
  [DefaultEvent("SelectionChanged")]
  [ToolboxItemFilter("System.Web.UI")]
  public class BocCheckBox : BocBooleanValueBase, IBocCheckBox
  {
    // constants
    private const string c_valueName = "_Value";

    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.ObjectBinding.Web.Globalization.BocCheckBox")]
    public enum ResourceIdentifier
    {
      /// <summary> The descripton rendered next the check box when it is checked. </summary>
      TrueDescription,
      /// <summary> The descripton rendered next the check box when it is not checked.  </summary>
      FalseDescription,
    }

    // static members

    private static readonly Type[] s_supportedPropertyInterfaces = new[] { typeof(IBusinessObjectBooleanProperty) };

    // member fields

    private bool _value;
    private bool? _defaultValue;
    private bool _isActive = true;

    private readonly Style _labelStyle;

    private bool? _showDescription;
    private ReadOnlyCollection<BaseValidator>? _validators;

    // construction and disposing

    public BocCheckBox ()
    {
      _labelStyle = new Style();
    }

    // methods and properties

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents(htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender);
    }

    protected override IBusinessObjectConstraintVisitor CreateBusinessObjectConstraintVisitor ()
    {
      return new BocCheckBoxConstraintVisitor(this);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer));
    }

    protected virtual IBocCheckBoxRenderer CreateRenderer ()
    {
      return ServiceLocator.GetInstance<IBocCheckBoxRenderer>();
    }

    protected virtual BocCheckBoxRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      Assertion.IsNotNull(Context, "Context must not be null.");

      return new BocCheckBoxRenderingContext(Context, writer, this);
    }

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed 
    ///   between postbacks.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocCheckBox.xml' path='BocCheckBox/LoadPostData/*' />
    protected override bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (! _isActive)
        return false;

      string? newValue = PageUtility.GetPostBackCollectionItem(Page!, GetValueName());
      bool newBooleanValue = ! string.IsNullOrEmpty(newValue);
      bool isDataChanged = _value != newBooleanValue;
      if (isDataChanged)
      {
        _value = newBooleanValue;
        IsDirty = true;
      }
      return isDataChanged;
    }

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected override void RaisePostDataChangedEvent ()
    {
      if (! IsReadOnly && Enabled)
        OnCheckedChanged();
    }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender(e);

      LoadResources(GetResourceManager(), GlobalizationService);

      _isActive = !IsReadOnly && Enabled;
    }

    /// <summary>
    /// Loads the control's value and a flag determining whether the value can be changed in addition to the base state.
    /// </summary>
    /// <param name="savedState">The state object created by <see cref="SaveControlState"/>.</param>
    protected override void LoadControlState (object? savedState)
    {
      object?[] values = (object?[])savedState!;

      base.LoadControlState(values[0]);
      _value = (bool)values[1]!;
      _isActive = (bool)values[2]!;
    }

    /// <summary>
    /// Saves the control's value and a flag determining whether the value can be changed in addition to the base state.
    /// </summary>
    /// <returns>An object containing the state to be loaded during the next lifecycle.</returns>
    protected override object SaveControlState ()
    {
      object[] values = new object[3];

      values[0] = base.SaveControlState();
      values[1] = _value;
      values[2] = _isActive;

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

      string? key;
      key = ResourceManagerUtility.GetGlobalResourceKey(TrueDescription.GetValue());
      if (! string.IsNullOrEmpty(key))
        TrueDescription = resourceManager.GetText(key);

      key = ResourceManagerUtility.GetGlobalResourceKey(FalseDescription.GetValue());
      if (! string.IsNullOrEmpty(key))
        FalseDescription = resourceManager.GetText(key);
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
    ///   interface.
    /// </summary>
    /// <returns> 
    ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
    ///   <see cref="CheckBox"/> if the control is in edit mode, or an empty array if the control is read-only.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      return IsReadOnly ? new string[0] : new[] { GetValueName() };
    }

    /// <summary>
    ///   The <see cref="BocCheckBox"/> supports properties of type <see cref="IBusinessObjectBooleanProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary>
    ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
    ///   <see cref="Control.ClientID"/>.
    /// </summary>
    /// <value> The <see cref="HyperLink"/> if the control is in edit mode, otherwise the control itself. </value>
    public override Control TargetControl
    {
      get { return this; }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the <see cref="CheckBox"/> if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public override string? FocusID
    {
      get { return IsReadOnly ? null : GetValueName(); }
    }

    /// <summary> Gets the string representation of this control's <see cref="BocBooleanValueBase.Value"/>. </summary>
    /// <remarks> 
    ///   <para>
    ///     Values can be <c>True</c>, <c>False</c>, and <c>null</c>. 
    ///   </para><para>
    ///     This property is used for validation.
    ///   </para>
    /// </remarks>
    [Browsable(false)]
    public bool ValidationValue
    {
      get { return Value!.Value; }
    }


    /// <summary>
    ///   Gets the <see cref="Style"/> that you want to apply to the <see cref="Label"/> used for displaying the 
    ///   description. 
    /// </summary>
    [Category("Style")]
    [Description("The style that you want to apply to the label used for displaying the description.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public override Style LabelStyle
    {
      get { return _labelStyle; }
    }

    /// <summary> Gets or sets the flag that determines whether to show the description next to the checkbox during editing. </summary>
    /// <value> 
    ///   <see langword="true"/> to enable the description during editing. 
    ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///   Use <see cref="IsDescriptionEnabled"/> to evaluate this property.
    ///   Note that the description is always displayed instead of the checkbox when the <see cref="BocCheckBox"/> is readonly.
    /// </remarks>
    [Description("The flag that determines whether to show the description next to the checkbox during editing. Undefined is interpreted as false.")]
    [Category("Appearance")]
    [DefaultValue(typeof(bool?), "")]
    public bool? ShowDescription
    {
      get { return _showDescription; }
      set { _showDescription = value; }
    }

    /// <summary> Gets a flag that determines whether the control is to be treated as a required value. </summary>
    /// <value> Always <see langword="false"/> since the checkbox has no undefined state in the user interface. </value>
    [Browsable(false)]
    public override bool IsRequired
    {
      get { return false; }
    }

    protected override bool? GetValue ()
    {
      return _value;
    }

    protected override void SetValue (bool? value)
    {
      _value = value ?? GetDefaultValue();
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocCheckBox"/> contains a value. </summary>
    public override bool HasValue
    {
      get { return true; }
    }

    /// <summary> The boolean value to which this control defaults if the assigned value is <see langword="null"/>. </summary>
    /// <value> 
    ///   <see langword="true"/> or <see langword="false"/> to explicitly specify the default value, or <see langword="null"/> to leave the decision 
    ///   to the object model. If the control  is unbound and no default value is specified, <see langword="false"/> is assumed as default value.
    /// </value>
    [Category("Behavior")]
    [Description("The boolean value to which this control defaults if the assigned value is null.")]
    [NotifyParentProperty(true)]
    [DefaultValue(typeof(bool?), "")]
    public bool? DefaultValue
    {
      get { return _defaultValue; }
      set { _defaultValue = value; }
    }

    /// <summary>
    ///   Evaluates the default value settings using the <see cref="DefaultValue"/> and the <see cref="BusinessObjectBoundWebControl.Property"/>'s
    ///   default value.
    /// </summary>
    /// <returns>
    ///   <list type="bullet">
    ///     <item> 
    ///       If <see cref="DefaultValue"/> is set to <see langword="true"/> or 
    ///       <see langword="false"/>, <see langword="true"/> or <see langword="false"/> is returned 
    ///       respectivly.
    ///     </item>
    ///     <item>
    ///       If <see cref="DefaultValue"/> is set to <see langword="null"/>, the <see cref="BusinessObjectBoundWebControl.Property"/>
    ///       is queried for its default value using the <see cref="IBusinessObjectBooleanProperty.GetDefaultValue"/>
    ///       method.
    ///       <list type="bullet">
    ///         <item> 
    ///           If <see cref="IBusinessObjectBooleanProperty.GetDefaultValue"/> returns 
    ///           <see langword="true"/> or <see langword="false"/>, <see langword="true"/> or 
    ///           <see langword="false"/> is returned respectivly.
    ///         </item>
    ///         <item>
    ///           Otherwise <see langword="false"/> is returned.
    ///         </item>
    ///       </list>
    ///     </item>
    ///   </list>
    /// </returns>
    protected bool GetDefaultValue ()
    {
      if (_defaultValue == null)
      {
        if (DataSource != null && DataSource.BusinessObjectClass != null && DataSource.BusinessObject != null && Property != null)
          return Property.GetDefaultValue(DataSource.BusinessObjectClass) ?? false;
        else
          return false;
      }
      else
        return _defaultValue == true ? true : false;
    }

    protected override IEnumerable<BaseValidator> CreateValidators (bool isReadOnly)
    {
      var validatorFactory = ServiceLocator.GetInstance<IBocCheckBoxValidatorFactory>();
      _validators = validatorFactory.CreateValidators(this, isReadOnly).ToList().AsReadOnly();
      return _validators;
    }

    /// <summary> Gets the evaluated value for the <see cref="ShowDescription"/> property. </summary>
    /// <value>
    ///   <see langowrd="true"/> if WAI conformity is not required 
    ///   and <see cref="ShowDescription"/> is <see langword="true"/>. 
    /// </value>
    protected bool IsDescriptionEnabled
    {
      get { return _showDescription == true; }
    }

    string IBocCheckBox.GetValueName ()
    {
      return GetValueName();
    }

    protected string GetValueName ()
    {
      return ClientID + c_valueName;
    }

    bool IBocCheckBox.IsDescriptionEnabled
    {
      get { return IsDescriptionEnabled; }
    }

    WebString IBocCheckBox.DefaultTrueDescription
    {
      get { return GetResourceManager().GetText(ResourceIdentifier.TrueDescription); }
    }

    WebString IBocCheckBox.DefaultFalseDescription
    {
      get { return GetResourceManager().GetText(ResourceIdentifier.FalseDescription); }
    }

    protected override string ControlType
    {
      get { return "BocCheckBox"; }
    }
  }
}
