﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Web;
using Remotion.Web.Resources;

namespace Remotion.Development.Web.UnitTesting.Resources
{
  /// <summary>
  /// Fake implementation of the <see cref="IResourceUrlFactory"/> interface, intended for use in unit testing.
  /// </summary>
  public class FakeResourceUrlFactory : IResourceUrlFactory
  {
    private readonly IResourcePathBuilder _builder = new FakeResourcePathBuilder();
    private readonly ResourceTheme _resourceTheme=  new ResourceTheme ("Fake");

    public FakeResourceUrlFactory ()
    {
    }

    public IResourceUrl CreateResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
    {
      return new ResourceUrl (_builder, definingType, resourceType, relativeUrl);
    }

    public IResourceUrl CreateThemedResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
    {
      return new ThemedResourceUrl (_builder, definingType, resourceType, _resourceTheme, relativeUrl);
    }
  }
}