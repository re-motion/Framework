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
using System.ComponentModel;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IDataEditControl: IControl
  {
    /// <summary>
    ///   Gets or sets the BusinessObject used by this control. 
    /// </summary>
    /// <remarks>
    ///   If the control uses multiple business objects, only one can be exposed using this property.
    /// </remarks>
    IBusinessObject BusinessObject { get; set; }

    /// <summary>
    ///   Load the values into bound controls. 
    /// </summary>
    /// <remarks>
    ///   If the control uses multiple data sources, all data sources will be loaded using this method.
    /// </remarks>
    /// <param name="interim"> Indicates whether loading is initial (all values must be loaded) or interim (preserving values between requests).  </param>
    void LoadValues (bool interim);

    /// <summary>
    ///   Saves the values from bound controls. 
    /// </summary>
    /// <remarks>
    ///   If the control uses multiple data sources, all data sources will be saved using this method.
    /// </remarks>
    /// <param name="interim"> 
    ///   Indicates whether saving is interim (preserving values between requests) or final (all values must be saved). Before final saving,
    ///   <see cref="Validate"/> must be called and succeeded. 
    /// </param>
    /// <returns><see langword="true"/> if all bound controls have saved their value into the <see cref="BusinessObject"/>.</returns>
    bool SaveValues (bool interim);

    /// <summary>
    ///   Notifies the control that editing is cancelled. 
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This method can be used to release locks. If the control's <see cref="Mode"/> is not set to 
    ///     <see cref="DataSourceMode.Edit"/>, this will usually be ignored.
    ///   </para><para>
    ///     If the control uses multiple data sources, all data sources will be affected by this method.
    ///   </para>
    /// </remarks>
    void CancelEdit ();

    /// <summary>
    ///   Gets or sets a value indicating whether this control is in read, edit or search mode.
    /// </summary>
    /// <remarks>
    ///   If the control uses multiple data sources, all data sources will be modified by this property.
    /// </remarks>
    DataSourceMode Mode { get; set; }

    /// <summary> Prepares all bound controls for validation. </summary>
    void PrepareValidation ();

    /// <summary> Validates all bound controls and displays error hints if the validation failed. /// </summary>
    /// <returns> True if validation succeeded, false if validation failed. </returns>
    bool Validate ();

    /// <summary>
    /// Provides access to the data source object. For common operations, use the methods of <see cref="IDataEditControl"/> instead.
    /// </summary>
    [EditorBrowsable (EditorBrowsableState.Advanced)]
    IBusinessObjectDataSourceControl DataSource { get; }
  }
}
