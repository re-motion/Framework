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
using System.Linq;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Combines one or more <see cref="IBindablePropertyWriteAccessStrategy"/>-instances and delegates checking if the property can be written to.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IBindablePropertyWriteAccessStrategy), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public sealed class CompundBindablePropertyWriteAccessStrategy : IBindablePropertyWriteAccessStrategy
  {
    // Using an array instead of IReadOnlyList to support performance critical loop
    private readonly IBindablePropertyWriteAccessStrategy[] _bindablePropertyWriteAccessStrategies;

    public CompundBindablePropertyWriteAccessStrategy (IEnumerable<IBindablePropertyWriteAccessStrategy> bindablePropertyWriteAccessStrategies)
    {
      ArgumentUtility.CheckNotNull ("bindablePropertyWriteAccessStrategies", bindablePropertyWriteAccessStrategies);

      _bindablePropertyWriteAccessStrategies = bindablePropertyWriteAccessStrategies.ToArray();
    }

    public IReadOnlyList<IBindablePropertyWriteAccessStrategy> BindablePropertyWriteAccessStrategies
    {
      get { return _bindablePropertyWriteAccessStrategies.ToList().AsReadOnly(); }
    }

    public bool CanWrite (IBusinessObject businessObject, PropertyBase bindableProperty)
    {
      // businessObject can be null
      ArgumentUtility.DebugCheckNotNull ("bindableProperty", bindableProperty);

      // This section is performance critical. No closure should be created, therefor converting this code to Linq is not possible.
      // return _strategies.All (s => s.CanRead (propertyBase, businessObject));
      // ReSharper disable once LoopCanBeConvertedToQuery
      // ReSharper disable once ForCanBeConvertedToForeach
      for (int i = 0; i < _bindablePropertyWriteAccessStrategies.Length; i++)
      {
        if (!_bindablePropertyWriteAccessStrategies[i].CanWrite (businessObject, bindableProperty))
          return false;
      }
      return true;
    }

    public bool IsPropertyAccessException (
        IBusinessObject businessObject,
        PropertyBase bindableProperty,
        Exception exception,
        out BusinessObjectPropertyAccessException propertyAccessException)
    {
      ArgumentUtility.DebugCheckNotNull ("businessObject", businessObject);
      ArgumentUtility.DebugCheckNotNull ("bindableProperty", bindableProperty);
      ArgumentUtility.DebugCheckNotNull ("exception", exception);

      // This section does represent an inherrent hot-path but the for-loop is chosen for symmetry with the CanRead()-method.
      // ReSharper disable once ForCanBeConvertedToForeach
      for (int i = 0; i < _bindablePropertyWriteAccessStrategies.Length; i++)
      {
        var bindablePropertyReadAccessStrategy = _bindablePropertyWriteAccessStrategies[i];
        if (bindablePropertyReadAccessStrategy.IsPropertyAccessException (businessObject, bindableProperty, exception, out propertyAccessException))
          return true;
      }
      propertyAccessException = null;
      return false;
    }
  }
}