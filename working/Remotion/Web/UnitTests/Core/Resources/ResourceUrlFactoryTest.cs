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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Web.Resources;

namespace Remotion.Web.UnitTests.Core.Resources
{
  [TestFixture]
  public class ResourceUrlFactoryTest
  {
    private ResourceTheme _resourceTheme;
    private IResourceUrlFactory _factory;
    private FakeResourcePathBuilder _resourcePathBuilder;

    [SetUp]
    public void SetUp ()
    {
      _resourcePathBuilder = new FakeResourcePathBuilder();
      _resourceTheme = new ResourceTheme ("TestTheme");
      _factory = new ResourceUrlFactory (_resourcePathBuilder, _resourceTheme);
    }

    [Test]
    public void CreateResourceUrl ()
    {
      var resourceUrl = _factory.CreateResourceUrl (typeof (ResourceUrlFactoryTest), ResourceType.Image, "theRelativeUrl.img");

      Assert.That (resourceUrl, Is.InstanceOf (typeof (ResourceUrl)));
      Assert.That (((ResourceUrl) resourceUrl).ResourcePathBuilder, Is.SameAs (_resourcePathBuilder));
      Assert.That (((ResourceUrl) resourceUrl).DefiningType, Is.EqualTo (typeof (ResourceUrlFactoryTest)));
      Assert.That (((ResourceUrl) resourceUrl).ResourceType, Is.EqualTo (ResourceType.Image));
      Assert.That (((ResourceUrl) resourceUrl).RelativeUrl, Is.EqualTo ("theRelativeUrl.img"));
    }

    [Test]
    public void CreateThemedResourceUrl ()
    {
      var resourceUrl = _factory.CreateThemedResourceUrl (typeof (ResourceUrlFactoryTest), ResourceType.Image, "theRelativeUrl.img");

      Assert.That (resourceUrl, Is.InstanceOf (typeof (ThemedResourceUrl)));
      Assert.That (((ThemedResourceUrl) resourceUrl).ResourcePathBuilder, Is.SameAs (_resourcePathBuilder));
      Assert.That (((ThemedResourceUrl) resourceUrl).DefiningType, Is.EqualTo (typeof (ResourceUrlFactoryTest)));
      Assert.That (((ThemedResourceUrl) resourceUrl).ResourceType, Is.EqualTo (ResourceType.Image));
      Assert.That (((ThemedResourceUrl) resourceUrl).ResourceTheme, Is.SameAs (_resourceTheme));
      Assert.That (((ThemedResourceUrl) resourceUrl).RelativeUrl, Is.EqualTo ("theRelativeUrl.img"));
    }
  }
}