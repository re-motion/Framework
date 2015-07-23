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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Identifies the <see cref="IDomainObjectHandle{T}"/> interface as a handle to the security infrastructure.
  /// </summary>
  [AttributeUsage (AttributeTargets.Interface, AllowMultiple = false)]
  public class DomainObjectHandleAttribute : Attribute, IHandleAttribute
  {
    public Type GetReferencedType (Type handleType)
    {
      ArgumentUtility.CheckNotNull ("handleType", handleType);
      if (!handleType.IsGenericType || handleType.GetGenericTypeDefinition() != typeof (IDomainObjectHandle<>))
        throw new ArgumentException ("The handleType parameter must be an instantiation of 'IDomainObjectHandle<T>'.", "handleType");

      return handleType.GetGenericArguments().Single();
    }

    public object GetReferencedInstance (object handleInstance)
    {
      var typedHandleInstance = ArgumentUtility.CheckNotNullAndType<IDomainObjectHandle<DomainObject>> ("handleInstance", handleInstance);

      return LifetimeService.GetObject (ClientTransactionScope.CurrentTransaction, typedHandleInstance.ObjectID, true);
    }
  }
}