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
  public class DisabledIdentifiersEnumerationFilterTest
  {
    private IBusinessObject _businessObjectStub;
    private IBusinessObjectEnumerationProperty _propertyStub;
    private EnumerationValueInfo _value1;
    private EnumerationValueInfo _value2;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectStub = MockRepository.GenerateStub<IBusinessObject> ();
      _propertyStub = MockRepository.GenerateStub<IBusinessObjectEnumerationProperty> ();
      _value1 = new EnumerationValueInfo ("Value1", "ID1", "Value 1", true);
      _value2 = new EnumerationValueInfo ("Value2", "ID2", "Value 2", true);
    }

    [Test]
    public void IsEnabled_True ()
    {
      var filter = new DisabledIdentifiersEnumerationFilter (new[] { "ID3", "ID4" });

      Assert.That (filter.IsEnabled (_value1, _businessObjectStub, _propertyStub), Is.True);
      Assert.That (filter.IsEnabled (_value2, _businessObjectStub, _propertyStub), Is.True);
    }

    [Test]
    public void IsEnabled_False ()
    {
      var filter = new DisabledIdentifiersEnumerationFilter (new[] { "ID1", "ID2" });

      Assert.That (filter.IsEnabled (_value1, _businessObjectStub, _propertyStub), Is.False);
      Assert.That (filter.IsEnabled (_value2, _businessObjectStub, _propertyStub), Is.False);
    }

    [Test]
    public void IsEnabled_NullBusinessObject ()
    {
      var filter = new DisabledIdentifiersEnumerationFilter (new string[0]);

      Assert.That (filter.IsEnabled (_value1, null, _propertyStub), Is.True);
    }
  }
}