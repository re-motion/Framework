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
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class CompundBindablePropertyWriteAccessStrategyTest : TestBase
  {
    private MockRepository _mockRepository;
    private CompundBindablePropertyWriteAccessStrategy _strategy;
    private IBindablePropertyWriteAccessStrategy _innerStrategy1;
    private IBindablePropertyWriteAccessStrategy _innerStrategy2;
    private IBindablePropertyWriteAccessStrategy _innerStrategy3;
    private BindableObjectProvider _businessObjectProvider;
    private PropertyBase _property;
    private IBusinessObject _businessObject;

    public override void SetUp ()
    {
      base.SetUp();
      _mockRepository = new MockRepository();

      _innerStrategy1 = _mockRepository.StrictMock<IBindablePropertyWriteAccessStrategy>();
      _innerStrategy2 = _mockRepository.StrictMock<IBindablePropertyWriteAccessStrategy>();
      _innerStrategy3 = _mockRepository.StrictMock<IBindablePropertyWriteAccessStrategy>();

      _strategy = new CompundBindablePropertyWriteAccessStrategy (new[] { _innerStrategy1, _innerStrategy2, _innerStrategy3 });

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _property = new StubPropertyBase (GetPropertyParameters (GetPropertyInfo (typeof (ClassWithAllDataTypes), "Byte"), _businessObjectProvider));
      _businessObject = MockRepository.GenerateStub<IBusinessObject>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_strategy.BindablePropertyWriteAccessStrategies, Is.EqualTo (new[] { _innerStrategy1, _innerStrategy2, _innerStrategy3 }));
    }

    [Test]
    public void CanWrite_WithoutStrategies_ReturnsTrue ()
    {

      var strategy = new CompundBindablePropertyWriteAccessStrategy (Enumerable.Empty<IBindablePropertyWriteAccessStrategy>());
      var result = strategy.CanWrite (_businessObject, _property);

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanRead_WithNullBusinessObject_ReturnsValue ()
    {
      using (_mockRepository.Ordered())
      {
        _innerStrategy1.Expect (mock => mock.CanWrite (null, _property)).Return (true);
        _innerStrategy2.Expect (mock => mock.CanWrite (null, _property)).Return (true);
        _innerStrategy3.Expect (mock => mock.CanWrite (null, _property)).Return (true);
      }
      _mockRepository.ReplayAll();

      var result = _strategy.CanWrite (null, _property);

      Assert.That (result, Is.True);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CanWrite_WithAllStrategiesReturingTrue_ReturnsTrue ()
    {
      using (_mockRepository.Ordered())
      {
        _innerStrategy1.Expect (mock => mock.CanWrite (_businessObject, _property)).Return (true);
        _innerStrategy2.Expect (mock => mock.CanWrite (_businessObject, _property)).Return (true);
        _innerStrategy3.Expect (mock => mock.CanWrite (_businessObject, _property)).Return (true);
      }
      _mockRepository.ReplayAll();

      var result = _strategy.CanWrite (_businessObject, _property);

      Assert.That (result, Is.True);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void CanWrite_WithOneStrategyReturingFalse_ReturnsFalse_AndAbortsChecks ()
    {
      using (_mockRepository.Ordered())
      {
        _innerStrategy1.Expect (mock => mock.CanWrite (_businessObject, _property)).Return (true);
        _innerStrategy2.Expect (mock => mock.CanWrite (_businessObject, _property)).Return (false);
        _innerStrategy3.Expect (mock => mock.CanWrite (_businessObject, _property)).Repeat.Never();
      }
      _mockRepository.ReplayAll();

      var result = _strategy.CanWrite (_businessObject, _property);

      Assert.That (result, Is.False);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void IsPropertyAccessException_WithAllStrategiesReturningFalse_ReturnsFalse ()
    {
      var exception = new Exception();
      using (_mockRepository.Ordered())
      {
        _innerStrategy1
            .Expect (
                mock => mock.IsPropertyAccessException (
                    Arg.Is (_businessObject),
                    Arg.Is (_property),
                    Arg.Is (exception),
                    out Arg<BusinessObjectPropertyAccessException>.Out (null).Dummy))
            .Return (false);
        _innerStrategy2
            .Expect (
                mock => mock.IsPropertyAccessException (
                    Arg.Is (_businessObject),
                    Arg.Is (_property),
                    Arg.Is (exception),
                    out Arg<BusinessObjectPropertyAccessException>.Out (new BusinessObjectPropertyAccessException ("Unexpected", null)).Dummy))
            .Return (false);
        _innerStrategy3
            .Expect (
                mock => mock.IsPropertyAccessException (
                    Arg.Is (_businessObject),
                    Arg.Is (_property),
                    Arg.Is (exception),
                    out Arg<BusinessObjectPropertyAccessException>.Out (null).Dummy))
            .Return (false);
      }
      _mockRepository.ReplayAll();

      BusinessObjectPropertyAccessException actualException;
      var result = _strategy.IsPropertyAccessException (_businessObject, _property, exception, out actualException);

      Assert.That (result, Is.False);
      Assert.That (actualException, Is.Null);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void IsPropertyAccessException_WithOneStrategyReturningTrue_ReturnsTrue_SetsResultValue_AndAbortsChecks ()
    {
      var exception = new Exception();
      var expectedException = new BusinessObjectPropertyAccessException ("The Message", null);
      using (_mockRepository.Ordered())
      {
        _innerStrategy1
            .Expect (
                mock => mock.IsPropertyAccessException (
                    Arg.Is (_businessObject),
                    Arg.Is (_property),
                    Arg.Is (exception),
                    out Arg<BusinessObjectPropertyAccessException>.Out (null).Dummy))
            .Return (false);
        _innerStrategy2
            .Expect (
                mock => mock.IsPropertyAccessException (
                    Arg.Is (_businessObject),
                    Arg.Is (_property),
                    Arg.Is (exception),
                    out Arg<BusinessObjectPropertyAccessException>.Out (expectedException).Dummy))
            .Return (true);
        _innerStrategy3.Expect (
            mock => mock.IsPropertyAccessException (
                Arg.Is (_businessObject),
                Arg.Is (_property),
                Arg.Is (exception),
                out Arg<BusinessObjectPropertyAccessException>.Out (null).Dummy))
            .Repeat.Never();
      }
      _mockRepository.ReplayAll();

      BusinessObjectPropertyAccessException actualException;
      var result = _strategy.IsPropertyAccessException (_businessObject, _property, exception, out actualException);

      Assert.That (result, Is.True);
      Assert.That (actualException, Is.SameAs (expectedException));

      _mockRepository.VerifyAll();
    }
  }
}
