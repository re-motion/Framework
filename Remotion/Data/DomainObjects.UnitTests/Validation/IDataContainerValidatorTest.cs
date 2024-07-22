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
using Remotion.Data.DomainObjects.Validation;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Validation
{
  [TestFixture]
  public class IDataContainerValidatorTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var validator = _serviceLocator.GetInstance<IDataContainerValidator>();

      Assert.That(validator, Is.TypeOf(typeof(CompoundDataContainerValidator)));
      var compoundValidators = ((CompoundDataContainerValidator)validator).Validators;
      Assert.That(
          compoundValidators.Select(v => v.GetType()),
          Is.EqualTo(
              new[]
              {
                  typeof(NotNullablePropertyValidator),
                  typeof(StringPropertyMaxLengthValidator),
                  typeof(BinaryPropertyMaxLengthValidator)
              }));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var validator1 = _serviceLocator.GetInstance<IDataContainerValidator>();
      var validator2 = _serviceLocator.GetInstance<IDataContainerValidator>();

      Assert.That(validator1, Is.SameAs(validator2));
    }
  }
}
