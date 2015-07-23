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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class MixinConfigurationAttributeAnalyzerTest
  {
    private string _fakeEntity;

    private MixinConfigurationBuilder _fakeConfigurationBuilder;

    [SetUp]
    public void SetUp ()
    {
      _fakeEntity = "test entity";

      _fakeConfigurationBuilder = MockRepository.GenerateStub<MixinConfigurationBuilder> (new object[] { null });
    }

    [Test]
    public void Analyze ()
    {
      var attributeMock1 = MockRepository.GenerateStrictMock<IMixinConfigurationAttribute<string>> ();
      var attributeMock2 = MockRepository.GenerateStrictMock<IMixinConfigurationAttribute<string>> ();

      Func<string, IEnumerable<IMixinConfigurationAttribute<string>>> fakeAttributeProvider = s =>
      {
        Assert.That (s, Is.EqualTo (_fakeEntity));
        return new[] { attributeMock1, attributeMock2 };
      };
      var analyzer = new MixinConfigurationAttributeAnalyzer<string> (fakeAttributeProvider);

      attributeMock1.Stub (mock => mock.IgnoresDuplicates).Return (false);
      attributeMock2.Stub (mock => mock.IgnoresDuplicates).Return (false);

      attributeMock1.Expect (mock => mock.Apply (_fakeConfigurationBuilder, _fakeEntity));
      attributeMock2.Expect (mock => mock.Apply (_fakeConfigurationBuilder, _fakeEntity));
      
      analyzer.Analyze (_fakeEntity, _fakeConfigurationBuilder);

      attributeMock1.VerifyAllExpectations ();
      attributeMock2.VerifyAllExpectations ();
    }
  }
}