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
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class HasComposedInterfaceMarkerAnalyzerTest
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;

    private HasComposedInterfaceMarkerAnalyzer _analyzer;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder>((MixinConfiguration) null);

      _analyzer = new HasComposedInterfaceMarkerAnalyzer();
    }

    [Test]
    public void Analyze_IncludesClasses_ImplementingIHasComposedInterface ()
    {
      var classBuilderMock = MockRepository.GenerateStrictMock<ClassContextBuilder> (typeof (int));

      _configurationBuilderMock.Expect (mock => mock.ForClass (typeof (ClassWithHasComposedInterfaces))).Return (classBuilderMock);
      _configurationBuilderMock.Replay ();

      classBuilderMock
          .Expect (mock => mock.AddComposedInterfaces (
              typeof (ClassWithHasComposedInterfaces.IComposedInterface1), 
              typeof (ClassWithHasComposedInterfaces.IComposedInterface2)))
          .Return (null);
      classBuilderMock.Replay ();

      _analyzer.Analyze (typeof (ClassWithHasComposedInterfaces), _configurationBuilderMock);

      _configurationBuilderMock.VerifyAllExpectations ();
      classBuilderMock.VerifyAllExpectations ();
    }

    [Test]
    public void Analyze_IgnoresClasses_ImplementingIHasComposedInterfaceWithGenericParameters ()
    {
      _configurationBuilderMock.Replay ();

      _analyzer.Analyze (typeof (BaseClassWithHasComposedInterface<>), _configurationBuilderMock);

      _configurationBuilderMock.AssertWasNotCalled (mock => mock.ForClass (Arg<Type>.Is.Anything));
    }

    [Test]
    public void Analyze_IgnoresClasses_NotImplementingIHasComposedInterface ()
    {
      _configurationBuilderMock.Replay ();

      _analyzer.Analyze (typeof (object), _configurationBuilderMock);

      _configurationBuilderMock.VerifyAllExpectations ();
    }
  }
}
