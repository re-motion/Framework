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
using System.Web;
using NUnit.Framework;
using Remotion.Web.Infrastructure;
using Remotion.Web.Resources;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.Resources
{
  [TestFixture]
  public class ResourcePathBuilderTest
  {
    [Test]
    public void BuildAbsolutePath_MultiplePathParts_ResultingPathDoesNotEndWithTrailingSlash ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/appDir/file"), "/appDir");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "part1", "part2"),
          Is.EqualTo ("/appDir/resourceRoot/Remotion.Web.UnitTests/part1/part2"));
    }

    [Test]
    public void BuildAbsolutePath_MiddlePartBeginsIsDot_SkipsPart ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/appDir/file"), "/appDir");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, ".", "part2"),
          Is.EqualTo ("/appDir/resourceRoot/Remotion.Web.UnitTests/part2"));
    }
    
    [Test]
    public void BuildAbsolutePath_LastPathPartIsDot_SkipsPart ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/appDir/file"), "/appDir");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "part1", "."),
          Is.EqualTo ("/appDir/resourceRoot/Remotion.Web.UnitTests/part1"));
    }

    [Test]
    public void BuildAbsolutePath_EmptyPathParts_ResultingPathDoesNotEndWithTrailingSlash ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/appDir/file"), "/appDir");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, new string[0]),
          Is.EqualTo ("/appDir/resourceRoot/Remotion.Web.UnitTests"));
    }

    [Test]
    public void BuildAbsolutePath_UsesVirtualApplicationPathFromUrl ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/AppdiR/file"), "/appDir");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "part"),
          Is.EqualTo ("/AppdiR/resourceRoot/Remotion.Web.UnitTests/part"));
    }

    [Test]
    public void BuildAbsolutePath_RequestToRoot ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost"), "/");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "path"),
          Is.StringStarting ("/resourceRoot"));
    }

    [Test]
    public void BuildAbsolutePath_RequestToRootWithTrailingSlash ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/"), "/");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "path"),
          Is.StringStarting ("/resourceRoot"));
    }

    [Test]
    public void BuildAbsolutePath_RequestToRootWithApplicationPathNull_SubstitutesTrailingSlash ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost"), null);

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "path"),
          Is.StringStarting ("/resourceRoot"));
    }

    [Test]
    public void BuildAbsolutePath_RequestToVirtualApplicationPathRoot ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/appDir"), "/appDir");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "path"),
          Is.StringStarting ("/appDir/resourceRoot"));
    }

    [Test]
    public void BuildAbsolutePath_RequestToVirtualApplicationPathRootWithTrailingSlash ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/appDir/"), "/appDir");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "path"),
          Is.StringStarting ("/appDir/resourceRoot"));
    }

    [Test]
    public void BuildAbsolutePath_UsesVirtualApplicationPathFromUrl_ComparesUsingDecodedPath ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/_%20_%C3%84_%c3%a4_/file"), "/_ _Ä_ä_");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "part"),
          Is.StringStarting ("/_%20_%C3%84_%c3%a4_/resourceRoot"));
    }

    [Test]
    public void BuildAbsolutePath_UsesVirtualApplicationPathFromUrl_ComparesUsingDecodedPathWithTrailingSlash ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/_%20_%C3%84_%c3%a4_/file"), "/_ _Ä_ä_/");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "part"),
          Is.StringStarting ("/_%20_%C3%84_%c3%a4_/resourceRoot"));
    }

    [Test]
    public void BuildAbsolutePath_UsesVirtualApplicationPathFromRootUrl_ComparesUsingDecodedPath ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/_%20_%C3%84_%c3%a4_"), "/_ _Ä_ä_");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "part"),
          Is.StringStarting ("/_%20_%C3%84_%c3%a4_/resourceRoot"));
    }

    [Test]
    public void BuildAbsolutePath_UsesVirtualApplicationPathFromRootUrlWithTrailingSlash_ComparesUsingDecodedPath ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/_%20_%C3%84_%c3%a4_/"), "/_ _Ä_ä_");

      Assert.That (
          builder.BuildAbsolutePath (GetType().Assembly, "part"),
          Is.StringStarting ("/_%20_%C3%84_%c3%a4_/resourceRoot"));
    }

    [Test]
    public void BuildAbsolutePath_DecodedVirtualApplicationPathsDoNotMatch_ThrowsInvalidOperationException ()
    {
      var builder = CreateResourcePathBuilder (new Uri ("http://localhost/AppDir1/file"), "/AppDir");

      Assert.That (
          () =>
          builder.BuildAbsolutePath (GetType().Assembly, "part"),
          Throws.InvalidOperationException
                .With.Message.StartsWith ("Cannot calculate the application path when the request URL does not start with the application path."));
    }

    [Test]
    public void BuildAbsolutePath_MultipleCalls_DoesNotCacheHttpContext ()
    {
      var builder = (TestableResourcePathBuilder) CreateResourcePathBuilder (new Uri ("http://localhost/appDir/file"), "/appDir");

      builder.BuildAbsolutePath (GetType().Assembly, "part1");
      builder.HttpContextProvider.AssertWasCalled (_ => _.GetCurrentHttpContext(), options => options.Repeat.Once());

      builder.BuildAbsolutePath (GetType().Assembly, "part1");
      builder.HttpContextProvider.AssertWasCalled (_ => _.GetCurrentHttpContext(), options => options.Repeat.Twice());
    }

    private ResourcePathBuilder CreateResourcePathBuilder (Uri url, string applicationPath)
    {
      var httpRequestStub = MockRepository.GenerateStub<HttpRequestBase>();
      httpRequestStub.Stub (_ => _.Url).Return (url);
      httpRequestStub.Stub (_ => _.ApplicationPath).Return (applicationPath);

      var httpContextStub = MockRepository.GenerateStub<HttpContextBase>();
      httpContextStub.Stub (_ => _.Request).Return (httpRequestStub);

      var httpContextProviderStub = MockRepository.GenerateStub<IHttpContextProvider>();
      httpContextProviderStub.Stub (_ => _.GetCurrentHttpContext()).Return (httpContextStub);

      return new TestableResourcePathBuilder (httpContextProviderStub, "resourceRoot");
    }
  }
}