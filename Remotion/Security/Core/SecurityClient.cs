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
using System.ComponentModel;
using System.Reflection;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Security
{
  public class SecurityClient : INullObject
  {
    public static readonly SecurityClient Null = new NullSecurityClient();

    private static readonly Enum s_readAccessTypeAsEnum = GeneralAccessTypes.Read;
    private static readonly Enum s_editAccessTypeAsEnum = GeneralAccessTypes.Edit;
    private static readonly Enum[] s_readAccessTypeEnumAsArray = { s_readAccessTypeAsEnum };
    private static readonly Enum[] s_editAccessTypeEnumAsArray = { s_editAccessTypeAsEnum };

    private static readonly AccessType s_readAccessType = AccessType.Get(s_readAccessTypeAsEnum);
    private static readonly AccessType s_editAccessType = AccessType.Get(s_editAccessTypeAsEnum);
    private static readonly IReadOnlyList<AccessType> s_createAccessTypeAsList = ImmutableSingleton.Create(AccessType.Get(GeneralAccessTypes.Create));
    private static readonly IReadOnlyList<AccessType> s_readAccessTypeAsList = ImmutableSingleton.Create(s_readAccessType);
    private static readonly IReadOnlyList<AccessType> s_editAccessTypeAsList = ImmutableSingleton.Create(s_editAccessType);

    public static SecurityClient CreateSecurityClientFromConfiguration ()
    {
      var serviceLocator = SafeServiceLocator.Current;

      var securityProvider = serviceLocator.GetInstance<ISecurityProvider>();
      if (securityProvider.IsNull)
        return SecurityClient.Null;

      return new SecurityClient(
          securityProvider,
          serviceLocator.GetInstance<IPermissionProvider>(),
          serviceLocator.GetInstance<IPrincipalProvider>(),
          serviceLocator.GetInstance<IFunctionalSecurityStrategy>(),
          serviceLocator.GetInstance<IMemberResolver>());
    }

    private readonly ISecurityProvider _securityProvider;
    private readonly IPermissionProvider _permissionProvider;
    private readonly IPrincipalProvider _principalProvider;
    private readonly IFunctionalSecurityStrategy _functionalSecurityStrategy;
    private readonly IMemberResolver _memberResolver;

    public SecurityClient (
        ISecurityProvider securityProvider,
        IPermissionProvider permissionProvider,
        IPrincipalProvider principalProvider,
        IFunctionalSecurityStrategy functionalSecurityStrategy,
        IMemberResolver memberResolver)
    {
      ArgumentUtility.CheckNotNull("securityProvider", securityProvider);
      ArgumentUtility.CheckNotNull("permissionProvider", permissionProvider);
      ArgumentUtility.CheckNotNull("principalProvider", principalProvider);
      ArgumentUtility.CheckNotNull("functionalSecurityStrategy", functionalSecurityStrategy);
      ArgumentUtility.CheckNotNull("memberResolver", memberResolver);

      _securityProvider = securityProvider;
      _permissionProvider = permissionProvider;
      _principalProvider = principalProvider;
      _functionalSecurityStrategy = functionalSecurityStrategy;
      _memberResolver = memberResolver;
    }

    public IPermissionProvider PermissionProvider
    {
      get { return _permissionProvider; }
    }

    public IPrincipalProvider PrincipalProvider
    {
      get { return _principalProvider; }
    }

    public IFunctionalSecurityStrategy FunctionalSecurityStrategy
    {
      get { return _functionalSecurityStrategy; }
    }

    public IMemberResolver MemberResolver
    {
      get { return _memberResolver; }
    }

    public ISecurityProvider SecurityProvider
    {
      get { return _securityProvider; }
    }

    public bool HasAccess (ISecurableObject securableObject, params AccessType[] requiredAccessTypes)
    {
      return HasAccess(securableObject, _principalProvider.GetPrincipal(), (IReadOnlyList<AccessType>)requiredAccessTypes);
    }

    public bool HasAccess (ISecurableObject securableObject, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      return HasAccess(securableObject, _principalProvider.GetPrincipal(), requiredAccessTypes);
    }

    public bool HasAccess (ISecurableObject securableObject, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      return HasAccess(securableObject, principal, (IReadOnlyList<AccessType>)requiredAccessTypes);
    }

    public virtual bool HasAccess (ISecurableObject securableObject, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull("securableObject", securableObject);
      ArgumentUtility.CheckNotNull("principal", principal);
      ArgumentUtility.CheckNotNull("requiredAccessTypes", requiredAccessTypes);

      if (SecurityFreeSection.IsActive)
        return true;

      var objectSecurityStrategy = securableObject.GetSecurityStrategy();
      Assertion.DebugIsNotNull(objectSecurityStrategy, "The securableObject did not return an IObjectSecurityStrategy.");

      return objectSecurityStrategy.HasAccess(_securityProvider, principal, requiredAccessTypes);
    }

    public void CheckAccess (ISecurableObject securableObject, params AccessType[] requiredAccessTypes)
    {
      CheckAccess(securableObject, _principalProvider.GetPrincipal(), (IReadOnlyList<AccessType>)requiredAccessTypes);
    }

    public void CheckAccess (ISecurableObject securableObject, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      CheckAccess(securableObject, _principalProvider.GetPrincipal(), requiredAccessTypes);
    }

    public void CheckAccess (ISecurableObject securableObject, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      CheckAccess(securableObject, principal, (IReadOnlyList<AccessType>)requiredAccessTypes);
    }

    public void CheckAccess (ISecurableObject securableObject, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      ArgumentUtility.DebugCheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull("principal", principal);
      ArgumentUtility.DebugCheckNotNull("requiredAccessTypes", requiredAccessTypes);

      if (!HasAccess(securableObject, principal, requiredAccessTypes))
        throw CreatePermissionDeniedException("Access has been denied.");
    }


    public bool HasStatelessAccess (Type securableClass, params AccessType[] requiredAccessTypes)
    {
      return HasStatelessAccess(securableClass, _principalProvider.GetPrincipal(), (IReadOnlyList<AccessType>)requiredAccessTypes);
    }

    public bool HasStatelessAccess (Type securableClass, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      return HasStatelessAccess(securableClass, _principalProvider.GetPrincipal(), requiredAccessTypes);
    }

    public bool HasStatelessAccess (Type securableClass, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      return HasStatelessAccess(securableClass, principal, (IReadOnlyList<AccessType>)requiredAccessTypes);
    }

    public virtual bool HasStatelessAccess (Type securableClass, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull("securableClass", securableClass);
      ArgumentUtility.CheckNotNull("principal", principal);
      ArgumentUtility.CheckNotNull("requiredAccessTypes", requiredAccessTypes);

      if (SecurityFreeSection.IsActive)
        return true;

      return _functionalSecurityStrategy.HasAccess(securableClass, _securityProvider, principal, requiredAccessTypes);
    }

    public void CheckStatelessAccess (Type securableClass, params AccessType[] requiredAccessTypes)
    {
      CheckStatelessAccess(securableClass, _principalProvider.GetPrincipal(), (IReadOnlyList<AccessType>)requiredAccessTypes);
    }

    public void CheckStatelessAccess (Type securableClass, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      CheckStatelessAccess(securableClass, _principalProvider.GetPrincipal(), requiredAccessTypes);
    }

    public void CheckStatelessAccess (Type securableClass, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      CheckStatelessAccess(securableClass, principal, (IReadOnlyList<AccessType>)requiredAccessTypes);
    }

    public void CheckStatelessAccess (Type securableClass, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      ArgumentUtility.DebugCheckNotNull("securableClass", securableClass);
      ArgumentUtility.DebugCheckNotNull("principal", principal);
      ArgumentUtility.DebugCheckNotNull("requiredAccessTypes", requiredAccessTypes);

      if (!HasStatelessAccess(securableClass, principal, requiredAccessTypes))
        throw CreatePermissionDeniedException("Access has been denied.");
    }


    public bool HasMethodAccess (ISecurableObject securableObject, string methodName)
    {
      return HasMethodAccess(securableObject, methodName, _principalProvider.GetPrincipal());
    }

    public bool HasMethodAccess (ISecurableObject securableObject, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty("methodName", methodName);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var methodInformation = _memberResolver.GetMethodInformation(securableObject.GetSecurableType(), methodName, MemberAffiliation.Instance);
      return HasMethodAccess(securableObject, methodInformation, principal);
    }

    public bool HasMethodAccess (ISecurableObject securableObject, MethodInfo methodInfo)
    {
      return HasMethodAccess(securableObject, methodInfo, _principalProvider.GetPrincipal());
    }

    public bool HasMethodAccess (ISecurableObject securableObject, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableObject", securableObject);
      ArgumentUtility.CheckNotNull("methodInfo", methodInfo);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var methodInformation = _memberResolver.GetMethodInformation(securableObject.GetSecurableType(), methodInfo, MemberAffiliation.Instance);
      return HasMethodAccess(securableObject, methodInformation, principal);
    }

    public bool HasMethodAccess (ISecurableObject securableObject, IMethodInformation methodInformation)
    {
      return HasMethodAccess(securableObject, methodInformation, _principalProvider.GetPrincipal());
    }

    public virtual bool HasMethodAccess (ISecurableObject securableObject, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableObject", securableObject);
      ArgumentUtility.CheckNotNull("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions(securableObject.GetSecurableType(), methodInformation);
      Assertion.DebugIsNotNull(requiredAccessTypeEnums, "IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.");

      return HasAccess(securableObject, methodInformation, requiredAccessTypeEnums, principal);
    }

    public void CheckMethodAccess (ISecurableObject securableObject, string methodName)
    {
      CheckMethodAccess(securableObject, methodName, _principalProvider.GetPrincipal());
    }

    public void CheckMethodAccess (ISecurableObject securableObject, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNullOrEmpty("methodName", methodName);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasMethodAccess(securableObject, methodName, principal))
      {
        ArgumentUtility.CheckNotNull("securableObject", securableObject);

        throw CreatePermissionDeniedException(
            "Access to method '{0}' on type '{1}' has been denied.", methodName, securableObject.GetSecurableType().GetFullNameSafe());
      }
    }

    public void CheckMethodAccess (ISecurableObject securableObject, MethodInfo methodInfo)
    {
      CheckMethodAccess(securableObject, methodInfo, _principalProvider.GetPrincipal());
    }

    public void CheckMethodAccess (ISecurableObject securableObject, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull("methodInfo", methodInfo);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasMethodAccess(securableObject, methodInfo, principal))
      {
        ArgumentUtility.CheckNotNull("securableObject", securableObject);
        ArgumentUtility.CheckNotNull("methodInfo", methodInfo);

        throw CreatePermissionDeniedException(
            "Access to method '{0}' on type '{1}' has been denied.", methodInfo.Name, securableObject.GetSecurableType().GetFullNameSafe());
      }
    }

    public void CheckMethodAccess (ISecurableObject securableObject, IMethodInformation methodInformation)
    {
      CheckMethodAccess(securableObject, methodInformation, _principalProvider.GetPrincipal());
    }

    public void CheckMethodAccess (ISecurableObject securableObject, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasMethodAccess(securableObject, methodInformation, principal))
      {
        ArgumentUtility.CheckNotNull("securableObject", securableObject);
        ArgumentUtility.CheckNotNull("methodInformation", methodInformation);

        throw CreatePermissionDeniedException(
            "Access to method '{0}' on type '{1}' has been denied.",
            methodInformation.Name,
            securableObject.GetSecurableType().GetFullNameSafe());
      }
    }


    public bool HasPropertyReadAccess (ISecurableObject securableObject, string propertyName)
    {
      return HasPropertyReadAccess(securableObject, propertyName, _principalProvider.GetPrincipal());
    }

    public bool HasPropertyReadAccess (ISecurableObject securableObject, string propertyName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var methodInformation = _memberResolver.GetMethodInformation(securableObject.GetSecurableType(), "get_" + propertyName, MemberAffiliation.Instance);
      return HasPropertyReadAccess(securableObject, methodInformation, principal);
    }

    public bool HasPropertyReadAccess (ISecurableObject securableObject, MethodInfo methodInfo)
    {
      return HasPropertyReadAccess(securableObject, methodInfo, _principalProvider.GetPrincipal());
    }

    public bool HasPropertyReadAccess (ISecurableObject securableObject, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull("methodInfo", methodInfo );
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var methodInformation = _memberResolver.GetMethodInformation(securableObject.GetSecurableType(), methodInfo, MemberAffiliation.Instance);
      return HasPropertyReadAccess(securableObject, methodInformation, principal);
    }

    public bool HasPropertyReadAccess (ISecurableObject securableObject, IMethodInformation methodInformation)
    {
      return HasPropertyReadAccess(securableObject, methodInformation, _principalProvider.GetPrincipal());
    }

    public virtual bool HasPropertyReadAccess (ISecurableObject securableObject, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableObject", securableObject);
      ArgumentUtility.CheckNotNull("methodInformation", methodInformation);
      ArgumentUtility.CheckNotNull("principal", principal);

      var requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions(securableObject.GetSecurableType(), methodInformation);
      Assertion.DebugIsNotNull(requiredAccessTypeEnums, "IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.");

      if (requiredAccessTypeEnums.Count == 0)
        requiredAccessTypeEnums = s_readAccessTypeEnumAsArray;

      return HasAccess(securableObject, methodInformation, requiredAccessTypeEnums, principal);
    }

    public void CheckPropertyReadAccess (ISecurableObject securableObject, string propertyName)
    {
      CheckPropertyReadAccess(securableObject, propertyName, _principalProvider.GetPrincipal());
    }

    public void CheckPropertyReadAccess (ISecurableObject securableObject, string propertyName, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNullOrEmpty("propertyName", propertyName);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasPropertyReadAccess(securableObject, propertyName, principal))
      {
        ArgumentUtility.CheckNotNull("securableObject", securableObject);
        ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);

        throw CreatePermissionDeniedException(
            "Access to method '{0}' on type '{1}' has been denied.",
            propertyName,
            securableObject.GetSecurableType().GetFullNameSafe());
      }
    }

    public void CheckPropertyReadAccess (ISecurableObject securableObject, MethodInfo methodInfo)
    {
      CheckPropertyReadAccess(securableObject, methodInfo, _principalProvider.GetPrincipal());
    }

    public void CheckPropertyReadAccess (ISecurableObject securableObject, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull("methodInfo", methodInfo);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasPropertyReadAccess(securableObject, methodInfo, principal))
      {
        ArgumentUtility.CheckNotNull("securableObject", securableObject);
        ArgumentUtility.CheckNotNull("methodInfo", methodInfo);

        throw CreatePermissionDeniedException(
            "Access to get-accessor of property '{0}' on type '{1}' has been denied.",
            methodInfo.Name,
            securableObject.GetSecurableType().GetFullNameSafe());
      }
    }

    public void CheckPropertyReadAccess (ISecurableObject securableObject, IMethodInformation methodInformation)
    {
      CheckPropertyReadAccess(securableObject, methodInformation, _principalProvider.GetPrincipal());
    }

    public void CheckPropertyReadAccess (ISecurableObject securableObject, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasPropertyReadAccess(securableObject, methodInformation, principal))
      {
        ArgumentUtility.CheckNotNull("securableObject", securableObject);
        ArgumentUtility.CheckNotNull("methodInformation", methodInformation);

        throw CreatePermissionDeniedException(
            "Access to method '{0}' on type '{1}' has been denied.",
            methodInformation.Name,
            securableObject.GetSecurableType().GetFullNameSafe());
      }
    }


    public bool HasPropertyWriteAccess (ISecurableObject securableObject, string propertyName)
    {
      return HasPropertyWriteAccess(securableObject, propertyName, _principalProvider.GetPrincipal());
    }

    public bool HasPropertyWriteAccess (ISecurableObject securableObject, string propertyName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableObject", securableObject);
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var methodInformation = _memberResolver.GetMethodInformation(securableObject.GetSecurableType(), "set_" + propertyName, MemberAffiliation.Instance);
      return HasPropertyWriteAccess(securableObject, methodInformation, principal);
    }

    public bool HasPropertyWriteAccess (ISecurableObject securableObject, MethodInfo methodInfo)
    {
      return HasPropertyWriteAccess(securableObject, methodInfo, _principalProvider.GetPrincipal());
    }

    public bool HasPropertyWriteAccess (ISecurableObject securableObject, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableObject", securableObject);
      ArgumentUtility.CheckNotNull("methodInfo", methodInfo);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var methodInformation = _memberResolver.GetMethodInformation(securableObject.GetSecurableType(), methodInfo, MemberAffiliation.Instance);
      return HasPropertyWriteAccess(securableObject, methodInformation, principal);
    }

    public bool HasPropertyWriteAccess (ISecurableObject securableObject, IMethodInformation methodInformation)
    {
      return HasPropertyWriteAccess(securableObject, methodInformation, _principalProvider.GetPrincipal());
    }

    public virtual bool HasPropertyWriteAccess (ISecurableObject securableObject, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableObject", securableObject);
      ArgumentUtility.CheckNotNull("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions(securableObject.GetSecurableType(), methodInformation);
      Assertion.DebugIsNotNull(requiredAccessTypeEnums, "IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.");

      if (requiredAccessTypeEnums.Count == 0)
        requiredAccessTypeEnums = s_editAccessTypeEnumAsArray;

      return HasAccess(securableObject, methodInformation, requiredAccessTypeEnums, principal);
    }

    public void CheckPropertyWriteAccess (ISecurableObject securableObject, string propertyName)
    {
      CheckPropertyWriteAccess(securableObject, propertyName, _principalProvider.GetPrincipal());
    }

    public void CheckPropertyWriteAccess (ISecurableObject securableObject, string propertyName, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNullOrEmpty("propertyName", propertyName);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasPropertyWriteAccess(securableObject, propertyName, principal))
      {
        ArgumentUtility.CheckNotNull("securableObject", securableObject);
        ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);

        throw CreatePermissionDeniedException(
            "Access to set-accessor of property '{0}' on type '{1}' has been denied.",
            propertyName,
            securableObject.GetSecurableType().GetFullNameSafe());
      }
    }

    public void CheckPropertyWriteAccess (ISecurableObject securableObject, MethodInfo methodInfo)
    {
      CheckPropertyWriteAccess(securableObject, methodInfo, _principalProvider.GetPrincipal());
    }

    public void CheckPropertyWriteAccess (ISecurableObject securableObject, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull("methodInfo", methodInfo);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasPropertyWriteAccess(securableObject, methodInfo, principal))
      {
        ArgumentUtility.CheckNotNull("securableObject", securableObject);
        ArgumentUtility.CheckNotNull("methodInfo", methodInfo);

        throw CreatePermissionDeniedException(
            "Access to set-accessor of property '{0}' on type '{1}' has been denied.",
            methodInfo.Name,
            securableObject.GetSecurableType().GetFullNameSafe());
      }
    }

    public void CheckPropertyWriteAccess (ISecurableObject securableObject, IMethodInformation methodInformation)
    {
      CheckPropertyWriteAccess(securableObject, methodInformation, _principalProvider.GetPrincipal());
    }

    public void CheckPropertyWriteAccess (ISecurableObject securableObject, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasPropertyWriteAccess(securableObject, methodInformation, principal))
      {
        ArgumentUtility.CheckNotNull("securableObject", securableObject);
        ArgumentUtility.CheckNotNull("methodInformation", methodInformation);

        throw CreatePermissionDeniedException(
            "Access to set-accessor of property '{0}' on type '{1}' has been denied.",
            methodInformation.Name,
            securableObject.GetSecurableType().GetFullNameSafe());
      }
    }


    public bool HasConstructorAccess (Type securableClass)
    {
      return HasConstructorAccess(securableClass, _principalProvider.GetPrincipal());
    }

    public virtual bool HasConstructorAccess (Type securableClass, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableClass", securableClass);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      return HasStatelessAccess(securableClass, principal, s_createAccessTypeAsList);
    }

    public void CheckConstructorAccess (Type securableClass)
    {
      CheckConstructorAccess(securableClass, _principalProvider.GetPrincipal());
    }

    public void CheckConstructorAccess (Type securableClass, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableClass", securableClass);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasConstructorAccess(securableClass, principal))
      {
        ArgumentUtility.CheckNotNull("securableClass", securableClass);

        throw CreatePermissionDeniedException("Access to constructor of type '{0}' has been denied.", securableClass.GetFullNameSafe());
      }
    }


    public bool HasStaticMethodAccess (Type securableClass, string methodName)
    {
      return HasStaticMethodAccess(securableClass, methodName, _principalProvider.GetPrincipal());
    }

    public bool HasStaticMethodAccess (Type securableClass, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty("methodName", methodName);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var methodInformation = _memberResolver.GetMethodInformation(securableClass, methodName, MemberAffiliation.Static);
      return HasStaticMethodAccess(securableClass, methodInformation, principal);
    }

    public bool HasStaticMethodAccess (Type securableClass, MethodInfo methodInfo)
    {
      return HasStaticMethodAccess(securableClass, methodInfo, _principalProvider.GetPrincipal());
    }

    public bool HasStaticMethodAccess (Type securableClass, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableClass", securableClass);
      ArgumentUtility.CheckNotNull("methodInfo", methodInfo);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var methodInformation = _memberResolver.GetMethodInformation(securableClass, methodInfo, MemberAffiliation.Static);
      return HasStaticMethodAccess(securableClass, methodInformation, principal);
    }

    public bool HasStaticMethodAccess (Type securableClass, IMethodInformation methodInformation)
    {
      return HasStaticMethodAccess(securableClass, methodInformation, _principalProvider.GetPrincipal());
    }

    public virtual bool HasStaticMethodAccess (Type securableClass, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableClass", securableClass);
      ArgumentUtility.CheckNotNull("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions(securableClass, methodInformation);
      Assertion.DebugIsNotNull(requiredAccessTypeEnums, "IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.");

      return HasStatelessAccess(securableClass, methodInformation, requiredAccessTypeEnums, principal);
    }

    public void CheckStaticMethodAccess (Type securableClass, string methodName)
    {
      CheckStaticMethodAccess(securableClass, methodName, _principalProvider.GetPrincipal());
    }

    public void CheckStaticMethodAccess (Type securableClass, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableClass", securableClass);
      ArgumentUtility.DebugCheckNotNullOrEmpty("methodName", methodName);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasStaticMethodAccess(securableClass, methodName, principal))
      {
        ArgumentUtility.CheckNotNull("securableClass", securableClass);
        ArgumentUtility.CheckNotNullOrEmpty("methodName", methodName);

        throw CreatePermissionDeniedException("Access to static method '{0}' on type '{1}' has been denied.", methodName, securableClass.GetFullNameSafe());
      }
    }

    public void CheckStaticMethodAccess (Type securableClass, MethodInfo methodInfo)
    {
      CheckStaticMethodAccess(securableClass, methodInfo, _principalProvider.GetPrincipal());
    }

    public void CheckStaticMethodAccess (Type securableClass, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableClass", securableClass);
      ArgumentUtility.DebugCheckNotNull("methodInfo", methodInfo);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasStaticMethodAccess(securableClass, methodInfo, principal))
      {
        ArgumentUtility.CheckNotNull("securableClass", securableClass);
        ArgumentUtility.CheckNotNull("methodInfo", methodInfo);

        throw CreatePermissionDeniedException(
            "Access to static method '{0}' on type '{1}' has been denied.",
            methodInfo.Name,
            securableClass.GetFullNameSafe());
      }
    }

    public void CheckStaticMethodAccess (Type securableClass, IMethodInformation methodInformation)
    {
      CheckStaticMethodAccess(securableClass, methodInformation, _principalProvider.GetPrincipal());
    }

    public void CheckStaticMethodAccess (Type securableClass, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull("securableClass", securableClass);
      ArgumentUtility.DebugCheckNotNull("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      if (!HasStaticMethodAccess(securableClass, methodInformation, principal))
      {
        ArgumentUtility.CheckNotNull("securableClass", securableClass);
        ArgumentUtility.CheckNotNull("methodInformation", methodInformation);

        throw CreatePermissionDeniedException(
            "Access to static method '{0}' on type '{1}' has been denied.",
            methodInformation.Name,
            securableClass.GetFullNameSafe());
      }
    }


    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasStatelessMethodAccess (Type securableClass, string methodName)
    {
      return HasStatelessMethodAccess(securableClass, methodName, _principalProvider.GetPrincipal());
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasStatelessMethodAccess (Type securableClass, string methodName, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableClass", securableClass);
      ArgumentUtility.CheckNotNullOrEmpty("methodName", methodName);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var methodInformation = _memberResolver.GetMethodInformation(securableClass, methodName, MemberAffiliation.Instance);
      return HasStatelessMethodAccess(securableClass, methodInformation, principal);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasStatelessMethodAccess (Type securableClass, MethodInfo methodInfo)
    {
      return HasStatelessMethodAccess(securableClass, methodInfo, _principalProvider.GetPrincipal());
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasStatelessMethodAccess (Type securableClass, MethodInfo methodInfo, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableClass", securableClass);
      ArgumentUtility.CheckNotNull("methodInfo", methodInfo);
      ArgumentUtility.CheckNotNull("principal", principal);

      var methodInformation = _memberResolver.GetMethodInformation(securableClass, methodInfo, MemberAffiliation.Instance);
      return HasStatelessMethodAccess(securableClass, methodInformation, principal);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasStatelessMethodAccess (Type securableClass, IMethodInformation methodInformation)
    {
      return HasStatelessMethodAccess(securableClass, methodInformation, _principalProvider.GetPrincipal());
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool HasStatelessMethodAccess (Type securableClass, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.CheckNotNull("securableClass", securableClass);
      ArgumentUtility.CheckNotNull("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull("principal", principal);

      var requiredAccessTypeEnums = _permissionProvider.GetRequiredMethodPermissions(securableClass, methodInformation);
      Assertion.DebugIsNotNull(requiredAccessTypeEnums, "IPermissionProvider.GetRequiredMethodPermissions evaluated and returned null.");

      return HasStatelessAccess(securableClass, methodInformation, requiredAccessTypeEnums, principal);
    }


    private bool HasAccess (
        ISecurableObject securableObject,
        IMemberInformation memberInformation,
        IReadOnlyList<Enum> requiredAccessTypeEnums,
        ISecurityPrincipal principal)
    {
      if (requiredAccessTypeEnums.Count == 0)
        throw new ArgumentException(string.Format("The member '{0}' does not define required permissions.", memberInformation.Name), "requiredAccessTypeEnums");

      return HasAccess(securableObject, principal, ConvertRequiredAccessTypeEnums(requiredAccessTypeEnums));
    }

    private bool HasStatelessAccess (
        Type securableClass,
        IMemberInformation memberInformation,
        IReadOnlyList<Enum> requiredAccessTypeEnums,
        ISecurityPrincipal principal)
    {
      if (requiredAccessTypeEnums.Count == 0)
        throw new ArgumentException(string.Format("The member '{0}' does not define required permissions.", memberInformation.Name), "requiredAccessTypeEnums");

      return HasStatelessAccess(securableClass, principal, ConvertRequiredAccessTypeEnums(requiredAccessTypeEnums));
    }

    private IReadOnlyList<AccessType> ConvertRequiredAccessTypeEnums (IReadOnlyList<Enum> requiredAccessTypeEnums)
    {
      if (requiredAccessTypeEnums.Count == 1)
      {
        var requiredAccessTypeEnum = requiredAccessTypeEnums[0];
        if (s_readAccessTypeAsEnum.Equals(requiredAccessTypeEnum))
          return s_readAccessTypeAsList;
        if (s_editAccessTypeAsEnum.Equals(requiredAccessTypeEnum))
          return s_editAccessTypeAsList;
        return ImmutableSingleton.Create(AccessType.Get(requiredAccessTypeEnum));
      }

      var requiredAccessTypes = new AccessType[requiredAccessTypeEnums.Count];
      for (int i = 0; i < requiredAccessTypeEnums.Count; i++)
        requiredAccessTypes[i] = ConvertEnumToAccessType(requiredAccessTypeEnums[i]);

      return requiredAccessTypes;
    }

    private static AccessType ConvertEnumToAccessType (Enum accessTypeEnum)
    {
      if (s_readAccessTypeAsEnum.Equals(accessTypeEnum))
        return s_readAccessType;
      if (s_editAccessTypeAsEnum.Equals(accessTypeEnum))
        return s_editAccessType;
      return AccessType.Get(accessTypeEnum);
    }

    private PermissionDeniedException CreatePermissionDeniedException (string message, params object[] args)
    {
      return new PermissionDeniedException(string.Format(message, args));
    }


    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
