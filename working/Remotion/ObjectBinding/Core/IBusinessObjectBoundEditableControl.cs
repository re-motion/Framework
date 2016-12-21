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

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   Extends an <see cref="IBusinessObjectBoundControl"/> with the option of writing the 
  ///   <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
  /// </summary>
  /// <remarks>
  ///   See <see cref="SaveValue"/> for a description of the data binding process.
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundControl"/>
  /// <seealso cref="IBusinessObjectDataSource"/>
  public interface IBusinessObjectBoundEditableControl: IBusinessObjectBoundControl
  {
    /// <summary>
    ///   Saves the <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     When <see cref="SaveValue"/> is executed, the object held by <see cref="IBusinessObjectBoundControl.Value"/>
    ///     is written back into the <see cref="IBusinessObject"/> provided by the <see cref="IBusinessObjectBoundControl.DataSource"/>.
    ///   </para><para>
    ///     This method is usually called by <see cref="IBusinessObjectDataSource.SaveValues">IBusinessObjectDataSource.SaveValues</see>.
    ///   </para>
    /// </remarks>
    /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
    /// <returns><see langword="true"/> if the value was saved into the bound <see cref="IBusinessObjectDataSource.BusinessObject"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///   Thrown if the bound <see cref="IBusinessObjectBoundControl.Property"/> is read-only but the <see cref="IBusinessObjectBoundControl.Value"/> is dirty.
    /// </exception>
    bool SaveValue (bool interim);
  
    /// <summary>
    ///   Gets a flag that determines whether the control is to be displayed in read-only mode.
    /// </summary>
    bool IsReadOnly { get; }
  }
}
