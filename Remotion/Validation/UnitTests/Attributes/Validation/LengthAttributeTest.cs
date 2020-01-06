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
using Remotion.Reflection;
using Remotion.Validation.Attributes.Validation;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Attributes.Validation
{
  [TestFixture]
  public class LengthAttributeTest
  {
    private LengthAttribute _attribute;
    private IValidationMessageFactory _validationMessageFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _attribute = new LengthAttribute (10, 20);
      _validationMessageFactoryStub = MockRepository.GenerateStub<IValidationMessageFactory>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_attribute.MinLength, Is.EqualTo (10));
      Assert.That (_attribute.MaxLength, Is.EqualTo (20));
    }

    [Test]
    public void GetPropertyValidator ()
    {
      var propertyInformation = PropertyInfoAdapter.Create (typeof (Customer).GetProperty ("LastName"));
      var invariantValidationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
      _validationMessageFactoryStub
          .Stub (_ => _.CreateValidationMessageForPropertyValidator (typeof (LengthValidator), propertyInformation))
          .Return (invariantValidationMessageStub);

      var result = _attribute.GetPropertyValidators (propertyInformation, _validationMessageFactoryStub).ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (LengthValidator)));
      Assert.That (((LengthValidator) result[0]).ValidationMessage, Is.SameAs (invariantValidationMessageStub));
      Assert.That (((LengthValidator) result[0]).Min, Is.EqualTo (10));
      Assert.That (((LengthValidator) result[0]).Max, Is.EqualTo (20));
    }

    [Test]
    public void GetPropertyValidator_CustomMessage ()
    {
      var propertyInformation = PropertyInfoAdapter.Create (typeof (Customer).GetProperty ("LastName"));
      _attribute.ErrorMessage = "CustomMessage";

      var result = _attribute.GetPropertyValidators (propertyInformation, _validationMessageFactoryStub).ToArray();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (((LengthValidator) result[0]).ValidationMessage, Is.InstanceOf<InvariantValidationMessage>());
      Assert.That (((LengthValidator) result[0]).ValidationMessage.ToString(), Is.EqualTo ("CustomMessage"));
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void GetPropertyValidator_WithValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }
  }
}