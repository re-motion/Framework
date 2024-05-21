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
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Metadata
{

  [TestFixture]
  public class StatePropertyReflectorTest
  {
    // types

    // static members

    // member fields

    private Mock<IEnumerationReflector> _enumeratedTypeReflectorMock;
    private StatePropertyReflector _statePropertyReflector;
    private MetadataCache _cache;

    // construction and disposing

    public StatePropertyReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _enumeratedTypeReflectorMock = new Mock<IEnumerationReflector>(MockBehavior.Strict);
      _statePropertyReflector = new StatePropertyReflector(_enumeratedTypeReflectorMock.Object);
      _cache = new MetadataCache();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_statePropertyReflector, Is.InstanceOf(typeof(IStatePropertyReflector)));
      Assert.That(_statePropertyReflector.EnumerationTypeReflector, Is.SameAs(_enumeratedTypeReflectorMock.Object));
    }

    [Test]
    public void GetMetadata ()
    {
      Dictionary<Enum, EnumValueInfo> values = new Dictionary<Enum, EnumValueInfo>();
      values.Add(Confidentiality.Normal, PropertyStates.ConfidentialityNormal);
      values.Add(Confidentiality.Confidential, PropertyStates.ConfidentialityConfidential);
      values.Add(Confidentiality.Private, PropertyStates.ConfidentialityPrivate);

      _enumeratedTypeReflectorMock.Setup(_ => _.GetValues(typeof(Confidentiality), _cache)).Returns(values).Verifiable();

      StatePropertyInfo info = _statePropertyReflector.GetMetadata(typeof(PaperFile).GetProperty("Confidentiality"), _cache);

      _enumeratedTypeReflectorMock.Verify();

      Assert.That(info, Is.Not.Null);
      Assert.That(info.Name, Is.EqualTo("Confidentiality"));
      Assert.That(info.ID, Is.EqualTo("00000000-0000-0000-0001-000000000001"));

      Assert.That(info.Values, Is.Not.Null);
      Assert.That(info.Values.Count, Is.EqualTo(3));
      Assert.That(info.Values, Has.Member(PropertyStates.ConfidentialityNormal));
      Assert.That(info.Values, Has.Member(PropertyStates.ConfidentialityPrivate));
      Assert.That(info.Values, Has.Member(PropertyStates.ConfidentialityConfidential));
    }

    [Test]
    public void GetMetadataFromCache ()
    {
      StatePropertyReflector reflector = new StatePropertyReflector();
      reflector.GetMetadata(typeof(PaperFile).GetProperty("Confidentiality"), _cache);
      reflector.GetMetadata(typeof(File).GetProperty("Confidentiality"), _cache);

      StatePropertyInfo paperFileConfidentialityInfo = _cache.GetStatePropertyInfo(typeof(PaperFile).GetProperty("Confidentiality"));
      Assert.That(paperFileConfidentialityInfo, Is.Not.Null);
      Assert.That(paperFileConfidentialityInfo.Name, Is.EqualTo("Confidentiality"));
      Assert.That(_cache.GetStatePropertyInfo(typeof(File).GetProperty("Confidentiality")), Is.SameAs(paperFileConfidentialityInfo));
    }

    [Test]
    public void GetMetadataWithInvalidType ()
    {
      Assert.That(
          () => new StatePropertyReflector().GetMetadata(typeof(PaperFile).GetProperty("ID"), _cache),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The type of the property 'ID' in type 'Remotion.Security.UnitTests.TestDomain.File' is not an enumerated type.", "property"));
    }

    [Test]
    public void GetMetadataWithInvalidEnum ()
    {
      Assert.That(
          () => new StatePropertyReflector().GetMetadata(typeof(PaperFile).GetProperty("SimpleEnum"), _cache),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The type of the property 'SimpleEnum' in type 'Remotion.Security.UnitTests.TestDomain.File' does not have the Remotion.Security.SecurityStateAttribute applied.",
                  "property"));
    }
  }
}
