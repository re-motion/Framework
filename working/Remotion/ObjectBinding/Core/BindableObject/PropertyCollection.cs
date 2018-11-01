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
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Provides a read-only collection for <see cref="PropertyBase"/> objects.
  /// </summary>
  public sealed class PropertyCollection
  {
    private class PropertyKeyedCollection : KeyedCollection<string, PropertyBase>
    {
      public PropertyKeyedCollection ()
      {
      }

      protected override string GetKeyForItem (PropertyBase item)
      {
        return item.Identifier;
      }
    }

    private readonly PropertyKeyedCollection _innerCollection = new PropertyKeyedCollection();

    public PropertyCollection (IEnumerable<PropertyBase> properties)
    {
      ArgumentUtility.CheckNotNull ("properties", properties);

      foreach (var property in properties)
        _innerCollection.Add (property);
    }

    public bool Contains (string propertyIdentifier)
    {
      return _innerCollection.Contains (propertyIdentifier);
    }

    public PropertyBase this[string propertyIdentifier]
    {
      get { return _innerCollection[propertyIdentifier]; }
    }

    public PropertyBase[] ToArray ()
    {
      return ArrayUtility.Convert (_innerCollection);
    }
  }
}