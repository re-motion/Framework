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

namespace Remotion.Mixins.Context
{
  public class ClassContextCombiner
  {
    private readonly List<ClassContext> _classContexts = new List<ClassContext>();

    public IEnumerable<ClassContext> CollectedContexts
    {
      get { return _classContexts; }
    }

    public void AddIfNotNull (ClassContext context)
    {
      if (context != null)
        _classContexts.Add (context);
    }

    public void AddRangeAllowingNulls (IEnumerable<ClassContext> contexts)
    {
      ArgumentUtility.CheckNotNull ("contexts", contexts);

      foreach (var context in contexts)
        AddIfNotNull (context);
    }
    
    public ClassContext GetCombinedContexts (Type contextType)
    {
      switch (_classContexts.Count)
      {
        case 0:
          return null;
        case 1:
          return _classContexts[0].CloneForSpecificType (contextType);
        default:
          return new ClassContext (contextType, Enumerable.Empty<MixinContext>(), Enumerable.Empty<Type>()).InheritFrom (_classContexts);
      }
    }
  }
}
