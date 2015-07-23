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
  public class AggregatingValidationCollectorProviderTest
  {
    private IInvolvedTypeProvider _involvedTypeProviderStub;
    private IValidationCollectorProvider _validationCollectorProviderMock1;
    private IValidationCollectorProvider _validationCollectorProviderMock2;
    private IValidationCollectorProvider _validationCollectorProviderMock3;
    private IValidationCollectorProvider _validationCollectorProviderMock4;
    private AggregatingValidationCollectorProvider _componentValidationCollectorProvider;
    private MockRepository _mockRepository;
    private ValidationCollectorInfo _validationCollectorInfo1;
    private ValidationCollectorInfo _validationCollectorInfo2;
    private ValidationCollectorInfo _validationCollectorInfo3;
    private ValidationCollectorInfo _validationCollectorInfo4;
    private ValidationCollectorInfo _validationCollectorInfo5;
    private ValidationCollectorInfo _validationCollectorInfo6;
    private ValidationCollectorInfo _validationCollectorInfo7;
    private ValidationCollectorInfo _validationCollectorInfo8;
    private ValidationCollectorInfo _validationCollectorInfo9;
    private ValidationCollectorInfo _validationCollectorInfo10;

    [SetUp]
    public void SetUp ()
    {
      _involvedTypeProviderStub = MockRepository.GenerateStub<IInvolvedTypeProvider>();

      _mockRepository = new MockRepository();

      _validationCollectorProviderMock1 = _mockRepository.StrictMock<IValidationCollectorProvider>();
      _validationCollectorProviderMock2 = _mockRepository.StrictMock<IValidationCollectorProvider>();
      _validationCollectorProviderMock3 = _mockRepository.StrictMock<IValidationCollectorProvider>();
      _validationCollectorProviderMock4 = _mockRepository.StrictMock<IValidationCollectorProvider>();

      _componentValidationCollectorProvider = new AggregatingValidationCollectorProvider (
          _involvedTypeProviderStub,
          new[]
          {
              _validationCollectorProviderMock1, _validationCollectorProviderMock2,
              _validationCollectorProviderMock3, _validationCollectorProviderMock4
          });

      var componentValidationCollector = MockRepository.GenerateStub<IComponentValidationCollector>();
      _validationCollectorInfo1 = new ValidationCollectorInfo (componentValidationCollector, typeof (AggregatingValidationCollectorProvider));
      _validationCollectorInfo2 = new ValidationCollectorInfo (componentValidationCollector, typeof (AggregatingValidationCollectorProvider));
      _validationCollectorInfo3 = new ValidationCollectorInfo (componentValidationCollector, typeof (AggregatingValidationCollectorProvider));
      _validationCollectorInfo4 = new ValidationCollectorInfo (componentValidationCollector, typeof (AggregatingValidationCollectorProvider));
      _validationCollectorInfo5 = new ValidationCollectorInfo (componentValidationCollector, typeof (AggregatingValidationCollectorProvider));
      _validationCollectorInfo6 = new ValidationCollectorInfo (componentValidationCollector, typeof (AggregatingValidationCollectorProvider));
      _validationCollectorInfo7 = new ValidationCollectorInfo (componentValidationCollector, typeof (AggregatingValidationCollectorProvider));
      _validationCollectorInfo8 = new ValidationCollectorInfo (componentValidationCollector, typeof (AggregatingValidationCollectorProvider));
      _validationCollectorInfo9 = new ValidationCollectorInfo (componentValidationCollector, typeof (AggregatingValidationCollectorProvider));
      _validationCollectorInfo10 = new ValidationCollectorInfo (componentValidationCollector, typeof (AggregatingValidationCollectorProvider));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_componentValidationCollectorProvider.InvolvedTypeProvider, Is.SameAs (_involvedTypeProviderStub));
      Assert.That (
          new[]
          {
              _validationCollectorProviderMock1, _validationCollectorProviderMock2, _validationCollectorProviderMock3,
              _validationCollectorProviderMock4
          },
          Is.EqualTo (_componentValidationCollectorProvider.ValidationCollectorProviders));
    }

    [Test]
    public void GetValidationCollectors ()
    {
      var typeGroup1 = new[] { typeof (IPerson), typeof(ICollection) };
      var typeGroup2 = new[] { typeof (Person) };
      var typeGroup3 = new[] { typeof (Customer) };
      _involvedTypeProviderStub.Stub (
          stub => stub.GetTypes (Arg<Type>.Is.Equal (typeof (Customer))))
          .Return (new[] { typeGroup1, typeGroup2, typeGroup3 });

      using (_mockRepository.Ordered())
      {
        _validationCollectorProviderMock1
            .Expect (mock => mock.GetValidationCollectors (typeGroup1))
            .Return (new[] { new[] { _validationCollectorInfo1 }, new[] { _validationCollectorInfo2 } });
        _validationCollectorProviderMock2
            .Expect (mock => mock.GetValidationCollectors (typeGroup1))
            .Return (new[] { new[] { _validationCollectorInfo3 }, new[] { _validationCollectorInfo4 } });

        _validationCollectorProviderMock3
            .Expect (mock => mock.GetValidationCollectors (typeGroup1))
            .Return (new[] { new[] { _validationCollectorInfo5 } });
        _validationCollectorProviderMock4
            .Expect (mock => mock.GetValidationCollectors (typeGroup1))
            .Return (Enumerable.Empty<IEnumerable<ValidationCollectorInfo>>());

        _validationCollectorProviderMock1
            .Expect (mock => mock.GetValidationCollectors (typeGroup2))
            .Return (Enumerable.Empty<IEnumerable<ValidationCollectorInfo>>());
        _validationCollectorProviderMock2
            .Expect (mock => mock.GetValidationCollectors (typeGroup2))
            .Return (Enumerable.Empty<IEnumerable<ValidationCollectorInfo>>());

        _validationCollectorProviderMock3
            .Expect (mock => mock.GetValidationCollectors (typeGroup2)).Return (new[] { new[] { _validationCollectorInfo6 } });
        _validationCollectorProviderMock4
            .Expect (mock => mock.GetValidationCollectors (typeGroup2)).Return (new[] { new[] { _validationCollectorInfo7 } });

        _validationCollectorProviderMock1
            .Expect (mock => mock.GetValidationCollectors (typeGroup3))
            .Return (new[] { new[] { _validationCollectorInfo8, _validationCollectorInfo9 }, new[] { _validationCollectorInfo10 } });
        _validationCollectorProviderMock2
            .Expect (mock => mock.GetValidationCollectors (typeGroup3))
            .Return (Enumerable.Empty<IEnumerable<ValidationCollectorInfo>>());

        _validationCollectorProviderMock3
            .Expect (mock => mock.GetValidationCollectors (typeGroup3)).Return (Enumerable.Empty<IEnumerable<ValidationCollectorInfo>>());
        _validationCollectorProviderMock4
            .Expect (mock => mock.GetValidationCollectors (typeGroup3)).Return (Enumerable.Empty<IEnumerable<ValidationCollectorInfo>>());
      }
      _mockRepository.ReplayAll();

      var result = _componentValidationCollectorProvider.GetValidationCollectors (new[] { typeof (Customer) }).SelectMany (g => g).ToArray();

      _mockRepository.VerifyAll();
      Assert.That (
          new[]
          {
              _validationCollectorInfo1, _validationCollectorInfo2, _validationCollectorInfo3, _validationCollectorInfo4, _validationCollectorInfo5,
              _validationCollectorInfo6, _validationCollectorInfo7, _validationCollectorInfo8, _validationCollectorInfo9, _validationCollectorInfo10
          },
          Is.EqualTo (result));
    }
  }
}