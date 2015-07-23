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
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Specifies the type of items for properties returning an <see cref="IList"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ItemTypeAttribute : Attribute
  {
    private Type _itemType;

    /// <summary>
    /// Instantiates a new object.
    /// </summary>
    /// <param name="itemType">The type of items returned by the property. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="itemType"/> is <see langword="null"/>.</exception>
    public ItemTypeAttribute (Type itemType)
    {
      ArgumentUtility.CheckNotNull ("itemType", itemType);

      _itemType = itemType;
    }

    /// <summary>
    /// Gets the type of items returned by the property.
    /// </summary>
    public Type ItemType
    {
      get { return _itemType; }
    }
  }
}
