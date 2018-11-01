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
using Remotion.ExtensibleEnums.Infrastructure;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Provides a non-generic interface for the <see cref="ExtensibleEnum{T}"/> class.
  /// </summary>
  /// <remarks>
  /// Do not implement this interface yourself, use it only when working with extensible enums in a reflective context, e.g. via the 
  /// <see cref="ExtensibleEnumDefinitionCache"/> class.
  /// </remarks>
  public interface IExtensibleEnum
  {
    /// <summary>
    /// Gets the identifier representing this extensible enum value. This is the combination of <see cref="DeclarationSpace"/> and 
    /// <see cref="ValueName"/>. Use <see cref="IExtensibleEnumDefinition.GetValueInfoByID"/> to retrieve an <see cref="IExtensibleEnum"/>
    /// value by its <see cref="ID"/>.
    /// </summary>
    /// <value>The ID of this value.</value>
    string ID { get; }

    /// <summary>
    /// Gets a string identifying the declaration space of the identifier of the value being created. This can be a 
    /// namespace, a type name, or anything else that helps in uniquely identifying the enum value. It is used as a prefix to the <see cref="ID"/>
    /// of the value. Can be <see langword="null" />.
    /// </summary>
    /// <value>The declaration space of this value, or <see langword="null" /> if the value does not define a declaration space.</value>
    string DeclarationSpace { get; }

    /// <summary>
    /// Gets name of this value. This is a part of the <see cref="ID"/> of this extensible enum value.
    /// </summary>
    /// <value>The name of this value.</value>
    string ValueName { get; }

    /// <summary>
    /// Gets the type of the extensible enum this value belongs to.
    /// </summary>
    Type GetEnumType ();

    /// <summary>
    /// Gets the <see cref="IExtensibleEnumInfo"/> object describing the value represented by this instance.
    /// </summary>
    /// <returns>The <see cref="IExtensibleEnumInfo"/> for this value.</returns>
    IExtensibleEnumInfo GetValueInfo ();
  }
}
