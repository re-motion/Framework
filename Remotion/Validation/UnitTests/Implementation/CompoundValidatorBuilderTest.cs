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
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestDomain;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundValidatorBuilderTest
  {
    private Mock<IValidator<Customer>> _validatorStub1;
    private Mock<IValidator<Customer>> _validatorStub2;
    private Mock<IValidatorBuilder> _validatorBuilderStub1;
    private Mock<IValidatorBuilder> _validatorBuilderStub2;
    private CompoundValidatorBuilder _compoundValidatorBuilder;

    [SetUp]
    public void SetUp ()
    {
      _validatorStub1 = new Mock<IValidator<Customer>>();
      _validatorStub2 = new Mock<IValidator<Customer>>();

      _validatorBuilderStub1 = new Mock<IValidatorBuilder>();
      _validatorBuilderStub2 = new Mock<IValidatorBuilder>();

      _validatorBuilderStub1.Setup (stub => stub.BuildValidator (typeof (Customer))).Returns (_validatorStub1.Object);
      _validatorBuilderStub2.Setup (stub => stub.BuildValidator (typeof (Customer))).Returns (_validatorStub2.Object);

      _compoundValidatorBuilder = new CompoundValidatorBuilder (new[] { _validatorBuilderStub1.Object, _validatorBuilderStub2.Object });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_compoundValidatorBuilder.Builders, Is.EquivalentTo (new[] { _validatorBuilderStub1.Object, _validatorBuilderStub2.Object }));
    }

    [Test]
    public void BuildValidator ()
    {
      var result = _compoundValidatorBuilder.BuildValidator (typeof (Customer));

      Assert.That (result, Is.TypeOf (typeof (CompoundValidator)));
      var compoundValidator = (CompoundValidator) result;
      Assert.That (compoundValidator.Validators, Is.EquivalentTo (new[] { _validatorStub1.Object, _validatorStub2.Object }));
    }
  }
}