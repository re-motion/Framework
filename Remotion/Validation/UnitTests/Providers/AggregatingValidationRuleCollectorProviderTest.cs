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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Providers;
using Remotion.Validation.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Providers
{
  [TestFixture]
  public class AggregatingValidationRuleCollectorProviderTest
  {
    private IInvolvedTypeProvider _involvedTypeProviderStub;
    private IValidationRuleCollectorProvider _validationRuleCollectorProviderMock1;
    private IValidationRuleCollectorProvider _validationRuleCollectorProviderMock2;
    private IValidationRuleCollectorProvider _validationRuleCollectorProviderMock3;
    private IValidationRuleCollectorProvider _validationRuleCollectorProviderMock4;
    private AggregatingValidationRuleCollectorProvider _validationRuleCollectorProvider;
    private MockRepository _mockRepository;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo1;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo2;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo3;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo4;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo5;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo6;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo7;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo8;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo9;
    private ValidationRuleCollectorInfo _validationRuleCollectorInfo10;

    [SetUp]
    public void SetUp ()
    {
      _involvedTypeProviderStub = MockRepository.GenerateStub<IInvolvedTypeProvider>();

      _mockRepository = new MockRepository();

      _validationRuleCollectorProviderMock1 = _mockRepository.StrictMock<IValidationRuleCollectorProvider>();
      _validationRuleCollectorProviderMock2 = _mockRepository.StrictMock<IValidationRuleCollectorProvider>();
      _validationRuleCollectorProviderMock3 = _mockRepository.StrictMock<IValidationRuleCollectorProvider>();
      _validationRuleCollectorProviderMock4 = _mockRepository.StrictMock<IValidationRuleCollectorProvider>();

      _validationRuleCollectorProvider = new AggregatingValidationRuleCollectorProvider (
          _involvedTypeProviderStub,
          new[]
          {
              _validationRuleCollectorProviderMock1, _validationRuleCollectorProviderMock2,
              _validationRuleCollectorProviderMock3, _validationRuleCollectorProviderMock4
          });

      var validationRuleCollector = MockRepository.GenerateStub<IValidationRuleCollector>();
      _validationRuleCollectorInfo1 = new ValidationRuleCollectorInfo (validationRuleCollector, typeof (AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo2 = new ValidationRuleCollectorInfo (validationRuleCollector, typeof (AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo3 = new ValidationRuleCollectorInfo (validationRuleCollector, typeof (AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo4 = new ValidationRuleCollectorInfo (validationRuleCollector, typeof (AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo5 = new ValidationRuleCollectorInfo (validationRuleCollector, typeof (AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo6 = new ValidationRuleCollectorInfo (validationRuleCollector, typeof (AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo7 = new ValidationRuleCollectorInfo (validationRuleCollector, typeof (AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo8 = new ValidationRuleCollectorInfo (validationRuleCollector, typeof (AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo9 = new ValidationRuleCollectorInfo (validationRuleCollector, typeof (AggregatingValidationRuleCollectorProvider));
      _validationRuleCollectorInfo10 = new ValidationRuleCollectorInfo (validationRuleCollector, typeof (AggregatingValidationRuleCollectorProvider));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_validationRuleCollectorProvider.InvolvedTypeProvider, Is.SameAs (_involvedTypeProviderStub));
      Assert.That (
          new[]
          {
              _validationRuleCollectorProviderMock1, _validationRuleCollectorProviderMock2, _validationRuleCollectorProviderMock3,
              _validationRuleCollectorProviderMock4
          },
          Is.EqualTo (_validationRuleCollectorProvider.ValidationCollectorProviders));
    }

    [Test]
    public void GetValidationRuleCollectors ()
    {
      var typeGroup1 = new[] { typeof (IPerson), typeof(ICollection) };
      var typeGroup2 = new[] { typeof (Person) };
      var typeGroup3 = new[] { typeof (Customer) };
      _involvedTypeProviderStub.Stub (
          stub => stub.GetTypes (Arg<Type>.Is.Equal (typeof (Customer))))
          .Return (new[] { typeGroup1, typeGroup2, typeGroup3 });

      using (_mockRepository.Ordered())
      {
        _validationRuleCollectorProviderMock1
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup1))
            .Return (new[] { new[] { _validationRuleCollectorInfo1 }, new[] { _validationRuleCollectorInfo2 } });
        _validationRuleCollectorProviderMock2
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup1))
            .Return (new[] { new[] { _validationRuleCollectorInfo3 }, new[] { _validationRuleCollectorInfo4 } });

        _validationRuleCollectorProviderMock3
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup1))
            .Return (new[] { new[] { _validationRuleCollectorInfo5 } });
        _validationRuleCollectorProviderMock4
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup1))
            .Return (Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>());

        _validationRuleCollectorProviderMock1
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup2))
            .Return (Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>());
        _validationRuleCollectorProviderMock2
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup2))
            .Return (Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>());

        _validationRuleCollectorProviderMock3
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup2)).Return (new[] { new[] { _validationRuleCollectorInfo6 } });
        _validationRuleCollectorProviderMock4
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup2)).Return (new[] { new[] { _validationRuleCollectorInfo7 } });

        _validationRuleCollectorProviderMock1
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup3))
            .Return (new[] { new[] { _validationRuleCollectorInfo8, _validationRuleCollectorInfo9 }, new[] { _validationRuleCollectorInfo10 } });
        _validationRuleCollectorProviderMock2
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup3))
            .Return (Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>());

        _validationRuleCollectorProviderMock3
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup3)).Return (Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>());
        _validationRuleCollectorProviderMock4
            .Expect (mock => mock.GetValidationRuleCollectors (typeGroup3)).Return (Enumerable.Empty<IEnumerable<ValidationRuleCollectorInfo>>());
      }
      _mockRepository.ReplayAll();

      var result = _validationRuleCollectorProvider.GetValidationRuleCollectors (new[] { typeof (Customer) }).SelectMany (g => g).ToArray();

      _mockRepository.VerifyAll();
      Assert.That (
          new[]
          {
              _validationRuleCollectorInfo1, _validationRuleCollectorInfo2, _validationRuleCollectorInfo3, _validationRuleCollectorInfo4, _validationRuleCollectorInfo5,
              _validationRuleCollectorInfo6, _validationRuleCollectorInfo7, _validationRuleCollectorInfo8, _validationRuleCollectorInfo9, _validationRuleCollectorInfo10
          },
          Is.EqualTo (result));
    }
  }
}