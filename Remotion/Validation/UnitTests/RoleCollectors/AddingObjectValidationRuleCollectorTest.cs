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
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestDomain.Validators;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.RoleCollectors
{
  [TestFixture]
  public class AddingObjectValidationRuleCollectorTest
  {
    [Test]
    public void Create_CollectorTypeDoesNotImplementIValidationRuleCollector_ThrowsArgumentException ()
    {
      Assert.That(
          () => AddingObjectValidationRuleCollector.Create<Customer>(typeof(Customer)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'collectorType' is a 'Remotion.Validation.UnitTests.TestDomain.Customer', "
                  + "which cannot be assigned to type 'Remotion.Validation.IValidationRuleCollector'.",
                  "collectorType"));
    }

    [Test]
    public void Create_CollectorTypeIsNull_ThrowsArgumentNullException ()
    {
      Assert.That(
          () => AddingObjectValidationRuleCollector.Create<Customer>(null),
          Throws.ArgumentNullException
              .With.ArgumentExceptionMessageEqualTo("Value cannot be null.", "collectorType"));
    }

    [Test]
    public void Create_ReturnsAddingObjectValidationRuleCollectorOfTValidatedType ()
    {
      var collectorType = typeof(CustomerValidationRuleCollector1);
      var addingObjectValidationRuleCollector = AddingObjectValidationRuleCollector.Create<Customer>(collectorType);

      Assert.That(addingObjectValidationRuleCollector, Is.InstanceOf<AddingObjectValidationRuleCollector<Customer>>());
      Assert.That(addingObjectValidationRuleCollector.CollectorType, Is.EqualTo(collectorType));
    }
  }
}
