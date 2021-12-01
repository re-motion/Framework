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
using Moq;
using NUnit.Framework;
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class ComposedInterfaceAttributeTest
  {
    private Mock<MixinConfigurationBuilder> _configurationBuilderMock;

    [SetUp]
    public void SetUp ()
    {
      _configurationBuilderMock = new Mock<MixinConfigurationBuilder>(MockBehavior.Strict, (MixinConfiguration)null);
    }

    [Test]
    public void IgnoresDuplicates ()
    {
      var attribute = new ComposedInterfaceAttribute(typeof(string));
      Assert.That(attribute.IgnoresDuplicates, Is.False);
    }

    [Test]
    public void Apply ()
    {
      var attribute = new ComposedInterfaceAttribute(typeof(string));
      var classBuilderMock = new Mock<ClassContextBuilder>(MockBehavior.Strict, _configurationBuilderMock.Object, typeof(string));

      _configurationBuilderMock.Setup(mock => mock.ForClass(typeof(string))).Returns(classBuilderMock.Object).Verifiable();
      classBuilderMock.Setup(mock => mock.AddComposedInterface(typeof(IServiceProvider))).Returns(classBuilderMock.Object).Verifiable();

      attribute.Apply(_configurationBuilderMock.Object, typeof(IServiceProvider));
      _configurationBuilderMock.Verify();
      classBuilderMock.Verify();
    }
  }
}
