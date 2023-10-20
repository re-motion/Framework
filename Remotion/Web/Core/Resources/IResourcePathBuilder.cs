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
using Remotion.Web.Configuration;

namespace Remotion.Web.Resources
{
  /// <summary>
  /// Defines an API for building a path located within the application resource directory.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The default implementation (<see cref="ResourcePathBuilder"/>) builds paths in the following format:
  ///     &lt;resource root&gt;/&lt;Assembly-Name&gt;/part-1/.../part-n.
  ///   </para><para>
  ///     The <b>resource root</b> is loaded from the application configuration, <see cref="ResourceRoot"/>, and
  ///     defaults to <c>/&lt;AppDir&gt;/res</c>, e.g. <c>/WebApplication/res/Remotion.Web/Html/Utilities.js</c>.
  ///   </para><para>
  ///     The <b>resource root</b> is mapped to the environment variable <c>REMOTIONRESOURCES</c>,
  ///     or if the variable does not exist, <c>C:\Remotion.Resources</c>.
  ///   </para>
  /// </remarks>
  /// <seealso cref="ResourcePathBuilder"/>
  /// <seealso cref="T:Remotion.Development.Web.UnitTesting.Resources.FakeResourcePathBuilder"/>
 public interface IResourcePathBuilder
  {
    string BuildAbsolutePath (Assembly assembly, params string[] assemblyRelativePathParts);
  }
}
