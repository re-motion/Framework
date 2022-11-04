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
using Moq;
using NUnit.Framework;
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class DeclarativeConfigurationAnalyzerTest
  {

    private Mock<IMixinDeclarationAnalyzer<Type>> _typeAnalyzerMock;
    private Mock<IMixinDeclarationAnalyzer<Assembly>> _assemblyAnalyzerMock;

    private Mock<MixinConfigurationBuilder> _fakeConfigurationBuilder;

    [SetUp]
    public void SetUp ()
    {
      _typeAnalyzerMock = new Mock<IMixinDeclarationAnalyzer<Type>>(MockBehavior.Strict);
      _assemblyAnalyzerMock = new Mock<IMixinDeclarationAnalyzer<Assembly>>(MockBehavior.Strict);

      _fakeConfigurationBuilder = new Mock<MixinConfigurationBuilder>((MixinConfiguration)null);
    }

    [Test]
    public void Analyze ()
    {
      var types = new[] { typeof(object), typeof(string), typeof(DeclarativeConfigurationAnalyzerTest) };

      var sequence = new VerifiableSequence();
      _typeAnalyzerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Analyze(typeof(object), _fakeConfigurationBuilder.Object))
          .Verifiable();
      _assemblyAnalyzerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Analyze(typeof(object).Assembly, _fakeConfigurationBuilder.Object))
          .Verifiable();
      _typeAnalyzerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Analyze(typeof(string), _fakeConfigurationBuilder.Object))
          .Verifiable();
      _typeAnalyzerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Analyze(typeof(DeclarativeConfigurationAnalyzerTest), _fakeConfigurationBuilder.Object))
          .Verifiable();
      _assemblyAnalyzerMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Analyze(typeof(DeclarativeConfigurationAnalyzerTest).Assembly, _fakeConfigurationBuilder.Object))
          .Verifiable();

      var analyzer = new DeclarativeConfigurationAnalyzer(new[] { _typeAnalyzerMock.Object }, new[] {_assemblyAnalyzerMock.Object });
      analyzer.Analyze(types, _fakeConfigurationBuilder.Object);

      _typeAnalyzerMock.Verify();
      _assemblyAnalyzerMock.Verify();
      sequence.Verify();
    }
  }
}
