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
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

  /// <summary> This control can be used to render text without any escaping applied. </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocLiteral.xml' path='BocLiteral/Class/*' />
  [ToolboxItemFilter("System.Web.UI")]
  public class BocLiteral : Control, IBusinessObjectBoundWebControl
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

    //  constants

    //  statics

    private static readonly Type[] s_supportedPropertyInterfaces = new Type[] { typeof(IBusinessObjectStringProperty) };

    // types

    // fields

    private BusinessObjectBinding _binding;
    private string? _value = string.Empty;
    private LiteralMode _mode = LiteralMode.Transform;

    public BocLiteral ()
    {
      _binding = new BusinessObjectBinding(this);
    }

    /// <remarks>Calls <see cref="Control.EnsureChildControls"/> and the <see cref="BusinessObjectBinding.EnsureDataSource"/> method.</remarks>
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      EnsureChildControls();
      _binding.EnsureDataSource();
      Page!.RegisterRequiresControlState(this);
    }

    protected override void OnUnload (EventArgs e)
    {
      _binding.UnregisterDataSource();
      base.OnUnload(e);
    }

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

    protected override void Render (HtmlTextWriter writer)
    {
      if (!string.IsNullOrEmpty(_value))
      {
        if (_mode != LiteralMode.Encode)
          writer.Write(_value);
        else
          HttpUtility.HtmlEncode(_value, writer);
      }
    }

    protected override void LoadControlState (object? savedState)
    {
      object?[] values = (object[]?)savedState!;
      base.LoadControlState(values[0]);
      _mode = (LiteralMode)values[1]!;
    }

    protected override object SaveControlState ()
    {
      object?[] values = new object?[4];
      values[0] = base.SaveControlState();
      values[1] = _mode;
      return values;
    }


    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocLiteral.xml' path='BocLiteral/LoadValue/*' />
    public virtual void LoadValue (bool interim)
    {
      if (Property == null)
        return;

      if (DataSource == null)
        return;

      string? value = null;

      if (DataSource.BusinessObject != null)
        value = (string?)DataSource.BusinessObject.GetProperty(Property);

      LoadValueInternal(value, interim);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <param name="value"> A <see cref="String"/> to load or <see langword="null"/>. </param>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <include file='..\..\doc\include\UI\Controls\BocLiteral.xml' path='BocLiteral/LoadUnboundValue/*' />
    public void LoadUnboundValue (string? value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (string? value, bool interim)
    {
      Value = value;
    }

    /// <summary> Gets or sets the current value. </summary>
    [Description("The text to be shown in for the BocLiteral.")]
    [Category("Data")]
    [DefaultValue("")]
    public string? Value
    {
      get { return _value; }
      set { _value = value; }
    }

    /// <summary>Gets a flag indicating whether the <see cref="BocLiteral"/> contains a value. </summary>
    public bool HasValue
    {
      get { return _value != null && _value.Trim().Length > 0; }
    }

    object? IBusinessObjectBoundControl.Value
    {
      get { return ValueImplementation; }
      set { ValueImplementation = value; }
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    protected virtual object? ValueImplementation
    {
      get { return Value; }
      set { Value = (string?)value; }
    }

    bool IBusinessObjectBoundWebControl.SupportsPropertyMultiplicity (bool isList)
    {
      return SupportsPropertyMultiplicity(isList);
    }

    /// <summary> The <see cref="BocLiteral"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <remarks> Used by <see cref="SupportsProperty"/>. </remarks>
    /// <param name="isList"> <see langword="true"/> if the property is a list property. </param>
    protected virtual bool SupportsPropertyMultiplicity (bool isList)
    {
      return !isList;
    }

    Type[] IBusinessObjectBoundWebControl.SupportedPropertyInterfaces
    {
      get { return SupportedPropertyInterfaces; }
    }

    /// <summary>The <see cref="BocLiteral"/> supports properties of type <see cref="IBusinessObjectStringProperty"/>.</summary>
    /// <remarks> Used by <see cref="SupportsProperty"/>. </remarks>
    protected virtual Type[] SupportedPropertyInterfaces
    {
      get { return s_supportedPropertyInterfaces; }
    }

    /// <summary> Gets the text to be written into the label for this control. </summary>
    /// <value> <see cref="WebString.Empty"/> for the default implementation. </value>
    [Browsable(false)]
    public virtual WebString DisplayName
    {
      get { return (Property != null) ? WebString.CreateFromText(Property.DisplayName) : WebString.Empty; }
    }

    void ISmartControl.RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
    }

    HelpInfo? ISmartControl.HelpInfo
    {
      get { return BusinessObjectBoundWebControl.GetHelpInfo(this); }
    }

    bool ISmartControl.UseLabel
    {
      get { return false; }
    }

    void ISmartControl.AssignLabels (IEnumerable<string> labelIDs)
    {
      ArgumentUtility.CheckNotNull("labelIDs", labelIDs);

      //BocLiteral does not have a root element that could be labeled.
    }

    Control ISmartControl.TargetControl
    {
      get { return (Control)this; }
    }

    bool ISmartControl.IsRequired
    {
      get { return false; }
    }

    IEnumerable<BaseValidator> ISmartControl.CreateValidators ()
    {
      return Enumerable.Empty<BaseValidator>();
    }

    IPage? IControl.Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }

    [Category("Behavior")]
    [Description("Determines whether the text is transformed or encoded.")]
    [DefaultValue(LiteralMode.Transform)]
    public LiteralMode Mode
    {
      get { return _mode; }
      set { _mode = value; }
    }
  }
}
