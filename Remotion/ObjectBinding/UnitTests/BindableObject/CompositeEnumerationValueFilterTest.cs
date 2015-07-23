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
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class CompositeEnumerationValueFilterTest
  {
    private IBusinessObject _businessObjectStub;
    private IBusinessObjectEnumerationProperty _propertyStub;
    private EnumerationValueInfo _value1;
    private IEnumerationValueFilter _trueFilterStub;
    private IEnumerationValueFilter _falseFilterStub;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();
      _propertyStub = MockRepository.GenerateStub<IBusinessObjectEnumerationProperty>();

      _value1 = new EnumerationValueInfo ("Value1", "ID1", "Value 1", true);

      _trueFilterStub = MockRepository.GenerateStub<IEnumerationValueFilter>();
      _trueFilterStub.Stub (stub => stub.IsEnabled (_value1, _businessObjectStub, _propertyStub)).Return (true);
      
      _falseFilterStub = MockRepository.GenerateStub<IEnumerationValueFilter>();
      _falseFilterStub.Stub (stub => stub.IsEnabled (_value1, _businessObjectStub, _propertyStub)).Return (false);
    }

    [Test]
    public void IsEnabled_True ()
    {
      var filter = new CompositeEnumerationValueFilter (new IEnumerationValueFilter[0]);
      Assert.That (filter.IsEnabled (_value1, _businessObjectStub, _propertyStub), Is.True);
    }

    [Test]
    public void IsEnabled_True_WithTrueFilter ()
    {
      var filter = new CompositeEnumerationValueFilter (new[] { _trueFilterStub});
      Assert.That (filter.IsEnabled (_value1, _businessObjectStub, _propertyStub), Is.True);
    }

    [Test]
    public void IsEnabled_False ()
    {
      var filter = new CompositeEnumerationValueFilter (new[] { _falseFilterStub });
      Assert.That (filter.IsEnabled (_value1, _businessObjectStub, _propertyStub), Is.False);
    }

    [Test]
    public void IsEnabled_False_SecondFilterFalse ()
    {
      var filter = new CompositeEnumerationValueFilter (new[] {_trueFilterStub, _falseFilterStub });
      Assert.That (filter.IsEnabled (_value1, _businessObjectStub, _propertyStub), Is.False);
    }

    [Test]
    public void IsEnabled_NullBusinessObject ()
    {
      var filter = new CompositeEnumerationValueFilter (new IEnumerationValueFilter[0]);
      Assert.That (filter.IsEnabled (_value1, null, _propertyStub), Is.True);
    }
  }
}