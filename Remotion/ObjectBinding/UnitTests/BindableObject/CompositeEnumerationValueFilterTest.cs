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
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class CompositeEnumerationValueFilterTest
  {
    private Mock<IBusinessObject> _businessObjectStub;
    private Mock<IBusinessObjectEnumerationProperty> _propertyStub;
    private EnumerationValueInfo _value1;
    private Mock<IEnumerationValueFilter> _trueFilterStub;
    private Mock<IEnumerationValueFilter> _falseFilterStub;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectStub = new Mock<IBusinessObject>();
      _propertyStub = new Mock<IBusinessObjectEnumerationProperty>();

      _value1 = new EnumerationValueInfo("Value1", "ID1", "Value 1", true);

      _trueFilterStub = new Mock<IEnumerationValueFilter>();
      _trueFilterStub.Setup(stub => stub.IsEnabled(_value1, _businessObjectStub.Object, _propertyStub.Object)).Returns(true);
      
      _falseFilterStub = new Mock<IEnumerationValueFilter>();
      _falseFilterStub.Setup(stub => stub.IsEnabled(_value1, _businessObjectStub.Object, _propertyStub.Object)).Returns(false);
    }

    [Test]
    public void IsEnabled_True ()
    {
      var filter = new CompositeEnumerationValueFilter(new IEnumerationValueFilter[0]);
      Assert.That(filter.IsEnabled(_value1, _businessObjectStub.Object, _propertyStub.Object), Is.True);
    }

    [Test]
    public void IsEnabled_True_WithTrueFilter ()
    {
      var filter = new CompositeEnumerationValueFilter(new[] { _trueFilterStub.Object});
      Assert.That(filter.IsEnabled(_value1, _businessObjectStub.Object, _propertyStub.Object), Is.True);
    }

    [Test]
    public void IsEnabled_False ()
    {
      var filter = new CompositeEnumerationValueFilter(new[] { _falseFilterStub.Object });
      Assert.That(filter.IsEnabled(_value1, _businessObjectStub.Object, _propertyStub.Object), Is.False);
    }

    [Test]
    public void IsEnabled_False_SecondFilterFalse ()
    {
      var filter = new CompositeEnumerationValueFilter(new[] {_trueFilterStub.Object, _falseFilterStub.Object });
      Assert.That(filter.IsEnabled(_value1, _businessObjectStub.Object, _propertyStub.Object), Is.False);
    }

    [Test]
    public void IsEnabled_NullBusinessObject ()
    {
      var filter = new CompositeEnumerationValueFilter(new IEnumerationValueFilter[0]);
      Assert.That(filter.IsEnabled(_value1, null, _propertyStub.Object), Is.True);
    }
  }
}