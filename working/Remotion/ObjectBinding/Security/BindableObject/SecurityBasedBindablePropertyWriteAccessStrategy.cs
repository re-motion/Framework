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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Security.BindableObject
{
  /// <summary>
  /// Checks if the current <see cref="ISecurityPrincipal"/> can access the property's setter.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IBindablePropertyWriteAccessStrategy),
      Lifetime = LifetimeKind.Singleton,
      RegistrationType = RegistrationType.Multiple,
      Position = Position)]
  public sealed class SecurityBasedBindablePropertyWriteAccessStrategy : IBindablePropertyWriteAccessStrategy
  {
    public const int Position = 117;

    private static readonly NullMethodInformation s_nullMethodInformation = new NullMethodInformation();

    private readonly SecurityClient _securityClient;

    public SecurityBasedBindablePropertyWriteAccessStrategy ()
    {
      _securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
    }

    public bool CanWrite (IBusinessObject businessObject, PropertyBase bindableProperty)
    {
      // businessObject can be null
      ArgumentUtility.CheckNotNull ("bindableProperty", bindableProperty);

      var securableObject = businessObject as ISecurableObject;
      if (securableObject == null)
        return true;

      var setter = bindableProperty.PropertyInfo.GetSetMethod (true) ?? s_nullMethodInformation;
      return _securityClient.HasPropertyWriteAccess (securableObject, setter);
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

      if (exception is PermissionDeniedException)
      {
        ArgumentUtility.CheckNotNull ("businessObject", businessObject);
        ArgumentUtility.CheckNotNull ("bindableProperty", bindableProperty);

        var classOrInstance = businessObject is IBusinessObjectWithIdentity
            ? string.Format ("for business object with ID '{0}'", ((IBusinessObjectWithIdentity) businessObject).UniqueIdentifier)
            : string.Format ("for business object type '{0}'", businessObject.BusinessObjectClass.Identifier);

        var message = string.Format (
            "A PermissionDeniedException occured while getting the value of property '{0}' {1}.",
            bindableProperty.Identifier,
            classOrInstance);

        propertyAccessException = new BusinessObjectPropertyAccessException (message, exception);
        return true;
      }

      propertyAccessException = null;
      return false;
    }
  }
}