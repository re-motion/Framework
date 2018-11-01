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
using Remotion.Globalization.Implementation;
using Remotion.Globalization.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundEnumerationGlobalizationServiceTest
  {
    private MockRepository _mockRepository;
    private CompoundEnumerationGlobalizationService _service;
    private IEnumerationGlobalizationService _innerService1;
    private IEnumerationGlobalizationService _innerService2;
    private IEnumerationGlobalizationService _innerService3;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _innerService1 = _mockRepository.StrictMock<IEnumerationGlobalizationService>();
      _innerService2 = _mockRepository.StrictMock<IEnumerationGlobalizationService>();
      _innerService3 = _mockRepository.StrictMock<IEnumerationGlobalizationService>();

      _service = new CompoundEnumerationGlobalizationService (new[] { _innerService1, _innerService2, _innerService3 });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_service.EnumerationGlobalizationServices, Is.EqualTo (new[] { _innerService1, _innerService2, _innerService3 }));
    }

    [Test]
    public void TryGetTypeDisplayName_WithInnerServiceHavingResult_ReturnsResult ()
    {
      var enumValue = EnumWithResources.Value1;

      using (_mockRepository.Ordered())
      {
        _innerService1.Expect (
            mock => mock.TryGetEnumerationValueDisplayName (
                Arg.Is (enumValue),
                out Arg<string>.Out (null).Dummy)).Return (false);
        _innerService2.Expect (
            mock => mock.TryGetEnumerationValueDisplayName (
                Arg.Is (enumValue),
                out Arg<string>.Out ("The Name").Dummy)).Return (true);
        _innerService3.Expect (
            mock => mock.TryGetEnumerationValueDisplayName (
                Arg<Enum>.Is.Anything,
                out Arg<string>.Out (null).Dummy))
            .Repeat.Never();
      }
      _mockRepository.ReplayAll();

      string value;
      var result = _service.TryGetEnumerationValueDisplayName (enumValue, out value);

      Assert.That (result, Is.True);
      Assert.That (value, Is.EqualTo ("The Name"));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void TryGetTypeDisplayName_WithoutInnerServiceHavingResult_ReturnsNull ()
    {
      var enumValue = EnumWithResources.Value1;

      using (_mockRepository.Ordered())
      {
        _innerService1.Expect (
            mock => mock.TryGetEnumerationValueDisplayName (
                Arg.Is (enumValue),
                out Arg<string>.Out (null).Dummy)).Return (false);
        _innerService2.Expect (
            mock => mock.TryGetEnumerationValueDisplayName (
                Arg.Is (enumValue),
                out Arg<string>.Out (null).Dummy)).Return (false);
        _innerService3.Expect (
            mock => mock.TryGetEnumerationValueDisplayName (
                Arg.Is (enumValue),
                out Arg<string>.Out (null).Dummy)).Return (false);
      }
      _mockRepository.ReplayAll();

      string value;
      var result = _service.TryGetEnumerationValueDisplayName (enumValue, out value);

      Assert.That (result, Is.False);
      Assert.That (value, Is.Null);

      _mockRepository.VerifyAll();
    }
  }
}