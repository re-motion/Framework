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
using System.Reflection;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.SampleDomain;
using Remotion.Security.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Metadata
{
  [TestFixture]
  public class LocalizingMetadataConverterTest
  {
    private MockRepository _mocks;
    private IMetadataLocalizationConverter _localizationConverter;
    private IMetadataConverter _metadataConverter;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _localizationConverter = _mocks.StrictMock<IMetadataLocalizationConverter> ();
      _metadataConverter = _mocks.StrictMock<IMetadataConverter> ();
    }

    [Test]
    public void ConvertAndSave_OnlyOneLocalization ()
    {
      CultureInfo[] cultures = CreateCultureInfos ("de");
      LocalizingMetadataConverter converter = new LocalizingMetadataConverter (_localizationConverter, cultures);
      string filename = "metadata.xml";

      _localizationConverter.ConvertAndSave (new LocalizedName[0], cultures[0], filename);
      _mocks.ReplayAll ();

      converter.ConvertAndSave (new MetadataCache (), filename);

      _mocks.VerifyAll ();
    }

    [Test]
    public void ConvertAndSave_TwoLocalizations ()
    {
      CultureInfo[] cultures = CreateCultureInfos ("de", "en");
      LocalizingMetadataConverter converter = new LocalizingMetadataConverter (_localizationConverter, cultures);
      string filename = "metadata.xml";

      _localizationConverter.ConvertAndSave (new LocalizedName[0], cultures[0], filename);
      _localizationConverter.ConvertAndSave (new LocalizedName[0], cultures[1], filename);
      _mocks.ReplayAll ();

      converter.ConvertAndSave (new MetadataCache (), filename);

      _mocks.VerifyAll ();
    }

    [Test]
    public void ConvertAndSave_LocalizedClassName ()
    {
      CultureInfo[] cultures = CreateCultureInfos ("de", "en");
      LocalizingMetadataConverter converter = new LocalizingMetadataConverter (_localizationConverter, cultures);
      string filename = "metadata.xml";
      MetadataCache cache = new MetadataCache ();
      SecurableClassInfo classInfo = AddSecurableClassInfo (cache, typeof (SecurableObject), "21df1db3-affd-4c1a-b14e-340c1405bd69");

      LocalizedName expectedGermanName = CreateLocalizedName (classInfo);
      _localizationConverter.ConvertAndSave (new LocalizedName[] { expectedGermanName }, cultures[0], filename);
      LocalizedName expectedEnglishName = CreateLocalizedName (classInfo);
      _localizationConverter.ConvertAndSave (new LocalizedName[] { expectedEnglishName }, cultures[1], filename);
      _mocks.ReplayAll ();

      converter.ConvertAndSave (cache, filename);

      _mocks.VerifyAll ();
    }

    [Test]
    public void ConvertAndSave_LocalizedAbstractRoleName ()
    {
      CultureInfo[] cultures = CreateCultureInfos ("de", "en");
      LocalizingMetadataConverter converter = new LocalizingMetadataConverter (_localizationConverter, cultures);
      string filename = "metadata.xml";
      MetadataCache cache = new MetadataCache ();
      EnumValueInfo abstractRoleInfo = AddAbstractRoleInfo (cache, TestAbstractRoles.Developer, "6aba5c1a-cf54-4a12-9523-204fe0b56fd5", "Developer", "Remotion.Security.UnitTests.Core.SampleDomain.TestAbstractRoles", 0);

      LocalizedName expectedGermanName = CreateLocalizedName (abstractRoleInfo);
      _localizationConverter.ConvertAndSave (new LocalizedName[] { expectedGermanName }, cultures[0], filename);
      LocalizedName expectedEnglishName = CreateLocalizedName (abstractRoleInfo);
      _localizationConverter.ConvertAndSave (new LocalizedName[] { expectedEnglishName }, cultures[1], filename);
      _mocks.ReplayAll ();

      converter.ConvertAndSave (cache, filename);

      _mocks.VerifyAll ();
    }

    [Test]
    public void ConvertAndSave_LocalizedAccessTypeName ()
    {
      CultureInfo[] cultures = CreateCultureInfos ("de", "en");
      LocalizingMetadataConverter converter = new LocalizingMetadataConverter (_localizationConverter, cultures);
      string filename = "metadata.xml";
      MetadataCache cache = new MetadataCache ();
      EnumValueInfo accessTypeInfo = AddAccessTypeInfo (cache, TestAccessTypes.First, "31ba143f-bef0-442b-a6dd-3b36a390e639", "First", "Remotion.Security.UnitTests.Core.SampleDomain.TestAccessTypes", 1);

      LocalizedName expectedGermanName = CreateLocalizedName (accessTypeInfo);
      _localizationConverter.ConvertAndSave (new LocalizedName[] { expectedGermanName }, cultures[0], filename);
      LocalizedName expectedEnglishName = CreateLocalizedName (accessTypeInfo);
      _localizationConverter.ConvertAndSave (new LocalizedName[] { expectedEnglishName }, cultures[1], filename);
      _mocks.ReplayAll ();

      converter.ConvertAndSave (cache, filename);

      _mocks.VerifyAll ();
    }

    [Test]
    public void ConvertAndSave_LocalizedStatePropertyName ()
    {
      CultureInfo[] cultures = CreateCultureInfos ("de", "en");
      LocalizingMetadataConverter converter = new LocalizingMetadataConverter (_localizationConverter, cultures);
      string filename = "metadata.xml";
      MetadataCache cache = new MetadataCache ();
      StatePropertyInfo propertyInfo = AddStatePropertyInfo (cache, typeof (PaperFile), "State", "00000000-0000-0000-0002-000000000001", new List<EnumValueInfo> ());

      LocalizedName expectedGermanName = CreateLocalizedName (propertyInfo);
      _localizationConverter.ConvertAndSave (new LocalizedName[] { expectedGermanName }, cultures[0], filename);
      LocalizedName expectedEnglishName = CreateLocalizedName (propertyInfo);
      _localizationConverter.ConvertAndSave (new LocalizedName[] { expectedEnglishName }, cultures[1], filename);
      _mocks.ReplayAll ();

      converter.ConvertAndSave (cache, filename);

      _mocks.VerifyAll ();
    }

    [Test]
    public void ConvertAndSave_LocalizedStateName ()
    {
      CultureInfo[] cultures = CreateCultureInfos ("de", "en");
      LocalizingMetadataConverter converter = new LocalizingMetadataConverter (_localizationConverter, cultures);
      string filename = "metadata.xml";
      MetadataCache cache = new MetadataCache ();
      List<EnumValueInfo> states = new List<EnumValueInfo> ();
      states.Add (new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.FileState", "Archived", 2));
      StatePropertyInfo propertyInfo = AddStatePropertyInfo (cache, typeof (PaperFile), "State", "00000000-0000-0000-0002-000000000001", states);

      string stateDescription = propertyInfo.Description + "|Archived";
      LocalizedName[] expectedGermanNames = new LocalizedName[] {
          CreateLocalizedName (propertyInfo),
          CreateLocalizedName (propertyInfo, 0, stateDescription)
        };

      _localizationConverter.ConvertAndSave (expectedGermanNames, cultures[0], filename);

      LocalizedName[] expectedEnglishNames = new LocalizedName[] {
          CreateLocalizedName (propertyInfo),
          CreateLocalizedName (propertyInfo, 0, stateDescription)
        };
      _localizationConverter.ConvertAndSave (expectedEnglishNames, cultures[1], filename);
      _mocks.ReplayAll ();

      converter.ConvertAndSave (cache, filename);

      _mocks.VerifyAll ();
    }

    [Test]
    public void ConvertAndSave_IncludingMetadataConverter ()
    {
      CultureInfo[] cultures = CreateCultureInfos ("de", "en");
      LocalizingMetadataConverter converter = new LocalizingMetadataConverter (_localizationConverter, cultures);
      converter.MetadataConverter = _metadataConverter;
      string filename = "metadata.xml";
      MetadataCache cache = new MetadataCache ();
      SecurableClassInfo classInfo = AddSecurableClassInfo (cache, typeof (SecurableObject), "21df1db3-affd-4c1a-b14e-340c1405bd69");

      _metadataConverter.ConvertAndSave (cache, filename);
      LocalizedName expectedGermanName = CreateLocalizedName (classInfo);
      _localizationConverter.ConvertAndSave (new LocalizedName[] { expectedGermanName }, cultures[0], filename);
      LocalizedName expectedEnglishName = CreateLocalizedName (classInfo);
      _localizationConverter.ConvertAndSave (new LocalizedName[] { expectedEnglishName }, cultures[1], filename);
      _mocks.ReplayAll ();

      converter.ConvertAndSave (cache, filename);

      _mocks.VerifyAll ();
    }

    private SecurableClassInfo AddSecurableClassInfo (MetadataCache metadataCache, Type type, string id)
    {
      SecurableClassInfo classInfo = new SecurableClassInfo ();

      classInfo.Name = type.FullName;
      classInfo.ID = id;

      metadataCache.AddSecurableClassInfo (type, classInfo);

      return classInfo;
    }

    private EnumValueInfo AddAbstractRoleInfo (MetadataCache metadataCache, Enum abstractRole, string id, string name, string typeName, int value)
    {
      EnumValueInfo abstractRoleInfo = new EnumValueInfo (typeName, name, value);
      abstractRoleInfo.ID = id;

      metadataCache.AddAbstractRole (abstractRole, abstractRoleInfo);

      return abstractRoleInfo;
    }

    private EnumValueInfo AddAccessTypeInfo (MetadataCache metadataCache, Enum accessType, string id, string name, string typeName, int value)
    {
      EnumValueInfo accessTypeInfo = new EnumValueInfo (typeName, name, value);
      accessTypeInfo.ID = id;

      metadataCache.AddAccessType (accessType, accessTypeInfo);

      return accessTypeInfo;
    }

    private StatePropertyInfo AddStatePropertyInfo (MetadataCache metadataCache, Type type, string propertyName, string id, List<EnumValueInfo> states)
    {
      StatePropertyInfo propertyInfo = new StatePropertyInfo ();
      propertyInfo.ID = id;
      propertyInfo.Name = type.FullName;
      propertyInfo.Values = states;

      PropertyInfo property = type.GetProperty (propertyName);
      metadataCache.AddStatePropertyInfo (property, propertyInfo);

      return propertyInfo;
    }

    private LocalizedName CreateLocalizedName (SecurableClassInfo classInfo)
    {
      return new LocalizedName (classInfo.ID, classInfo.Name, classInfo.Name);
    }

    private LocalizedName CreateLocalizedName (EnumValueInfo enumValueInfo)
    {
      string comment = enumValueInfo.Name + "|" + enumValueInfo.TypeName;
      return new LocalizedName (enumValueInfo.ID, comment, enumValueInfo.Name);
    }

    private LocalizedName CreateLocalizedName (StatePropertyInfo statePropertyInfo)
    {
      return new LocalizedName (statePropertyInfo.ID, statePropertyInfo.Name, statePropertyInfo.Name);
    }

    private LocalizedName CreateLocalizedName (StatePropertyInfo statePropertyInfo, int stateIndex, string text)
    {
      EnumValueInfo stateInfo = statePropertyInfo.Values[stateIndex];
      string id = statePropertyInfo.ID + "|" + stateInfo.Value;
      string name = statePropertyInfo.Name + "|" + stateInfo.Name;

      return new LocalizedName (id, name, text);
    }

    private CultureInfo[] CreateCultureInfos (params string[] cultureNames)
    {
      List<CultureInfo> cultureInfos = new List<CultureInfo> ();

      foreach (string cultureName in cultureNames)
        cultureInfos.Add (new CultureInfo (cultureName));

      return cultureInfos.ToArray ();
    }
  }
}
