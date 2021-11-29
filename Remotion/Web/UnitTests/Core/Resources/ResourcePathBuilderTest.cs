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
using System.Web;
using Moq;
using NUnit.Framework;
using Remotion.Web.Infrastructure;
using Remotion.Web.Resources;

namespace Remotion.Web.UnitTests.Core.Resources
{
  [TestFixture]
  public class ResourcePathBuilderTest
  {
    [Test]
    public void BuildAbsolutePath_MultiplePathParts_ResultingPathDoesNotEndWithTrailingSlash ()
    {
      var builder = CreateResourcePathBuilder(new Uri("http://localhost/appDir/file"), "/appDir");

      Assert.That(
          builder.BuildAbsolutePath(GetType().Assembly, "part1", "part2"),
          Is.EqualTo("/appDir/resourceRoot/Remotion.Web.UnitTests/part1/part2"));
    }

    [Test]
    public void BuildAbsolutePath_MiddlePartBeginsIsDot_SkipsPart ()
    {
      var builder = CreateResourcePathBuilder(new Uri("http://localhost/appDir/file"), "/appDir");

      Assert.That(
          builder.BuildAbsolutePath(GetType().Assembly, ".", "part2"),
          Is.EqualTo("/appDir/resourceRoot/Remotion.Web.UnitTests/part2"));
    }

    [Test]
    public void BuildAbsolutePath_LastPathPartIsDot_SkipsPart ()
    {
      var builder = CreateResourcePathBuilder(new Uri("http://localhost/appDir/file"), "/appDir");

      Assert.That(
          builder.BuildAbsolutePath(GetType().Assembly, "part1", "."),
          Is.EqualTo("/appDir/resourceRoot/Remotion.Web.UnitTests/part1"));
    }

    [Test]
    public void BuildAbsolutePath_EmptyPathParts_ResultingPathDoesNotEndWithTrailingSlash ()
    {
      var builder = CreateResourcePathBuilder(new Uri("http://localhost/appDir/file"), "/appDir");

      Assert.That(
          builder.BuildAbsolutePath(GetType().Assembly, new string[0]),
          Is.EqualTo("/appDir/resourceRoot/Remotion.Web.UnitTests"));
    }

    [Test]
    public void BuildAbsolutePath_UsesVirtualApplicationPathFromUrl ()
    {
      var builder = CreateResourcePathBuilder(new Uri("http://localhost/AppdiR/file"), "/appDir");

      Assert.That(
          builder.BuildAbsolutePath(GetType().Assembly, "part"),
          Is.EqualTo("/AppdiR/resourceRoot/Remotion.Web.UnitTests/part"));
    }

    [Test]
    public void BuildAbsolutePath_MultipleCalls_DoesNotCacheHttpContext ()
    {
      var builder = (TestableResourcePathBuilder) CreateResourcePathBuilder(new Uri("http://localhost/appDir/file"), "/appDir");

      builder.BuildAbsolutePath(GetType().Assembly, "part1");
      Mock.Get(builder.HttpContextProvider).Verify(_ => _.GetCurrentHttpContext());

      builder.BuildAbsolutePath(GetType().Assembly, "part1");
      Mock.Get(builder.HttpContextProvider).Verify(_ => _.GetCurrentHttpContext(), Times.Exactly(2));
    }

    private ResourcePathBuilder CreateResourcePathBuilder (Uri url, string applicationPath)
    {
      var httpRequestStub = new Mock<HttpRequestBase>();
      httpRequestStub.Setup(_ => _.Url).Returns(url);
      httpRequestStub.Setup(_ => _.ApplicationPath).Returns(applicationPath);

      var httpContextStub = new Mock<HttpContextBase>();
      httpContextStub.Setup(_ => _.Request).Returns(httpRequestStub.Object);

      var httpContextProviderStub = new Mock<IHttpContextProvider>();
      httpContextProviderStub.Setup(_ => _.GetCurrentHttpContext()).Returns(httpContextStub.Object);

      return new TestableResourcePathBuilder(httpContextProviderStub.Object, "resourceRoot");
    }
  }
}
