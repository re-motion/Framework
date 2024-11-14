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
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> This control can be used to display or edit a tri-state value (true, false, and undefined). </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/Class/*' />
  [ValidationProperty("Value")]
  [DefaultEvent("SelectionChanged")]
  [ToolboxItemFilter("System.Web.UI")]
  public class BocBooleanValue : BocBooleanValueBase, IBocBooleanValue
  {
    // constants

    private const string c_nullString = "null";
    private const string c_valueName = "_Value";
    private const string c_displayValueName = "_DisplayValue";

    // types

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.ObjectBinding.Web.Globalization.BocBooleanValue")]
    public enum ResourceIdentifier
    {
      /// <summary> The descripton rendered next the check box when it is checked. </summary>
      TrueDescription,
      /// <summary> The descripton rendered next the check box when it is not checked.  </summary>
      FalseDescription,
      /// <summary> The descripton rendered next the check box when its state is undefined.  </summary>
      NullDescription,
      /// <summary> The validation error message displayed when the null item is selected. </summary>
      NullItemValidationMessage
    }

    // static members

    private static readonly Type[] s_supportedPropertyInterfaces = new[]
                                                                   {
                                                                       typeof(IBusinessObjectBooleanProperty)
                                                                   };

    // member fields
    private bool? _value;

    private readonly Style _labelStyle;

    private bool _showDescription = true;

    private PlainTextString _errorMessage;
    private ReadOnlyCollection<BaseValidator>? _validators;

    // construction and disposing

    public BocBooleanValue ()
    {
      _labelStyle = new Style();
    }

    // methods and properties

    protected override IBusinessObjectConstraintVisitor CreateBusinessObjectConstraintVisitor ()
    {
      return new BocBooleanValueConstraintVisitor(this);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer));
    }

    protected virtual IBocBooleanValueRenderer CreateRenderer ()
    {
      return ServiceLocator.GetInstance<IBocBooleanValueRenderer>();
    }

    protected virtual BocBooleanValueRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      Assertion.IsNotNull(Context, "Context must not be null.");

      return new BocBooleanValueRenderingContext(Context, writer, this);
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
      var validatorFactory = ServiceLocator.GetInstance<IBocBooleanValueValidatorFactory>();
      _validators = validatorFactory.CreateValidators(this, isReadOnly).ToList().AsReadOnly();

      OverrideValidatorErrorMessages();

      return _validators;
    }

    private void OverrideValidatorErrorMessages ()
    {
      if (!_errorMessage.IsEmpty)
        UpdateValidatorErrorMessages<CompareValidator>(_errorMessage);
    }

    private void UpdateValidatorErrorMessages<T> (PlainTextString errorMessage) where T : BaseValidator
    {
      var validator = _validators.GetValidator<T>();
      if (validator != null)
        validator.ErrorMessage = errorMessage.GetValue();
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents(htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender);
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
    ///   interface.
    /// </summary>
    /// <returns> 
    ///   A <see cref="String"/> <see cref="Array"/> containing the <see cref="Control.ClientID"/> of the
    ///   <see cref="HiddenField"/> if the control is in edit mode, or an empty array if the control is read-only.
    /// </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      return IsReadOnly ? new string[0] : new[] { GetValueName() };
    }

    string IBocBooleanValue.GetValueName ()
    {
      return GetValueName();
    }

    /// <summary>
    /// Gets a name (ID) to use for the hidden field needed to store the value of the control client-side.
    /// </summary>
    /// <returns>The control's <see cref="Control.ClientID"/> postfixed with a constant id for the hidden field.</returns>
    protected string GetValueName ()
    {
      return ClientID + c_valueName;
    }

    string IBocBooleanValue.GetDisplayValueName ()
    {
      return GetDisplayValueName();
    }

    /// <summary>
    /// Gets a name (ID) to use for the hyperlink used to change the value of the control client-side.
    /// </summary>
    /// <returns>The control's <see cref="Control.ClientID"/> postfixed with a constant id for the hyperlink.</returns>
    protected string GetDisplayValueName ()
    {
      return ClientID + c_displayValueName;
    }

    protected override bool? GetValue ()
    {
      return _value;
    }

    protected override void SetValue (bool? value)
    {
      _value = value;
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocBooleanValue"/> contains a value. </summary>
    public override bool HasValue
    {
      get { return _value.HasValue; }
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
        UpdateValidatorErrorMessages<CompareValidator>(_errorMessage);
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
    /// <value> The control itself. </value>
    public override Control TargetControl
    {
      get { return this; }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the <see cref="HyperLink"/> if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public override string? FocusID
    {
      get { return IsReadOnly ? null : GetValueName(); }
    }

    /// <summary>
    ///   Gets the <see cref="Style"/> that you want to apply to the <see cref="Label"/> used for displaying the 
    ///   description. 
    /// </summary>
    /// <value>The <see cref="Style"/> object to be applied on the label part of the control.</value>
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
    /// <value> <see langword="true"/> to enable the description. The default value is <see langword="true"/>. </value>
    /// <remarks>
    ///   Note that the description is always displayed instead of the checkbox when the <see cref="BocBooleanValue"/> is readonly.
    /// </remarks>
    [Description("The flag that determines whether to show the description next to the checkbox during editing.")]
    [Category("Appearance")]
    [DefaultValue(true)]
    public bool ShowDescription
    {
      get { return _showDescription; }
      set { _showDescription = value; }
    }

    /// <summary>
    ///   The <see cref="BocBooleanValue"/> supports properties of type <see cref="IBusinessObjectBooleanProperty"/>.
    /// </summary>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed 
    ///   between postbacks.
    /// </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadPostData/*' />
    protected override bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      string? newValueAsString = PageUtility.GetPostBackCollectionItem(Page!, GetValueName());
      bool? newValue = null;
      bool isDataChanged = false;
      if (newValueAsString != null)
      {
        if (newValueAsString != c_nullString)
          newValue = bool.Parse(newValueAsString);
        isDataChanged = _value != newValue;
      }
      if (isDataChanged)
      {
        _value = newValue;
        IsDirty = true;
      }
      return isDataChanged;
    }

    /// <summary>
    /// Loads the necessary resources from the <see cref="IResourceManager"/> obtained from <see cref="GetResourceManager"/>.
    /// </summary>
    /// <param name="e">ignored</param>
    protected override void OnPreRender (EventArgs e)
    {
      EnsureChildControls();
      base.OnPreRender(e);

      var resourceManager = GetResourceManager();
      LoadResources(resourceManager, GlobalizationService);
    }

    /// <summary>
    /// Loads the value in addition to the base state.
    /// </summary>
    /// <param name="savedState">The state object created by <see cref="SaveControlState"/>.</param>
    protected override void LoadControlState (object? savedState)
    {
      object?[] values = (object?[])savedState!;

      base.LoadControlState(values[0]);
      _value = (bool?)values[1];
    }

    /// <summary>
    /// Saves the current value in addtion to the base state.
    /// </summary>
    /// <returns>An object containing the state to load in the next lifecycle.</returns>
    protected override object SaveControlState ()
    {
      object?[] values = new object?[2];

      values[0] = base.SaveControlState();
      values[1] = _value;

      return values;
    }

    /// <summary>
    /// Override this method to change the default look of the <see cref="BocBooleanValue"/>
    /// </summary>
    /// <returns>A <see cref="BocBooleanValueResourceSet"/> containing the icons and descriptions to use
    /// for representing the control's value.</returns>
    protected virtual BocBooleanValueResourceSet? CreateResourceSet ()
    {
      return null;
    }

    /// <summary> Returns the <see cref="IResourceManager"/> used to access the resources for this control. </summary>
    public override IResourceManager GetResourceManager ()
    {
      return GetResourceManager(typeof(ResourceIdentifier));
    }

    IResourceManager IControlWithResourceManager.GetResourceManager ()
    {
      return GetResourceManager();
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected override void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      base.LoadResources(resourceManager, globalizationService);

      string? key;
      key = ResourceManagerUtility.GetGlobalResourceKey(TrueDescription.GetValue());
      if (!string.IsNullOrEmpty(key))
        TrueDescription = resourceManager.GetText(key);

      key = ResourceManagerUtility.GetGlobalResourceKey(FalseDescription.GetValue());
      if (! string.IsNullOrEmpty(key))
        FalseDescription = resourceManager.GetText(key);

      key = ResourceManagerUtility.GetGlobalResourceKey(NullDescription.GetValue());
      if (! string.IsNullOrEmpty(key))
        NullDescription = resourceManager.GetText(key);

      key = ResourceManagerUtility.GetGlobalResourceKey(ErrorMessage.GetValue());
      if (! string.IsNullOrEmpty(key))
        ErrorMessage = resourceManager.GetText(key);
    }

    /// <summary> <see cref="BocBooleanValue"/> supports only scalar properties. </summary>
    /// <param name="isList">Determines whether the property is a scalar (<see langword="false"/>) or a list (<see langword="true"/>). </param>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return ! isList;
    }

    BocBooleanValueResourceSet? IBocBooleanValue.CreateResourceSet ()
    {
      return CreateResourceSet();
    }

    protected override string ControlType
    {
      get { return "BocBooleanValue"; }
    }
  }
}
