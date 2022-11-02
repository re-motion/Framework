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
using Remotion.Globalization.Implementation;
using Remotion.Globalization.UnitTests.TestDomain;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundEnumerationGlobalizationServiceTest
  {
    private CompoundEnumerationGlobalizationService _service;
    private Mock<IEnumerationGlobalizationService> _innerService1;
    private Mock<IEnumerationGlobalizationService> _innerService2;
    private Mock<IEnumerationGlobalizationService> _innerService3;

    [SetUp]
    public void SetUp ()
    {
      _innerService1 = new Mock<IEnumerationGlobalizationService>(MockBehavior.Strict);
      _innerService2 = new Mock<IEnumerationGlobalizationService>(MockBehavior.Strict);
      _innerService3 = new Mock<IEnumerationGlobalizationService>(MockBehavior.Strict);

      _service = new CompoundEnumerationGlobalizationService(new[] { _innerService1.Object, _innerService2.Object, _innerService3.Object });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_service.EnumerationGlobalizationServices, Is.EqualTo(new[] { _innerService1.Object, _innerService2.Object, _innerService3.Object }));
    }

    [Test]
    public void TryGetTypeDisplayName_WithInnerServiceHavingResult_ReturnsResult ()
    {
      var enumValue = EnumWithResources.Value1;
      string nullOutValue = null;
      var theNameOutValue = "The Name";

      var sequence = new VerifiableSequence();
      _innerService1
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetEnumerationValueDisplayName(
                enumValue,
                out nullOutValue))
          .Returns(false)
          .Verifiable();
      _innerService2
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetEnumerationValueDisplayName(
                enumValue,
                out theNameOutValue))
          .Returns(true)
          .Verifiable();
      _innerService3
          // Not in sequence because not called
          .Setup(
            mock => mock.TryGetEnumerationValueDisplayName(
                It.IsAny<Enum>(),
                out nullOutValue))
          .Verifiable();

      string value;
      var result = _service.TryGetEnumerationValueDisplayName(enumValue, out value);

      Assert.That(result, Is.True);
      Assert.That(value, Is.EqualTo("The Name"));

      _innerService1.Verify();
      _innerService2.Verify();
      _innerService3.Verify(
          mock => mock.TryGetEnumerationValueDisplayName(
                It.IsAny<Enum>(),
                out nullOutValue),
          Times.Never());
      sequence.Verify();
    }

    [Test]
    public void TryGetTypeDisplayName_WithoutInnerServiceHavingResult_ReturnsNull ()
    {
      var enumValue = EnumWithResources.Value1;
      string nullOutValue = null;

      var sequence = new VerifiableSequence();
      _innerService1
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetEnumerationValueDisplayName(
                enumValue,
                out nullOutValue))
          .Returns(false)
          .Verifiable();
      _innerService2
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetEnumerationValueDisplayName(
                enumValue,
                out nullOutValue))
          .Returns(false)
          .Verifiable();
      _innerService3
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetEnumerationValueDisplayName(
                enumValue,
                out nullOutValue))
          .Returns(false)
          .Verifiable();

      string value;
      var result = _service.TryGetEnumerationValueDisplayName(enumValue, out value);

      Assert.That(result, Is.False);
      Assert.That(value, Is.Null);

      _innerService1.Verify();
      _innerService2.Verify();
      _innerService3.Verify();
      sequence.Verify();
    }
  }
}
