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
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   <b>BusinessObjectBoundWebControl</b> is the <see langword="abstract"/> default implementation of 
  ///   <see cref="IBusinessObjectBoundWebControl"/>.
  /// </summary>
  /// <seealso cref="IBusinessObjectBoundWebControl"/>
  // It is required to use a Designer from the same assambly as is the control (or the GAC etc), 
  // otherwise the VS 2003 Toolbox will have trouble loading the assembly.
  public abstract class BusinessObjectBoundWebControl : WebControl, IBusinessObjectBoundWebControl
  {
    #region BusinessObjectBinding implementation

    /// <summary>Gets the <see cref="BusinessObjectBinding"/> object used to manage the binding for this <see cref="BusinessObjectBoundWebControl"/>.</summary>
    /// <value> The <see cref="BusinessObjectBinding"/> instance used to manage this control's binding. </value>
    [Browsable(false)]
    public BusinessObjectBinding Binding
    {
      get { return _binding; }
    }

    /// <summary>Gets or sets the <see cref="IBusinessObjectDataSource"/> this <see cref="IBusinessObjectBoundWebControl"/> is bound to.</summary>
    /// <value> An <see cref="IBusinessObjectDataSource"/> providing the current <see cref="IBusinessObject"/>. </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public IBusinessObjectDataSource? DataSource
    {
      get { return _binding.DataSource; }
      set { _binding.DataSource = value; }
    }

    /// <summary>Gets or sets the string representation of the <see cref="Property"/>.</summary>
    /// <value> 
    ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for the 
    ///   <see cref="IBusinessObjectProperty"/>. 
    /// </value>
    [Category("Data")]
    [Description("The string representation of the Property.")]
    [DefaultValue("")]
    [MergableProperty(false)]
    public string? PropertyIdentifier
    {
      get { return _binding.PropertyIdentifier; }
      set { _binding.PropertyIdentifier = value; }
    }

    /// <summary>Gets or sets the <see cref="IBusinessObjectProperty"/> used for accessing the data to be loaded into <see cref="Value"/>.</summary>
    /// <value>An <see cref="IBusinessObjectProperty"/> that is part of the bound <see cref="IBusinessObject"/>'s <see cref="IBusinessObjectClass"/>.</value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectProperty? Property
    {
      get { return _binding.Property; }
      set { _binding.Property = value; }
    }

    /// <summary>
    ///   Gets or sets the <b>ID</b> of the <see cref="IBusinessObjectDataSourceControl"/> encapsulating the <see cref="IBusinessObjectDataSource"/> 
    ///   this  <see cref="IBusinessObjectBoundWebControl"/> is bound to.
    /// </summary>
    /// <value>A string set to the <b>ID</b> of an <see cref="IBusinessObjectDataSourceControl"/> inside the current naming container.</value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Data")]
    [Description("The ID of the BusinessObjectDataSourceControl control used as data source.")]
    [DefaultValue("")]
    public string? DataSourceControl
    {
      get { return _binding.DataSourceControl; }
      set { _binding.DataSourceControl = value; }
    }

    /// <summary>Tests whether this <see cref="BusinessObjectBoundWebControl"/> can be bound to the <paramref name="property"/>.</summary>
    /// <param name="property">The <see cref="IBusinessObjectProperty"/> to be tested. Must not be <see langword="null"/>.</param>
    /// <returns>
    ///   <list type="bullet">
    ///     <item><see langword="true"/> is <see cref="SupportedPropertyInterfaces"/> is null.</item>
    ///     <item><see langword="false"/> if the <see cref="DataSource"/> is in <see cref="DataSourceMode.Search"/> mode.</item>
    ///     <item>Otherwise, <see langword="IsPropertyInterfaceSupported"/> is evaluated and returned as result.</item>
    ///   </list>
    /// </returns>
    public virtual bool SupportsProperty (IBusinessObjectProperty property)
    {
      return _binding.SupportsProperty(property);
    }

    /// <summary>Gets a flag specifying whether the <see cref="IBusinessObjectBoundControl"/> has a valid binding configuration.</summary>
    /// <remarks>
    ///   The configuration is considered invalid if data binding is configured for a property that is not available for the bound class or object.
    /// </remarks>
    /// <value> 
    ///   <list type="bullet">
    ///     <item><see langword="true"/> if the <see cref="DataSource"/> or the <see cref="Property"/> is <see langword="null"/>.</item>
    ///     <item>The result of the <see cref="IBusinessObjectProperty.IsAccessible">IBusinessObjectProperty.IsAccessible</see> method.</item>
    ///     <item>Otherwise, <see langword="false"/> is returned.</item>
    ///   </list>
    /// </value>
    [Browsable(false)]
    public bool HasValidBinding
    {
      get { return _binding.HasValidBinding; }
    }

    #endregion

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectService"/> from the <paramref name="businessObjectProvider"/> and queries it for an <see cref="IconInfo"/> 
    ///   object.
    /// </summary>
    /// <param name="businessObject"> 
    ///   The <see cref="IBusinessObject"/> to be passed to the <see cref="IBusinessObjectWebUIService"/>'s 
    ///   <see cref="IBusinessObjectWebUIService.GetIcon"/> method.
    /// </param>
    /// <param name="businessObjectProvider"> 
    ///   The <see cref="IBusinessObjectProvider"/> to be used to get the <see cref="IconInfo"/> object. Must not be <see langowrd="null"/>. 
    /// </param>
    public static IconInfo? GetIcon (IBusinessObject? businessObject, IBusinessObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull("businessObjectProvider", businessObjectProvider);

      var webUIService = businessObjectProvider.GetService<IBusinessObjectWebUIService>();

      if (webUIService != null)
        return webUIService.GetIcon(businessObject);

      return null;
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectService"/> from the <paramref name="businessObjectProvider"/> and queries it for a <see cref="string"/>
    ///   to be used as tool-tip.
    /// </summary>
    /// <param name="businessObject"> 
    ///   The <see cref="IBusinessObject"/> to be passed to the <see cref="IBusinessObjectWebUIService"/>'s 
    ///   <see cref="IBusinessObjectWebUIService.GetIcon"/> method.
    /// </param>
    /// <param name="businessObjectProvider"> 
    ///   The <see cref="IBusinessObjectProvider"/> to be used to get the <see cref="IconInfo"/> object. Must not be <see langowrd="null"/>. 
    /// </param>
    public static string? GetToolTip (IBusinessObject businessObject, IBusinessObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull("businessObjectProvider", businessObjectProvider);

      var webUIService = businessObjectProvider.GetService<IBusinessObjectWebUIService>();

      if (webUIService != null)
        return webUIService.GetToolTip(businessObject);

      return null;
    }

    public static HelpInfo? GetHelpInfo (IBusinessObjectBoundWebControl control)
    {
      ArgumentUtility.CheckNotNull("control", control);

      var dataSource = control.DataSource;
      if (dataSource == null)
        return null;

      var businessObjectClass = dataSource.BusinessObjectClass;
      if (businessObjectClass == null)
        return null;

      var businessObjectProvider = businessObjectClass.BusinessObjectProvider;
      if (businessObjectProvider == null)
        return null;

      var webUIService = businessObjectProvider.GetService<IBusinessObjectWebUIService>();
      if (webUIService != null)
        return webUIService.GetHelpInfo(control, businessObjectClass, control.Property, dataSource.BusinessObject);

      return null;
    }

    private readonly BusinessObjectBinding _binding;

    private readonly Dictionary<Tuple<Type, Control>, IResourceManager> _resourceManagerCache =
        new Dictionary<Tuple<Type, Control>, IResourceManager>();

    private bool _controlExistedInPreviousRequest;

    private IReadOnlyCollection<string> _assignedLabelIDs = Array.Empty<string>();

    /// <summary> Creates a new instance of the BusinessObjectBoundWebControl type. </summary>
    protected BusinessObjectBoundWebControl ()
    {
      _binding = new BusinessObjectBinding(this);
    }

    /// <remarks>Calls <see cref="Control.EnsureChildControls"/> and the <see cref="BusinessObjectBinding.EnsureDataSource"/> method.</remarks>
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      EnsureChildControls();
      _binding.EnsureDataSource();
      if (Page != null)
      {
        Page.RegisterRequiresControlState(this);
        RegisterHtmlHeadContents(HtmlHeadAppender.Current);
      }
    }

    protected override void OnUnload (EventArgs e)
    {
      _binding.UnregisterDataSource();
      base.OnUnload(e);
    }

    /// <summary>Gets or sets a value that determines whether a server control is rendered as UI on the page.</summary>
    /// <value> 
    ///   <para>
    ///     The <b>set accessor</b> passes the value to the base class's <b>Visible</b> property.
    ///   </para><para>
    ///     The <b>get accessor</b> ANDs the base class's <b>Visible</b> setting with the value of the <see cref="HasValidBinding"/> property.
    ///   </para>
    /// </value>
    /// <remarks>
    ///   The control only saves the set value of <b>Visible</b> into the view state. Therefor the control can change its visibilty during during 
    ///   subsequent postbacks.
    /// </remarks>
    public override bool Visible
    {
      get
      {
        if (!base.Visible)
          return false;

        return HasValidBinding;
      }
      set { base.Visible = value; }
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <seealso cref="IBusinessObjectBoundControl.LoadValue">IBusinessObjectBoundControl.LoadValue</seealso>
    public abstract void LoadValue (bool interim);

    /// <summary> Gets or sets the value provided by the <see cref="IBusinessObjectBoundControl"/>. </summary>
    /// <value> An object or boxed value. </value>
    /// <remarks>
    ///   <para>
    ///     Override <see cref="ValueImplementation"/> to define the behaviour of <see cref="Value"/>. 
    ///   </para><para>
    ///     Redefine <see cref="Value"/> using the keyword <see langword="new"/> to provide a typesafe implementation in derived classes.
    ///   </para>
    /// </remarks>
    [Browsable(false)]
    public object? Value
    {
      get { return ValueImplementation; }
      set { ValueImplementation = value; }
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    /// <value> An object or boxed value. </value>
    /// <remarks>The implementation should be <see langword="sealed"/> since it is only possible to do one meaningful re-definition of <see cref="Value"/>.</remarks>
    [Browsable(false)]
    protected abstract object? ValueImplementation { get; set; }

    /// <summary>Gets a flag indicating whether the <see cref="BusinessObjectBoundWebControl"/> contains a value. </summary>
    /// <value><see langword="true" /> if the <see cref="BusinessObjectBoundWebControl"/> contains a value. </value>
    /// <remarks>
    /// The flag only specifies the presense of data. It does not specify whether the data is in a format compatible with the <see cref="Property"/>.
    /// For this, a separate validation step is required.
    /// </remarks>
    [Browsable(false)]
    public abstract bool HasValue { get; }

    /// <summary>
    /// Overrides the base method to temporarily enable the control before adding attributes.
    /// </summary>
    /// <param name="writer">The <see cref="HtmlTextWriter"/> object to use for adding attributes.</param>
    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      bool tempEnabled = Enabled;
      if (!tempEnabled)
        Enabled = true;
      base.AddAttributesToRender(writer);
      if (!tempEnabled)
        Enabled = false;
    }

    /// <summary>Gets a <see cref="ControlCollection"/> object that represents the child
    /// controls for a specified server control in the UI hierarchy.</summary>
    /// <remarks> Calls <see cref="Control.EnsureChildControls"/>. </remarks>
    public override ControlCollection Controls
    {
      get
      {
        EnsureChildControls();
        return base.Controls;
      }
    }

    Type[]? IBusinessObjectBoundWebControl.SupportedPropertyInterfaces
    {
      get { return SupportedPropertyInterfaces; }
    }

    /// <summary>
    ///   Gets the interfaces derived from <see cref="IBusinessObjectProperty"/> supported by this control, or <see langword="null"/> if no 
    ///   restrictions are made.
    /// </summary>
    /// <value> <see langword="null"/> in the default implementation. </value>
    /// <remarks> Used by <see cref="SupportsProperty"/>. </remarks>
    [Browsable(false)]
    protected virtual Type[]? SupportedPropertyInterfaces
    {
      get { return null; }
    }

    bool IBusinessObjectBoundWebControl.SupportsPropertyMultiplicity (bool isList)
    {
      return SupportsPropertyMultiplicity(isList);
    }

    /// <summary> Indicates whether properties with the specified multiplicity are supported. </summary>
    /// <remarks> Used by <see cref="SupportsProperty"/>. </remarks>
    /// <param name="isList"> <see langword="true"/> if the property is a list property. </param>
    /// <returns> <see langword="true"/> if the multiplicity specified by <paramref name="isList"/> is supported. </returns>
    protected virtual bool SupportsPropertyMultiplicity (bool isList)
    {
      return !isList;
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
    /// <param name="localResourcesType"> 
    ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it. Typically an <b>enum</b> or the derived class itself.
    /// </param>
    /// <returns>An <see cref="IResourceManager"/> from which all resources for this control can be obtained.</returns>
    protected IResourceManager GetResourceManager (Type localResourcesType)
    {
      ArgumentUtility.CheckNotNull("localResourcesType", localResourcesType);

      return _resourceManagerCache.GetOrCreateValue(
          Tuple.Create(localResourcesType, NamingContainer),
          key =>
          {
            var localResourceManager = GlobalizationService.GetResourceManager(localResourcesType);
            var namingContainerResourceManager = ResourceManagerUtility.GetResourceManager(NamingContainer, true);

            return ResourceManagerSet.Create(namingContainerResourceManager, localResourceManager);
          });
    }

    /// <summary> Gets the text to be written into the label for this control. </summary>
    /// <value> <see cref="WebString.Empty"/> for the default implementation. </value>
    [Browsable(false)]
    public virtual WebString DisplayName
    {
      get { return (Property != null) ? WebString.CreateFromText(Property.DisplayName) : WebString.Empty; }
    }

    /// <summary>Regsiteres stylesheet and script files with the <see cref="HtmlHeadAppender"/>.</summary>
    public virtual void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
    }

    /// <summary>Gets an instance of the <see cref="HelpInfo"/> type, which contains all information needed for rendering a help-link.</summary>
    [Browsable(false)]
    public virtual HelpInfo? HelpInfo
    {
      get { return GetHelpInfo(this); }
    }

    /// <summary>Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its <see cref="Control.ClientID"/>.</summary>
    /// <value> This instance for the default implementation. </value>
    [Browsable(false)]
    public virtual Control TargetControl
    {
      get { return this; }
    }

    /// <summary>Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the <see cref="TargetControl"/>.</summary>
    [Browsable(false)]
    public abstract bool UseLabel { get; }

    public void AssignLabels (IEnumerable<string> labelIDs)
    {
      ArgumentUtility.CheckNotNull("labelIDs", labelIDs);

      _assignedLabelIDs = labelIDs.ToList().AsReadOnly();
    }

    protected virtual IEnumerable<string> GetLabelIDs ()
    {
      return _assignedLabelIDs;
    }

    /// <summary> Evaluates whether this control is in <b>Design Mode</b>. </summary>
    /// <value><see langword="true"/> if the control is currently rendered by the Visual Studio Designer.</value>
    [Obsolete("Design-mode support has been removed, method always returns false. (Version: 3.0.0)", false)]
    protected bool IsDesignMode
    {
      get { return false; }
    }

    bool ISmartControl.IsRequired
    {
      get { return false; }
    }

    IEnumerable<BaseValidator> ISmartControl.CreateValidators ()
    {
      return Enumerable.Empty<BaseValidator>();
    }

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }

    /// <summary> Gets a flag whether the control already existed in the previous page life cycle. </summary>
    /// <remarks> 
    ///   This property utilizes the <see cref="LoadControlState"/> method for determining a post back. 
    /// It is therefor only useful after the load control state phase of the page life cycle.
    /// </remarks>
    /// <value> <see langword="true"/> if the control has been on the page in the previous life cycle. </value>
    protected bool ControlExistedInPreviousRequest
    {
      get { return _controlExistedInPreviousRequest; }
    }

    protected virtual IServiceLocator ServiceLocator
    {
      get { return SafeServiceLocator.Current; }
    }

    protected new HttpContextBase? Context
    {
      get { return Page != null ? Page.Context : null; }
    }

    protected ResourceTheme ResourceTheme
    {
      get { return ServiceLocator.GetInstance<ResourceTheme>(); }
    }

    protected IGlobalizationService GlobalizationService
    {
      get { return ServiceLocator.GetInstance<IGlobalizationService>(); }
    }

    protected override void LoadControlState (object? savedState)
    {
      base.LoadControlState(savedState);
      _controlExistedInPreviousRequest = true;
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected virtual void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      string? key;
      key = ResourceManagerUtility.GetGlobalResourceKey(AccessKey);
      if (!string.IsNullOrEmpty(key))
        AccessKey = resourceManager.GetString(key);

      key = ResourceManagerUtility.GetGlobalResourceKey(ToolTip);
      if (!string.IsNullOrEmpty(key))
        ToolTip = resourceManager.GetString(key);
    }

    //  /// <summary>
    //  ///   Occurs after either the <see cref="Property"/> property or the <see cref="PropertyIdentifier"/> property is assigned a new value.
    //  /// </summary>
    //  /// <remarks>
    //  ///   Note that this event does not occur if the property path is modified, only if a new one is assigned.
    //  /// </remarks>
    //  public event BindingChangedEventHandler BindingChanged;

    //  private bool _onLoadCalled = false;
    //  private bool _propertyBindingChangedBeforeOnLoad = false;

    //  protected override void OnLoad (EventArgs e)
    //  {
    //    base.OnLoad (e);
    //    _onLoadCalled = true;
    //    if (_propertyBindingChangedBeforeOnLoad)
    //      OnBindingChanged (null, null);
    //  }

    //  /// <summary>
    //  /// Raises the <see cref="PropertyChanged"/> event.
    //  /// </summary>
    //  protected virtual void OnBindingChanged (IBusinessObjectProperty previousProperty, IBusinessObjectDataSource previousDataSource)
    //  {
    //    if (! _onLoadCalled)
    //    {
    //      _propertyBindingChangedBeforeOnLoad = true;
    //      return;
    //    }
    //    if (BindingChanged != null)
    //      BindingChanged (this, new BindingChangedEventArgs (previousProperty, previousDataSource));
    //  }
  }

  ///// <summary>
  /////   Provides data for the <cBindingChanged</c> event.
  /////   <seealso cref="BusinessObjectBoundControl.BindingChanged"/>
  ///// </summary>
  //public class BindingChangedEventArgs: EventArgs
  //{
  //  /// <summary>
  //  ///   The value of the <c>PropertyPath</c> property before the change took place.
  //  /// </summary>
  //  public readonly IBusinessObjectProperty PreviousProperty;
  //  public readonly IBusinessObjectDataSource PreviuosDataSource;
  //
  //  public BindingChangedEventArgs (IBusinessObjectProperty previousProperty, IBusinessObjectDataSource previousDataSource)
  //  {
  //    PreviousProperty = previousProperty;
  //    PreviuosDataSource = previousDataSource;
  //  }
  //}
  //
  //public delegate void BindingChangedEventHandler (object sender, BindingChangedEventArgs e);
}
