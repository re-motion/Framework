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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// A common base class for <see cref="IClassContextSerializer"/> implementations.
  /// </summary>
  public class ClassContextSerializerBase : IClassContextSerializer
  {
    private Type _type;
    private ReadOnlyCollection<MixinContext> _mixinContexts;
    private ReadOnlyCollection<Type> _composedInterfaces;

    public Type Type
    {
      get { return _type; }
    }

    public ReadOnlyCollection<MixinContext> MixinContexts
    {
      get { return _mixinContexts; }
    }

    public ReadOnlyCollection<Type> ComposedInterfaces
    {
      get { return _composedInterfaces; }
    }

    public void AddClassType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _type = type;
    }

    public void AddMixins (IEnumerable<MixinContext> mixinContexts)
    {
      ArgumentUtility.CheckNotNull ("mixinContexts", mixinContexts);

      _mixinContexts = mixinContexts.ToList().AsReadOnly();
    }

    public void AddComposedInterfaces (IEnumerable<Type> composedInterfaces)
    {
      ArgumentUtility.CheckNotNull ("composedInterfaces", composedInterfaces);

      _composedInterfaces = composedInterfaces.ToList().AsReadOnly();
    }
  }
}