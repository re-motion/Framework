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
using Remotion.ObjectBinding.Web.UI.Design;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  ///   <see cref="BusinessObjectDataSourceControl"/> is the default implementation of
  ///   the <see cref="IBusinessObjectDataSourceControl"/> interface. Derive from this class
  ///   if you want to create an invisible control only providing an object of type
  ///   <see cref="IBusinessObjectDataSource"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     When creating a specialized implementation of this class, override the <see langword="abstract"/> 
  ///     <see cref="InnerDataSource"/> property. It is recommended to create the instance to be returned by 
  ///     <see cref="InnerDataSource"/> during the construction phase of the <see cref="BusinessObjectDataSourceControl"/>.
  ///   </para><para>
  ///     In addition, an identifier for the <see cref="BusinessObjectClass"/> must be provided in form of a 
  ///     property. See the remarks section of the <see cref="IBusinessObjectDataSource"/> for details on implementing 
  ///     this property.
  ///   </para>
  ///   <note>
  ///     Please refer to the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.IBusinessObjectDataSourceControl" />'s
  ///     documentation for an examples of the <b>LoadValues</b> and the <b>SaveValues</b> patterns.
  ///   </note>
  /// </remarks>
  [NonVisualControl]
  [Designer (typeof (BocDataSourceDesigner))]
  public abstract class BusinessObjectDataSourceControl : Control, IBusinessObjectDataSourceControl
  {
    #region Obsoletes

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectBoundControl"/> objects bound to this <see cref="IBusinessObjectDataSource"/>
    ///   that have a valid binding according to the <see cref="IBusinessObjectBoundControl.HasValidBinding"/> property.
    /// </summary>
    /// <returns> 
    ///   An array of <see cref="IBusinessObjectBoundControl"/> objects where the <see cref="IBusinessObjectBoundControl.HasValidBinding"/> property 
    ///   evaluates <see langword="true"/>. 
    /// </returns>
    [Obsolete ("The BoundControls property is now obsolete. Use GetBoundControlsWithValidBinding() instead. (Version 1.13.119)")]
    public IBusinessObjectBoundControl[] BoundControls
    {
      get { return GetBoundControlsWithValidBinding().ToArray(); }
    }

    [Obsolete ("The GetDataSource() method is now obsolete. Use the InnerDataSource property instead. (Version 1.13.119)", true)]
    protected IBusinessObjectDataSource GetDataSource ()
    {
      throw new NotSupportedException ("The GetDataSource() method is now obsolete. Use the InnerDataSource property instead. (Version 1.13.119)");
    }

    #endregion

    /// <summary>
    ///   Returns the <see cref="IBusinessObjectDataSource"/> encapsulated in this <see cref="BusinessObjectDataSourceControl"/>.
    /// </summary>
    /// <value> An <see cref="IBusinessObjectDataSource"/>. </value>
    /// <remarks>
    ///   For details on overriding this method, see <see cref="BusinessObjectDataSourceControl"/>'s remarks section.
    /// </remarks>
    protected abstract IBusinessObjectDataSource InnerDataSource { get; }

    /// <summary> Loads the values of the <see cref="BusinessObject"/> into all bound controls. </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <remarks> 
    ///   Executes the <see cref="IBusinessObjectDataSource.LoadValues"/> method of the encapsulated <see cref="IBusinessObjectDataSource"/>.
    ///   <note>
    ///     Please refer to the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.IBusinessObjectDataSourceControl" />'s
    ///     remarks section for an example of the <b>LoadValues Pattern</b>.
    ///   </note>
    /// </remarks>
    public virtual void LoadValues (bool interim)
    {
      InnerDataSource.LoadValues (interim);
    }

    /// <summary> 
    ///   Saves the values of the <see cref="BusinessObject"/> from all bound controls implementing <see cref="IBusinessObjectBoundEditableControl"/>.
    /// </summary>
    /// <param name="interim"> Spefifies whether this is the final saving, or an interim saving. </param>
    /// <returns><see langword="true"/> if all bound controls have saved their value into the <see cref="IBusinessObjectDataSource.BusinessObject"/>.</returns>
    /// <remarks> 
    ///   Executes the <see cref="IBusinessObjectDataSource.SaveValues"/> method of the encapsulated  <see cref="IBusinessObjectDataSource"/>.
    ///   <note>
    ///     Please refer to the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.IBusinessObjectDataSourceControl" />'s
    ///     remarks section for an example of the <b>SaveValues Pattern</b>.
    ///   </note>
    /// </remarks>
    public virtual bool SaveValues (bool interim)
    {
      return InnerDataSource.SaveValues (interim);
    }

    /// <summary>
    ///   Adds the passed <see cref="IBusinessObjectBoundControl"/> to the list of controls bound to this <see cref="BusinessObjectDataSourceControl"/>.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="IBusinessObjectBoundControl"/> to be registered with this <see cref="BusinessObjectDataSourceControl"/>.
    /// </param>
    /// <remarks> 
    ///   Executes the <see cref="IBusinessObjectDataSource.Register"/> method of the encapsulated <see cref="IBusinessObjectDataSource"/>.
    /// </remarks>
    public virtual void Register (IBusinessObjectBoundControl control)
    {
      InnerDataSource.Register (control);
    }

    /// <summary>
    ///   Removes the passed <see cref="IBusinessObjectBoundControl"/> from the list of controls bound to this <see cref="BusinessObjectDataSourceControl"/>.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="IBusinessObjectBoundControl"/> to be unregistered from this <see cref="BusinessObjectDataSourceControl"/>.
    /// </param>
    /// <remarks> 
    ///   Executes the <see cref="IBusinessObjectDataSource.Unregister"/> method of the encapsulated <see cref="IBusinessObjectDataSource"/>.
    /// </remarks>
    public virtual void Unregister (IBusinessObjectBoundControl control)
    {
      InnerDataSource.Unregister (control);
    }

    /// <summary>
    ///   Gets or sets the current <see cref="DataSourceMode"/> of this <see cref="BusinessObjectDataSourceControl"/>.
    /// </summary>
    /// <value> A value of the <see cref="DataSourceMode"/> enumeration. </value>
    /// <remarks> 
    ///   Gets or sets the <see cref="IBusinessObjectDataSource.Mode"/> property of the encapsulated <see cref="IBusinessObjectDataSource"/>.
    /// </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Data")]
    [DefaultValue (DataSourceMode.Edit)]
    public virtual DataSourceMode Mode
    {
      get { return InnerDataSource.Mode; }
      set { InnerDataSource.Mode = value; }
    }

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObject"/> connected to this <see cref="BusinessObjectDataSourceControl"/>.
    /// </summary>
    /// <value>
    ///   An <see cref="IBusinessObject"/> or <see langword="null"/>. Must be compatible with <see cref="BusinessObjectClass"/>.
    /// </value>
    /// <remarks> 
    ///   Gets or sets the <see cref="IBusinessObjectDataSource.BusinessObject"/> property of the encapsulated <see cref="IBusinessObjectDataSource"/>.
    /// </remarks>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public virtual IBusinessObject BusinessObject
    {
      get { return InnerDataSource.BusinessObject; }
      set { InnerDataSource.BusinessObject = value; }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/>
    ///   connected to this <see cref="BusinessObjectDataSourceControl"/>.
    /// </summary>
    /// <value> The <see cref="IBusinessObjectClass"/> of the connected <see cref="IBusinessObject"/>. </value>
    /// <remarks>
    ///   <para>
    ///     Usually set before the <see cref="IBusinessObject"/> is connected to the 
    ///     <see cref="IBusinessObjectDataSource"/> by utilizing Visual Studio .NET Designer. 
    ///   </para><para>
    ///     Gets the <see cref="IBusinessObjectDataSource.BusinessObjectClass"/> property of the encapsulated <see cref="IBusinessObjectDataSource"/>.
    ///   </para>
    /// </remarks>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public virtual IBusinessObjectClass BusinessObjectClass
    {
      get { return InnerDataSource.BusinessObjectClass; }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectProvider"/> of this <see cref="BusinessObjectDataSourceControl"/>.
    /// </summary>
    /// <value> The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>. </value>
    /// <remarks> 
    ///   Gets the <see cref="IBusinessObjectDataSource.BusinessObjectProvider"/> property of the encapsulated <see cref="IBusinessObjectDataSource"/>.
    /// </remarks>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public virtual IBusinessObjectProvider BusinessObjectProvider
    {
      get { return InnerDataSource.BusinessObjectProvider; }
    }

    /// <summary>Gets the <see cref="IBusinessObjectBoundControl"/> objects bound to this <see cref="BusinessObjectDataSourceControl"/>.</summary>
    /// <returns> A read-only collection of <see cref="IBusinessObjectBoundControl"/> objects. </returns>
    /// <remarks> 
    ///   Gets the <see cref="IBusinessObjectDataSource.GetAllBoundControls"/> method of the encapsulated <see cref="IBusinessObjectDataSource"/>.
    /// </remarks>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public ReadOnlyCollection<IBusinessObjectBoundControl> GetAllBoundControls ()
    {
      return InnerDataSource.GetAllBoundControls();
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectBoundControl"/> objects bound to this <see cref="BusinessObjectDataSourceControl"/>
    ///   that have a valid binding according to the <see cref="IBusinessObjectBoundControl.HasValidBinding"/> property.
    /// </summary>
    /// <returns>
    ///   A sequence of <see cref="IBusinessObjectBoundControl"/> objects where the <see cref="IBusinessObjectBoundControl.HasValidBinding"/> property 
    ///   evaluates <see langword="true"/>. 
    /// </returns>
    /// <remarks> 
    ///   Gets the <see cref="IBusinessObjectDataSource.GetBoundControlsWithValidBinding"/> method of the encapsulated <see cref="IBusinessObjectDataSource"/>.
    /// </remarks>
    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public IEnumerable<IBusinessObjectBoundControl> GetBoundControlsWithValidBinding ()
    {
      return InnerDataSource.GetBoundControlsWithValidBinding();
    }

    /// <summary> Prepares all bound controls implementing <see cref="IValidatableControl"/> for validation. </summary>
    public void PrepareValidation ()
    {
      foreach (var control in InnerDataSource.GetBoundControlsWithValidBinding().OfType<IValidatableControl>())
        control.PrepareValidation();
    }

    /// <summary> Validates all bound controls implementing <see cref="IValidatableControl"/>. </summary>
    /// <returns> <see langword="true"/> if no validation errors where found. </returns>
    public bool Validate ()
    {
      bool isValid = true;
      foreach (var control in InnerDataSource.GetBoundControlsWithValidBinding().OfType<IValidatableControl>())
        isValid &= control.Validate();
      return isValid;
    }

    IPage IControl.Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }

    /// <summary>
    ///   Overrides the implementation of <see cref="Control.Render">Control.Render</see>. Does not render any output.
    /// </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object that receives the server control content. </param>
    protected override void Render (HtmlTextWriter writer)
    {
      //  No output, control is invisible
    }

    protected override void OnUnload (EventArgs e)
    {
      foreach (var control in InnerDataSource.GetAllBoundControls().ToArray())
        InnerDataSource.Unregister (control);

      base.OnUnload (e);
    }
  }
}