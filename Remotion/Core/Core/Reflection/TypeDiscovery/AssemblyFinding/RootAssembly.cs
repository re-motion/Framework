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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Identifies a root assembly found by an implementation of <see cref="IRootAssemblyFinder"/>. This includes the assembly itself as well as a flag
  /// indicating whether to follow the assembly's references or not.
  /// </summary>
  public struct RootAssembly
  {
    public RootAssembly (Assembly assembly, bool followReferences)
        : this()
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);

      Assembly = assembly;
      FollowReferences = followReferences;
    }

    public Assembly Assembly { get; private set; }
    public bool FollowReferences { get; private set; }

    public override bool Equals (object? obj)
    {
      return obj is RootAssembly && Assembly.Equals(((RootAssembly)obj).Assembly);
    }

    public override int GetHashCode ()
    {
      return Assembly.GetHashCode();
    }

    public override string ToString ()
    {
      return Assembly.GetFullNameSafe() + (FollowReferences ? ", including references" : "");
    }
  }
}
