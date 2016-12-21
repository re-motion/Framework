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
using Remotion.Web.Resources;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Resources
{
  [TestFixture]
  public class ResourceUrlTest
  {
    [Test]
    public void GetUrl_BuildsUrlUsingBuilder ()
    {
      var resourceUrlBuilderStub = MockRepository.GenerateStub<IResourcePathBuilder>();

      var resourceUrl = new ResourceUrl (resourceUrlBuilderStub, typeof (ResourceUrlTest), ResourceType.Html, "theRelativeUrl.js");
      resourceUrlBuilderStub
          .Stub (_ => _.BuildAbsolutePath (typeof (ResourceUrlTest).Assembly, new[] { "Html", "theRelativeUrl.js" }))
          .Return ("expectedUrl");

      Assert.That (resourceUrl.GetUrl(), Is.EqualTo ("expectedUrl"));
    }

    [Test]
    public void GetUrl_DoesNotCacheUrls ()
    {
      var resourceUrlBuilderStub = MockRepository.GenerateStub<IResourcePathBuilder>();

      var resourceUrl = new ResourceUrl (resourceUrlBuilderStub, typeof (ResourceUrlTest), ResourceType.Html, "theRelativeUrl.js");
      int count = 0;
      resourceUrlBuilderStub
          .Stub (_ => _.BuildAbsolutePath (typeof (ResourceUrlTest).Assembly, new[] { "Html", "theRelativeUrl.js" }))
          .Return (null)
          .WhenCalled (
              mi =>
              {
                mi.ReturnValue = "expectedUrl " + count;
                count++;
              });

      Assert.That (resourceUrl.GetUrl(), Is.EqualTo ("expectedUrl 0"));
      Assert.That (resourceUrl.GetUrl(), Is.EqualTo ("expectedUrl 1"));
    }
  }
}