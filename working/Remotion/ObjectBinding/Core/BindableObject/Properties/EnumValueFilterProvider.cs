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
using System.Linq;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  /// <summary>
  /// Retrieves instances of <see cref="IEnumerationValueFilter"/> for enumeration properties.
  /// </summary>
  public class EnumValueFilterProvider<T>
      where T: class, IDisableEnumValuesAttribute
  {
    private readonly IPropertyInformation _propertyInformation;
    private readonly Func<Type, T[]> _typeAttributeProvider;

    public EnumValueFilterProvider (IPropertyInformation propertyInformation, Func<Type, T[]> typeAttributeProvider)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeAttributeProvider", typeAttributeProvider);
      
      _propertyInformation = propertyInformation;
      _typeAttributeProvider = typeAttributeProvider;
    }

    public IPropertyInformation PropertyInformation
    {
      get { return _propertyInformation; }
    }

    public IEnumerationValueFilter GetEnumerationValueFilter ()
    {
      return GetFilterFromProperty () ?? GetFilterFromType () ?? new NullEnumerationValueFilter();
    }

    private IEnumerationValueFilter GetFilterFromProperty ()
    {
      var attribute = PropertyInformation.GetCustomAttribute<T> (true);
      return attribute != null ? attribute.GetEnumerationValueFilter () : null;
    }

    private IEnumerationValueFilter GetFilterFromType ()
    {
      var underlyingType = Nullable.GetUnderlyingType (PropertyInformation.PropertyType) ?? PropertyInformation.PropertyType;
      var attributes = _typeAttributeProvider (underlyingType);
      Assertion.IsNotNull (attributes, "TypeAttributeProvider must return a non-null array.");

      if (attributes.Length == 0)
        return null;
      else if (attributes.Length == 1)
        return attributes[0].GetEnumerationValueFilter ();
      else
        return new CompositeEnumerationValueFilter (attributes.Select (attribute => attribute.GetEnumerationValueFilter ()).ToArray ());
    }
  }
}