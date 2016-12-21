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
using System.Collections.ObjectModel;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths
{
  /// <summary>
  /// Base-implemention of <see cref="IBusinessObjectPropertyPath"/>. 
  /// See <see cref="StaticBusinessObjectPropertyPath"/> and <see cref="DynamicBusinessObjectPropertyPath"/> concrete implementations.
  /// </summary>
  public abstract class BusinessObjectPropertyPathBase : IBusinessObjectPropertyPath
  {
    protected BusinessObjectPropertyPathBase ()
    {
    }

    public abstract bool IsDynamic { get; }
    public abstract string Identifier { get; }
    public abstract ReadOnlyCollection<IBusinessObjectProperty> Properties { get; }

    protected abstract IBusinessObjectPropertyPathPropertyEnumerator GetResultPropertyEnumerator ();

    public IBusinessObjectPropertyPathResult GetResult (
        IBusinessObject root,
        BusinessObjectPropertyPath.UnreachableValueBehavior unreachableValueBehavior,
        BusinessObjectPropertyPath.ListValueBehavior listValueBehavior)
    {
      ArgumentUtility.CheckNotNull ("root", root);

      var propertyEnumerator = GetResultPropertyEnumerator();
      var currentObject = root;
      int propertyIndex = 0;
      while (propertyEnumerator.MoveNext (currentObject.BusinessObjectClass))
      {
        var currentProperty = propertyEnumerator.Current;

        if (currentProperty == null)
          return new NullBusinessObjectPropertyPathResult();

        if (!propertyEnumerator.HasNext)
          return new EvaluatedBusinessObjectPropertyPathResult (currentObject, currentProperty);

        if (!currentProperty.IsAccessible (currentObject))
        {
          HandlePropertyAccessDenied (unreachableValueBehavior, propertyIndex);
          return new NotAccessibleBusinessObjectPropertyPathResult (currentObject.BusinessObjectClass.BusinessObjectProvider);
        }

        try
        {
          currentObject = GetPropertyValue (currentObject, (IBusinessObjectReferenceProperty) currentProperty, listValueBehavior, propertyIndex);
        }
        catch (BusinessObjectPropertyAccessException)
        {
          HandlePropertyAccessDenied (unreachableValueBehavior, propertyIndex);
          return new NotAccessibleBusinessObjectPropertyPathResult (currentObject.BusinessObjectClass.BusinessObjectProvider);
        }

        if (currentObject == null)
        {
          HandlePropertyValueNull(unreachableValueBehavior, propertyIndex);
          return new NullBusinessObjectPropertyPathResult();
        }

        propertyIndex++;
      }

      throw new InvalidOperationException ("Property path enumeration can never fall through.");
    }

    public override string ToString ()
    {
      return Identifier;
    }

    private IBusinessObject GetPropertyValue (
        IBusinessObject currentObject,
        IBusinessObjectReferenceProperty currentProperty,
        BusinessObjectPropertyPath.ListValueBehavior listValueBehavior,
        int propertyIndex)
    {
      if (currentProperty.IsList)
      {
        if (listValueBehavior == BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties)
        {
          throw new InvalidOperationException (
              string.Format ("Property #{0} of property path '{1}' is not a single-value property.", propertyIndex, Identifier));
        }

        var list = (IList) currentObject.GetProperty (currentProperty);
        if (list.Count > 0)
          return (IBusinessObject) list[0];
        else
          return null;
      }
      else
      {
        return (IBusinessObject) currentObject.GetProperty (currentProperty);
      }
    }

    private void HandlePropertyAccessDenied (BusinessObjectPropertyPath.UnreachableValueBehavior unreachableValueBehavior, int propertyIndex)
    {
      if (unreachableValueBehavior == BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue)
        throw new InvalidOperationException (
            string.Format (
                "Access was denied to property #{0} of property path '{1}'. Cannot evaluate rest of path.", propertyIndex, Identifier));
    }

    private void HandlePropertyValueNull (BusinessObjectPropertyPath.UnreachableValueBehavior unreachableValueBehavior, int propertyIndex)
    {
      if (unreachableValueBehavior == BusinessObjectPropertyPath.UnreachableValueBehavior.FailForUnreachableValue)
      {
        throw new InvalidOperationException (
            string.Format (
            "A null value was returned for property #{0} of property path '{1}'. Cannot evaluate rest of path.", propertyIndex, Identifier));
      }
    }
  }
}