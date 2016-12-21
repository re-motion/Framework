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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundMemberInformationGlobalizationServiceTest
  {
    private MockRepository _mockRepository;
    private CompoundMemberInformationGlobalizationService _service;
    private IMemberInformationGlobalizationService _innerService1;
    private IMemberInformationGlobalizationService _innerService2;
    private IMemberInformationGlobalizationService _innerService3;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _innerService1 = _mockRepository.StrictMock<IMemberInformationGlobalizationService>();
      _innerService2 = _mockRepository.StrictMock<IMemberInformationGlobalizationService>();
      _innerService3 = _mockRepository.StrictMock<IMemberInformationGlobalizationService>();

      _service = new CompoundMemberInformationGlobalizationService (new[] { _innerService1, _innerService2, _innerService3 });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_service.MemberInformationGlobalizationServices, Is.EqualTo (new[] { _innerService1, _innerService2, _innerService3 }));
    }

    [Test]
    public void TryGetTypeDisplayName_WithInnerServiceHavingResult_ReturnsResult ()
    {
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      using (_mockRepository.Ordered())
      {
        _innerService1.Expect (
            mock => mock.TryGetTypeDisplayName (
                Arg.Is (typeInformationStub),
                Arg.Is (typeInformationForResourceResolutionStub),
                out Arg<string>.Out (null).Dummy)).Return (false);
        _innerService2.Expect (
            mock => mock.TryGetTypeDisplayName (
                Arg.Is (typeInformationStub),
                Arg.Is (typeInformationForResourceResolutionStub),
                out Arg<string>.Out ("The Name").Dummy)).Return (true);
        _innerService3.Expect (
            mock => mock.TryGetTypeDisplayName (
                Arg<ITypeInformation>.Is.Anything,
                Arg<ITypeInformation>.Is.Anything,
                out Arg<string>.Out (null).Dummy))
            .Repeat.Never();
      }
      _mockRepository.ReplayAll();

      string value;
      var result = _service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out value);

      Assert.That (result, Is.True);
      Assert.That (value, Is.EqualTo ("The Name"));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void TryGetTypeDisplayName_WithoutInnerServiceHavingResult_ReturnsNull ()
    {
      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      using (_mockRepository.Ordered())
      {
        _innerService1.Expect (
            mock => mock.TryGetTypeDisplayName (
                Arg.Is (typeInformationStub),
                Arg.Is (typeInformationForResourceResolutionStub),
                out Arg<string>.Out (null).Dummy)).Return (false);
        _innerService2.Expect (
            mock => mock.TryGetTypeDisplayName (
                Arg.Is (typeInformationStub),
                Arg.Is (typeInformationForResourceResolutionStub),
                out Arg<string>.Out (null).Dummy)).Return (false);
        _innerService3.Expect (
            mock => mock.TryGetTypeDisplayName (
                Arg.Is (typeInformationStub),
                Arg.Is (typeInformationForResourceResolutionStub),
                out Arg<string>.Out (null).Dummy)).Return (false);
      }
      _mockRepository.ReplayAll();

      string value;
      var result = _service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out value);

      Assert.That (result, Is.False);
      Assert.That (value, Is.Null);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void TryGetPropertyDisplayName_WithInnerServiceHavingResult_ReturnsResult ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      using (_mockRepository.Ordered())
      {
        _innerService1.Expect (
            mock => mock.TryGetPropertyDisplayName (
                Arg.Is (propertyInformationStub),
                Arg.Is (typeInformationForResourceResolutionStub),
                out Arg<string>.Out (null).Dummy)).Return (false);
        _innerService2.Expect (
            mock => mock.TryGetPropertyDisplayName (
                Arg.Is (propertyInformationStub),
                Arg.Is (typeInformationForResourceResolutionStub),
                out Arg<string>.Out ("The Name").Dummy)).Return (true);
        _innerService3.Expect (
            mock => mock.TryGetPropertyDisplayName (
                Arg<IPropertyInformation>.Is.Anything,
                Arg<ITypeInformation>.Is.Anything,
                out Arg<string>.Out (null).Dummy))
            .Repeat.Never();
      }
      _mockRepository.ReplayAll();

      string value;
      var result = _service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out value);

      Assert.That (result, Is.True);
      Assert.That (value, Is.EqualTo ("The Name"));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void TryGetPropertyeDisplayName_WithoutInnerServiceHavingResult_ReturnsNull ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      using (_mockRepository.Ordered())
      {
        _innerService1.Expect (
            mock => mock.TryGetPropertyDisplayName (
                Arg.Is (propertyInformationStub),
                Arg.Is (typeInformationForResourceResolutionStub),
                out Arg<string>.Out (null).Dummy)).Return (false);
        _innerService2.Expect (
            mock => mock.TryGetPropertyDisplayName (
                Arg.Is (propertyInformationStub),
                Arg.Is (typeInformationForResourceResolutionStub),
                out Arg<string>.Out (null).Dummy)).Return (false);
        _innerService3.Expect (
            mock => mock.TryGetPropertyDisplayName (
                Arg.Is (propertyInformationStub),
                Arg.Is (typeInformationForResourceResolutionStub),
                out Arg<string>.Out (null).Dummy)).Return (false);
      }
      _mockRepository.ReplayAll();

      string value;
      var result = _service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out value);

      Assert.That (result, Is.False);
      Assert.That (value, Is.Null);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetAvailablePropertyDisplayNames_ReturnsResultInRightOverriddenOrder ()
    {
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      var dictionary1 = new Dictionary<CultureInfo, string>();
      dictionary1.Add (new CultureInfo ("de-AT"), "AT_Wert1");

      var dictionary2 = new Dictionary<CultureInfo, string>();
      dictionary2.Add (new CultureInfo ("de-AT"), "AT_Wert2");

      var dictionary3 = new Dictionary<CultureInfo, string>();
      dictionary3.Add (new CultureInfo ("de-DE"), "DE_Wert1");
      dictionary3.Add (new CultureInfo ("en-US"), "EN_Wert1");
      dictionary3.Add (new CultureInfo ("de-AT"), "AT_Wert3");

      var expected = new List<KeyValuePair<CultureInfo, string>>();
      expected.Add (new KeyValuePair<CultureInfo, string> (new CultureInfo ("de-AT"), "AT_Wert1"));
      expected.Add (new KeyValuePair<CultureInfo, string> (new CultureInfo ("de-DE"), "DE_Wert1"));
      expected.Add (new KeyValuePair<CultureInfo, string> (new CultureInfo ("en-US"), "EN_Wert1"));

      List<KeyValuePair<CultureInfo, string>> result;
      using (_mockRepository.Ordered())
      {

        var innerService1 = MockRepository.GenerateMock<IMemberInformationGlobalizationService>();
        var innerService2 = MockRepository.GenerateMock<IMemberInformationGlobalizationService>();
        var innerService3 = MockRepository.GenerateMock<IMemberInformationGlobalizationService>();
        var service = new CompoundMemberInformationGlobalizationService (new[] { innerService1, innerService2, innerService3 });

        innerService1.Expect (s => s.GetAvailablePropertyDisplayNames (propertyInformationStub, typeInformationForResourceResolutionStub))
            .Return (dictionary1);
        innerService2.Expect (s => s.GetAvailablePropertyDisplayNames (propertyInformationStub, typeInformationForResourceResolutionStub))
            .Return (dictionary2.AsReadOnly());
        innerService3.Expect (s => s.GetAvailablePropertyDisplayNames (propertyInformationStub, typeInformationForResourceResolutionStub))
            .Return (dictionary3.AsReadOnly());

        result = service.GetAvailablePropertyDisplayNames (propertyInformationStub, typeInformationForResourceResolutionStub).ToList();

        innerService1.AssertWasCalled (s => s.GetAvailablePropertyDisplayNames (propertyInformationStub, typeInformationForResourceResolutionStub));
        innerService2.AssertWasCalled (s => s.GetAvailablePropertyDisplayNames (propertyInformationStub, typeInformationForResourceResolutionStub));
        innerService3.AssertWasCalled (s => s.GetAvailablePropertyDisplayNames (propertyInformationStub, typeInformationForResourceResolutionStub));
      }

      Assert.That (expected.Count, Is.EqualTo (result.Count));
      Assert.That (expected[0], Is.EqualTo (result[0]));
      Assert.That (expected[1], Is.EqualTo (result[1]));
      Assert.That (expected[2], Is.EqualTo (result[2]));
    }
  }
}