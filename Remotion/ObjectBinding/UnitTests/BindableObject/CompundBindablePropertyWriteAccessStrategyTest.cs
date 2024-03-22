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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class CompundBindablePropertyWriteAccessStrategyTest : TestBase
  {
    private CompundBindablePropertyWriteAccessStrategy _strategy;
    private Mock<IBindablePropertyWriteAccessStrategy> _innerStrategy1;
    private Mock<IBindablePropertyWriteAccessStrategy> _innerStrategy2;
    private Mock<IBindablePropertyWriteAccessStrategy> _innerStrategy3;
    private BindableObjectProvider _businessObjectProvider;
    private PropertyBase _property;
    private Mock<IBusinessObject> _businessObject;

    public override void SetUp ()
    {
      base.SetUp();

      _innerStrategy1 = new Mock<IBindablePropertyWriteAccessStrategy>(MockBehavior.Strict);
      _innerStrategy2 = new Mock<IBindablePropertyWriteAccessStrategy>(MockBehavior.Strict);
      _innerStrategy3 = new Mock<IBindablePropertyWriteAccessStrategy>(MockBehavior.Strict);

      _strategy = new CompundBindablePropertyWriteAccessStrategy(new[] { _innerStrategy1.Object, _innerStrategy2.Object, _innerStrategy3.Object });

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _property = new StubPropertyBase(GetPropertyParameters(GetPropertyInfo(typeof(ClassWithAllDataTypes), "Byte"), _businessObjectProvider));
      _businessObject = new Mock<IBusinessObject>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_strategy.BindablePropertyWriteAccessStrategies, Is.EqualTo(new[] { _innerStrategy1.Object, _innerStrategy2.Object, _innerStrategy3.Object }));
    }

    [Test]
    public void CanWrite_WithoutStrategies_ReturnsTrue ()
    {
      var strategy = new CompundBindablePropertyWriteAccessStrategy(Enumerable.Empty<IBindablePropertyWriteAccessStrategy>());
      var result = strategy.CanWrite(_businessObject.Object, _property);

      Assert.That(result, Is.True);
    }

    [Test]
    public void CanRead_WithNullBusinessObject_ReturnsValue ()
    {
      var sequence = new VerifiableSequence();
      _innerStrategy1.InVerifiableSequence(sequence).Setup(mock => mock.CanWrite(null, _property)).Returns(true).Verifiable();
      _innerStrategy2.InVerifiableSequence(sequence).Setup(mock => mock.CanWrite(null, _property)).Returns(true).Verifiable();
      _innerStrategy3.InVerifiableSequence(sequence).Setup(mock => mock.CanWrite(null, _property)).Returns(true).Verifiable();

      var result = _strategy.CanWrite(null, _property);

      Assert.That(result, Is.True);

      _innerStrategy1.Verify();
      _innerStrategy2.Verify();
      _innerStrategy3.Verify();
      sequence.Verify();
    }

    [Test]
    public void CanWrite_WithAllStrategiesReturingTrue_ReturnsTrue ()
    {
      var sequence = new VerifiableSequence();
      _innerStrategy1.InVerifiableSequence(sequence).Setup(mock => mock.CanWrite(_businessObject.Object, _property)).Returns(true).Verifiable();
      _innerStrategy2.InVerifiableSequence(sequence).Setup(mock => mock.CanWrite(_businessObject.Object, _property)).Returns(true).Verifiable();
      _innerStrategy3.InVerifiableSequence(sequence).Setup(mock => mock.CanWrite(_businessObject.Object, _property)).Returns(true).Verifiable();

      var result = _strategy.CanWrite(_businessObject.Object, _property);

      Assert.That(result, Is.True);

      _innerStrategy1.Verify();
      _innerStrategy2.Verify();
      _innerStrategy3.Verify();
      sequence.Verify();
    }

    [Test]
    public void CanWrite_WithOneStrategyReturingFalse_ReturnsFalse_AndAbortsChecks ()
    {
      var sequence = new VerifiableSequence();
      _innerStrategy1.InVerifiableSequence(sequence).Setup(mock => mock.CanWrite(_businessObject.Object, _property)).Returns(true).Verifiable();
      _innerStrategy2.InVerifiableSequence(sequence).Setup(mock => mock.CanWrite(_businessObject.Object, _property)).Returns(false).Verifiable();
      _innerStrategy3.Setup(mock => mock.CanWrite(_businessObject.Object, _property)).Verifiable(); // Not in sequence because not called

      var result = _strategy.CanWrite(_businessObject.Object, _property);

      Assert.That(result, Is.False);

      _innerStrategy1.Verify();
      _innerStrategy2.Verify();
      _innerStrategy3.Verify(mock => mock.CanWrite(_businessObject.Object, _property), Times.Never());
      sequence.Verify();
    }

    [Test]
    public void IsPropertyAccessException_WithAllStrategiesReturningFalse_ReturnsFalse ()
    {
      var exception = new Exception();
      var expectedException = new BusinessObjectPropertyAccessException("Unexpected", null);
      BusinessObjectPropertyAccessException nullOutValue = null;
      var sequence = new VerifiableSequence();
      _innerStrategy1
            .InVerifiableSequence(sequence)
            .Setup(
                mock => mock.IsPropertyAccessException(
                    _businessObject.Object,
                    _property,
                    exception,
                    out nullOutValue))
            .Returns(false)
            .Verifiable();
      _innerStrategy2
            .InVerifiableSequence(sequence)
            .Setup(
                mock => mock.IsPropertyAccessException(
                    _businessObject.Object,
                    _property,
                    exception,
                    out expectedException))
            .Returns(false)
            .Verifiable();
      _innerStrategy3
            .InVerifiableSequence(sequence)
            .Setup(
                mock => mock.IsPropertyAccessException(
                    _businessObject.Object,
                    _property,
                    exception,
                    out nullOutValue))
            .Returns(false)
            .Verifiable();

      BusinessObjectPropertyAccessException actualException;
      var result = _strategy.IsPropertyAccessException(_businessObject.Object, _property, exception, out actualException);

      Assert.That(result, Is.False);
      Assert.That(actualException, Is.Null);

      _innerStrategy1.Verify();
      _innerStrategy2.Verify();
      _innerStrategy3.Verify();
      sequence.Verify();
    }

    [Test]
    public void IsPropertyAccessException_WithOneStrategyReturningTrue_ReturnsTrue_SetsResultValue_AndAbortsChecks ()
    {
      var exception = new Exception();
      var expectedException = new BusinessObjectPropertyAccessException("The Message", null);
      BusinessObjectPropertyAccessException nullOutValue = null;
      var sequence = new VerifiableSequence();
      _innerStrategy1
            .InVerifiableSequence(sequence)
            .Setup(
                mock => mock.IsPropertyAccessException(
                    _businessObject.Object,
                    _property,
                    exception,
                    out nullOutValue))
            .Returns(false)
            .Verifiable();
      _innerStrategy2
            .InVerifiableSequence(sequence)
            .Setup(
                mock => mock.IsPropertyAccessException(
                    _businessObject.Object,
                    _property,
                    exception,
                    out expectedException))
            .Returns(true)
            .Verifiable();
      _innerStrategy3
            // Not in sequence because not called
            .Setup(
                mock => mock.IsPropertyAccessException(
                    _businessObject.Object,
                    _property,
                    exception,
                    out nullOutValue))
            .Verifiable();

      BusinessObjectPropertyAccessException actualException;
      var result = _strategy.IsPropertyAccessException(_businessObject.Object, _property, exception, out actualException);

      Assert.That(result, Is.True);
      Assert.That(actualException, Is.SameAs(expectedException));

      _innerStrategy1.Verify();
      _innerStrategy2.Verify();
      _innerStrategy3.Verify(
          mock => mock.IsPropertyAccessException(
                _businessObject.Object,
                _property,
                exception,
                out nullOutValue),
          Times.Never());
      sequence.Verify();
    }
  }
}
