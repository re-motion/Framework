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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.Resources
{
  /// <summary>
  /// Responsible for creating objects that implement <see cref="IResourceUrl"/>.
  /// </summary>
  /// <seealso cref="ResourceUrl"/>
  /// <seealso cref="ThemedResourceUrl"/>
  /// <seealso cref="T:Remotion.Development.Web.UnitTesting.Resources.FakeResourceUrlFactory"/>
  [ImplementationFor (typeof (IResourceUrlFactory), Lifetime = LifetimeKind.Singleton)]
  public class ResourceUrlFactory : IResourceUrlFactory
  {
    private readonly IResourcePathBuilder _builder;
    private readonly ResourceTheme _resourceTheme;

    public ResourceUrlFactory (IResourcePathBuilder builder, ResourceTheme resourceTheme)
    {
      ArgumentUtility.CheckNotNull ("builder", builder);
      ArgumentUtility.CheckNotNull ("resourceTheme", resourceTheme);
      _builder = builder;
      _resourceTheme = resourceTheme;
    }

    public IResourceUrl CreateResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
    {
      ArgumentUtility.CheckNotNull ("definingType", definingType);
      ArgumentUtility.CheckNotNull ("resourceType", resourceType);
      ArgumentUtility.CheckNotNullOrEmpty ("relativeUrl", relativeUrl);

      return new ResourceUrl (_builder, definingType, resourceType, relativeUrl);
    }

    public IResourceUrl CreateThemedResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
    {
      ArgumentUtility.CheckNotNull ("definingType", definingType);
      ArgumentUtility.CheckNotNull ("resourceType", resourceType);
      ArgumentUtility.CheckNotNullOrEmpty ("relativeUrl", relativeUrl);

      return new ThemedResourceUrl (_builder, definingType, resourceType, _resourceTheme, relativeUrl);
    }
  }
}