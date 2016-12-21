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
using System.Collections;
using JetBrains.Annotations;

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   The <b>IBusinessObjectProperty</b> interface provides functionality common to all business object property 
  ///   spezializations for the individual data types.
  /// </summary>
  /// <remarks>
  ///   A business object property only implementing the generic <b>IBusinessObjectProperty</b> should only be used
  ///   if the data type of the value accessed through this property is unknown. This rule is espacially important 
  ///   when using business object binding, since the controls used to access the values require a strong typed 
  ///   business object property.
  /// </remarks>
  public interface IBusinessObjectProperty
  {
    /// <summary> Gets a flag indicating whether this property contains multiple values. </summary>
    /// <value> <see langword="true"/> if this property contains multiple values. </value>
    /// <remarks> Multiple values are provided via any type implementing <see cref="IList"/>. </remarks>
    bool IsList { get; }

    /// <summary>Gets the <see cref="IListInfo"/> for this <see cref="IBusinessObjectProperty"/>.</summary>
    /// <value>An onject implementing <see cref="IListInfo"/> or <see langword="null"/> if <see cref="IsList"/> evaluates <see langword="false"/>.</value>
    IListInfo ListInfo { get; }

    /// <summary> Gets the type of the property. </summary>
    /// <remarks> 
    ///   <para>
    ///     This is the type of elements returned by the <see cref="IBusinessObject.GetProperty"/> method
    ///     and set via the <see cref="IBusinessObject.SetProperty"/> method.
    ///   </para><para>
    ///     If <see cref="IsList"/> is <see langword="true"/>, the property type must implement the <see cref="IList"/> 
    ///     interface, and the items contained in this list must have a type of <see cref="ListInfo"/>.<see cref="IListInfo.ItemType"/>.
    ///   </para>
    /// </remarks>
    Type PropertyType { get; }

    /// <summary> Gets an identifier that uniquely defines this property within its class. </summary>
    /// <value> A <see cref="String"/> by which this property can be found within its <see cref="IBusinessObjectClass"/>. </value>
    string Identifier { get; }

    /// <summary> Gets the property name as presented to the user. </summary>
    /// <value> The human readable identifier of this property. </value>
    /// <remarks> The value of this property may depend on the current culture. </remarks>
    string DisplayName { get; }

    /// <summary> Gets a flag indicating whether this property is required. </summary>
    /// <value> <see langword="true"/> if this property is required. </value>
    /// <remarks> Setting required properties to <see langword="null"/> may result in an error. </remarks>
    bool IsRequired { get; }

    /// <summary> Indicates whether this property can be accessed by the user. </summary>
    /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the user can access this property. </returns>
    /// <remarks> The result may depend on the user's authorization and/or the object. </remarks>
    bool IsAccessible ([CanBeNull] IBusinessObject obj);

    /// <summary> Indicates whether this property can be modified by the user. </summary>
    /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the user can set this property. </returns>
    /// <remarks> The result may depend on the user's authorization and/or the object. </remarks>
    bool IsReadOnly ([CanBeNull] IBusinessObject obj);

    /// <summary>Gets the <see cref="IBusinessObjectClass"/> that was used to retrieve this property.</summary>
    /// <value>An instance of the <see cref="IBusinessObjectClass"/> type.</value>
    /// <remarks>
    ///   <note type="inotes">
    ///     Must not return <see langword="null"/> if the property is part of a <see cref="IBusinessObjectClass"/>.
    ///   </note>
    /// </remarks>
    IBusinessObjectClass ReflectedClass { get; }

    /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this property. </summary>
    /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type. </value>
    /// <remarks>
    ///   <note type="inotes">
    ///     Must not return <see langword="null"/>.
    ///   </note>
    /// </remarks>
    IBusinessObjectProvider BusinessObjectProvider { get; }
  }
}
