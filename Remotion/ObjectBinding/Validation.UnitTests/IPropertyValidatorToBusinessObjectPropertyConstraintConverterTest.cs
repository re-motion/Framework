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
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Validation.UnitTests
{
  [TestFixture]
  public class IPropertyValidatorToBusinessObjectPropertyConstraintConverterTest
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
      var instance = _serviceLocator.GetInstance<IPropertyValidatorToBusinessObjectPropertyConstraintConverter>();

      Assert.That(instance, Is.InstanceOf<CompoundPropertyValidatorToBusinessObjectPropertyConstraintConverter>());

      var compound = (CompoundPropertyValidatorToBusinessObjectPropertyConstraintConverter)instance;
      Assert.That(
          compound.Converters.Select(c => c.GetType()),
          Is.EquivalentTo(new[] { typeof(PropertyValidatorToBusinessObjectPropertyConstraintConverter) }));

    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IPropertyValidatorToBusinessObjectPropertyConstraintConverter>();
      var instance2 = _serviceLocator.GetInstance<IPropertyValidatorToBusinessObjectPropertyConstraintConverter>();

      Assert.That(instance1, Is.InstanceOf<CompoundPropertyValidatorToBusinessObjectPropertyConstraintConverter>());
      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
