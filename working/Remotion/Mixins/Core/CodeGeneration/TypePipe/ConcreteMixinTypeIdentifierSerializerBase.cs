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
using Remotion.Collections;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// A common base class for <see cref="IConcreteMixinTypeIdentifierSerializer"/> implementations.
  /// </summary>
  public class ConcreteMixinTypeIdentifierSerializerBase : IConcreteMixinTypeIdentifierSerializer
  {
    private Type _mixinType;
    private ReadOnlyCollectionDecorator<MethodInfo> _overriders;
    private ReadOnlyCollectionDecorator<MethodInfo> _overridden;

    public Type MixinType
    {
      get { return _mixinType; }
    }

    public ReadOnlyCollectionDecorator<MethodInfo> Overriders
    {
      get { return _overriders; }
    }

    public ReadOnlyCollectionDecorator<MethodInfo> Overridden
    {
      get { return _overridden; }
    }

    public void AddMixinType (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      _mixinType = mixinType;
    }

    public void AddOverriders (HashSet<MethodInfo> overriders)
    {
      ArgumentUtility.CheckNotNull ("overriders", overriders);

      _overriders = overriders.AsReadOnly();
    }

    public void AddOverridden (HashSet<MethodInfo> overridden)
    {
      ArgumentUtility.CheckNotNull ("overridden", overridden);

      _overridden = overridden.AsReadOnly();
    }
  }
}