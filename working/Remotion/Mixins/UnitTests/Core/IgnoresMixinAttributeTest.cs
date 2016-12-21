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
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class IgnoresMixinAttributeTest
  {
    private static readonly Type s_targetClassType = typeof (string);
    private static readonly Type s_mixinType = typeof (int);

    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private ClassContextBuilder _classBuilderMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder> ((MixinConfiguration) null);
      _classBuilderMock = _mockRepository.StrictMock<ClassContextBuilder> (_configurationBuilderMock, s_targetClassType);
    }

    [Test]
    public void IgnoresDuplicates ()
    {
      var attribute = new IgnoresMixinAttribute (typeof (string));
      Assert.That (attribute.IgnoresDuplicates, Is.False);
    }
    
    [Test]
    public void AnalyzeIgnoresMixinAttribute ()
    {
      IgnoresMixinAttribute attribute = new IgnoresMixinAttribute (s_mixinType);

      _configurationBuilderMock.Expect (mock => mock.ForClass (s_targetClassType)).Return (_classBuilderMock);
      _classBuilderMock.Expect (mock => mock.SuppressMixin (s_mixinType)).Return (_classBuilderMock);

      _mockRepository.ReplayAll ();
      attribute.Apply (_configurationBuilderMock, s_targetClassType);
      _mockRepository.VerifyAll ();
    }
  }
}
