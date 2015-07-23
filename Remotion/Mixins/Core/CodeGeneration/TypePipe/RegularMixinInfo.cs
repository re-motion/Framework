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
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Encapsulates meta data about a regular mixin type, that is, a mixin type that is used as-is wihout the need to generate a derived mixin type.
  /// </summary>
  public class RegularMixinInfo : IMixinInfo
  {
    private readonly Type _mixinType;

    public RegularMixinInfo (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      _mixinType = mixinType;
    }

    public Type MixinType
    {
      get { return _mixinType; }
    }

    public IEnumerable<Type> GetInterfacesToImplement ()
    {
      return Type.EmptyTypes;
    }

    public MethodInfo GetPubliclyCallableMixinMethod (MethodInfo methodToBeCalled)
    {
      if (!methodToBeCalled.IsPublic)
        throw new NotSupportedException ("If a non-public method is to be called, a derived mixin type must have been created for it.");

      return methodToBeCalled;
    }

    public MethodInfo GetOverrideInterfaceMethod (MethodInfo mixinMethod)
    {
      throw new NotSupportedException ("If a mixin method is overridden, a derived mixin type must have been created for it.");
    }
  }
}