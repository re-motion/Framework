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
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.Utilities.TestDomain;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Utilities
{
  [TestFixture]
  public class ConstraintBasedGenericParameterInstantiatorTest
  {
    private ConstraintBasedGenericParameterInstantiator _instantiator;

    [SetUp]
    public void SetUp ()
    {
      _instantiator = new ConstraintBasedGenericParameterInstantiator ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type must be a generic parameter.\r\nParameter name: typeParameter")]
    public void Instantiate_NoGenericParameter ()
    {
      _instantiator.Instantiate (typeof (object));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The generic type parameter has a "
        + "constraint 'T' which itself contains generic parameters.")]
    public void Instantiate_InferFromGenericParameterConstraints_ConstraintIsGenericParameter ()
    {
      _instantiator.Instantiate (typeof (GenericClassWithOneConstraint<>.GenericClassWithDependentConstraint<>).GetGenericArguments()[1]);
    }

    [Test]
    public void Instantiate_InferFromGenericParameterConstraints_NoConstraint ()
    {
      var result = _instantiator.Instantiate (typeof (GenericClassWithNoConstraint<>).GetGenericArguments ()[0]);
      Assert.That (result, Is.SameAs (typeof (object)));
    }

    [Test]
    public void Instantiate_InferFromGenericParameterConstraints_OneConstraint ()
    {
      var result = _instantiator.Instantiate (typeof (GenericClassWithOneConstraint<>).GetGenericArguments ()[0]);
      Assert.That (result, Is.SameAs (typeof (ICloneable)));
    }

    [Test]
    public void Instantiate_InferFromGenericParameterConstraints_MoreConstraints_SecondAssignableFromFirst ()
    {
      var result = _instantiator.Instantiate (typeof (GenericClassWithMoreConstraints<>).GetGenericArguments ()[0]);
      Assert.That (result, Is.SameAs (typeof (List<int>)));
    }

    [Test]
    public void Instantiate_InferFromGenericParameterConstraints_MoreConstraints_FirstAssignableFromSecond ()
    {
      var result = _instantiator.Instantiate (typeof (GenericClassWithMoreConstraintsOtherWay<>).GetGenericArguments ()[0]);
      Assert.That (result, Is.SameAs (typeof (ICollection<int>)));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The generic type parameter has incompatible constraints "
        + "'System.Collections.Generic.List`1[System.Int32]' and 'System.IServiceProvider'.")]
    public void Instantiate_InferFromGenericParameterConstraints_ConflictingConstraints ()
    {
      _instantiator.Instantiate (typeof (GenericClassWithConflictingConstraints<>).GetGenericArguments ()[0]);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The generic type parameter has inconclusive constraints "
        + "'System.Collections.IEnumerable' and 'System.IServiceProvider', which cannot be unified into a single type.")]
    public void Instantiate_InferFromGenericParameterConstraints_InconclusiveConstraints ()
    {
      _instantiator.Instantiate (typeof (GenericClassWithInconclusiveConstraints<>).GetGenericArguments ()[0]);
    }

    [Test]
    public void Instantiate_StructConstraint ()
    {
      var result = _instantiator.Instantiate (typeof (GenericClassWithStructConstraint<>).GetGenericArguments ()[0]);
      Assert.That (result, Is.SameAs (typeof (int)));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The generic type parameter has inconclusive constraints 'System.ValueType' "
        + "and 'System.ICloneable', which cannot be unified into a single type.")]
    public void Instantiate_StructConstraintAndInterfaceConstraint ()
    {
      _instantiator.Instantiate (typeof (GenericClassWithStructAndInterfaceConstraint<>).GetGenericArguments ()[0]);
    }
  }
}
