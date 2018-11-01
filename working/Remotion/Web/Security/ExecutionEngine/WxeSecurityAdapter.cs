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
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Security.ExecutionEngine
{
  [ImplementationFor (typeof (IWxeSecurityAdapter), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class WxeSecurityAdapter : IWxeSecurityAdapter
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeSecurityAdapter ()
    {
    }

    // methods and properties

    public void CheckAccess (WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("function", function);

      if (SecurityFreeSection.IsActive)
        return;

      WxeDemandTargetPermissionAttribute attribute = GetPermissionAttribute (function.GetType ());
      if (attribute == null)
        return;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (function.GetType (), attribute);
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();

      switch (helper.MethodType)
      {
        case MethodType.Instance:
          securityClient.CheckMethodAccess (helper.GetSecurableObject (function), helper.MethodName);
          break;
        case MethodType.Static:
          securityClient.CheckStaticMethodAccess (helper.SecurableClass, helper.MethodName);
          break;
        case MethodType.Constructor:
          securityClient.CheckConstructorAccess (helper.SecurableClass);
          break;
        default:
          throw new InvalidOperationException (string.Format (
              "Value '{0}' is not supported by the MethodType property of the WxeDemandMethodPermissionAttribute.",
              helper.MethodType));
      }
    }

    public bool HasAccess (WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("function", function);

      if (SecurityFreeSection.IsActive)
        return true;

      WxeDemandTargetPermissionAttribute attribute = GetPermissionAttribute (function.GetType ());
      if (attribute == null)
        return true;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (function.GetType (), attribute);
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();

      switch (helper.MethodType)
      {
        case MethodType.Instance:
          return securityClient.HasMethodAccess (helper.GetSecurableObject (function), helper.MethodName);
        case MethodType.Static:
          return securityClient.HasStaticMethodAccess (helper.SecurableClass, helper.MethodName);
        case MethodType.Constructor:
          return securityClient.HasConstructorAccess (helper.SecurableClass);
        default:
          throw new InvalidOperationException (string.Format (
              "Value '{0}' is not supported by the MethodType property of the WxeDemandMethodPermissionAttribute.",
              helper.MethodType));
      }
    }

    public bool HasStatelessAccess (Type functionType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("functionType", functionType, typeof (WxeFunction));

      if (SecurityFreeSection.IsActive)
        return true;

      WxeDemandTargetPermissionAttribute attribute = GetPermissionAttribute (functionType);
      if (attribute == null)
        return true;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (functionType, attribute);
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
     
      switch (helper.MethodType)
      {
        case MethodType.Instance:
          return securityClient.HasStatelessMethodAccess (helper.GetTypeOfSecurableObject(), helper.MethodName);
        case MethodType.Static:
          return securityClient.HasStaticMethodAccess (helper.SecurableClass, helper.MethodName);
        case MethodType.Constructor:
          return securityClient.HasConstructorAccess (helper.SecurableClass);
        default:
          throw new InvalidOperationException (string.Format (
              "Value '{0}' is not supported by the MethodType property of the WxeDemandMethodPermissionAttribute.",
              helper.MethodType));
      }
    }

    private WxeDemandTargetPermissionAttribute GetPermissionAttribute (Type functionType)
    {
      return (WxeDemandTargetPermissionAttribute) Attribute.GetCustomAttribute (functionType, typeof (WxeDemandTargetPermissionAttribute), true);
    }
  }
}
