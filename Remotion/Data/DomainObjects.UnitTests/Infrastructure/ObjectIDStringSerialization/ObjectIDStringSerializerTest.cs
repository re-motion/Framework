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
using Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectIDStringSerialization
{
  [TestFixture]
  public class ObjectIDStringSerializerTest : StandardMappingTest
  {
    [Test]
    public void Serialize_WithStringValue ()
    {
      var id = new ObjectID("Official", "Arthur Dent");
      Assert.That(ObjectIDStringSerializer.Instance.Serialize(id), Is.EqualTo("Official|Arthur Dent|System.String"));
    }

    [Test]
    public void Serialize_WithStringValueContainsDelimiter ()
    {
      var id = new ObjectID("Official", "Arthur" + ObjectIDStringSerializer.Delimiter + "Dent");
      Assert.That(ObjectIDStringSerializer.Instance.Serialize(id), Is.EqualTo("Official|Arthur|Dent|System.String"));
    }

    [Test]
    public void Serialize_WithInt32Value ()
    {
      var id = new ObjectID("Official", 42);
      Assert.That(ObjectIDStringSerializer.Instance.Serialize(id), Is.EqualTo("Official|42|System.Int32"));
    }

    [Test]
    public void Serialize_WithGuidValue ()
    {
      var id = new ObjectID("Order", new Guid("{5D09030C-25C2-4735-B514-46333BD28AC8}"));
      Assert.That(ObjectIDStringSerializer.Instance.Serialize(id), Is.EqualTo("Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid"));
    }

    [Test]
    public void Serialize_WithClassIDContainingDelimiter_FailsInDebugBuild ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithTable(
          id: "Class" + ObjectIDStringSerializer.Delimiter + "ID",
          storageProviderDefinition: new UnitTestStorageProviderStubDefinition("stub"));

      var id = new ObjectID(classDefinition, "Arthur Dent");

#if DEBUG
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Serialize(id),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The class id 'Class|ID' contains the delimiter character ('|'). This is not allowed when serializing the ObjectID.", "objectID"));
#else
      Assert.That(ObjectIDStringSerializer.Instance.Serialize(id), Is.EqualTo("Class|ID|Arthur Dent|System.String"));
#endif
    }

    [Test]
    public void Parse_StringValue ()
    {
      string idString = "Official|Arthur Dent|System.String";
      ObjectID id = ObjectIDStringSerializer.Instance.Parse(idString);

      Assert.That(id.StorageProviderDefinition.Name, Is.EqualTo("UnitTestStorageProviderStub"));
      Assert.That(id.ClassID, Is.EqualTo("Official"));
      Assert.That(id.Value.GetType(), Is.EqualTo(typeof(string)));
      Assert.That(id.Value, Is.EqualTo("Arthur Dent"));
    }

    [Test]
    public void Parse_Int32Value ()
    {
      string idString = "Official|42|System.Int32";
      ObjectID id = ObjectIDStringSerializer.Instance.Parse(idString);

      Assert.That(id.StorageProviderDefinition.Name, Is.EqualTo("UnitTestStorageProviderStub"));
      Assert.That(id.ClassID, Is.EqualTo("Official"));
      Assert.That(id.Value.GetType(), Is.EqualTo(typeof(int)));
      Assert.That(id.Value, Is.EqualTo(42));
    }

    [Test]
    public void Parse_GuidValue ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";
      ObjectID id = ObjectIDStringSerializer.Instance.Parse(idString);

      Assert.That(id.StorageProviderDefinition.Name, Is.EqualTo("TestDomain"));
      Assert.That(id.ClassID, Is.EqualTo("Order"));
      Assert.That(id.Value.GetType(), Is.EqualTo(typeof(Guid)));
      Assert.That(id.Value, Is.EqualTo(new Guid("{5D09030C-25C2-4735-B514-46333BD28AC8}")));
    }

    [Test]
    public void Parse_WithValueContainsDelimiter_ReturnsValueIncludingDelimiter ()
    {
      string idString = "Official|Multi|Value|System.String";
      ObjectID id = ObjectIDStringSerializer.Instance.Parse(idString);

      Assert.That(id.StorageProviderDefinition.Name, Is.EqualTo("UnitTestStorageProviderStub"));
      Assert.That(id.ClassID, Is.EqualTo("Official"));
      Assert.That(id.Value.GetType(), Is.EqualTo(typeof(string)));
      Assert.That(id.Value, Is.EqualTo("Multi|Value"));
    }

    [Test]
    public void Parse_EmptyString_ThrowsFormatException ()
    {
      string idString = "";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID '' is not correctly formatted: it must not be empty."));
    }

    [Test]
    public void Parse_EmptyClassIDAndEmptyValueAndMissingType_ThrowsFormatException ()
    {
      string idString = "|";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID '|' is not correctly formatted: it must have three parts."));
    }

    [Test]
    public void Parse_EmptyClassIDAndEmptyValueAndEmptyType_ThrowsFormatException ()
    {
      string idString = "||";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID '||' is not correctly formatted: the class id must not be empty."));
    }

    [Test]
    public void Parse_MissingClassID_ThrowsFormatException ()
    {
      string idString = "12|System.Int32";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID '12|System.Int32' is not correctly formatted: it must have three parts."));
    }

    [Test]
    public void Parse_EmptyClassID_ThrowsFormatException ()
    {
      string idString = "|12|System.Int32";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID '|12|System.Int32' is not correctly formatted: the class id must not be empty."));
    }

    [Test]
    public void Parse_MissingClassIDAndEmptyValue_ThrowsFormatException ()
    {
      string idString = "|System.Int32";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID '|System.Int32' is not correctly formatted: it must have three parts."));
    }

    [Test]
    public void Parse_EmptyClassIDAndEmptyValue_ThrowsFormatException ()
    {
      string idString = "||System.Int32";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID '||System.Int32' is not correctly formatted: the class id must not be empty."));
    }

    [Test]
    public void Parse_MissingValueAndMissingType_ThrowsFormatException ()
    {
      string idString = "Official";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID 'Official' is not correctly formatted: it must have three parts."));
    }

    [Test]
    public void Parse_EmptyValueAndMissingType_ThrowsFormatException ()
    {
      string idString = "Official|";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID 'Official|' is not correctly formatted: it must have three parts."));
    }

    [Test]
    public void Parse_EmptyValueAndEmptyType_ThrowsFormatException ()
    {
      string idString = "Official||";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID 'Official||' is not correctly formatted: the value must not be empty."));
    }

    [Test]
    public void Parse_EmptyValue_ThrowsFormatException ()
    {
      string idString = "Official||System.String";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID 'Official||System.String' is not correctly formatted: the value must not be empty."));
    }

    [Test]
    public void Parse_MissingType_ThrowsFormatException ()
    {
      string idString = "Official|v";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID 'Official|v' is not correctly formatted: it must have three parts."));
    }

    [Test]
    public void Parse_EmptyType_ThrowsFormatException ()
    {
      string idString = "Official|v|";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("Serialized ObjectID 'Official|v|' is not correctly formatted: the type must not be empty."));
    }

    [Test]
    public void Parse_WithInvalidValueType_ThrowsFormatException ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Double";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo(
              "Serialized ObjectID 'Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Double' is invalid: type 'System.Double' is not supported."));
    }

    [Test]
    public void Parse_WithInvalidValue_ThrowsFormatException ()
    {
      string idString = "Order|12|System.Guid";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo(
              "Serialized ObjectID 'Order|12|System.Guid' is not correctly formatted: value '12' is not in the correct format for type 'System.Guid'."));
    }

    [Test]
    public void Parse_WithInvalidTypeName_ThrowsFormatException ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Goid";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo(
              "Serialized ObjectID 'Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Goid' is invalid: type 'System.Goid' is not supported."));
    }

    [Test]
    public void Parse_WithInvalidClassID_ThrowsFormatException ()
    {
      string idString = "Arder|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo(
              "Serialized ObjectID 'Arder|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid' is invalid: 'Arder' is not a valid class ID."));
    }

    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid, mscorlib", typeof(Guid), "System.Guid, mscorlib")]
    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid, mscorlib, Version=4.0.0.0", typeof(Guid), "System.Guid, mscorlib, Version=4.0.0.0")]
    [TestCase("Official|23|System.Int32, mscorlib", typeof(Int32), "System.Int32, mscorlib")]
    [TestCase("Official|23|System.Int32 , mscorlib , Version=4.0.0.0", typeof(Int32), "System.Int32 , mscorlib , Version=4.0.0.0")]
    [TestCase("Official|test|System.String, mscorlib", typeof(String), "System.String, mscorlib")]
    [TestCase("Official|test| System.String , mscorlib , Version=4.0.0.0", typeof(String), " System.String , mscorlib , Version=4.0.0.0")]
    [Test]
    public void Parse_WithAssemblyNameValidForNetFramwork (string idString, Type type, string typePart)
    {
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo(
              $"Serialized ObjectID '{idString}' is invalid: type '{typePart}' is not supported."));
    }

    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.Goid, mscorlib", "System.Goid, mscorlib")]
    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.guid, mscorlib", "System.guid, mscorlib")]
    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid, badassembly, mscorlib", "System.Guid, badassembly, mscorlib")]
    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid, badassembly, mscorlib, Version=4.0.0.0", "System.Guid, badassembly, mscorlib, Version=4.0.0.0")]
    [TestCase("Official|23|System.Int32, badassembly, mscorlib", "System.Int32, badassembly, mscorlib")]
    [TestCase("Official|23|System.Int32 , badassembly, mscorlib , Version=4.0.0.0", "System.Int32 , badassembly, mscorlib , Version=4.0.0.0")]
    [TestCase("Official|test|System.String, badassembly, mscorlib", "System.String, badassembly, mscorlib")]
    [TestCase("Official|test| System.String , badassembly, mscorlib , Version=4.0.0.0", " System.String , badassembly, mscorlib , Version=4.0.0.0")]
    [Test]
    public void Parse_WithInvalidAssemblyName_ThrowsFormatException (string idString, string typePart)
    {
      Assert.That(
          () => ObjectIDStringSerializer.Instance.Parse(idString),
          Throws.TypeOf<FormatException>().With.Message.EqualTo(
              $"Serialized ObjectID '{idString}' is invalid: type '{typePart}' is not supported."));
    }

    [Test]
    public void TryParse ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.True);
      Assert.That(id.StorageProviderDefinition.Name, Is.EqualTo("TestDomain"));
      Assert.That(id.ClassID, Is.EqualTo("Order"));
      Assert.That(id.Value.GetType(), Is.EqualTo(typeof(Guid)));
      Assert.That(id.Value, Is.EqualTo(new Guid("{5D09030C-25C2-4735-B514-46333BD28AC8}")));
    }

    [Test]
    public void TryParse_EmptyString ()
    {
      string idString = "";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_EmptyClassIDAndEmptyValueAndMissingType_ReturnsFalse ()
    {
      string idString = "|";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_EmptyClassIDAndEmptyValueAndEmptyType_ReturnsFalse ()
    {
      string idString = "||";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_MissingClassID_ReturnsFalse ()
    {
      string idString = "12|System.Int32";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_EmptyClassID_ReturnsFalse ()
    {
      string idString = "|12|System.Int32";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_MissingClassIDAndEmptyValue_ReturnsFalse ()
    {
      string idString = "|System.Int32";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_EmptyClassIDAndEmptyValue_ReturnsFalse ()
    {
      string idString = "||System.Int32";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_MissingValueAndMissingType_ReturnsFalse ()
    {
      string idString = "Official";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_EmptyValueAndMissingType_ReturnsFalse ()
    {
      string idString = "Official|";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_EmptyValueAndEmptyType_ReturnsFalse ()
    {
      string idString = "Official||";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_EmptyValue_ReturnsFalse ()
    {
      string idString = "Official||System.String";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_MissingType_ReturnsFalse ()
    {
      string idString = "Official|v";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_EmptyType_ReturnsFalse ()
    {
      string idString = "Official|v|";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_WithInvalidValueType_ReturnsFalse ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Double";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_WithInvalidValue_ReturnsFalse ()
    {
      string idString = "Order|12|System.Guid";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_WithInvalidTypeName_ReturnsFalse ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Goid";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_WithInvalidClassID_ReturnsFalse ()
    {
      string idString = "Arder|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [Test]
    public void TryParse_InvalidValueType_ReturnsFalse ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Double";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse(idString, out id);

      Assert.That(result, Is.False);
      Assert.That(id, Is.Null);
    }

    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid, mscorlib", typeof(Guid))]
    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid, mscorlib, Version=4.0.0.0", typeof(Guid))]
    [TestCase("Official|23|System.Int32, mscorlib", typeof(Int32))]
    [TestCase("Official|23|System.Int32 , mscorlib , Version=4.0.0.0", typeof(Int32))]
    [TestCase("Official|test|System.String, mscorlib", typeof(String))]
    [TestCase("Official|test| System.String , mscorlib , Version=4.0.0.0", typeof(String))]
    [Test]
    public void TryParse_WithAssemblyNameValidForNetFramework (string idString, Type type)
    {
      Assert.That(ObjectIDStringSerializer.Instance.TryParse(idString, out _), Is.False);
    }

    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.Goid, mscorlib")]
    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.guid, mscorlib")]
    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid, badassembly, mscorlib")]
    [TestCase("Official|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid, badassembly, mscorlib, Version=4.0.0.0")]
    [TestCase("Official|23|System.Int32, badassembly, mscorlib")]
    [TestCase("Official|23|System.Int32 , badassembly, mscorlib , Version=4.0.0.0")]
    [TestCase("Official|test|System.String, badassembly, mscorlib")]
    [TestCase("Official|test| System.String , badassembly, mscorlib , Version=4.0.0.0")]
    [Test]
    public void TryParse_WithInvalidAssemblyName_ReturnsFalse (string idString)
    {
      Assert.That(ObjectIDStringSerializer.Instance.TryParse(idString, out _), Is.False);
    }
  }
}
