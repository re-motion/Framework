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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation
{
  /// <summary>
  /// Abstract base class for <see cref="BocBooleanValue"/> and <see cref="BocCheckBox"/>, 
  /// both of which represent boolean values with checkboxes.
  /// </summary>
  public abstract class BocBooleanValueBase : BusinessObjectBoundEditableWebControl, IBocBooleanValueBase, IPostBackDataHandler, IFocusableControl
  {
    private bool? _autoPostBack;
    private PlainTextString _trueDescription = PlainTextString.Empty;
    private PlainTextString _falseDescription = PlainTextString.Empty;
    private PlainTextString _nullDescription = PlainTextString.Empty;

    /// <summary> Flag that determines whether the client script will be rendered. </summary>

    private static readonly object s_checkedChangedEvent = new object();

    protected BocBooleanValueBase ()
    {
    }

    /// <summary>
    /// Registers the control as requiring a postback. <seealso cref="Page.RegisterRequiresPostBack"/>.
    /// </summary>
    /// <param name="e">ignored</param>
    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      Page!.RegisterRequiresPostBack(this);
    }

    /// <summary> Occurs when the <see cref="Value"/> property changes between postbacks. </summary>
    [Category("Action")]
    [Description("Fires when the value of the control has changed.")]
    public event EventHandler CheckedChanged
    {
      add { Events.AddHandler(s_checkedChangedEvent, value); }
      remove { Events.RemoveHandler(s_checkedChangedEvent, value); }
    }

    /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\..\doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadValue/*' />
    public override void LoadValue (bool interim)
    {
      if (interim)
        return;

      if (Property == null)
        return;

      if (DataSource == null)
        return;

      bool? value = null;

      if (DataSource.BusinessObject != null)
        value = (bool?)DataSource.BusinessObject.GetProperty(Property);

      LoadValueInternal(value, false);
    }

    /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
    /// <include file='..\..\..\doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/LoadUnboundValue/*' />
    public void LoadUnboundValue (bool? value, bool interim)
    {
      LoadValueInternal(value, interim);
    }

    /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
    protected virtual void LoadValueInternal (bool? value, bool interim)
    {
      if (interim)
        return;

      SetValue(value);
      IsDirty = false;
    }

    /// <summary> Saves the <see cref="Value"/> into the bound <see cref="IBusinessObject"/>. </summary>
    /// <include file='..\..\..\doc\include\UI\Controls\BocBooleanValue.xml' path='BocBooleanValue/SaveValue/*' />
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


    /// <summary> Gets or sets the current value. </summary>
    /// <value> The boolean value currently displayed or <see langword="null"/>. </value>
    /// <remarks> The dirty state is set when the value is set. </remarks>
    [Browsable(false)]
    public new bool? Value
    {
      get { return GetValue(); }
      set
      {
        IsDirty = true;
        SetValue(value);
      }
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    /// <value>The control's current value, which is a nullable boolean.</value>
    protected override sealed object? ValueImplementation
    {
      get { return Value; }
      set { Value = ArgumentUtility.CheckType<bool?>("value", value); }
    }

    /// <summary>
    /// Gets the value from the backing field.
    /// </summary>
    /// <remarks> Override this member to modify the storage of the value. </remarks>
    protected abstract bool? GetValue ();

    /// <summary>
    /// Sets the value from the backing field.
    /// </summary>
    /// <remarks>
    /// <para>Setting the value via this method does not affect the control's dirty state.</para>
    /// <para>Override this member to modify the storage of the value.</para>
    /// </remarks>
    protected abstract void SetValue (bool? value);

    /// <summary> Gets or sets the <see cref="IBusinessObjectBooleanProperty"/> object this control is bound to. </summary>
    /// <value> An instance of type <see cref="IBusinessObjectBooleanProperty"/>. </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new IBusinessObjectBooleanProperty? Property
    {
      get { return (IBusinessObjectBooleanProperty?)base.Property; }
      set { base.Property = value; }
    }

    /// <summary> Gets the ID of the element to receive the focus when the page is loaded. </summary>
    /// <value>
    ///   Returns the <see cref="Control.ClientID"/> of the <see cref="CheckBox"/> if the control is in edit mode, 
    ///   otherwise <see langword="null"/>. 
    /// </value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public abstract string? FocusID { get; }

    /// <summary>
    ///   Gets the <see cref="Style"/> that you want to apply to the <see cref="Label"/> used for displaying the 
    ///   description. 
    /// </summary>
    public abstract Style LabelStyle { get; }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="BusinessObjectBoundWebControl.TargetControl"/>.
    /// </summary>
    /// <value> Always <see langword="true"/>. </value>
    public override bool UseLabel
    {
      get { return true; }
    }

    /// <summary> Gets a flag that determines whether changing the checked state causes an automatic postback.</summary>
    /// <value> 
    ///   <see langword="true"/> to enable automatic postbacks. 
    ///   Defaults to <see langword="null"/>, which is interpreted as 
    ///   <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///   Use <see cref="IsAutoPostBackEnabled"/> to evaluate this property.
    /// </remarks>
    [Description("Automatically postback to the server after the checked state is modified. Undefined is interpreted as false.")]
    [Category("Behavior")]
    [DefaultValue(typeof(bool?), "")]
    [NotifyParentProperty(true)]
    public bool? AutoPostBack
    {
      get { return _autoPostBack; }
      set { _autoPostBack = value; }
    }

    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="true"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="true"/>. The default value is an empty <see cref="PlainTextString"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("The description displayed when the checkbox is set to True.")]
    [Category("Appearance")]
    [DefaultValue(typeof(PlainTextString), "")]
    public PlainTextString TrueDescription
    {
      get { return _trueDescription; }
      set { _trueDescription = value; }
    }

    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="false"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="false"/>. The default value is an empty <see cref="PlainTextString"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("The description displayed when the checkbox is set to False.")]
    [Category("Appearance")]
    [DefaultValue(typeof(PlainTextString), "")]
    public PlainTextString FalseDescription
    {
      get { return _falseDescription; }
      set { _falseDescription = value; }
    }

    /// <summary> Gets or sets the description displayed when the checkbox is set to <see langword="null"/>. </summary>
    /// <value> 
    ///   The text displayed for <see langword="null"/>. The default value is an empty <see cref="PlainTextString"/>.
    ///   In case of the default value, the text is read from the resources for this control.
    /// </value>
    [Description("The description displayed when the checkbox is set to null.")]
    [Category("Appearance")]
    [DefaultValue(typeof(PlainTextString), "")]
    public PlainTextString NullDescription
    {
      get { return _nullDescription; }
      set { _nullDescription = value; }
    }

    /// <summary> Gets the evaluated value for the <see cref="AutoPostBack"/> property. </summary>
    /// <value> <see langword="true"/> if <see cref="AutoPostBack"/> is <see langword="true"/>. </value>
    protected bool IsAutoPostBackEnabled
    {
      get { return AutoPostBack == true; }
    }

    /// <summary>
    ///   Uses the <paramref name="postCollection"/> to determine whether the value of this control has been changed 
    ///   between postbacks.
    /// </summary>
    protected abstract bool LoadPostData (string postDataKey, NameValueCollection postCollection);

    /// <summary> Called when the state of the control has changed between postbacks. </summary>
    protected virtual void RaisePostDataChangedEvent ()
    {
      if (! IsReadOnly && Enabled)
        OnCheckedChanged();
    }

    /// <summary> Fires the <see cref="CheckedChanged"/> event. </summary>
    protected virtual void OnCheckedChanged ()
    {
      EventHandler? eventHandler = (EventHandler?)Events[s_checkedChangedEvent];
      if (eventHandler != null)
        eventHandler(this, EventArgs.Empty);
    }

    /// <summary> The <see cref="BocCheckBox"/> supports only scalar properties. </summary>
    /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
    /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
    protected override bool SupportsPropertyMultiplicity (bool isList)
    {
      return ! isList;
    }

    /// <summary> Invokes the <see cref="BocBooleanValue.LoadPostData(string,System.Collections.Specialized.NameValueCollection)"/> method. </summary>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      if (IsLoadPostDataRequired())
        return LoadPostData(postDataKey, postCollection);

      return false;
    }

    /// <summary> Invokes the <see cref="RaisePostDataChangedEvent"/> method. </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
      RaisePostDataChangedEvent();
    }

    bool IBocBooleanValueBase.IsAutoPostBackEnabled
    {
      get { return IsAutoPostBackEnabled; }
    }

    IEnumerable<string> IControlWithLabel.GetLabelIDs ()
    {
      return GetLabelIDs();
    }

    IEnumerable<PlainTextString> IBocBooleanValueBase.GetValidationErrors ()
    {
      return GetRegisteredValidators()
          .Where(v => !v.IsValid)
          .Select(v => v.ErrorMessage)
          .Select(PlainTextString.CreateFromText)
          .Distinct();
    }

    protected abstract string ControlType { get; }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return ControlType; }
    }

    public abstract IResourceManager GetResourceManager ();
  }
}
