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
using Remotion.Validation.Implementation;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundValidationTypeFilterTest
  {
    private IValidationTypeFilter _validationTypeFilter1;
    private IValidationTypeFilter _validationTypeFilter2;
    private CompoundValidationTypeFilter _compoundValidationTypeFilter;
    private Type _type;

    [SetUp]
    public void SetUp ()
    {
      _type = typeof (string);

      _validationTypeFilter1 = MockRepository.GenerateStub<IValidationTypeFilter> ();
      _validationTypeFilter2 = MockRepository.GenerateStub<IValidationTypeFilter> ();

      _compoundValidationTypeFilter = new CompoundValidationTypeFilter (new[] { _validationTypeFilter1, _validationTypeFilter2 });
    }

    [Test]
    public void IsValid_AllValid ()
    {
      _validationTypeFilter1.Stub (stub => stub.IsValidatableType (_type)).Return (true);
      _validationTypeFilter2.Stub (stub => stub.IsValidatableType (_type)).Return (true);

      var result = _compoundValidationTypeFilter.IsValidatableType (_type);

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsValid_NoneValid ()
    {
      _validationTypeFilter1.Stub (stub => stub.IsValidatableType (_type)).Return (false);
      _validationTypeFilter2.Stub (stub => stub.IsValidatableType (_type)).Return (false);

      var result = _compoundValidationTypeFilter.IsValidatableType (_type);

      Assert.That (result, Is.False);
    }

    [Test]
    public void IsValid_OneValid ()
    {
      _validationTypeFilter1.Stub (stub => stub.IsValidatableType (_type)).Return (false);
      _validationTypeFilter2.Stub (stub => stub.IsValidatableType (_type)).Return (true);

      var result = _compoundValidationTypeFilter.IsValidatableType (_type);

      Assert.That (result, Is.False);
    }
  }
}