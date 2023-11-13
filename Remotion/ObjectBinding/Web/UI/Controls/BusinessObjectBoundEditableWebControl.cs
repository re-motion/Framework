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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   <b>BusinessObjectBoundEditableWebControl</b> is the <see langword="abstract"/> default implementation of 
  ///   <see cref="IBusinessObjectBoundEditableWebControl"/>.
  /// </summary>
  /// <seealso cref="IBusinessObjectBoundEditableWebControl"/>
  public abstract class BusinessObjectBoundEditableWebControl : BusinessObjectBoundWebControl, IBusinessObjectBoundEditableWebControl
  {
    private bool? _required;
    private bool? _readOnly;
    private HashSet<BaseValidator> _validators;
    private bool _isDirty;
    private bool _hasBeenRenderedInPreviousLifecycle;
    private bool _isRenderedInCurrentLifecycle;
    private bool _hasBeenReadOnlyInPreviousLifecycle;
    private bool _hasBeenEnabledInPreviousLifecycle;
    private bool _hasBeenVisibleInPreviousLifecycle;

    /// <summary>
    /// Overrides the base method to call <see cref="ISmartPage.RegisterControlForClientSideDirtyStateTracking"/>
    /// after base initialization.
    /// </summary>
    /// <param name="e">ignored</param>
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      if (Page is ISmartPage)
        ((ISmartPage) Page).RegisterControlForDirtyStateTracking (this);
    }

    /// <summary> Gets or sets a flag that specifies whether the value of the control is required. </summary>
    /// <remarks>
    ///   Set this property to <see langword="null"/> in order to use the default value 
    ///   (see <see cref="IsRequired"/>).
    /// </remarks>
    [Description ("Explicitly specifies whether the control is required.")]
    [Category ("Data")]
    [DefaultValue (typeof (bool?), "")]
    public bool? Required
    {
      get { return _required; }
      set { _required = value; }
    }

    /// <summary> Gets or sets a flag that specifies whether the control should be displayed in read-only mode. </summary>
    /// <remarks>
    ///   Set this property to <see langword="null"/> in order to use the default value 
    ///   (see <see cref="IsReadOnly"/>). Note that if the data source is in read-only mode, the
    ///   control is read-only too, even if this property is set to <see langword="false" />.
    /// </remarks>
    [Description ("Explicitly specifies whether the control should be displayed in read-only mode.")]
    [Category ("Data")]
    [DefaultValue (typeof (bool?), "")]
    public bool? ReadOnly
    {
      get { return _readOnly; }
      set { _readOnly = value; }
    }

    /// <summary> Gets or sets the dirty flag. </summary>
    /// <remarks>
    ///   <para>
    ///     Initially, the <see cref="IsDirty"/> flag is <see langword="false"/>. It is reset to <see langword="false"/>
    ///     when using <see cref="BusinessObjectBoundWebControl.LoadValue"/> to read the value from, or using
    ///     <see cref="SaveValue"/> to write the value back into the <see cref="IBusinessObject"/> bound to the 
    ///     <see cref="BusinessObjectBoundWebControl.DataSource"/>. It is also reset when using <b>LoadUnboundValue</b>
    ///     to set the value for an unbound control.
    ///   </para><para>
    ///     It is set to <see langword="true"/> when the control's <see cref="BusinessObjectBoundWebControl.Value"/> 
    ///     is set by means other than <see cref="BusinessObjectBoundWebControl.LoadValue"/> or <b>LoadUnboundValue</b>.
    ///     It is also set when value changes due to the user submitting new data in the user interface or through the 
    ///     application using the control to modify the contents of the <see cref="BusinessObjectBoundWebControl.Value"/>. 
    ///     (E.g. a row is added to the list of values by invoking a method on the <see cref="BocList"/>.)
    ///   </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   Thrown if the <see cref="IsDirty"/> flag is set to <see langword="true"/> and the control is bound to a read-only
    ///   <see cref="BusinessObjectBoundWebControl.Property"/>.
    /// </exception>
    [Browsable (false)]
    public virtual bool IsDirty
    {
      get { return _isDirty; }
      set
      {
        if (value && DataSource != null && Property != null && IsReadOnlyInDomainModel)
        {
          throw new InvalidOperationException (
              string.Format (
                  "The {0} '{1}' could not be marked as dirty because the bound property '{2}' is read only.",
                  GetType().Name,
                  ID,
                  Property.Identifier));
        }
        _isDirty = value;
      }
    }

    /// <summary> 
    ///   Returns the <see cref="Control.ClientID"/> values of all controls whose value can be modified in the user 
    ///   interface.
    /// </summary>
    /// <returns> A string array containing zero or more client ids. </returns>
    public abstract string[] GetTrackedClientIDs ();

    /// <summary>
    ///   Saves the <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
    /// <returns><see langword="true"/> if the value was saved into the bound <see cref="IBusinessObjectDataSource.BusinessObject"/>.</returns>
    public abstract bool SaveValue (bool interim);

    /// <summary>
    ///   Gets a flag that determines whether the control is to be displayed in read-only mode.
    /// </summary>
    /// <remarks>
    ///     In read-only mode, a <see cref="System.Web.UI.WebControls.Label"/> control is used to display the value.
    ///     Otherwise, a <see cref="System.Web.UI.WebControls.TextBox"/> control is used to display and edit the value.
    /// </remarks>
    /// <value>
    ///   <list type="bullet">
    ///     <item>
    ///       Whether the control is bound or unbound, if the value of the <see cref="ReadOnly"/> property is 
    ///       <see langword="true"/>, <see langword="true"/> is returned.
    ///     </item>
    ///     <item>
    ///       If the control is bound to an <see cref="IBusinessObjectDataSourceControl"/> and 
    ///       <see cref="IBusinessObjectDataSource.Mode">DataSource.Mode</see> is set to 
    ///       <see cref="DataSourceMode.Search"/>, <see langword="false"/> is returned.
    ///     </item>
    ///     <item>
    ///       If the control is unbound (<see cref="BusinessObjectBoundWebControl.DataSource"/> or 
    ///       <see cref="BusinessObjectBoundWebControl.Property"/> is <see langword="null"/>) and the
    ///       <see cref="ReadOnly"/> property is not <see langword="true"/>, 
    ///       <see langword="false"/> is returned.
    ///     </item>
    ///     <item>
    ///       If the control is bound (<see cref="BusinessObjectBoundWebControl.DataSource"/> and  
    ///       <see cref="BusinessObjectBoundWebControl.Property"/> are not <see langword="null"/>), 
    ///       the following rules are used to determine the value of this property:
    ///       <list type="bullet">
    ///         <item>
    ///           If the <see cref="IBusinessObjectDataSource.Mode">DataSource.Mode</see> of the control's
    ///           <see cref="BusinessObjectBoundWebControl.DataSource"/> is set to <see cref="DataSourceMode.Read"/>, 
    ///           <see langword="true"/> is returned.
    ///         </item>
    ///         <item>
    ///           If the <see cref="IBusinessObjectDataSource.BusinessObject">DataSource.BusinessObject</see> is 
    ///           <see langword="null"/> and the control is not in <b>Design Mode</b>, 
    ///           <see langword="true"/> is returned.
    ///         </item>
    ///         <item>
    ///           If the control's <see cref="ReadOnly"/> property is <see langword="false"/>, 
    ///           <see langword="false"/> is returned.
    ///         </item>
    ///         <item>
    ///           Otherwise, <see langword="Property.IsReadOnly"/> is evaluated and returned.
    ///         </item>
    ///       </list>
    ///     </item>
    ///   </list>
    /// </value>
    [Browsable (false)]
    public virtual bool IsReadOnly
    {
      get
      {
        if (_readOnly == true) // (Bound Control || Unbound Control) && ReadOnly==true
          return true;
        if (DataSource != null && DataSource.Mode == DataSourceMode.Search) // Search DataSource 
          return false;
        if (Property == null || DataSource == null) // Unbound Control && (ReadOnly==false || ReadOnly==undefined)
          return false;
        if (DataSource.Mode == DataSourceMode.Read) // Bound Control && Reader DataSource
          return true;
        if (!IsDesignMode && DataSource.BusinessObject == null) // Bound Control but no BusinessObject
          return true;
        if (_readOnly == false) // Bound Control && ReadOnly==false
          return false;
        return IsReadOnlyInDomainModel; // ReadOnly==undefined: DomainModel pulls
      }
    }

    /// <summary>
    ///   Gets a flag that determines whether the control is to be treated as a required value.
    /// </summary>
    /// <remarks>
    ///     The value of this property is used to decide whether <see cref="CreateValidators"/> should 
    ///     include a <see cref="RequiredFieldValidator"/> for this control.
    /// </remarks>
    /// <value>
    ///   The following rules are used to determine the value of this property:
    ///   <list type="bullet">
    ///     <item>If the control is read-only, <see langword="false"/> is returned.</item>
    ///     <item>
    ///       If the <see cref="Required"/> property is not <see langword="null"/>, 
    ///       the value of <see cref="Required"/> is returned.
    ///     </item>
    ///     <item>
    ///       If the <see cref="BusinessObjectBoundWebControl.Property"/> contains a property definition with the
    ///       <see cref="IBusinessObjectProperty.IsRequired"/> flag set, <see langword="true"/> is returned. 
    ///     </item>
    ///     <item>Otherwise, <see langword="false"/> is returned.</item>
    ///   </list>
    /// </value>
    [Browsable (false)]
    public virtual bool IsRequired
    {
      get
      {
        if (IsReadOnly)
          return false;
        if (_required != null)
          return _required == true;
        if (Property != null)
          return Property.IsRequired;
        return false;
      }
    }

    /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
    /// <remarks> Override <see cref="CreateValidators(bool)"/> to define the validators for this control.</remarks>
    public IEnumerable<BaseValidator> CreateValidators ()
    {
      return CreateValidators (IsReadOnly);
    }

    /// <summary> Creates the list of validators required for the current binding and property settings. </summary>
    /// <param name="isReadOnly">
    /// This flag is initialized with the value of <see cref="IsReadOnly"/>. 
    /// Implemantations should consider whether they require a validator also when the control is rendered as read-only.
    /// </param>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.CreateValidators()">BusinessObjectBoundEditableWebControl.CreateValidators()</seealso>
    protected abstract IEnumerable<BaseValidator> CreateValidators (bool isReadOnly);

    /// <summary> Registers a validator that references this control. </summary>
    /// <remarks> 
    ///   <para>
    ///     The control may choose to ignore this call. 
    ///   </para><para>
    ///     The registered validators are evaluated when <see cref="Validate"/> is called.
    ///   </para>
    /// </remarks>
    public virtual void RegisterValidator (BaseValidator validator)
    {
      if (_validators == null)
        _validators = new HashSet<BaseValidator>();

      _validators.Add (validator);
    }

    /// <summary>
    /// Gets the validators associated with this control via <see cref="RegisterValidator"/>.
    /// </summary>
    protected IEnumerable<IValidator> GetRegisteredValidators ()
    {
      if (_validators == null)
        return Enumerable.Empty<IValidator>();

      return _validators.Select (v => v);
    }

    /// <summary>
    /// When overridden in derived classes, this method puts the control into a state in which validation
    /// can be performed. This may include populating control values from the view state explicitly.
    /// </summary>
    public virtual void PrepareValidation ()
    {
    }

    /// <summary> Calls <see cref="BaseValidator.Validate"/> on all registered validators. </summary>
    /// <returns> <see langword="true"/>, if all validators validated. </returns>
    public virtual bool Validate ()
    {
      if (_validators == null)
        return true;

      bool isValid = true;
      foreach (var validator in _validators)
      {
        validator.Validate();
        isValid &= validator.IsValid;
      }
      return isValid;
    }

    /// <summary>
    /// Overrides the base method to set a flag storing that the control has been rendered during the current lifecycle.
    /// </summary>
    /// <param name="e">ignored</param>
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      _isRenderedInCurrentLifecycle = true;
    }

    /// <summary>
    /// Gets a value that determines whether a server control needs to load data from the posted form values
    /// to its internal state.
    /// </summary>
    /// <returns>
    ///   <see langword="true"/> if the control has been rendered as a visible, enabled, and editable element in the previous lifecycle,
    ///   or if it is on a <see cref="IWxePage"/> and <see cref="IWxePage.IsOutOfSequencePostBack"/> is <see langword="true"/>.
    /// </returns>
    protected virtual bool IsLoadPostDataRequired ()
    {
      IWxePage wxePage = Page as IWxePage;
      if (wxePage != null && wxePage.IsOutOfSequencePostBack == true)
        return true;

      if (_hasBeenReadOnlyInPreviousLifecycle)
        return false;

      if (!_hasBeenEnabledInPreviousLifecycle)
        return false;

      if (!_hasBeenVisibleInPreviousLifecycle)
        return false;

      if (!_hasBeenRenderedInPreviousLifecycle)
        return false;

      return true;
    }

    /// <summary>
    /// Overrides the base method to load <see cref="IsDirty"/> and a flag determining whether the control
    /// has been rendered in the previous lifecycle in addition to the state loaded by <see cref="BusinessObjectBoundWebControl.LoadControlState"/>.
    /// </summary>
    /// <param name="savedState">The object saved by <see cref="SaveControlState"/>.</param>
    protected override void LoadControlState (object savedState)
    {
      object[] values = (object[]) savedState;
      base.LoadControlState (values[0]);
      _isDirty = (bool) values[1];
      _hasBeenRenderedInPreviousLifecycle = (bool) values[2];
      _hasBeenReadOnlyInPreviousLifecycle = (bool) values[3];
      _hasBeenEnabledInPreviousLifecycle = (bool) values[4];
      _hasBeenVisibleInPreviousLifecycle = (bool) values[5];
    }

    /// <summary>
    /// Overrides the base method to save <see cref="IsDirty"/> and a flag determining whether the control
    /// has been rendered in the current lifecycle in addition to the state saved by <see cref="Control.SaveControlState"/>.
    /// </summary>
    /// <returns>An object containing the state required to be loaded in the next lifecycle.</returns>
    protected override object SaveControlState ()
    {
      object[] values = new object[6];
      values[0] = base.SaveControlState();
      values[1] = _isDirty;
      values[2] = _isRenderedInCurrentLifecycle;
      values[3] = IsReadOnly;
      values[4] = Enabled;
      values[5] = Visible;
      return values;
    }

    /// <summary>
    /// Saves the value into the bound <see cref="BusinessObjectBoundWebControl.Property"/>.
    /// </summary>
    /// <returns>
    ///   <see langword="true"/> if the value was valid and saved into the bound <see cref="IBusinessObjectDataSource"/>.<see cref="IBusinessObjectDataSource.BusinessObject"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///   Thrown if the bound <see cref="BusinessObjectBoundWebControl.Property"/> is read-only but the <see cref="BusinessObjectBoundWebControl.Value"/> is dirty.
    /// </exception>
    protected bool SaveValueToDomainModel ()
    {
      if (Property == null)
        return false;

      if (DataSource == null)
        return false;

      if (DataSource.BusinessObject == null)
      {
        if (!HasValue)
          return true;

        return false;
      }

      if (IsReadOnlyInDomainModel) // also check when setting IsDirty
      {
        throw new InvalidOperationException (
            string.Format (
                "The value of the {0} '{1}' could not be saved into the domain model because the property '{2}' is read only.",
                GetType().Name,
                ID,
                Property.Identifier));
      }

      var requiresWriteBack = !Property.IsList || Property.ListInfo.RequiresWriteBack;
      if (requiresWriteBack)
        DataSource.BusinessObject.SetProperty (Property, Value);

      return true;
    }

    private bool IsReadOnlyInDomainModel
    {
      get
      {
        var dataSource = DataSource;
        var property = Property;
        Assertion.IsNotNull (dataSource, "DataSource is null.");
        Assertion.IsNotNull (property, "Property is null.");

        return property.IsReadOnly (dataSource.BusinessObject);
      }
    }
  }
}