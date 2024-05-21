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
using Remotion.Security.UnitTests.SampleDomain;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Metadata
{

  [TestFixture]
  public class ClassReflectorTest
  {
    // types

    // static members

    // member fields

    private Mock<IStatePropertyReflector> _statePropertyReflectorMock;
    private Mock<IAccessTypeReflector> _accessTypeReflectorMock;
    private ClassReflector _classReflector;
    private MetadataCache _cache;
    private StatePropertyInfo _confidentialityProperty;
    private StatePropertyInfo _stateProperty;

    // construction and disposing

    public ClassReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _statePropertyReflectorMock = new Mock<IStatePropertyReflector>(MockBehavior.Strict);
      _accessTypeReflectorMock = new Mock<IAccessTypeReflector>(MockBehavior.Strict);
      _classReflector = new ClassReflector(_statePropertyReflectorMock.Object, _accessTypeReflectorMock.Object);
      _cache = new MetadataCache();

      _confidentialityProperty = new StatePropertyInfo();
      _confidentialityProperty.ID = Guid.NewGuid().ToString();
      _confidentialityProperty.Name = "Confidentiality";

      _stateProperty = new StatePropertyInfo();
      _stateProperty.ID = Guid.NewGuid().ToString();
      _stateProperty.Name = "State";
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_classReflector, Is.InstanceOf(typeof(IClassReflector)));
      Assert.That(_classReflector.StatePropertyReflector, Is.SameAs(_statePropertyReflectorMock.Object));
      Assert.That(_classReflector.AccessTypeReflector, Is.SameAs(_accessTypeReflectorMock.Object));
    }

    [Test]
    public void GetMetadata ()
    {
      List<EnumValueInfo> fileAccessTypes = new List<EnumValueInfo>();
      fileAccessTypes.Add(AccessTypes.Read);
      fileAccessTypes.Add(AccessTypes.Write);
      fileAccessTypes.Add(AccessTypes.Journalize);

      List<EnumValueInfo> paperFileAccessTypes = new List<EnumValueInfo>();
      paperFileAccessTypes.Add(AccessTypes.Read);
      paperFileAccessTypes.Add(AccessTypes.Write);
      paperFileAccessTypes.Add(AccessTypes.Journalize);
      paperFileAccessTypes.Add(AccessTypes.Archive);

      _statePropertyReflectorMock.Setup(_ => _.GetMetadata(typeof(PaperFile).GetProperty("Confidentiality"), _cache)).Returns(_confidentialityProperty).Verifiable();
      _statePropertyReflectorMock.Setup(_ => _.GetMetadata(typeof(PaperFile).GetProperty("State"), _cache)).Returns(_stateProperty).Verifiable();
      _statePropertyReflectorMock.Setup(_ => _.GetMetadata(typeof(File).GetProperty("Confidentiality"), _cache)).Returns(_confidentialityProperty).Verifiable();
      _accessTypeReflectorMock.Setup(_ => _.GetAccessTypesFromType(typeof(File), _cache)).Returns(fileAccessTypes).Verifiable();
      _accessTypeReflectorMock.Setup(_ => _.GetAccessTypesFromType(typeof(PaperFile), _cache)).Returns(paperFileAccessTypes).Verifiable();

      SecurableClassInfo info = _classReflector.GetMetadata(typeof(PaperFile), _cache);

      _statePropertyReflectorMock.Verify();
      _accessTypeReflectorMock.Verify();

      Assert.That(info, Is.Not.Null);
      Assert.That(info.Name, Is.EqualTo("Remotion.Security.UnitTests.TestDomain.PaperFile, Remotion.Security.UnitTests.TestDomain"));
      Assert.That(info.ID, Is.EqualTo("00000000-0000-0000-0002-000000000000"));

      Assert.That(info.DerivedClasses.Count, Is.EqualTo(0));
      Assert.That(info.BaseClass, Is.Not.Null);
      Assert.That(info.BaseClass.Name, Is.EqualTo("Remotion.Security.UnitTests.TestDomain.File, Remotion.Security.UnitTests.TestDomain"));
      Assert.That(info.BaseClass.DerivedClasses.Count, Is.EqualTo(1));
      Assert.That(info.BaseClass.DerivedClasses, Has.Member(info));

      Assert.That(info.Properties.Count, Is.EqualTo(2));
      Assert.That(info.Properties, Has.Member(_confidentialityProperty));
      Assert.That(info.Properties, Has.Member(_stateProperty));

      Assert.That(info.AccessTypes.Count, Is.EqualTo(4));
      foreach (EnumValueInfo accessType in paperFileAccessTypes)
        Assert.That(info.AccessTypes, Has.Member(accessType));
    }

    [Test]
    public void GetMetadataFromCache ()
    {
      ClassReflector reflector = new ClassReflector();
      SecurableClassInfo paperFileInfo = reflector.GetMetadata(typeof(PaperFile), _cache);

      Assert.That(paperFileInfo, Is.Not.Null);
      Assert.That(_cache.GetSecurableClassInfo(typeof(PaperFile)), Is.EqualTo(paperFileInfo));

      SecurableClassInfo fileInfo = _cache.GetSecurableClassInfo(typeof(File));
      Assert.That(fileInfo, Is.Not.Null);
      Assert.That(fileInfo.Name, Is.EqualTo("Remotion.Security.UnitTests.TestDomain.File, Remotion.Security.UnitTests.TestDomain"));
    }

    [Test]
    public void GetMetadataWithInvalidType ()
    {
      Assert.That(
          () => new ClassReflector().GetMetadata(typeof(Role), _cache),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'type' is a 'Remotion.Security.UnitTests.TestDomain.Role', which cannot be assigned to type 'Remotion.Security.ISecurableObject'.",
                  "type"));
    }

    [Test]
    public void GetMetadataWithInvalidValueType ()
    {
      Assert.That(
          () => new ClassReflector().GetMetadata(typeof(TestValueType), _cache),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Value types are not supported.", "type"));
    }
  }
}
