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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class DeclarativeConfigurationAnalyzerTest
  {
    private MockRepository _mockRepository;

    private IMixinDeclarationAnalyzer<Type> _typeAnalyzerMock;
    private IMixinDeclarationAnalyzer<Assembly> _assemblyAnalyzerMock;

    private MixinConfigurationBuilder _fakeConfigurationBuilder;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _typeAnalyzerMock = _mockRepository.StrictMock<IMixinDeclarationAnalyzer<Type>> ();
      _assemblyAnalyzerMock = _mockRepository.StrictMock<IMixinDeclarationAnalyzer<Assembly>> ();

      _fakeConfigurationBuilder = _mockRepository.Stub<MixinConfigurationBuilder> ((MixinConfiguration) null);
    }

    [Test]
    public void Analyze ()
    {
      var types = new[] { typeof (object), typeof (string), typeof (DeclarativeConfigurationAnalyzerTest) };

      using (_mockRepository.Ordered ())
      {
        _typeAnalyzerMock.Expect (mock => mock.Analyze (typeof (object), _fakeConfigurationBuilder));
        _assemblyAnalyzerMock.Expect (mock => mock.Analyze (typeof (object).Assembly, _fakeConfigurationBuilder));
        _typeAnalyzerMock.Expect (mock => mock.Analyze (typeof (string), _fakeConfigurationBuilder));
        _typeAnalyzerMock.Expect (mock => mock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest), _fakeConfigurationBuilder));
        _assemblyAnalyzerMock.Expect (mock => mock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest).Assembly, _fakeConfigurationBuilder));
      }

      _mockRepository.ReplayAll();

      var analyzer = new DeclarativeConfigurationAnalyzer (new[] { _typeAnalyzerMock }, new[] {_assemblyAnalyzerMock });
      analyzer.Analyze (types, _fakeConfigurationBuilder);

      _mockRepository.VerifyAll();
    }
  }
}
