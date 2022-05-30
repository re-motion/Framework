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
  public class IgnoresMixinAttributeTest
  {
    private static readonly Type s_targetClassType = typeof(string);
    private static readonly Type s_mixinType = typeof(int);

    private Mock<MixinConfigurationBuilder> _configurationBuilderMock;
    private Mock<ClassContextBuilder> _classBuilderMock;

    [SetUp]
    public void SetUp ()
    {
      _configurationBuilderMock = new Mock<MixinConfigurationBuilder>(MockBehavior.Strict, (MixinConfiguration)null);
      _classBuilderMock = new Mock<ClassContextBuilder>(MockBehavior.Strict, _configurationBuilderMock.Object, s_targetClassType);
    }

    [Test]
    public void IgnoresDuplicates ()
    {
      var attribute = new IgnoresMixinAttribute(typeof(string));
      Assert.That(attribute.IgnoresDuplicates, Is.False);
    }

    [Test]
    public void AnalyzeIgnoresMixinAttribute ()
    {
      IgnoresMixinAttribute attribute = new IgnoresMixinAttribute(s_mixinType);

      _configurationBuilderMock.Setup(mock => mock.ForClass(s_targetClassType)).Returns(_classBuilderMock.Object).Verifiable();
      _classBuilderMock.Setup(mock => mock.SuppressMixin(s_mixinType)).Returns(_classBuilderMock.Object).Verifiable();

      attribute.Apply(_configurationBuilderMock.Object, s_targetClassType);
      _configurationBuilderMock.Verify();
      _classBuilderMock.Verify();
    }
  }
}
