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
using Remotion.Utilities;

namespace Remotion.Mixins.Context.Suppression
{
  /// <summary>
  /// Suppresses all mixins that can be ascribed to a given base class.
  /// </summary>
  public class MixinTreeSuppressionRule : IMixinSuppressionRule
  {
    public MixinTreeSuppressionRule (Type mixinBaseTypeToSuppress)
    {
      ArgumentUtility.CheckNotNull ("mixinBaseTypeToSuppress", mixinBaseTypeToSuppress);
      MixinBaseTypeToSuppress = mixinBaseTypeToSuppress;
    }

    public Type MixinBaseTypeToSuppress { get; private set; }

    public void RemoveAffectedMixins (Dictionary<Type, MixinContext> configuredMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("configuredMixinTypes", configuredMixinTypes);

      foreach (var configuredMixinType in configuredMixinTypes.Keys.ToList()) // need to clone collection, otherwise we can't remove
      {
        if (Reflection.TypeExtensions.CanAscribeTo (configuredMixinType, MixinBaseTypeToSuppress))
          configuredMixinTypes.Remove (configuredMixinType);
      }
    }
  }
}
