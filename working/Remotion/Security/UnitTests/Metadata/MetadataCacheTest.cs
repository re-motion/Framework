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
using System.Reflection;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Metadata
{

  [TestFixture]
  public class MetadataCacheTest
  {
    // types

    // static members

    // member fields

    private MetadataCache _cache;

    // construction and disposing

    public MetadataCacheTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _cache = new MetadataCache ();
    }

    [Test]
    public void CacheSecurableClassInfos ()
    {
      Type fileType = typeof (File);
      Type paperFileType = typeof (PaperFile);

      SecurableClassInfo fileTypeInfo = new SecurableClassInfo ();
      SecurableClassInfo paperFileTypeInfo = new SecurableClassInfo ();

      Assert.That (_cache.ContainsSecurableClassInfo (fileType), Is.False);
      Assert.That (_cache.GetSecurableClassInfo (fileType), Is.Null);

      _cache.AddSecurableClassInfo (fileType, fileTypeInfo);
      Assert.That (_cache.GetSecurableClassInfo (fileType), Is.SameAs (fileTypeInfo));
      Assert.That (_cache.ContainsSecurableClassInfo (paperFileType), Is.False);
      Assert.That (_cache.GetSecurableClassInfo (paperFileType), Is.Null);

      _cache.AddSecurableClassInfo (paperFileType, paperFileTypeInfo);
      Assert.That (_cache.GetSecurableClassInfo (fileType), Is.SameAs (fileTypeInfo));
      Assert.That (_cache.GetSecurableClassInfo (paperFileType), Is.SameAs (paperFileTypeInfo));
    }

    [Test]
    public void CacheStatePropertyInfos ()
    {
      PropertyInfo fileConfidentialityProperty = typeof (File).GetProperty ("Confidentiality");
      Assert.That (fileConfidentialityProperty, Is.Not.Null);

      PropertyInfo paperFileConfidentialityProperty = typeof (PaperFile).GetProperty ("Confidentiality");
      Assert.That (paperFileConfidentialityProperty, Is.Not.Null);

      PropertyInfo paperFileStateProperty = typeof (PaperFile).GetProperty ("State");
      Assert.That (paperFileStateProperty, Is.Not.Null);

      StatePropertyInfo confidentialityPropertyInfo = new StatePropertyInfo ();
      StatePropertyInfo statePropertyInfo = new StatePropertyInfo ();

      Assert.That (_cache.ContainsStatePropertyInfo (fileConfidentialityProperty), Is.False);
      Assert.That (_cache.GetStatePropertyInfo (fileConfidentialityProperty), Is.Null);

      _cache.AddStatePropertyInfo (fileConfidentialityProperty, confidentialityPropertyInfo);
      Assert.That (_cache.GetStatePropertyInfo (fileConfidentialityProperty), Is.SameAs (confidentialityPropertyInfo));
      Assert.That (_cache.GetStatePropertyInfo (paperFileConfidentialityProperty), Is.SameAs (_cache.GetStatePropertyInfo (fileConfidentialityProperty)));
      Assert.That (_cache.ContainsStatePropertyInfo (paperFileStateProperty), Is.False);
      Assert.That (_cache.GetStatePropertyInfo (paperFileStateProperty), Is.Null);

      _cache.AddStatePropertyInfo (paperFileStateProperty, statePropertyInfo);
      Assert.That (_cache.GetStatePropertyInfo (fileConfidentialityProperty), Is.SameAs (confidentialityPropertyInfo));
      Assert.That (_cache.GetStatePropertyInfo (paperFileStateProperty), Is.SameAs (statePropertyInfo));
    }

    [Test]
    public void CacheEnumValueInfos ()
    {
      Assert.That (_cache.ContainsEnumValueInfo (FileState.New), Is.False);
      Assert.That (_cache.GetEnumValueInfo (FileState.New), Is.Null);

      _cache.AddEnumValueInfo (FileState.New, PropertyStates.FileStateNew);
      Assert.That (_cache.GetEnumValueInfo (FileState.New), Is.SameAs (PropertyStates.FileStateNew));
      Assert.That (_cache.ContainsEnumValueInfo (FileState.Normal), Is.False);
      Assert.That (_cache.GetEnumValueInfo (FileState.Normal), Is.Null);

      _cache.AddEnumValueInfo (FileState.Normal, PropertyStates.FileStateNormal);
      Assert.That (_cache.GetEnumValueInfo (FileState.New), Is.SameAs (PropertyStates.FileStateNew));
      Assert.That (_cache.GetEnumValueInfo (FileState.Normal), Is.SameAs (PropertyStates.FileStateNormal));
    }

    [Test]
    public void CacheAccessTypes ()
    {
      Assert.That (_cache.ContainsAccessType (DomainAccessTypes.Journalize), Is.False);
      Assert.That (_cache.GetAccessType (DomainAccessTypes.Journalize), Is.Null);

      _cache.AddAccessType (DomainAccessTypes.Journalize, AccessTypes.Journalize);
      Assert.That (_cache.GetAccessType (DomainAccessTypes.Journalize), Is.SameAs (AccessTypes.Journalize));
      Assert.That (_cache.ContainsAccessType (DomainAccessTypes.Archive), Is.False);
      Assert.That (_cache.GetAccessType (DomainAccessTypes.Archive), Is.Null);

      _cache.AddAccessType (DomainAccessTypes.Archive, AccessTypes.Archive);
      Assert.That (_cache.GetAccessType (DomainAccessTypes.Journalize), Is.SameAs (AccessTypes.Journalize));
      Assert.That (_cache.GetAccessType (DomainAccessTypes.Archive), Is.SameAs (AccessTypes.Archive));
    }

    [Test]
    public void CacheAbstractRoles ()
    {
      Assert.That (_cache.ContainsAbstractRole (DomainAbstractRoles.Clerk), Is.False);
      Assert.That (_cache.GetAbstractRole (DomainAbstractRoles.Secretary), Is.Null);

      _cache.AddAbstractRole (DomainAbstractRoles.Clerk, AbstractRoles.Clerk);
      Assert.That (_cache.GetAbstractRole (DomainAbstractRoles.Clerk), Is.SameAs (AbstractRoles.Clerk));
      Assert.That (_cache.ContainsAbstractRole (DomainAbstractRoles.Secretary), Is.False);
      Assert.That (_cache.GetAbstractRole (DomainAbstractRoles.Secretary), Is.Null);

      _cache.AddAbstractRole (DomainAbstractRoles.Secretary, AbstractRoles.Secretary);
      Assert.That (_cache.GetAbstractRole (DomainAbstractRoles.Clerk), Is.SameAs (AbstractRoles.Clerk));
      Assert.That (_cache.GetAbstractRole (DomainAbstractRoles.Secretary), Is.SameAs (AbstractRoles.Secretary));
    }

    [Test]
    public void GetCachedSecurableClassInfos ()
    {
      SecurableClassInfo fileTypeInfo = new SecurableClassInfo ();
      SecurableClassInfo paperFileTypeInfo = new SecurableClassInfo ();

      _cache.AddSecurableClassInfo (typeof (File), fileTypeInfo);
      _cache.AddSecurableClassInfo (typeof (PaperFile), paperFileTypeInfo);

      List<SecurableClassInfo> infos = _cache.GetSecurableClassInfos ();

      Assert.That (infos, Is.Not.Null);
      Assert.That (infos.Count, Is.EqualTo (2));
      Assert.That (infos, Has.Member (fileTypeInfo));
      Assert.That (infos, Has.Member (paperFileTypeInfo));
    }

    [Test]
    public void GetCachedPropertyInfos ()
    {
      PropertyInfo confidentialityProperty = typeof (PaperFile).GetProperty ("Confidentiality");
      Assert.That (confidentialityProperty, Is.Not.Null);

      PropertyInfo stateProperty = typeof (PaperFile).GetProperty ("State");
      Assert.That (stateProperty, Is.Not.Null);

      StatePropertyInfo confidentialityPropertyInfo = new StatePropertyInfo ();
      StatePropertyInfo statePropertyInfo = new StatePropertyInfo ();

      _cache.AddStatePropertyInfo (confidentialityProperty, confidentialityPropertyInfo);
      _cache.AddStatePropertyInfo (stateProperty, statePropertyInfo);

      List<StatePropertyInfo> infos = _cache.GetStatePropertyInfos ();

      Assert.That (infos, Is.Not.Null);
      Assert.That (infos.Count, Is.EqualTo (2));
      Assert.That (infos, Has.Member (confidentialityPropertyInfo));
      Assert.That (infos, Has.Member (statePropertyInfo));
    }

    [Test]
    public void GetCachedAccessTypes ()
    {
      _cache.AddAccessType (DomainAccessTypes.Journalize, AccessTypes.Journalize);
      _cache.AddAccessType (DomainAccessTypes.Archive, AccessTypes.Archive);

      List<EnumValueInfo> infos = _cache.GetAccessTypes ();

      Assert.That (infos, Is.Not.Null);
      Assert.That (infos.Count, Is.EqualTo (2));
      Assert.That (infos, Has.Member (AccessTypes.Journalize));
      Assert.That (infos, Has.Member (AccessTypes.Archive));
    }

    [Test]
    public void GetCachedAbstractRoles ()
    {
      _cache.AddAbstractRole (DomainAbstractRoles.Clerk, AbstractRoles.Clerk);
      _cache.AddAbstractRole (DomainAbstractRoles.Secretary, AbstractRoles.Secretary);

      List<EnumValueInfo> infos = _cache.GetAbstractRoles ();

      Assert.That (infos, Is.Not.Null);
      Assert.That (infos.Count, Is.EqualTo (2));
      Assert.That (infos, Has.Member (AbstractRoles.Clerk));
      Assert.That (infos, Has.Member (AbstractRoles.Secretary));
    }
  }
}
