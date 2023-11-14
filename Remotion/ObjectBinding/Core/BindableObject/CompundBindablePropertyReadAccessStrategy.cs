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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Combines one or more <see cref="IBindablePropertyReadAccessStrategy"/>-instances and delegates checking if the property can be read from.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IBindablePropertyReadAccessStrategy), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public sealed class CompundBindablePropertyReadAccessStrategy : IBindablePropertyReadAccessStrategy
  {
    // Using an array instead of IReadOnlyList to support performance critical loop
    private readonly IBindablePropertyReadAccessStrategy[] _bindablePropertyReadAccessStrategies;

    public CompundBindablePropertyReadAccessStrategy (IEnumerable<IBindablePropertyReadAccessStrategy> bindablePropertyReadAccessStrategies)
    {
      ArgumentUtility.CheckNotNull("bindablePropertyReadAccessStrategies", bindablePropertyReadAccessStrategies);

      _bindablePropertyReadAccessStrategies = bindablePropertyReadAccessStrategies.ToArray();
    }

    public IReadOnlyList<IBindablePropertyReadAccessStrategy> BindablePropertyReadAccessStrategies
    {
      get { return _bindablePropertyReadAccessStrategies.ToList().AsReadOnly(); }
    }


    public bool CanRead (IBusinessObject? businessObject, PropertyBase bindableProperty)
    {
      // businessObject can be null
      ArgumentUtility.DebugCheckNotNull("bindableProperty", bindableProperty);

      // This section is performance critical. No closure should be created, therefor converting this code to Linq is not possible.
      // return _strategies.All (s => s.CanRead (propertyBase, businessObject));
      // ReSharper disable once LoopCanBeConvertedToQuery
      // ReSharper disable once ForCanBeConvertedToForeach
      for (int i = 0; i < _bindablePropertyReadAccessStrategies.Length; i++)
      {
        var bindablePropertyReadAccessStrategy = _bindablePropertyReadAccessStrategies[i];
        if (!bindablePropertyReadAccessStrategy.CanRead(businessObject, bindableProperty))
          return false;
      }
      return true;
    }

    public bool IsPropertyAccessException (
        IBusinessObject businessObject,
        PropertyBase bindableProperty,
        Exception exception,
        [MaybeNullWhen(false)] out BusinessObjectPropertyAccessException propertyAccessException)
    {
      ArgumentUtility.DebugCheckNotNull("businessObject", businessObject);
      ArgumentUtility.DebugCheckNotNull("bindableProperty", bindableProperty);
      ArgumentUtility.DebugCheckNotNull("exception", exception);

      // This section does represent an inherrent hot-path but the for-loop is chosen for symmetry with the CanRead()-method.
      // ReSharper disable once ForCanBeConvertedToForeach
      for (int i = 0; i < _bindablePropertyReadAccessStrategies.Length; i++)
      {
        var bindablePropertyReadAccessStrategy = _bindablePropertyReadAccessStrategies[i];
        if (bindablePropertyReadAccessStrategy.IsPropertyAccessException(businessObject, bindableProperty, exception, out propertyAccessException))
          return true;
      }
      propertyAccessException = null;
      return false;
    }
  }
}
