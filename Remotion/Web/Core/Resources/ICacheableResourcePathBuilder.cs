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
namespace Remotion.Web.Resources
{
  /// <summary>
  /// Marker interface for IoC resolution of <see cref="IResourcePathBuilder"/> that are used for cacheable resources.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The default implementation (<see cref="CacheableResourcePathBuilder"/>) builds paths in the following format:
  ///     &lt;resource root&gt;/[cache_&lt;cache key&gt;/]&lt;Assembly-Name&gt;/part-1/.../part-n.
  ///   </para><para>
  ///     The <b>resource root</b> is loaded from the application configuration, <see cref="ResourceRoot"/>, and
  ///     defaults to <c>/&lt;AppDir&gt;/res</c>, e.g. <c>/WebApplication/res/Remotion.Web/Html/Utilities.js</c>.
  ///   </para>
  /// </remarks>
  public interface ICacheableResourcePathBuilder : IResourcePathBuilder;
}
