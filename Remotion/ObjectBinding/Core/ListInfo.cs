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
  public class ListInfo : IListInfo
  {
    private readonly Type _propertyType;
    private readonly Type _itemType;

    public ListInfo (Type propertyType, Type itemType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("propertyType", propertyType, typeof (IList));
      ArgumentUtility.CheckNotNull ("itemType", itemType);
    
      _propertyType = propertyType;
      _itemType = itemType;
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public Type ItemType
    {
      get { return _itemType; }
    }

    public bool RequiresWriteBack
    {
      get { return _propertyType.IsArray; }
    }

    public IList CreateList (int count)
    {
      return Array.CreateInstance (_itemType, count);
    }

    public IList InsertItem (IList list, object item, int index)
    {
      throw new NotImplementedException();
    }

    public IList RemoveItem (IList list, object item)
    {
      throw new NotImplementedException();
    }
  }
}
