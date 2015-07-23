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
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Validation.Attributes.Validation;
using Remotion.Validation.UnitTests.TestDomain;

namespace Remotion.Validation.UnitTests.Attributes.Validation
{
  [TestFixture]
  public class LengthAttributeTest
  {
    private LengthAttribute _attribute;

    [SetUp]
    public void SetUp ()
    {
      _attribute = new LengthAttribute (10, 20);
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
      var result = _attribute.GetPropertyValidators (typeof (Customer).GetProperty ("LastName")).ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0], Is.TypeOf (typeof (LengthValidator)));
      Assert.That (
          result[0].ErrorMessageSource.GetString(),
          Is.EqualTo (
              "'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters."));
      Assert.That (((LengthValidator) result[0]).Min, Is.EqualTo (10));
      Assert.That (((LengthValidator) result[0]).Max, Is.EqualTo (20));
    }

    [Test]
    public void GetPropertyValidator_CustomMessage ()
    {
      _attribute.ErrorMessage = "CustomMessage";

      var result = _attribute.GetPropertyValidators (typeof (Customer).GetProperty ("LastName")).ToArray();

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (result[0].ErrorMessageSource.GetString(), Is.EqualTo ("CustomMessage"));
    }
  }
}