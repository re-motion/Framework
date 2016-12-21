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
using System.Collections.Generic;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation
{
  public delegate IList CreateListMethod (int count);

  /// <summary>
  /// Provides utility methods for processing IList instances. 
  /// </summary>
  public static class ListUtility
  {
    /// <summary>
    ///   Adds a range of objects to a list. The original list may be modified.
    /// </summary>
    public static IList AddRange (IList list, IList objects, IBusinessObjectReferenceProperty property, bool mustCreateCopy, bool createIfNull)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);

      CreateListMethod createListMethod = GetCreateListMethod (property);
      if (list == null)
      {
        if (! createIfNull)
          throw new ArgumentNullException ("list");

        list = CreateList (createListMethod, null, objects.Count);
        CopyTo (objects, list);
        return list;
      }

      if (list.IsFixedSize
          || (mustCreateCopy && ! (list is ICloneable)))
      {
        ArrayList arrayList = new ArrayList (list);
        arrayList.AddRange (objects);
        IList newList = CreateList (createListMethod, list, arrayList.Count);
        CopyTo (arrayList, newList);
        return newList;
      }
      else
      {
        if (mustCreateCopy)
          list = (IList) ((ICloneable) list).Clone();

        foreach (object obj in objects)
          list.Add (obj);
        return list;
      }
    }

    /// <summary>
    ///    Removes a range of values from a list and returns the resulting list. The original list may be modified.
    /// </summary>
    public static IList Remove (IList list, IList objects, IBusinessObjectReferenceProperty property, bool mustCreateCopy)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);

      if (list == null)
        return null;

      if (list.IsFixedSize
          || (mustCreateCopy && ! (list is ICloneable)))
      {
        ArrayList arrayList = new ArrayList (list);
        foreach (object obj in objects)
          arrayList.Remove (obj);

        IList newList = CreateList (GetCreateListMethod (property), list, arrayList.Count);
        CopyTo (arrayList, newList);
        return newList;
      }
      else
      {
        if (mustCreateCopy)
          list = (IList) ((ICloneable) list).Clone();

        foreach (object obj in objects)
          list.Remove (obj);
        return list;
      }
    }

    public static IEnumerable<BocListRow> IndicesOf (IList list, IEnumerable<IBusinessObject> values)
    {
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("values", values);

      var indicesMap = new Dictionary<IBusinessObject, BocListRow>();
      var listEnumerator = list.Cast<IBusinessObject>().Select ((o, i) => new BocListRow (i, o)).GetEnumerator();

      try
      {
        foreach (var obj in values)
        {
          BocListRow row;
          if (indicesMap.TryGetValue (obj, out row))
          {
            yield return row;
          }
          else
          {
            while (listEnumerator.MoveNext())
            {
              var bocListRow = Assertion.IsNotNull (listEnumerator.Current);
              indicesMap.Add (bocListRow.BusinessObject, bocListRow);
              if (bocListRow.BusinessObject.Equals (obj))
                yield return bocListRow;
            }
          }
        }
      }
      finally
      {
        listEnumerator.Dispose();
      }
    }

    private static CreateListMethod GetCreateListMethod (IBusinessObjectProperty property)
    {
      if (property == null)
        return null;
      if (!property.IsList)
        throw new ArgumentException (string.Format ("BusinessObjectProperty '{0}' is not a list property.", property.Identifier), "property");
      return property.ListInfo.CreateList;
    }

    private static IList CreateList (CreateListMethod createListMethod, IList template, int size)
    {
      if (createListMethod != null)
        return createListMethod (size);
      else if (template is Array)
        return Array.CreateInstance (template.GetType().GetElementType(), size);
      else 
        throw new NotSupportedException ("Cannot create instance if argument 'createListMethod' is null and 'template' is not an array.");
    }

    private static void CopyTo (IList source, IList destination)
    {
      int len = Math.Min (source.Count, destination.Count);
      for (int i = 0; i < len; ++i)
        destination[i] = source[i];
    }
  }
}
