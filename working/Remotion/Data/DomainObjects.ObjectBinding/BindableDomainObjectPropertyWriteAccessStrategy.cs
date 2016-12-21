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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Implementation of the <see cref="IBindablePropertyWriteAccessStrategy"/> interface for <see cref="DomainObject"/>.
  /// </summary>
  [ImplementationFor (typeof (IBindablePropertyWriteAccessStrategy),
      Lifetime = LifetimeKind.Singleton,
      RegistrationType = RegistrationType.Multiple,
      Position = Position)]
  public sealed class BindableDomainObjectPropertyWriteAccessStrategy : IBindablePropertyWriteAccessStrategy
  {
    public const int Position = 79;

    public bool CanWrite (IBusinessObject businessObject, PropertyBase bindableProperty)
    {
      // businessObject can be null
      ArgumentUtility.DebugCheckNotNull ("bindableProperty", bindableProperty);

      var domainObject = businessObject as DomainObject;
      if (domainObject == null)
        return true;

      switch (domainObject.State)
      {
        case StateType.Unchanged:
        case StateType.Changed:
        case StateType.New:
          return true;
        case StateType.Deleted:
        case StateType.Invalid:
          return false;
        case StateType.NotLoadedYet:
          return domainObject.TryEnsureDataAvailable();
        default:
          throw new NotSupportedException(string.Format ("The StateType '{0}' is not supported.", domainObject.State));
      }
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

      var isPropertyAccessException = exception is ObjectInvalidException
                                      || exception is ObjectDeletedException
                                      || exception is ObjectsNotFoundException;

      if (isPropertyAccessException && businessObject is DomainObject)
      {
        ArgumentUtility.CheckNotNull ("bindableProperty", bindableProperty);

        var message = string.Format (
            "An {0} occured while setting the value of property '{1}' for business object with ID '{2}'.",
            exception.GetType().Name,
            bindableProperty.Identifier,
            ((DomainObject) businessObject).ID);
        propertyAccessException = new BusinessObjectPropertyAccessException (message, exception);
        return true;
      }
      propertyAccessException = null;
      return false;
    }
  }
}