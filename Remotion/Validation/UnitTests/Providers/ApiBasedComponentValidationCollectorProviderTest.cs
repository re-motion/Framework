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
using Remotion.Validation.Implementation;
using Remotion.Validation.Providers;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Providers
{
  [TestFixture]
  public class ApiBasedComponentValidationCollectorProviderTest
  {
    private IValidationCollectorReflector _validationCollectorReflectorMock;
    private ApiBasedComponentValidationCollectorProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _validationCollectorReflectorMock = MockRepository.GenerateStrictMock<IValidationCollectorReflector>();

      _provider = new ApiBasedComponentValidationCollectorProvider (_validationCollectorReflectorMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_provider.ValidationCollectorReflector, Is.SameAs (_validationCollectorReflectorMock));
    }

    [Test]
    public void GetValidationCollectors ()
    {
      _validationCollectorReflectorMock.Expect (mock => mock.GetCollectorsForType (typeof (Customer)))
          .Return (new[] { typeof (CustomerValidationCollector1), typeof (CustomerValidationCollector2) });
      _validationCollectorReflectorMock.Expect (mock => mock.GetCollectorsForType (typeof (Person)))
          .Return (
              new[]
              { typeof (PersonValidationCollector1), typeof (IPersonValidationCollector2) });

      var result = _provider.GetValidationCollectors (new[] { typeof (Customer), typeof (Person) }).SelectMany (g => g).ToArray();

      _validationCollectorReflectorMock.VerifyAllExpectations();
      Assert.That (result[0].Collector.GetType(), Is.EqualTo (typeof (CustomerValidationCollector1)));
      Assert.That (result[0].ProviderType, Is.EqualTo (typeof (ApiBasedComponentValidationCollectorProvider)));
      Assert.That (result[1].Collector.GetType(), Is.EqualTo (typeof (CustomerValidationCollector2)));
      Assert.That (result[1].ProviderType, Is.EqualTo (typeof (ApiBasedComponentValidationCollectorProvider)));
      Assert.That (result[2].Collector.GetType (), Is.EqualTo (typeof (PersonValidationCollector1)));
      Assert.That (result[2].ProviderType, Is.EqualTo (typeof (ApiBasedComponentValidationCollectorProvider)));
      Assert.That (result[3].Collector.GetType (), Is.EqualTo (typeof (IPersonValidationCollector2)));
      Assert.That (result[3].ProviderType, Is.EqualTo (typeof (ApiBasedComponentValidationCollectorProvider)));
    }
  }
}