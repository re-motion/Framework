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
using Remotion.Collections.Caching;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Security;
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  [MultiLingualResources("Remotion.Data.DomainObjects.Web.Test.Globalization.ClassWithAllDataTypes")]
  [DBTable("TableWithAllDataTypes")]
  [Instantiable]
  [DBStorageGroup]
  public abstract class ClassWithAllDataTypes : BindableDomainObject, ISecurableObject, IDomainObjectSecurityContextFactory, ISupportsGetObject
  {
    // types
    [MultiLingualResources("Remotion.Data.DomainObjects.Web.Test.Globalization.ClassWithAllDataTypes")]
    public enum EnumType
    {
      Value1 = 1,
      Value2 = 2,
      Value3 = 3
    }

    // static members and constants

    public static ClassWithAllDataTypes NewObject ()
    {
      return NewObject<ClassWithAllDataTypes>();
    }

    private SecurityContext _securityContext;

    protected ClassWithAllDataTypes ()
    {
    }

    [StorageClassNone]
    public string[] StringArray
    {
      get { return (DelimitedStringArrayProperty ?? string.Empty).Split(';'); }
      set
      {
        if (value == null)
          DelimitedStringArrayProperty = String.Empty;
        else
          DelimitedStringArrayProperty = String.Join(";", value);
      }
    }

    [DBColumn("DelimitedStringArray")]
    [StringProperty(IsNullable = false, MaximumLength = 1000)]
    protected abstract string DelimitedStringArrayProperty { get; set; }


    [StorageClassNone]
    public string[] NullStringArray
    {
      get
      {
        string delimitedNullStringArray = DelimitedNullStringArrayProperty;

        if (delimitedNullStringArray == null)
          return null;

        return delimitedNullStringArray.Split(';');
      }
      set
      {
        if (value == null)
          DelimitedNullStringArrayProperty = null;
        else
          DelimitedNullStringArrayProperty = String.Join(";", value);
      }
    }

    [DBColumn("DelimitedNullStringArray")]
    [StringProperty(MaximumLength = 1000)]
    protected abstract string DelimitedNullStringArrayProperty { get; set; }

    [StorageClassNone]
    public ObjectID ObjectID
    {
      get { return base.ID; }
    }

    [DBColumn("Boolean")]
    public abstract bool BooleanProperty { get; set; }

    [DBColumn("Byte")]
    public abstract byte ByteProperty { get; set; }

    [DBColumn("Date")]
    public abstract DateOnly DateProperty { get; set; }

    [DBColumn("DateTime")]
    public abstract DateTime DateTimeProperty { get; set; }

    [StorageClassNone]
    public DateTime ReadOnlyDateTimeProperty { get { return DateTimeProperty; } }

    [DBColumn("Decimal")]
    public abstract decimal DecimalProperty { get; set; }

    [DBColumn("Double")]
    public abstract double DoubleProperty { get; set; }

    [DBColumn("Enum")]
    public abstract EnumType EnumProperty { get; set; }

    [DBColumn("ExtensibleEnum")]
    [ExtensibleEnumProperty(IsNullable = false)]
    public abstract Color ExtensibleEnumProperty { get; set; }

    [DBColumn("Guid")]
    public abstract Guid GuidProperty { get; set; }

    [DBColumn("Int16")]
    public abstract short Int16Property { get; set; }

    [DBColumn("Int32")]
    public abstract int Int32Property { get; set; }

    [DBColumn("Int64")]
    public abstract long Int64Property { get; set; }

    [DBColumn("Single")]
    public abstract float SingleProperty { get; set; }

    [DBColumn("String")]
    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string StringProperty { get; set; }

    [DBColumn("StringWithoutMaxLength")]
    [StringProperty(IsNullable = false)]
    public abstract string StringPropertyWithoutMaxLength { get; set; }

    [DBColumn("Binary")]
    [BinaryProperty(IsNullable = false)]
    public abstract byte[] BinaryProperty { get; set; }

    [DBColumn("NaBoolean")]
    public abstract bool? NaBooleanProperty { get; set; }

    [DBColumn("NaByte")]
    public abstract byte? NaByteProperty { get; set; }

    [DBColumn("NaDate")]
    public abstract DateOnly? NaDateProperty { get; set; }

    [DBColumn("NaDateTime")]
    public abstract DateTime? NaDateTimeProperty { get; set; }

    [DBColumn("NaDecimal")]
    public abstract decimal? NaDecimalProperty { get; set; }

    [DBColumn("NaDouble")]
    public abstract double? NaDoubleProperty { get; set; }

    [DBColumn("NaEnum")]
    public abstract EnumType? NaEnumProperty { get; set; }

    [DBColumn("NaGuid")]
    public abstract Guid? NaGuidProperty { get; set; }

    [DBColumn("NaInt16")]
    public abstract short? NaInt16Property { get; set; }

    [DBColumn("NaInt32")]
    public abstract int? NaInt32Property { get; set; }

    [DBColumn("NaInt64")]
    public abstract long? NaInt64Property { get; set; }

    [DBColumn("NaSingle")]
    public abstract float? NaSingleProperty { get; set; }

    [DBColumn("StringWithNullValue")]
    [StringProperty(MaximumLength = 100)]
    public abstract string StringWithNullValueProperty { get; set; }

    [DBColumn("ExtensibleEnumWithNullValue")]
    public abstract Color ExtensibleEnumWithNullValueProperty { get; set; }

    [DBColumn("NaBooleanWithNullValue")]
    public abstract bool? NaBooleanWithNullValueProperty { get; set; }

    [DBColumn("NaByteWithNullValue")]
    public abstract byte? NaByteWithNullValueProperty { get; set; }

    [DBColumn("NaDateWithNullValue")]
    public abstract DateOnly? NaDateWithNullValueProperty { get; set; }

    [DBColumn("NaDateTimeWithNullValue")]
    public abstract DateTime? NaDateTimeWithNullValueProperty { get; set; }

    [DBColumn("NaDecimalWithNullValue")]
    public abstract decimal? NaDecimalWithNullValueProperty { get; set; }

    [DBColumn("NaDoubleWithNullValue")]
    public abstract double? NaDoubleWithNullValueProperty { get; set; }

    [DBColumn("NaEnumWithNullValue")]
    public abstract EnumType? NaEnumWithNullValueProperty { get; set; }

    [DBColumn("NaGuidWithNullValue")]
    public abstract Guid? NaGuidWithNullValueProperty { get; set; }

    [DBColumn("NaInt16WithNullValue")]
    public abstract short? NaInt16WithNullValueProperty { get; set; }

    [DBColumn("NaInt32WithNullValue")]
    public abstract int? NaInt32WithNullValueProperty { get; set; }

    [DBColumn("NaInt64WithNullValue")]
    public abstract long? NaInt64WithNullValueProperty { get; set; }

    [DBColumn("NaSingleWithNullValue")]
    public abstract float? NaSingleWithNullValueProperty { get; set; }

    [DBColumn("NullableBinary")]
    [BinaryProperty(MaximumLength = 1000)]
    public abstract byte[] NullableBinaryProperty { get; set; }

    [DBColumn("TableForRelationTestMandatory")]
    [DBBidirectionalRelation("ClassesWithAllDataTypesMandatoryNavigateOnly")]
    [Mandatory]
    public abstract ClassForRelationTest ClassForRelationTestMandatory { get; set; }

    [DBColumn("TableForRelationTestOptional")]
    [DBBidirectionalRelation("ClassesWithAllDataTypesOptionalNavigateOnly")]
    public abstract ClassForRelationTest ClassForRelationTestOptional { get; set; }

    [ObjectBinding(ReadOnly = true)]
    [DBBidirectionalRelation("ClassWithAllDataTypesMandatory")]
    [Mandatory]
    public abstract ObjectList<ClassForRelationTest> ClassesForRelationTestMandatoryNavigateOnly { get; }

    [ObjectBinding(ReadOnly = true)]
    [DBBidirectionalRelation("ClassWithAllDataTypesOptional")]
    public abstract ObjectList<ClassForRelationTest> ClassesForRelationTestOptionalNavigateOnly { get; }

    public void FillMandatoryProperties ()
    {
      BooleanProperty = true;
      ByteProperty = 8;
      DateProperty = DateOnly.FromDateTime(DateTime.Today);
      DateTimeProperty = DateTime.Now;
      DecimalProperty = 10.3m;
      DoubleProperty = 2.0;
      EnumProperty = EnumType.Value1;
      GuidProperty = Guid.NewGuid();
      Int16Property = 16;
      Int32Property = 32;
      Int64Property = 64;
      SingleProperty = 1.0f;
      StringProperty = "string";
      StringPropertyWithoutMaxLength = "string without max length";
      BinaryProperty = new byte[] { 1, 2, 3 };
      var oppositeObject = ClassForRelationTest.NewObject();
      ClassForRelationTestMandatory = oppositeObject;
      ClassesForRelationTestMandatoryNavigateOnly.Add(oppositeObject);
    }

    IObjectSecurityStrategy ISecurableObject.GetSecurityStrategy ()
    {
      return new DomainObjectSecurityStrategyDecorator(
          ObjectSecurityStrategy.Create(this, InvalidationToken.Create()),
          this,
          RequiredSecurityForStates.NewAndDeleted);
    }

    Type ISecurableObject.GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      if (_securityContext == null)
      {
        _securityContext =
            SecurityContext.Create(
                GetPublicDomainObjectType(),
                null,
                null,
                ClassForRelationTestMandatory.Name.Substring(0, ClassForRelationTestMandatory.Name.Length - 1),
                new Dictionary<string, EnumWrapper>(),
                new EnumWrapper[0]);
      }
      return _securityContext;
    }

    bool IDomainObjectSecurityContextFactory.IsNew
    {
      get { return State.IsNew; }
    }

    bool IDomainObjectSecurityContextFactory.IsDeleted
    {
      get { return State.IsDeleted; }
    }
  }
}
