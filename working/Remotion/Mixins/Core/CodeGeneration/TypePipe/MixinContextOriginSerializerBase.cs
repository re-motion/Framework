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
using Remotion.Mixins.Context.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// A common base class for <see cref="IMixinContextOriginSerializer"/> implementations.
  /// </summary>
  public class MixinContextOriginSerializerBase : IMixinContextOriginSerializer
  {
    private string _kind;
    private Assembly _assembly;
    private string _locaction;

    public string Kind
    {
      get { return _kind; }
    }

    public Assembly Assembly
    {
      get { return _assembly; }
    }

    public string Locaction
    {
      get { return _locaction; }
    }

    public void AddKind (string kind)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("kind", kind);

      _kind = kind;
    }

    public void AddAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      _assembly = assembly;
    }

    public void AddLocation (string location)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("location", location);

      _locaction = location;
    }
  }
}