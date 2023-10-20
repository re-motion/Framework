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
using Remotion.ServiceLocation;

namespace Remotion.Web.Resources
{
  /// <summary> Default resource root. </summary>
  /// <seealso cref="ResourceUrlResolver"/>
  /// <remarks>
  ///  Configuring the resource root is done by adding a new implementation of
  ///  <see cref="ResourceRoot" />, overriding its <see cref="Value" /> property, and registering
  ///  it in the IoC container.
  /// </remarks>
  [ImplementationFor(typeof(ResourceRoot), Lifetime = LifetimeKind.Singleton)]
  public class ResourceRoot
  {
    /// <summary> Gets the default root folder for all resources. </summary>
    /// <value> A string specifying a path relative to the application root. Defaults to <c>res</c>. </value>
    /// <seealso cref="Remotion.Web.ResourceUrlResolver"/>
    public virtual string Value => "res";

    public ResourceRoot ()
    {
    }
  }
}
