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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// Implements <see cref="IInterceptedPropertyFinder"/> by delegating to a new instance of <see cref="InterceptedPropertyCollector"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public class InterceptedPropertyCollectorAdapter : IInterceptedPropertyFinder
  {
    private static readonly IRelatedMethodFinder s_relatedMethodFinder = new RelatedMethodFinder();
    private readonly ITypeConversionProvider _typeConversionProvider;

    public InterceptedPropertyCollectorAdapter ()
    {
      _typeConversionProvider = SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>();
    }

    public IEnumerable<IAccessorInterceptor> GetPropertyInterceptors (ClassDefinition classDefinition, Type concreteBaseType)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("concreteBaseType", concreteBaseType, typeof (DomainObject));

      var properties = new InterceptedPropertyCollector (classDefinition, _typeConversionProvider).GetProperties();

      var interceptors = new List<IAccessorInterceptor>();
      foreach (var propertyEntry in properties)
      {
        var property = propertyEntry.Item1;
        var propertyName = propertyEntry.Item2;

        var getter = property.GetGetMethod (true);
        var setter = property.GetSetMethod (true);

        AddAccessorInterceptor (interceptors, concreteBaseType, getter, propertyName, property.PropertyType, isGetter: true);
        AddAccessorInterceptor (interceptors, concreteBaseType, setter, propertyName, property.PropertyType, isGetter: false);
      }

      return interceptors;
    }

    private void AddAccessorInterceptor (
        List<IAccessorInterceptor> interceptors, Type concreteBaseType, MethodInfo accessor, string propertyName, Type propertyType, bool isGetter)
    {
      if (accessor == null)
        return;

      var mostDerivedAccessor = s_relatedMethodFinder.GetMostDerivedOverride (accessor, concreteBaseType);
      if (!InterceptedPropertyCollector.IsOverridable (mostDerivedAccessor))
        return;

      var interceptor = CreateAccessorInterceptor (mostDerivedAccessor, propertyName, propertyType, isGetter);
      interceptors.Add (interceptor);
    }

    private IAccessorInterceptor CreateAccessorInterceptor (MethodInfo interceptedAccessor, string propertyName, Type propertyType, bool isGetter)
    {
      if (InterceptedPropertyCollector.IsAutomaticPropertyAccessor (interceptedAccessor))
      {
        if (isGetter)
          return new ImplementingGetAccessorInterceptor (interceptedAccessor, propertyName, propertyType);
        else
          return new ImplementingSetAccessorInterceptor (interceptedAccessor, propertyName, propertyType);
      }
      else
        return new WrappingAccessorInterceptor (interceptedAccessor, propertyName);
    }
  }
}