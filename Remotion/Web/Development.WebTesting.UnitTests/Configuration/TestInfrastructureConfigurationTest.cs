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
using System.IO;
using Coypu;
using Moq;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting.UnitTests.Configuration
{
  [TestFixture]
  public class TestInfrastructureConfigurationTest
  {
    private readonly string _testRequestErrorDetectionStrategyAssemblyQualifiedName = typeof(TestRequestErrorDetectionStrategy).AssemblyQualifiedName;

    [Test]
    public void CreateFromWebTestConfigurationSection ()
    {
      var webTestSettingsStub = new Mock<IWebTestSettings>();
      webTestSettingsStub.Setup(_ => _.WebApplicationRoot).Returns("http://some.url:1337/");
      webTestSettingsStub.Setup(_ => _.ScreenshotDirectory).Returns(@".\SomeScreenshotDirectory");
      webTestSettingsStub.Setup(_ => _.CloseBrowserWindowsOnSetUpAndTearDown).Returns(false);
      webTestSettingsStub.Setup(_ => _.RequestErrorDetectionStrategyTypeName).Returns(_testRequestErrorDetectionStrategyAssemblyQualifiedName);

      var testInfrastructureConfiguration = new TestInfrastructureConfiguration(webTestSettingsStub.Object);

      Assert.That(testInfrastructureConfiguration.WebApplicationRoot, Is.EqualTo("http://some.url:1337/"));
      Assert.That(testInfrastructureConfiguration.ScreenshotDirectory, Is.EqualTo(Path.GetFullPath(@".\SomeScreenshotDirectory")));
      Assert.That(testInfrastructureConfiguration.CloseBrowserWindowsOnSetUpAndTearDown, Is.EqualTo(false));
      Assert.That(testInfrastructureConfiguration.RequestErrorDetectionStrategy, Is.InstanceOf<TestRequestErrorDetectionStrategy>());
    }

    private class TestRequestErrorDetectionStrategy : IRequestErrorDetectionStrategy
    {
      public void CheckPageForError (ElementScope scope) => throw new NotImplementedException();
    }
  }
}
