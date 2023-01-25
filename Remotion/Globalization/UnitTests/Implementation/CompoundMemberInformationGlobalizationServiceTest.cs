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
using System.Globalization;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundMemberInformationGlobalizationServiceTest
  {
    private CompoundMemberInformationGlobalizationService _service;
    private Mock<IMemberInformationGlobalizationService> _innerService1;
    private Mock<IMemberInformationGlobalizationService> _innerService2;
    private Mock<IMemberInformationGlobalizationService> _innerService3;

    [SetUp]
    public void SetUp ()
    {
      _innerService1 = new Mock<IMemberInformationGlobalizationService>(MockBehavior.Strict);
      _innerService2 = new Mock<IMemberInformationGlobalizationService>(MockBehavior.Strict);
      _innerService3 = new Mock<IMemberInformationGlobalizationService>(MockBehavior.Strict);

      _service = new CompoundMemberInformationGlobalizationService(new[] { _innerService1.Object, _innerService2.Object, _innerService3.Object });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_service.MemberInformationGlobalizationServices, Is.EqualTo(new[] { _innerService1.Object, _innerService2.Object, _innerService3.Object }));
    }

    [Test]
    public void TryGetTypeDisplayName_WithInnerServiceHavingResult_ReturnsResult ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();
      string nullOutValue = null;
      string theNameOutValue = "The Name";

      var sequence = new VerifiableSequence();
      _innerService1
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetTypeDisplayName(
                typeInformationStub.Object,
                typeInformationForResourceResolutionStub.Object,
                out nullOutValue))
          .Returns(false)
          .Verifiable();
      _innerService2
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetTypeDisplayName(
                typeInformationStub.Object,
                typeInformationForResourceResolutionStub.Object,
                out theNameOutValue))
          .Returns(true)
          .Verifiable();
      _innerService3
          // Not in sequence because not called
          .Setup(
            mock => mock.TryGetTypeDisplayName(
                It.IsAny<ITypeInformation>(),
                It.IsAny<ITypeInformation>(),
                out nullOutValue))
          .Verifiable();

      string value;
      var result = _service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out value);

      Assert.That(result, Is.True);
      Assert.That(value, Is.EqualTo("The Name"));

      _innerService1.Verify();
      _innerService2.Verify();
      _innerService3.Verify(
          mock => mock.TryGetTypeDisplayName(
                It.IsAny<ITypeInformation>(),
                It.IsAny<ITypeInformation>(),
                out nullOutValue),
          Times.Never());
      sequence.Verify();
    }

    [Test]
    public void TryGetTypeDisplayName_WithoutInnerServiceHavingResult_ReturnsNull ()
    {
      var typeInformationStub = new Mock<ITypeInformation>();
      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();
      string nullOutValue = null;

      var sequence = new VerifiableSequence();
      _innerService1
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetTypeDisplayName(
                typeInformationStub.Object,
                typeInformationForResourceResolutionStub.Object,
                out nullOutValue))
          .Returns(false)
          .Verifiable();
      _innerService2
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetTypeDisplayName(
                typeInformationStub.Object,
                typeInformationForResourceResolutionStub.Object,
                out nullOutValue))
          .Returns(false)
          .Verifiable();
      _innerService3
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetTypeDisplayName(
                typeInformationStub.Object,
                typeInformationForResourceResolutionStub.Object,
                out nullOutValue))
          .Returns(false)
          .Verifiable();

      string value;
      var result = _service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out value);

      Assert.That(result, Is.False);
      Assert.That(value, Is.Null);

      _innerService1.Verify();
      _innerService2.Verify();
      _innerService3.Verify();
      sequence.Verify();
    }

    [Test]
    public void TryGetPropertyDisplayName_WithInnerServiceHavingResult_ReturnsResult ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();
      string nullOutValue = null;
      string theNameOutValue = "The Name";

      var sequence = new VerifiableSequence();
      _innerService1
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetPropertyDisplayName(
                propertyInformationStub.Object,
                typeInformationForResourceResolutionStub.Object,
                out nullOutValue))
          .Returns(false)
          .Verifiable();

      _innerService2
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetPropertyDisplayName(
                propertyInformationStub.Object,
                typeInformationForResourceResolutionStub.Object,
                out theNameOutValue))
          .Returns(true)
          .Verifiable();
      _innerService3
          // Not in sequence because not called
          .Setup(
            mock => mock.TryGetPropertyDisplayName(
                It.IsAny<IPropertyInformation>(),
                It.IsAny<ITypeInformation>(),
                out nullOutValue))
          .Verifiable();

      string value;
      var result = _service.TryGetPropertyDisplayName(propertyInformationStub.Object, typeInformationForResourceResolutionStub.Object, out value);

      Assert.That(result, Is.True);
      Assert.That(value, Is.EqualTo("The Name"));

      _innerService1.Verify();
      _innerService2.Verify();
      _innerService3.Verify(
          mock => mock.TryGetPropertyDisplayName(
                It.IsAny<IPropertyInformation>(),
                It.IsAny<ITypeInformation>(),
                out nullOutValue),
          Times.Never());
      sequence.Verify();
    }

    [Test]
    public void TryGetPropertyeDisplayName_WithoutInnerServiceHavingResult_ReturnsNull ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();
      string nullOutValue = null;

      var sequence = new VerifiableSequence();
      _innerService1
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetPropertyDisplayName(
                propertyInformationStub.Object,
                typeInformationForResourceResolutionStub.Object,
                out nullOutValue))
          .Returns(false)
          .Verifiable();
      _innerService2
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetPropertyDisplayName(
                propertyInformationStub.Object,
                typeInformationForResourceResolutionStub.Object,
                out nullOutValue))
          .Returns(false)
          .Verifiable();
      _innerService3
          .InVerifiableSequence(sequence)
          .Setup(
            mock => mock.TryGetPropertyDisplayName(
                propertyInformationStub.Object,
                typeInformationForResourceResolutionStub.Object,
                out nullOutValue))
          .Returns(false)
          .Verifiable();

      string value;
      var result = _service.TryGetPropertyDisplayName(propertyInformationStub.Object, typeInformationForResourceResolutionStub.Object, out value);

      Assert.That(result, Is.False);
      Assert.That(value, Is.Null);

      _innerService1.Verify();
      _innerService2.Verify();
      _innerService3.Verify();
      sequence.Verify();
    }

    [Test]
    public void GetAvailablePropertyDisplayNames_ReturnsResultInRightOverriddenOrder ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();

      var dictionary1 = new Dictionary<CultureInfo, string>();
      dictionary1.Add(new CultureInfo("de-AT"), "AT_Wert1");

      var dictionary2 = new Dictionary<CultureInfo, string>();
      dictionary2.Add(new CultureInfo("de-AT"), "AT_Wert2");

      var dictionary3 = new Dictionary<CultureInfo, string>();
      dictionary3.Add(new CultureInfo("de-DE"), "DE_Wert1");
      dictionary3.Add(new CultureInfo("en-US"), "EN_Wert1");
      dictionary3.Add(new CultureInfo("de-AT"), "AT_Wert3");

      var expected = new List<KeyValuePair<CultureInfo, string>>();
      expected.Add(new KeyValuePair<CultureInfo, string>(new CultureInfo("de-AT"), "AT_Wert1"));
      expected.Add(new KeyValuePair<CultureInfo, string>(new CultureInfo("de-DE"), "DE_Wert1"));
      expected.Add(new KeyValuePair<CultureInfo, string>(new CultureInfo("en-US"), "EN_Wert1"));

      List<KeyValuePair<CultureInfo, string>> result;
      var sequence = new VerifiableSequence();
      var innerService1 = new Mock<IMemberInformationGlobalizationService>();
      var innerService2 = new Mock<IMemberInformationGlobalizationService>();
      var innerService3 = new Mock<IMemberInformationGlobalizationService>();
      var service = new CompoundMemberInformationGlobalizationService(new[] { innerService1.Object, innerService2.Object, innerService3.Object });

      innerService1
            .InVerifiableSequence(sequence)
            .Setup(s => s.GetAvailablePropertyDisplayNames(propertyInformationStub.Object, typeInformationForResourceResolutionStub.Object))
            .Returns(dictionary1)
            .Verifiable();
      innerService2
            .InVerifiableSequence(sequence)
            .Setup(s => s.GetAvailablePropertyDisplayNames(propertyInformationStub.Object, typeInformationForResourceResolutionStub.Object))
            .Returns(dictionary2.AsReadOnly())
            .Verifiable();
      innerService3
            .InVerifiableSequence(sequence)
            .Setup(s => s.GetAvailablePropertyDisplayNames(propertyInformationStub.Object, typeInformationForResourceResolutionStub.Object))
            .Returns(dictionary3.AsReadOnly())
            .Verifiable();

      result = service.GetAvailablePropertyDisplayNames(propertyInformationStub.Object, typeInformationForResourceResolutionStub.Object).ToList();

      innerService1.Verify(s => s.GetAvailablePropertyDisplayNames(propertyInformationStub.Object, typeInformationForResourceResolutionStub.Object), Times.AtLeastOnce());
      innerService2.Verify(s => s.GetAvailablePropertyDisplayNames(propertyInformationStub.Object, typeInformationForResourceResolutionStub.Object), Times.AtLeastOnce());
      innerService3.Verify(s => s.GetAvailablePropertyDisplayNames(propertyInformationStub.Object, typeInformationForResourceResolutionStub.Object), Times.AtLeastOnce());
      sequence.Verify();

      Assert.That(expected.Count, Is.EqualTo(result.Count));
      Assert.That(expected[0], Is.EqualTo(result[0]));
      Assert.That(expected[1], Is.EqualTo(result[1]));
      Assert.That(expected[2], Is.EqualTo(result[2]));
    }
  }
}
