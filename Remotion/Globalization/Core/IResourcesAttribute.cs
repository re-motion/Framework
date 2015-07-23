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
using JetBrains.Annotations;

namespace Remotion.Globalization
{
  public interface IResourcesAttribute
  {
    /// <summary>
    ///   Gets the base name of the resource container as specified by the attributes construction.
    /// </summary>
    /// <remarks>
    /// The base name of the resource conantainer to be used by this type
    /// (&lt;assembly&gt;.&lt;path inside project&gt;.&lt;resource file name without extension&gt;).
    /// </remarks>
    [NotNull]
    string BaseName { get; }

    [CanBeNull]
    Assembly ResourceAssembly { get; }
  }
}
