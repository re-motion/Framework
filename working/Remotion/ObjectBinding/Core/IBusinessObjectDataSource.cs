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

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   This interface provides funtionality for binding an <see cref="IBusinessObject"/> to an 
  ///   <see cref="IBusinessObjectBoundControl"/>. Each business object model requires a specialized 
  ///   implementation of this interface.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     For most situations, the default implementation provided by <see cref="BusinessObjectDataSource"/> can be
  ///     used as a base for the implementation.
  ///   </para><para>
  ///     The data source usually provides a way of specifying a type identifier. This identifier is used to
  ///     get or instantiate the matching <see cref="IBusinessObjectClass"/> from the object model.
  ///     <note type="inotes">
  ///       It is important to use an identifier that can be persisted as a string. Otherwise it would not be possible to 
  ///       specify and later persist the <see cref="IBusinessObjectClass"/> in the Visual Studio .NET Designer, 
  ///       preventing any further design time features from working.
  ///     </note>
  ///   </para>
  ///   <para>
  ///     See the <see href="Remotion.ObjectBinding.html">Remotion.ObjectBinding</see> namespace documentation for general 
  ///     information on the data binding process.
  ///   </para>
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundControl"/>
  /// <seealso cref="IBusinessObjectBoundEditableControl"/>
  public interface IBusinessObjectDataSource
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
    IBusinessObjectBoundControl[] BoundControls { get; }

    #endregion

    /// <summary> Gets or sets the current <see cref="DataSourceMode"/>. </summary>
    /// <remarks> The behavior of the bound controls depends on the current <see cref="DataSourceMode"/>. </remarks>
    /// <value> A value of the <see cref="DataSourceMode"/> enumeration. </value>
    DataSourceMode Mode { get; set; }

    /// <summary>
    ///   Adds the passed <see cref="IBusinessObjectBoundControl"/> to the list of controls bound to this
    ///   <see cref="IBusinessObjectDataSource"/>.
    /// </summary>
    /// <remarks>
    ///   <b>Register</b> is usually called by the <see cref="IBusinessObjectBoundControl"/> when its 
    ///   <see cref="IBusinessObjectBoundControl.DataSource"/> is set.
    /// </remarks>
    /// <param name="control"> 
    ///   The <see cref="IBusinessObjectBoundControl"/> to be added to <see cref="GetAllBoundControls"/>. 
    /// </param>
    void Register (IBusinessObjectBoundControl control);

    /// <summary>
    ///   Removes the passed <see cref="IBusinessObjectBoundControl"/> from the list of controls bound to this
    ///   <see cref="IBusinessObjectDataSource"/>.
    /// </summary>
    /// <remarks>
    ///   <b>Unregister</b> is usually called by the <see cref="IBusinessObjectBoundControl"/> when its 
    ///   <see cref="IBusinessObjectBoundControl.DataSource"/> is set to a new <see cref="IBusinessObjectDataSource"/>
    ///   or <see langword="null"/>.
    /// </remarks>
    /// <param name="control">
    ///   The <see cref="IBusinessObjectBoundControl"/> to be removed from <see cref="GetAllBoundControls"/>. 
    /// </param>
    void Unregister (IBusinessObjectBoundControl control);

    /// <summary> Loads the values of the <see cref="BusinessObject"/> into all bound controls. </summary>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    /// <remarks>
    ///   On initial loads, all values must be loaded. On interim loads, each <see cref="IBusinessObjectBoundControl"/>
    ///   decides whether it keeps its own value (e.g., using view state) or reloads the value (useful for complex 
    ///   structures that need no validation).
    /// </remarks>
    /// <seealso cref="IBusinessObjectBoundControl.LoadValue">IBusinessObjectBoundControl.LoadValue</seealso>
    void LoadValues (bool interim);

    /// <summary> 
    ///   Saves the values of the <see cref="BusinessObject"/> from all bound controls implementing
    ///   <see cref="IBusinessObjectBoundEditableControl"/>.
    /// </summary>
    /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
    /// <returns><see langword="true"/> if all bound controls have saved their value into the <see cref="IBusinessObjectDataSource.BusinessObject"/>.</returns>
    /// <remarks>
    ///   On final saves, all values must be saved. (It is assumed that invalid values were already identified using 
    ///   validators.) On interim saves, each <see cref="IBusinessObjectBoundEditableControl"/> decides whether it 
    ///   saves its values into the <see cref="BusinessObject"/> or uses an alternate mechanism (e.g. view state).
    /// </remarks>
    /// <seealso cref="IBusinessObjectBoundEditableControl.SaveValue">IBusinessObjectBoundEditableControl.SaveValue</seealso>
    bool SaveValues (bool interim);

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/> connected to this 
    ///   <see cref="IBusinessObjectDataSource"/>. 
    /// </summary>
    /// <value> The <see cref="IBusinessObjectClass"/> of the connected <see cref="IBusinessObject"/>. </value>
    /// <remarks>
    ///   Usually set before the an <see cref="IBusinessObject"/> is connected to the 
    ///   <see cref="IBusinessObjectDataSource"/>. 
    /// </remarks>
    IBusinessObjectClass BusinessObjectClass { get; }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectProvider"/> used for accessing supplementary information on the connected
    ///   <see cref="IBusinessObject"/> and assigned <see cref="IBusinessObjectClass"/>.
    /// </summary>
    /// <value> The <see cref="IBusinessObjectProvider"/> for the current <see cref="BusinessObjectClass"/>. </value>
    /// <remarks>
    ///   <note type="inotes">
    ///     Must not return <see langword="null"/> if the <see cref="BusinessObjectClass"/> is set.
    ///   </note>
    /// </remarks>
    IBusinessObjectProvider BusinessObjectProvider { get; }

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObject"/> connected to this <see cref="IBusinessObjectDataSource"/>.
    /// </summary>
    /// <value>
    ///   An <see cref="IBusinessObject"/> or <see langword="null"/>. Must be compatible with
    ///   the <see cref="BusinessObjectClass"/> assigned to this <see cref="IBusinessObjectDataSource"/>.
    /// </value>
    IBusinessObject BusinessObject { get; set; }

    /// <summary>Gets the <see cref="IBusinessObjectBoundControl"/> objects bound to this <see cref="IBusinessObjectDataSource"/>.</summary>
    /// <returns> A read-only collection of <see cref="IBusinessObjectBoundControl"/> objects. </returns>
    ReadOnlyCollection<IBusinessObjectBoundControl> GetAllBoundControls ();

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectBoundControl"/> objects bound to this <see cref="IBusinessObjectDataSource"/>
    ///   that have a valid binding according to the <see cref="IBusinessObjectBoundControl.HasValidBinding"/> property.
    /// </summary>
    /// <returns> 
    ///   A sequence of <see cref="IBusinessObjectBoundControl"/> objects where the <see cref="IBusinessObjectBoundControl.HasValidBinding"/> property 
    ///   evaluates <see langword="true"/>. 
    /// </returns>
    IEnumerable<IBusinessObjectBoundControl> GetBoundControlsWithValidBinding ();
  }
}
