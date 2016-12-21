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
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="SecurityClient"/> according to the "Null Object Pattern".
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class NullSecurityClient : SecurityClient, INullObject
  {
    public NullSecurityClient ()
        : base (
            new NullSecurityProvider(),
            new PermissionReflector(),
            new NullPrincipalProvider(),
            new NullFunctionalSecurityStrategy(),
            new NullMemberResolver())
    {
    }

    public override bool HasAccess (ISecurableObject securableObject, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      ArgumentUtility.DebugCheckNotNull ("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull ("principal", principal);
      ArgumentUtility.DebugCheckNotNullOrEmpty ("requiredAccessTypes", requiredAccessTypes);

      return true;
    }

    public override bool HasStatelessAccess (Type securableClass, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      ArgumentUtility.DebugCheckNotNull ("securableClass", securableClass);
      ArgumentUtility.DebugCheckNotNull ("principal", principal);
      ArgumentUtility.DebugCheckNotNullOrEmpty ("requiredAccessTypes", requiredAccessTypes);

      return true;
    }

    public override bool HasMethodAccess (ISecurableObject securableObject, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull ("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull ("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasPropertyReadAccess (ISecurableObject securableObject, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull ("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull ("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasPropertyWriteAccess (ISecurableObject securableObject, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull ("securableObject", securableObject);
      ArgumentUtility.DebugCheckNotNull ("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasConstructorAccess (Type securableClass, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull ("securableClass", securableClass);
      ArgumentUtility.DebugCheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasStaticMethodAccess (Type securableClass, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull ("securableClass", securableClass);
      ArgumentUtility.DebugCheckNotNull ("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull ("principal", principal);

      return true;
    }

    public override bool HasStatelessMethodAccess (Type securableClass, IMethodInformation methodInformation, ISecurityPrincipal principal)
    {
      ArgumentUtility.DebugCheckNotNull ("securableClass", securableClass);
      ArgumentUtility.DebugCheckNotNull ("methodInformation", methodInformation);
      ArgumentUtility.DebugCheckNotNull ("principal", principal);

      return true;
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}
