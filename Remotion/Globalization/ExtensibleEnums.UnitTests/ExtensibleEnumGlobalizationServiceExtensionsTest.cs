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
using Remotion.ExtensibleEnums;
using Rhino.Mocks;

namespace Remotion.Globalization.ExtensibleEnums.UnitTests
{
  [TestFixture]
  public class ExtensibleEnumGlobalizationServiceExtensionsTest
  {
    private IExtensibleEnumGlobalizationService _serviceStub;
    private IExtensibleEnum _valueStub;

    [SetUp]
    public void SetUp ()
    {
      _serviceStub = MockRepository.GenerateStub<IExtensibleEnumGlobalizationService>();
      _valueStub = MockRepository.GenerateStub<IExtensibleEnum>();
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithResourceManager_ReturnsLocalizedValue ()
    {
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      Assert.That (_serviceStub.GetExtensibleEnumValueDisplayName (_valueStub), Is.EqualTo ("expected"));
    }

    [Test]
    public void GetEnumerationValueDisplayNameOrDefault_WithResourceManager_ReturnsLocalizedValue ()
    {
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      Assert.That (_serviceStub.GetExtensibleEnumValueDisplayNameOrDefault (_valueStub), Is.EqualTo ("expected"));
    }

    [Test]
    public void ContainsExtensibleEnumerationValueDisplayName_WithResourceManager_ReturnsLocalizedValue ()
    {
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out ("expected").Dummy))
          .Return (true);

      Assert.That (_serviceStub.ContainsExtensibleEnumValueDisplayName (_valueStub), Is.True);
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithoutResourceManager_ReturnsValueName ()
    {
      _valueStub.Stub (_ => _.ValueName).Return ("expected");
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out (null).Dummy))
          .Return (false);

      Assert.That (_serviceStub.GetExtensibleEnumValueDisplayName (_valueStub), Is.EqualTo ("expected"));
    }

    [Test]
    public void GetEnumerationValueDisplayNameOrDefault_WithoutResourceManager_ReturnsNull ()
    {
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out (null).Dummy))
          .Return (false);

      Assert.That (_serviceStub.GetExtensibleEnumValueDisplayNameOrDefault (_valueStub), Is.Null);
    }

    [Test]
    public void ContainsExtensibleEnumerationValueDisplayName_WithoutResourceManager_ReturnsFalse ()
    {
      _serviceStub
          .Stub (_ => _.TryGetExtensibleEnumValueDisplayName (Arg.Is (_valueStub), out Arg<string>.Out (null).Dummy))
          .Return (false);

      Assert.That (_serviceStub.ContainsExtensibleEnumValueDisplayName (_valueStub), Is.False);
    }
  }
}