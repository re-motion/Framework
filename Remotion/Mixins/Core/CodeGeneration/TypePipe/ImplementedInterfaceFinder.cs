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
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Finds the interfaces to be implemented by the generated type.
  /// </summary>
  public class ImplementedInterfaceFinder
  {
    private readonly IEnumerable<IMixinInfo> _mixinInfos;
    private readonly IEnumerable<InterfaceIntroductionDefinition> _receivedInterfaces;
    private readonly IEnumerable<Type> _alreadyImplementedInterfaces;
    private readonly IEnumerable<RequiredTargetCallTypeDefinition> _requiredTargetCallTypes;

    public ImplementedInterfaceFinder (
        IEnumerable<Type> alreadyImplementedInterfaces, 
        IEnumerable<InterfaceIntroductionDefinition> receivedInterfaces, 
        IEnumerable<RequiredTargetCallTypeDefinition> requiredTargetCallTypes, 
        IEnumerable<IMixinInfo> mixinInfos)
    {
      ArgumentUtility.CheckNotNull ("alreadyImplementedInterfaces", alreadyImplementedInterfaces);
      ArgumentUtility.CheckNotNull ("receivedInterfaces", receivedInterfaces);
      ArgumentUtility.CheckNotNull ("requiredTargetCallTypes", requiredTargetCallTypes);
      ArgumentUtility.CheckNotNull ("mixinInfos", mixinInfos);

      _alreadyImplementedInterfaces = alreadyImplementedInterfaces;
      _receivedInterfaces = receivedInterfaces;
      _requiredTargetCallTypes = requiredTargetCallTypes;
      _mixinInfos = mixinInfos;
    }

    public Type[] GetInterfacesToImplement ()
    {
      var interfaces = new HashSet<Type> ();
      interfaces.UnionWith (_requiredTargetCallTypes
                                .Select (faceTypeDefinition => faceTypeDefinition.Type)
                                .Where (t => t.IsInterface));
      interfaces.ExceptWith (_alreadyImplementedInterfaces); // remove required interfaces the type already implements

      interfaces.UnionWith (_receivedInterfaces.Select (introduction => introduction.InterfaceType));
      interfaces.UnionWith (_mixinInfos.SelectMany (mixin => mixin.GetInterfacesToImplement()));
      interfaces.Add (typeof (IMixinTarget));

      return interfaces.ToArray ();
    }
  }
}
