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
using System.Text;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths
{
  /// <summary>
  /// Implements <see cref="IBusinessObjectPropertyPath"/> for static binding of the property path identifier. 
  /// If the specified property path cannot be parsed for the specified root <see cref="IBusinessObjectClass"/>, 
  /// a <see cref="ParseException"/> is thrown during initialization of the object.
  /// </summary>
  public sealed class StaticBusinessObjectPropertyPath : BusinessObjectPropertyPathBase
  {
    private readonly string _propertyPathIdentifier;
    private readonly IBusinessObjectProperty[] _properties;

    public static StaticBusinessObjectPropertyPath Parse (string propertyPathIdentifier, IBusinessObjectClass root)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);
      ArgumentUtility.CheckNotNull ("root", root);

      var properties = new List<IBusinessObjectProperty>();
      var currentClass = root;
      var propertyEnumerator = new StaticBusinessObjectPropertyPathPropertyEnumerator (propertyPathIdentifier);

      while (currentClass != null && propertyEnumerator.MoveNext (currentClass))
      {
        var currentProperty = propertyEnumerator.Current;
        Assertion.IsNotNull (currentProperty, "StaticPropertyPathPropertyEnumerator never returns null on successful enumeration.");
        properties.Add (currentProperty);

        var currentReferenceProperty = currentProperty as IBusinessObjectReferenceProperty;
        if (currentReferenceProperty != null)
          currentClass = currentReferenceProperty.ReferenceClass;
        else
          currentClass = null;
      }

      return new StaticBusinessObjectPropertyPath (properties.ToArray(), propertyPathIdentifier);
    }

    public static StaticBusinessObjectPropertyPath Create (IBusinessObjectProperty[] properties)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("properties", properties);

      var identifierBuilder = new StringBuilder();
      var currentClass = properties[0].ReflectedClass;
      for (int index = 0; index < properties.Length; index++)
      {
        var property = properties[index];

        if (!property.Equals (currentClass.GetPropertyDefinition (property.Identifier)))
        {
          throw new ArgumentException (
              string.Format (
                  "Property #{0} ('{1}') is not part of the previous business object class '{2}'. The property path must form a continuous chain.",
                  index,
                  property.Identifier,
                  currentClass.Identifier));
        }

        identifierBuilder.Append (property.Identifier);

        if (index < properties.Length - 1)
        {
          var referenceProperty = property as IBusinessObjectReferenceProperty;
          if (referenceProperty == null)
          {
            throw new ArgumentException (
                string.Format (
                    "Property #{0} ('{1}') is not of type {2}. Every property except the last property must be a reference property.",
                    index, property.Identifier, typeof (IBusinessObjectReferenceProperty).Name),
                "properties");
          }

          identifierBuilder.Append (currentClass.BusinessObjectProvider.GetPropertyPathSeparator());
          currentClass = referenceProperty.ReferenceClass;
        }
      }

      return new StaticBusinessObjectPropertyPath (properties, identifierBuilder.ToString());
    }

    private StaticBusinessObjectPropertyPath (IBusinessObjectProperty[] properties, string propertyPathIdentifier)
    {
      _properties = properties;
      _propertyPathIdentifier = propertyPathIdentifier;
    }

    public override bool IsDynamic
    {
      get { return false; }
    }

    public override string Identifier
    {
      get { return _propertyPathIdentifier; }
    }

    public override ReadOnlyCollection<IBusinessObjectProperty> Properties
    {
      get { return new ReadOnlyCollection<IBusinessObjectProperty> (_properties); }
    }

    protected override IBusinessObjectPropertyPathPropertyEnumerator GetResultPropertyEnumerator ()
    {
      return new ResolvedBusinessObjectPropertyPathPropertyEnumerator (_properties);
    }
  }
}