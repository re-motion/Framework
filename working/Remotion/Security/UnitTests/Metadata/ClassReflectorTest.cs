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
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.SampleDomain;
using Remotion.Security.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Metadata
{

  [TestFixture]
  public class ClassReflectorTest
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IStatePropertyReflector _statePropertyReflectorMock;
    private IAccessTypeReflector _accessTypeReflectorMock;
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
      _mocks = new MockRepository ();
      _statePropertyReflectorMock = _mocks.StrictMock<IStatePropertyReflector> ();
      _accessTypeReflectorMock = _mocks.StrictMock<IAccessTypeReflector> ();
      _classReflector = new ClassReflector (_statePropertyReflectorMock, _accessTypeReflectorMock);
      _cache = new MetadataCache ();

      _confidentialityProperty = new StatePropertyInfo ();
      _confidentialityProperty.ID = Guid.NewGuid ().ToString ();
      _confidentialityProperty.Name = "Confidentiality";

      _stateProperty = new StatePropertyInfo ();
      _stateProperty.ID = Guid.NewGuid().ToString();
      _stateProperty.Name = "State";
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOf (typeof (IClassReflector), _classReflector);
      Assert.That (_classReflector.StatePropertyReflector, Is.SameAs (_statePropertyReflectorMock));
      Assert.That (_classReflector.AccessTypeReflector, Is.SameAs (_accessTypeReflectorMock));
    }

    [Test]
    public void GetMetadata ()
    {
      List<EnumValueInfo> fileAccessTypes = new List<EnumValueInfo> ();
      fileAccessTypes.Add (AccessTypes.Read);
      fileAccessTypes.Add (AccessTypes.Write);
      fileAccessTypes.Add (AccessTypes.Journalize);

      List<EnumValueInfo> paperFileAccessTypes = new List<EnumValueInfo> ();
      paperFileAccessTypes.Add (AccessTypes.Read);
      paperFileAccessTypes.Add (AccessTypes.Write);
      paperFileAccessTypes.Add (AccessTypes.Journalize);
      paperFileAccessTypes.Add (AccessTypes.Archive);

      Expect.Call (_statePropertyReflectorMock.GetMetadata (typeof (PaperFile).GetProperty ("Confidentiality"), _cache)).Return (_confidentialityProperty);
      Expect.Call (_statePropertyReflectorMock.GetMetadata (typeof (PaperFile).GetProperty ("State"), _cache)).Return (_stateProperty);
      Expect.Call (_statePropertyReflectorMock.GetMetadata (typeof (File).GetProperty ("Confidentiality"), _cache)).Return (_confidentialityProperty);
      Expect.Call (_accessTypeReflectorMock.GetAccessTypesFromType (typeof (File), _cache)).Return (fileAccessTypes);
      Expect.Call (_accessTypeReflectorMock.GetAccessTypesFromType(typeof (PaperFile), _cache)).Return (paperFileAccessTypes);
      _mocks.ReplayAll ();

      SecurableClassInfo info = _classReflector.GetMetadata (typeof (PaperFile), _cache);

      _mocks.VerifyAll ();

      Assert.That (info, Is.Not.Null);
      Assert.That (info.Name, Is.EqualTo ("Remotion.Security.UnitTests.TestDomain.PaperFile, Remotion.Security.UnitTests.TestDomain"));
      Assert.That (info.ID, Is.EqualTo ("00000000-0000-0000-0002-000000000000"));

      Assert.That (info.DerivedClasses.Count, Is.EqualTo (0));
      Assert.That (info.BaseClass, Is.Not.Null);
      Assert.That (info.BaseClass.Name, Is.EqualTo ("Remotion.Security.UnitTests.TestDomain.File, Remotion.Security.UnitTests.TestDomain"));
      Assert.That (info.BaseClass.DerivedClasses.Count, Is.EqualTo (1));
      Assert.That (info.BaseClass.DerivedClasses, Has.Member (info));

      Assert.That (info.Properties.Count, Is.EqualTo (2));
      Assert.That (info.Properties, Has.Member (_confidentialityProperty));
      Assert.That (info.Properties, Has.Member (_stateProperty));

      Assert.That (info.AccessTypes.Count, Is.EqualTo (4));
      foreach (EnumValueInfo accessType in paperFileAccessTypes)
        Assert.That (info.AccessTypes, Has.Member (accessType));
    }

    [Test]
    public void GetMetadataFromCache ()
    {
      ClassReflector reflector = new ClassReflector ();
      SecurableClassInfo paperFileInfo = reflector.GetMetadata (typeof (PaperFile), _cache);

      Assert.That (paperFileInfo, Is.Not.Null);
      Assert.That (_cache.GetSecurableClassInfo (typeof (PaperFile)), Is.EqualTo (paperFileInfo));

      SecurableClassInfo fileInfo = _cache.GetSecurableClassInfo (typeof (File));
      Assert.That (fileInfo, Is.Not.Null);
      Assert.That (fileInfo.Name, Is.EqualTo ("Remotion.Security.UnitTests.TestDomain.File, Remotion.Security.UnitTests.TestDomain"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'type' is a 'Remotion.Security.UnitTests.TestDomain.Role', which cannot be assigned to type 'Remotion.Security.ISecurableObject'."
        + "\r\nParameter name: type")]
    public void GetMetadataWithInvalidType ()
    {
      new ClassReflector().GetMetadata (typeof (Role), _cache);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Value types are not supported.\r\nParameter name: type")]
    public void GetMetadataWithInvalidValueType ()
    {
      new ClassReflector ().GetMetadata (typeof (TestValueType), _cache);
    }
  }
}
