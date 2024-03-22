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
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   <b>BusinessObjectReferenceDataSourceControl</b> provides an <see cref="IBusinessObjectReferenceDataSource"/>
  ///   to controls of type <see cref="IBusinessObjectBoundWebControl"/> inside an <b>ASPX Web Form</b> or 
  ///   <b>ASCX User Control</b>.
  /// </summary>
  /// <seealso cref="IBusinessObjectReferenceDataSource"/>
  /// <seealso cref="IBusinessObjectDataSourceControl"/>
  [NonVisualControl]
  public class BusinessObjectReferenceDataSourceControl :
      BusinessObjectBoundEditableWebControl,
      IBusinessObjectDataSourceControl,
      IBusinessObjectReferenceDataSource,
      IControlWithResourceManager
  {
    private class InternalBusinessObjectReferenceDataSource : BusinessObjectReferenceDataSourceBase
    {
      private readonly BusinessObjectReferenceDataSourceControl _owner;

      public InternalBusinessObjectReferenceDataSource (BusinessObjectReferenceDataSourceControl owner)
      {
        _owner = owner;
      }

      public override IBusinessObjectReferenceProperty? ReferenceProperty
      {
        get { return (IBusinessObjectReferenceProperty?)_owner.Property; }
      }

      public override IBusinessObjectDataSource? ReferencedDataSource
      {
        get { return _owner.DataSource; }
      }

      public override DataSourceMode Mode
      {
        get { return _owner.Mode; }
        set { _owner.Mode = value; }
      }


      protected override string GetDataSourceIdentifier ()
      {
        return string.Format("{0} '{1}'", _owner.GetType(), _owner.ID);
      }
    }

    private readonly InternalBusinessObjectReferenceDataSource _internalDataSource;
    private ReadOnlyCollection<BaseValidator>? _validators;

    /// <summary>
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/> supports properties of type
    ///   <see cref="IBusinessObjectReferenceProperty"/>.
    /// </summary>
    protected override Type[] SupportedPropertyInterfaces
    {
      get { return new[] { typeof(IBusinessObjectReferenceProperty) }; }
    }

    // Default summary will be created.
    public BusinessObjectReferenceDataSourceControl ()
    {
      _internalDataSource = new InternalBusinessObjectReferenceDataSource(this);
    }

    /// <summary> See <see cref="BusinessObjectBoundWebControl.Value"/> for details on this property. </summary>
    /// <value> The value must be of type <see cref="IBusinessObject"/>. </value>
    protected override sealed object? ValueImplementation
    {
      get { return _internalDataSource.BusinessObject; }
      set { _internalDataSource.BusinessObject = (IBusinessObject?)value; }
    }

    public override bool HasValue
    {
      get { return _internalDataSource.HasValue(); }
    }

    /// <summary> Gets or sets the dirty flag. </summary>
    /// <value> 
    ///   Evaluates <see langword="true"/> if either the <see cref="BusinessObjectReferenceDataSourceControl"/> or one 
    ///   of the bound controls is dirty.
    /// </value>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.IsDirty">BusinessObjectBoundEditableWebControl.IsDirty</seealso>
    public override bool IsDirty
    {
      get
      {
        if (base.IsDirty)
          return true;

        if (_internalDataSource.HasBusinessObjectChanged)
          return true;

        return _internalDataSource.GetBoundControlsWithValidBinding().OfType<IBusinessObjectBoundEditableWebControl>().Any(
            control => control.IsDirty);
      }
      set { base.IsDirty = value; }
    }

    /// <summary> 
    ///   Returns the <see cref="System.Web.UI.Control.ClientID"/> values of all controls whose value can be modified 
    ///   in the user interface.
    /// </summary>
    /// <returns> An empty <see cref="String"/> <see cref="Array"/>. </returns>
    /// <seealso cref="BusinessObjectBoundEditableWebControl.GetTrackedClientIDs">BusinessObjectBoundEditableWebControl.GetTrackedClientIDs</seealso>
    public override string[] GetTrackedClientIDs ()
    {
      return new string[0];
    }

    /// <summary>
    ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
    ///   <see cref="BusinessObjectBoundEditableWebControl"/>.
    /// </summary>
    /// <value> Returns always <see langword="false"/>. </value>
    public override bool UseLabel
    {
      get { return false; }
    }

    /// <summary> 
    ///   Loads the <see cref="BusinessObject"/> from the <see cref="ReferencedDataSource"/> using 
    ///   <see cref="ReferenceProperty"/> and populates the bound controls using <see cref="LoadValues"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    public override void LoadValue (bool interim) // inherited from control interface
    {
      _internalDataSource.LoadValue(interim);
    }

    /// <summary> Loads the values of the <see cref="BusinessObject"/> into all bound controls. </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    public virtual void LoadValues (bool interim) // inherited from data source interface
    {
      _internalDataSource.LoadValues(interim);
    }

    /// <summary> 
    ///   Saves the values from the bound controls using <see cref="SaveValues"/>
    ///   and writes the <see cref="BusinessObject"/> back into the <see cref="ReferencedDataSource"/> using 
    ///   <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
    /// <returns>
    ///   <see langword="true"/> if the value was saved into the bound <see cref="IBusinessObjectDataSource"/>.<see cref="IBusinessObjectDataSource.BusinessObject"/>.
    /// </returns>
    public override bool SaveValue (bool interim) // inherited from control interface
    {
      // Validate to keep things consistent, i.e. all validators have executed during the save-operation.
      // Do not abort the save-operation because the value of the ReferenceDataSource should always be allowed to be written back into the parent.
      Validate();

      // Do not include check for IsDirty.
      // The wrapped reference data source has its own mechanism to prevent unnecessary write-backs.
      // The bound controls also have their own IsDirty-checks and do not concern the DataSourceControl's write-back semantics.
      return _internalDataSource.SaveValue(interim);
    }

    /// <summary> 
    ///   Saves the values of the <see cref="BusinessObject"/> from all bound controls implementing
    ///   <see cref="IBusinessObjectBoundEditableControl"/>.
    /// </summary>
    /// <param name="interim"> Spefifies whether this is the final saving, or an interim saving. </param>
    /// <returns>
    /// <see langword="true"/> if all bound controls have saved their value into the <see cref="IBusinessObjectDataSource.BusinessObject"/> 
    /// and the <see cref="BusinessObject"/> was saved back into the bound <see cref="IBusinessObjectDataSource"/>.<see cref="IBusinessObjectDataSource.BusinessObject"/>.
    /// </returns>
    public virtual bool SaveValues (bool interim) // inherited data source interface
    {
      return _internalDataSource.SaveValues(interim);
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
    ///   <see cref="IBusinessObject"/> to which this <see cref="BusinessObjectReferenceDataSourceControl"/> connects.
    /// </summary>
    /// <value> 
    ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
    /// </value>
    /// <remarks> Identical to <see cref="BusinessObjectBoundWebControl.Property"/>. </remarks>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectReferenceProperty? ReferenceProperty
    {
      get { return (IBusinessObjectReferenceProperty?)Property; }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
    ///   to which this <see cref="BusinessObjectReferenceDataSourceControl"/> connects.
    /// </summary>
    /// <value> 
    ///   The <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> to which this
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/> connects.
    ///  </value>
    /// <remarks> Identical to <see cref="BusinessObjectBoundWebControl.DataSource"/>. </remarks>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectDataSource? ReferencedDataSource
    {
      get { return _internalDataSource.ReferencedDataSource; }
    }


    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObject"/> accessed through the <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <value> An <see cref="IBusinessObject"/> or <see langword="null"/>. </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBusinessObject? BusinessObject
    {
      get { return _internalDataSource.BusinessObject; }
      set { _internalDataSource.BusinessObject = value; }
    }

    /// <summary> 
    ///   Gets the <see cref="IBusinessObjectReferenceProperty.ReferenceClass"/> of the <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <value> 
    ///   An <see cref="IBusinessObjectClass"/> or <see langword="null"/> if no <see cref="ReferenceProperty"/> is set.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectClass? BusinessObjectClass
    {
      get { return _internalDataSource.BusinessObjectClass; }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectProvider"/> of this <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </summary>
    /// <value> The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>. </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBusinessObjectProvider? BusinessObjectProvider
    {
      get { return _internalDataSource.BusinessObjectProvider; }
    }

    /// <summary>
    ///   Adds the passed <see cref="IBusinessObjectBoundControl"/> to the list of controls bound to this <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="IBusinessObjectBoundControl"/> to be registered with this <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </param>
    public void Register (IBusinessObjectBoundControl control)
    {
      _internalDataSource.Register(control);
    }

    /// <summary>
    ///   Removes the passed <see cref="IBusinessObjectBoundControl"/> from the list of controls bound to this <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="IBusinessObjectBoundControl"/> to be unregistered from this <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </param>
    public void Unregister (IBusinessObjectBoundControl control)
    {
      _internalDataSource.Unregister(control);
    }

    /// <summary>
    ///   Gets or sets the current <see cref="DataSourceMode"/> of this 
    ///   <see cref="BusinessObjectReferenceDataSourceControl"/>.
    /// </summary>
    /// <value> A value of the <see cref="DataSourceMode"/> enumeration. </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual DataSourceMode Mode
    {
      get
      {
        // return IsReadOnly ? DataSourceMode.Read : DataSourceMode.Edit; 
        if (IsReadOnly)
          return DataSourceMode.Read;
        if (DataSource != null && DataSource.Mode == DataSourceMode.Search)
          return DataSourceMode.Search;
        return DataSourceMode.Edit;
      }
      set
      {
        // "search" needs edit mode
        ReadOnly = value == DataSourceMode.Read;
      }
    }

    /// <summary>Gets the <see cref="IBusinessObjectBoundControl"/> objects bound to this <see cref="BusinessObjectReferenceDataSourceControl"/>.</summary>
    /// <returns> A read-only collection of <see cref="IBusinessObjectBoundControl"/> objects. </returns>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ReadOnlyCollection<IBusinessObjectBoundControl> GetAllBoundControls ()
    {
      return _internalDataSource.GetAllBoundControls();
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectBoundControl"/> objects bound to this <see cref="BusinessObjectReferenceDataSourceControl"/>
    ///   that have a valid binding according to the <see cref="IBusinessObjectBoundControl.HasValidBinding"/> property.
    /// </summary>
    /// <returns> 
    ///   A sequence of <see cref="IBusinessObjectBoundControl"/> objects where the <see cref="IBusinessObjectBoundControl.HasValidBinding"/> property 
    ///   evaluates <see langword="true"/>. 
    /// </returns>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEnumerable<IBusinessObjectBoundControl> GetBoundControlsWithValidBinding ()
    {
      return _internalDataSource.GetBoundControlsWithValidBinding();
    }

    /// <summary> Prepares all bound controls implementing <see cref="IValidatableControl"/> for validation. </summary>
    public override void PrepareValidation ()
    {
      base.PrepareValidation();
      foreach (var control in _internalDataSource.GetBoundControlsWithValidBinding().OfType<IValidatableControl>())
        control.PrepareValidation();
    }

    /// <summary> Validates all bound controls implementing <see cref="IValidatableControl"/>. </summary>
    /// <returns> <see langword="true"/> if no validation errors where found. </returns>
    public override bool Validate ()
    {
      bool isValid = base.Validate();
      if (IsRequired || _internalDataSource.HasValue())
      {
        foreach (var control in _internalDataSource.GetBoundControlsWithValidBinding().OfType<IValidatableControl>())
          isValid &= control.Validate();
      }
      return isValid;
    }

    /// <inheritdoc />
    protected override IEnumerable<BaseValidator> CreateValidators (bool isReadOnly)
    {
      var validatorFactory = ServiceLocator.GetInstance<IBusinessObjectReferenceDataSourceControlValidatorFactory>();
      _validators = validatorFactory.CreateValidators(this, isReadOnly).ToList().AsReadOnly();
      return _validators;
    }

    protected override IBusinessObjectConstraintVisitor CreateBusinessObjectConstraintVisitor ()
    {
      return NullBusinessObjectConstraintVisitor.Instance;
    }

    /// <summary>
    ///   Overrides the implementation of <see cref="System.Web.UI.Control.Render">Control.Render</see>. 
    ///   Does not render any output.
    /// </summary>
    /// <param name="writer">
    ///   The <see cref="System.Web.UI.HtmlTextWriter"/> object that receives the server control content. 
    /// </param>
    protected override void Render (HtmlTextWriter writer)
    {
      //  No output, control is invisible
    }

    protected override void OnUnload (EventArgs e)
    {
      foreach (var control in _internalDataSource.GetAllBoundControls().ToArray())
        _internalDataSource.Unregister(control);

      base.OnUnload(e);
    }

    public IResourceManager GetResourceManager ()
    {
      return GetResourceManager(typeof(BusinessObjectReferenceDataSourceControl));
    }
  }
}
