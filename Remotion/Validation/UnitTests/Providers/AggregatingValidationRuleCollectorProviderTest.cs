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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Providers;
using Remotion.Validation.UnitTests.TestDomain;

namespace Remotion.Validation.UnitTests.Providers
{
  [TestFixture]
  public class AggregatingValidationRuleCollectorProviderTest
  {
    private Mock<IInvolvedTypeProvider> _involvedTypeProviderStub;
    private Mock<IValidationRuleCollectorProvider> _validationRuleCollectorProviderMock1;
    private Mock<IValidationRuleCollectorProvider> _validationRuleCollectorProviderMock2;
    private Mock<IValidationRuleCollectorProvider> _validationRuleCollectorProviderMock3;
    private Mock<IValidationRuleCollectorProvider> _validationRuleCollectorProviderMock4;
    private AggregatingValidationRuleCollectorProvider _validationRuleCollectorProvider;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo1;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo2;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo3;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo4;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo5;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo6;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo7;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo8;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo9;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo10;

    [SetUp]
    public void SetUp ()
    {
      _involvedTypeProviderStub = new Mock<IInvolvedTypeProvider>();

      _validationRuleCollectorProviderMock1 = new Mock<IValidationRuleCollectorProvider>(MockBehavior.Strict);
      _validationRuleCollectorProviderMock2 = new Mock<IValidationRuleCollectorProvider>(MockBehavior.Strict);
      _validationRuleCollectorProviderMock3 = new Mock<IValidationRuleCollectorProvider>(MockBehavior.Strict);
      _validationRuleCollectorProviderMock4 = new Mock<IValidationRuleCollectorProvider>(MockBehavior.Strict);

      _validationRuleCollectorProvider = new AggregatingValidationRuleCollectorProvider(
          _involvedTypeProviderStub.Object,
          new[]
          {
              _validationRuleCollectorProviderMock1.Object, _validationRuleCollectorProviderMock2.Object,
              _validationRuleCollectorProviderMock3.Object, _validationRuleCollectorProviderMock4.Object
          });

      var validationRuleCollector = new Mock<IValidationRuleCollector>();
      _validationRuleCollectorInfo1 = new ValidationRuleCollectorInfo(validationRuleCollector.Object, typeof(AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo2 = new ValidationRuleCollectorInfo(validationRuleCollector.Object, typeof(AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo3 = new ValidationRuleCollectorInfo(validationRuleCollector.Object, typeof(AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo4 = new ValidationRuleCollectorInfo(validationRuleCollector.Object, typeof(AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo5 = new ValidationRuleCollectorInfo(validationRuleCollector.Object, typeof(AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo6 = new ValidationRuleCollectorInfo(validationRuleCollector.Object, typeof(AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo7 = new ValidationRuleCollectorInfo(validationRuleCollector.Object, typeof(AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo8 = new ValidationRuleCollectorInfo(validationRuleCollector.Object, typeof(AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo9 = new ValidationRuleCollectorInfo(validationRuleCollector.Object, typeof(AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo10 = new ValidationRuleCollectorInfo(validationRuleCollector.Object, typeof(AggregatingValidationRuleCollectorProvider));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_validationRuleCollectorProvider.InvolvedTypeProvider, Is.SameAs(_involvedTypeProviderStub.Object));
      Assert.That(
          new[]
          {
              _validationRuleCollectorProviderMock1.Object, _validationRuleCollectorProviderMock2.Object, _validationRuleCollectorProviderMock3.Object,
              _validationRuleCollectorProviderMock4.Object
          },
          Is.EqualTo(_validationRuleCollectorProvider.ValidationCollectorProviders));
    }

    [Test]
    public void GetValidationRuleCollectors ()
    {
      var typeGroup1 = new[] { typeof(IPerson), typeof(ICollection) };
      var typeGroup2 = new[] { typeof(Person) };
      var typeGroup3 = new[] { typeof(Customer) };
      _involvedTypeProviderStub.Setup(
          stub => stub.GetTypes(typeof(Customer)))
          .Returns(new[] { typeGroup1, typeGroup2, typeGroup3 });

      var sequence = new VerifiableSequence();
      _validationRuleCollectorProviderMock1
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup1))
            .Returns(new[] { new[] { _validationRuleCollectorInfo1 }, new[] { _validationRuleCollectorInfo2 } })
            .Verifiable();
      _validationRuleCollectorProviderMock2
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup1))
            .Returns(new[] { new[] { _validationRuleCollectorInfo3 }, new[] { _validationRuleCollectorInfo4 } })
            .Verifiable();
      _validationRuleCollectorProviderMock3
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup1))
            .Returns(new[] { new[] { _validationRuleCollectorInfo5 } })
            .Verifiable();
      _validationRuleCollectorProviderMock4
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup1))
            .Returns(Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>())
            .Verifiable();
      _validationRuleCollectorProviderMock1
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup2))
            .Returns(Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>())
            .Verifiable();
      _validationRuleCollectorProviderMock2
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup2))
            .Returns(Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>())
            .Verifiable();
      _validationRuleCollectorProviderMock3
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup2))
            .Returns(new[] { new[] { _validationRuleCollectorInfo6 } })
            .Verifiable();
      _validationRuleCollectorProviderMock4
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup2))
            .Returns(new[] { new[] { _validationRuleCollectorInfo7 } })
            .Verifiable();
      _validationRuleCollectorProviderMock1
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup3))
            .Returns(new[] { new[] { _validationRuleCollectorInfo8, _validationRuleCollectorInfo9 }, new[] { _validationRuleCollectorInfo10 } })
            .Verifiable();
      _validationRuleCollectorProviderMock2
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup3))
            .Returns(Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>())
            .Verifiable();
      _validationRuleCollectorProviderMock3
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup3))
            .Returns(Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>())
            .Verifiable();
      _validationRuleCollectorProviderMock4
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.GetValidationRuleCollectors(typeGroup3))
            .Returns(Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>())
            .Verifiable();

      var result = _validationRuleCollectorProvider.GetValidationRuleCollectors(new[] { typeof(Customer) }).SelectMany(g => g).ToArray();

      _validationRuleCollectorProviderMock1.Verify();
      _validationRuleCollectorProviderMock2.Verify();
      _validationRuleCollectorProviderMock3.Verify();
      _validationRuleCollectorProviderMock4.Verify();
      sequence.Verify();
      Assert.That(
          new[]
          {
              _validationRuleCollectorInfo1, _validationRuleCollectorInfo2, _validationRuleCollectorInfo3, _validationRuleCollectorInfo4, _validationRuleCollectorInfo5,
              _validationRuleCollectorInfo6, _validationRuleCollectorInfo7, _validationRuleCollectorInfo8, _validationRuleCollectorInfo9, _validationRuleCollectorInfo10
          },
          Is.EqualTo(result));
    }
  }
}
