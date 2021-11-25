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
using Remotion.Validation.Implementation;
using Remotion.Validation.Providers;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;

namespace Remotion.Validation.UnitTests.Providers
{
  [TestFixture]
  public class ApiBasedValidationRuleCollectorProviderTest
  {
    private Mock<IValidationRuleCollectorReflector> _validationRuleCollectorReflectorMock;
    private ApiBasedValidationRuleCollectorProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _validationRuleCollectorReflectorMock = new Mock<IValidationRuleCollectorReflector>(MockBehavior.Strict);

      _provider = new ApiBasedValidationRuleCollectorProvider(_validationRuleCollectorReflectorMock.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_provider.ValidationRuleCollectorReflector, Is.SameAs(_validationRuleCollectorReflectorMock.Object));
    }

    [Test]
    public void GetValidationRuleCollectors ()
    {
      _validationRuleCollectorReflectorMock.Setup(mock => mock.GetCollectorsForType(typeof (Customer)))
          .Returns(new[] { typeof (CustomerValidationRuleCollector1), typeof (CustomerValidationRuleCollector2) })
          .Verifiable();
      _validationRuleCollectorReflectorMock.Setup(mock => mock.GetCollectorsForType(typeof (Person)))
          .Returns(
              new[]
              { typeof (PersonValidationRuleCollector1), typeof (PersonValidationRuleCollector2) })
          .Verifiable();

      var result = _provider.GetValidationRuleCollectors(new[] { typeof (Customer), typeof (Person) }).SelectMany(g => g).ToArray();

      _validationRuleCollectorReflectorMock.Verify();
      Assert.That(result[0].Collector.GetType(), Is.EqualTo(typeof (CustomerValidationRuleCollector1)));
      Assert.That(result[0].ProviderType, Is.EqualTo(typeof (ApiBasedValidationRuleCollectorProvider)));
      Assert.That(result[1].Collector.GetType(), Is.EqualTo(typeof (CustomerValidationRuleCollector2)));
      Assert.That(result[1].ProviderType, Is.EqualTo(typeof (ApiBasedValidationRuleCollectorProvider)));
      Assert.That(result[2].Collector.GetType(), Is.EqualTo(typeof (PersonValidationRuleCollector1)));
      Assert.That(result[2].ProviderType, Is.EqualTo(typeof (ApiBasedValidationRuleCollectorProvider)));
      Assert.That(result[3].Collector.GetType(), Is.EqualTo(typeof (PersonValidationRuleCollector2)));
      Assert.That(result[3].ProviderType, Is.EqualTo(typeof (ApiBasedValidationRuleCollectorProvider)));
    }
  }
}