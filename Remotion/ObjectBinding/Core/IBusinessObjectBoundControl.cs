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

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   Provides functionality for binding an <see cref="IComponent"/> to an <see cref="IBusinessObject"/> using
  ///   an <see cref="IBusinessObjectDataSource"/>. 
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     See the <see href="Remotion.ObjectBinding.html">Remotion.ObjectBinding</see> namespace documentation for general 
  ///     information on the data binding process.
  ///   </para>
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundEditableControl"/>
  /// <seealso cref="IBusinessObjectDataSource"/>
  public interface IBusinessObjectBoundControl : IComponent
  {
    /// <summary>
    ///   Gets or sets the <see cref="DataSource"/> providing the <see cref="IBusinessObject"/> to which this
    ///   <see cref="IBusinessObjectBoundControl"/> is bound.
    /// </summary>
    /// <value> An <see cref="IBusinessObjectDataSource"/> providing the current <see cref="IBusinessObject"/>. </value>
    IBusinessObjectDataSource DataSource { get; set; }

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObjectProperty"/> used for accessing the data to be loaded into 
    ///   <see cref="Value"/>.
    /// </summary>
    /// <value> 
    ///   An <see cref="IBusinessObjectProperty"/> that is part of the bound <see cref="IBusinessObject"/>'s
    ///   <see cref="IBusinessObjectClass"/>
    /// </value>
    IBusinessObjectProperty Property { get; set; }

    /// <summary> Gets or sets the value provided by the <see cref="IBusinessObjectBoundControl"/>. </summary>
    /// <value> An object or boxed value. </value>
    object Value { get; set; }

    /// <summary>Gets a flag indicating whether the <see cref="IBusinessObjectBoundControl"/> contains a value. </summary>
    /// <value><see langword="true" /> if the <see cref="IBusinessObjectBoundControl"/> contains a value. </value>
    /// <remarks>
    /// The flag only specifies the presense of data. It does not specify whether the data is in a format compatible with the <see cref="Property"/>.
    /// For this, a separate validation step is required.
    /// </remarks>
    bool HasValue { get; }

    /// <summary> 
    ///   Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. 
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     The <see cref="IBusinessObjectBoundControl"/> is bound to an <see cref="IBusinessObjectDataSource"/>
    ///     and an <see cref="IBusinessObjectProperty"/>. When <see cref="LoadValue"/> is executed, the 
    ///     <see cref="Property"/> is used get the value from the <see cref="IBusinessObject"/> provided by the 
    ///     <see cref="DataSource"/>. This object is then used to populate <see cref="Value"/>.
    ///   </para><para>
    ///     This method is usually called by 
    ///     <see cref="IBusinessObjectDataSource.LoadValues">IBusinessObjectDataSource.LoadValues</see>.
    ///   </para>
    /// </remarks>
    /// <param name="interim"> Specifies whether this is the initial loading, or an interim loading. </param>
    void LoadValue (bool interim);

    /// <summary>
    ///   Tests whether the <see cref="IBusinessObjectBoundControl"/> can be bound to the <paramref name="property"/>.
    /// </summary>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to be testet.</param>
    /// <returns>
    ///   <see langword="true"/> if the <see cref="IBusinessObjectBoundControl"/> can be bound to the
    ///   <paramref name="property"/>.
    /// </returns>
    bool SupportsProperty (IBusinessObjectProperty property);

    /// <summary>
    ///   Gets a flag specifying whether the <see cref="IBusinessObjectBoundControl"/> has a valid binding configuration.
    /// </summary>
    /// <remarks>
    ///   The configuration is considered invalid if data binding is configured for a property 
    ///   that is not available for the bound class or object.
    /// </remarks>
    /// <value> <see langword="true"/> if the binding configuration is valid. </value>
    bool HasValidBinding { get; }
  }
}