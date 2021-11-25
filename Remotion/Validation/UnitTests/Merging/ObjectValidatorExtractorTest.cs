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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestDomain.Validators;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Merging
{
  [TestFixture]
  public class ObjectValidatorExtractorTest
  {
    private IObjectValidator _stubObjectValidator1;
    private IObjectValidator _stubObjectValidator2;
    private Mock<IObjectValidator> _stubObjectValidator3;
    private Mock<IObjectValidator> _stubObjectValidator4;
    private Mock<IObjectValidator> _stubObjectValidator5;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration1;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration2;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration3;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration4;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration5;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration6;
    private RemovingObjectValidatorRegistration _removingObjectValidatorRegistration7;
    private Mock<IRemovingObjectValidationRuleCollector> _removingObjectValidationRuleCollectorStub1;
    private Mock<IRemovingObjectValidationRuleCollector> _removingObjectValidationRuleCollectorStub2;
    private Mock<IRemovingObjectValidationRuleCollector> _removingObjectValidationRuleCollectorStub3;
    private Mock<IRemovingObjectValidationRuleCollector> _removingObjectValidationRuleCollectorStub4;
    private Mock<ILogContext> _logContextMock;

    [SetUp]
    public void SetUp ()
    {
      _stubObjectValidator1 = new FakeCustomerValidator(); //extracted
      _stubObjectValidator2 = new StubObjectValidator(); //extracted
      _stubObjectValidator3 = new Mock<IObjectValidator>(); //not extracted
      _stubObjectValidator4 = new Mock<IObjectValidator>(); //extracted
      _stubObjectValidator5 = new Mock<IObjectValidator>(); //extracted

      var registration1A = new {ValidatorType = typeof(FakeCustomerValidator), CollectorTypeToRemoveFrom = (Type) null };
      var registration2A = new {ValidatorType = typeof(StubObjectValidator), CollectorTypeToRemoveFrom = typeof(CustomerValidationRuleCollector1) };
      var registration2B = new {ValidatorType = typeof(StubObjectValidator), CollectorTypeToRemoveFrom = typeof(CustomerValidationRuleCollector2) };
      var registration2C = new {ValidatorType = typeof(StubObjectValidator), CollectorTypeToRemoveFrom = (Type) null };

      _removingObjectValidationRuleCollectorStub1 = new Mock<IRemovingObjectValidationRuleCollector>();
      _removingObjectValidationRuleCollectorStub1.Setup(stub => stub.ValidatedType).Returns(TypeAdapter.Create(typeof(Customer)));
      _removingObjectValidationRuleCollectorStub2 = new Mock<IRemovingObjectValidationRuleCollector>();
      _removingObjectValidationRuleCollectorStub2.Setup(stub => stub.ValidatedType).Returns(TypeAdapter.Create(typeof(Customer)));
      _removingObjectValidationRuleCollectorStub3 = new Mock<IRemovingObjectValidationRuleCollector>();
      _removingObjectValidationRuleCollectorStub3.Setup(stub => stub.ValidatedType).Returns(TypeAdapter.Create(typeof(Employee)));
      _removingObjectValidationRuleCollectorStub4 = new Mock<IRemovingObjectValidationRuleCollector>();
      _removingObjectValidationRuleCollectorStub4.Setup(stub => stub.ValidatedType).Returns(TypeAdapter.Create(typeof(SpecialCustomer1)));

      _removingObjectValidatorRegistration1 = new RemovingObjectValidatorRegistration(registration1A.ValidatorType, registration1A.CollectorTypeToRemoveFrom, null, _removingObjectValidationRuleCollectorStub1.Object);
      _removingObjectValidatorRegistration2 = new RemovingObjectValidatorRegistration(registration2A.ValidatorType, registration2A.CollectorTypeToRemoveFrom, null, _removingObjectValidationRuleCollectorStub1.Object);
      _removingObjectValidatorRegistration3 = new RemovingObjectValidatorRegistration(registration2B.ValidatorType, registration2B.CollectorTypeToRemoveFrom, null, _removingObjectValidationRuleCollectorStub2.Object);
      _removingObjectValidatorRegistration4 = new RemovingObjectValidatorRegistration(registration1A.ValidatorType, registration1A.CollectorTypeToRemoveFrom, null, _removingObjectValidationRuleCollectorStub1.Object);
      _removingObjectValidatorRegistration5 = new RemovingObjectValidatorRegistration(registration2C.ValidatorType, registration2C.CollectorTypeToRemoveFrom, null, _removingObjectValidationRuleCollectorStub3.Object);
      _removingObjectValidatorRegistration6 = new RemovingObjectValidatorRegistration(_stubObjectValidator4.Object.GetType(), null, v => ReferenceEquals(v, _stubObjectValidator4.Object), _removingObjectValidationRuleCollectorStub4.Object);
      _removingObjectValidatorRegistration7 = new RemovingObjectValidatorRegistration(_stubObjectValidator4.Object.GetType(), null, v => ReferenceEquals(v, _stubObjectValidator5.Object), _removingObjectValidationRuleCollectorStub1.Object);

      _logContextMock = new Mock<ILogContext>(MockBehavior.Strict);
    }

    [Test]
    public void ExtractObjectValidatorsToRemove ()
    {
      var addingObjectValidationRuleCollector = new Mock<IAddingObjectValidationRuleCollector>();
      addingObjectValidationRuleCollector.Setup(stub => stub.Validators)
          .Returns(new[] { _stubObjectValidator1, _stubObjectValidator3.Object, _stubObjectValidator2, _stubObjectValidator4.Object, _stubObjectValidator5.Object });
      addingObjectValidationRuleCollector.Setup(stub => stub.CollectorType).Returns(typeof(CustomerValidationRuleCollector1));
      addingObjectValidationRuleCollector.Setup(stub => stub.ValidatedType).Returns(TypeAdapter.Create(typeof(Customer)));

      _logContextMock.Setup(
          mock =>
              mock.ValidatorRemoved(
                  _stubObjectValidator1,
                  new[] { _removingObjectValidatorRegistration1, _removingObjectValidatorRegistration4 },
                  addingObjectValidationRuleCollector.Object))
          .Verifiable();
      _logContextMock.Setup(
          mock =>
              mock.ValidatorRemoved(
                  _stubObjectValidator2,
                  new[] { _removingObjectValidatorRegistration2 },
                  addingObjectValidationRuleCollector.Object))
          .Verifiable();
      _logContextMock.Setup(
          mock =>
              mock.ValidatorRemoved(
                  _stubObjectValidator4.Object,
                  new[] { _removingObjectValidatorRegistration6 },
                  addingObjectValidationRuleCollector.Object))
          .Verifiable();
      _logContextMock.Setup(
          mock =>
              mock.ValidatorRemoved(
                  _stubObjectValidator5.Object,
                  new[] { _removingObjectValidatorRegistration7 },
                  addingObjectValidationRuleCollector.Object))
          .Verifiable();

      var extractor = new ObjectValidatorExtractor(
          new[]
          {
              _removingObjectValidatorRegistration1, _removingObjectValidatorRegistration2, _removingObjectValidatorRegistration3, _removingObjectValidatorRegistration4,
              _removingObjectValidatorRegistration5, _removingObjectValidatorRegistration6, _removingObjectValidatorRegistration7
          },
          _logContextMock.Object);

      var result = extractor.ExtractObjectValidatorsToRemove(addingObjectValidationRuleCollector.Object).ToArray();

      _logContextMock.Verify(
          mock =>
              mock.ValidatorRemoved(
                  _stubObjectValidator5.Object,
                  new[] { _removingObjectValidatorRegistration7 },
                  addingObjectValidationRuleCollector.Object), Times.Once());
      Assert.That(result, Is.EqualTo(new[] { _stubObjectValidator1, _stubObjectValidator2, _stubObjectValidator4.Object, _stubObjectValidator5.Object }));
    }
  }
}
