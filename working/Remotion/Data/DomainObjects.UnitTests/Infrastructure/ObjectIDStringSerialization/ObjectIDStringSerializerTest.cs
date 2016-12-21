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

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectIDStringSerialization
{
  [TestFixture]
  public class ObjectIDStringSerializerTest : StandardMappingTest
  {
    [Test]
    public void CheckSerializableStringValue ()
    {
      ObjectIDStringSerializer.Instance.CheckSerializableStringValue ("Arthur|Dent &pipe;");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Value cannot contain '&amp;pipe;'.\r\nParameter name: value")]
    public void CheckSerializableStringValue_WithEscapeString ()
    {
      ObjectIDStringSerializer.Instance.CheckSerializableStringValue ("Arthur|Dent &pipe; &amp;pipe; Zaphod Beeblebrox");
    }

    [Test]
    public void SerializeStringValue ()
    {
      var id = new ObjectID("Official", "Arthur Dent");
      Assert.That (ObjectIDStringSerializer.Instance.Serialize (id), Is.EqualTo ("Official|Arthur Dent|System.String"));
    }

    [Test]
    public void SerializeInt32Value ()
    {
      var id = new ObjectID("Official", 42);
      Assert.That (ObjectIDStringSerializer.Instance.Serialize (id), Is.EqualTo ("Official|42|System.Int32"));
    }

    [Test]
    public void SerializeGuidValue ()
    {
      var id = new ObjectID("Order", new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}"));
      Assert.That (ObjectIDStringSerializer.Instance.Serialize (id), Is.EqualTo ("Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid"));
    }

    [Test]
    public void Parse_StringValue ()
    {
      string idString = "Official|Arthur Dent|System.String";
      ObjectID id = ObjectIDStringSerializer.Instance.Parse (idString);

      Assert.That (id.StorageProviderDefinition.Name, Is.EqualTo ("UnitTestStorageProviderStub"));
      Assert.That (id.ClassID, Is.EqualTo ("Official"));
      Assert.That (id.Value.GetType(), Is.EqualTo (typeof (string)));
      Assert.That (id.Value, Is.EqualTo ("Arthur Dent"));
    }

    [Test]
    public void Parse_Int32Value ()
    {
      string idString = "Official|42|System.Int32";
      ObjectID id = ObjectIDStringSerializer.Instance.Parse (idString);

      Assert.That (id.StorageProviderDefinition.Name, Is.EqualTo ("UnitTestStorageProviderStub"));
      Assert.That (id.ClassID, Is.EqualTo ("Official"));
      Assert.That (id.Value.GetType(), Is.EqualTo (typeof (int)));
      Assert.That (id.Value, Is.EqualTo (42));
    }

    [Test]
    public void Parse_GuidValue ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";
      ObjectID id = ObjectIDStringSerializer.Instance.Parse (idString);

      Assert.That (id.StorageProviderDefinition.Name, Is.EqualTo ("TestDomain"));
      Assert.That (id.ClassID, Is.EqualTo ("Order"));
      Assert.That (id.Value.GetType(), Is.EqualTo (typeof (Guid)));
      Assert.That (id.Value, Is.EqualTo (new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}")));
    }

    [Test]
    [ExpectedException (typeof (FormatException), ExpectedMessage = "Serialized ObjectID '' is not correctly formatted: it must not be empty.")]
    public void Parse_EmptyString ()
    {
      string idString = "";
      ObjectIDStringSerializer.Instance.Parse (idString);
    }

    [Test]
    [ExpectedException (typeof (FormatException), ExpectedMessage = "Serialized ObjectID 'Order|5d09030c-25"
        + "c2-4735-b514-46333bd28ac8|System.Guid|Zaphod' is not correctly formatted: it should have three parts.")]
    public void Parse_WithTooManyParts ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid|Zaphod";
      ObjectIDStringSerializer.Instance.Parse (idString);
    }

    [Test]
    [ExpectedException (typeof (FormatException), ExpectedMessage = "Serialized ObjectID 'Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Double' "
        + "is invalid: type 'System.Double' is not supported.")]
    public void Parse_WithInvalidValueType ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Double";
      ObjectIDStringSerializer.Instance.Parse (idString);
    }

    [Test]
    [ExpectedException (typeof (FormatException), ExpectedMessage = "Serialized ObjectID 'Order|12|System.Guid' is not correctly formatted: "
        + "value '12' is not in the correct format for type 'System.Guid'.")]
    public void Parse_WithErrorParsingInnerValue ()
    {
      string idString = "Order|12|System.Guid";
      ObjectIDStringSerializer.Instance.Parse (idString);
    }

    [Test]
    [ExpectedException (typeof (FormatException), ExpectedMessage = "Serialized ObjectID 'Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Goid' is "
        + "invalid: 'System.Goid' is not the name of a loadable type.")]
    public void Parse_WithErrorParsingTypeName ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Goid";
      ObjectIDStringSerializer.Instance.Parse (idString);
    }

    [Test]
    [ExpectedException (typeof (FormatException), 
        ExpectedMessage = "Serialized ObjectID 'Arder|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid' is invalid: 'Arder' is not a valid class ID.",
        MatchType = MessageMatch.Contains)]
    public void Parse_WithErrorParsingClassID ()
    {
      string idString = "Arder|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";
      ObjectIDStringSerializer.Instance.Parse (idString);
    }

    [Test]
    public void TryParse ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse (idString, out id);

      Assert.That (result, Is.True);
      Assert.That (id.StorageProviderDefinition.Name, Is.EqualTo ("TestDomain"));
      Assert.That (id.ClassID, Is.EqualTo ("Order"));
      Assert.That (id.Value.GetType(), Is.EqualTo (typeof (Guid)));
      Assert.That (id.Value, Is.EqualTo (new Guid ("{5D09030C-25C2-4735-B514-46333BD28AC8}")));
    }

    [Test]
    public void TryParse_EmptyString ()
    {
      string idString = "";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse (idString, out id);

      Assert.That (result, Is.False);
      Assert.That (id, Is.Null);
    }

    [Test]
    public void TryParse_WrongNumberOfParts ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid|Zaphod";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse (idString, out id);

      Assert.That (result, Is.False);
      Assert.That (id, Is.Null);
    }

    [Test]
    public void TryParse_InvalidValueType ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Double";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse (idString, out id);

      Assert.That (result, Is.False);
      Assert.That (id, Is.Null);
    }

    [Test]
    public void TryParse_ErrorParsingInnerValue ()
    {
      string idString = "Order|12|System.Guid";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse (idString, out id);

      Assert.That (result, Is.False);
      Assert.That (id, Is.Null);
    }

    [Test]
    public void TryParse_ErrorParsingTypeName ()
    {
      string idString = "Order|5d09030c-25c2-4735-b514-46333bd28ac8|System.Goid";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse (idString, out id);

      Assert.That (result, Is.False);
      Assert.That (id, Is.Null);
    }

    [Test]
    public void TryParse_ErrorParsingClassID ()
    {
      string idString = "Arder|5d09030c-25c2-4735-b514-46333bd28ac8|System.Guid";

      ObjectID id;
      bool result = ObjectIDStringSerializer.Instance.TryParse (idString, out id);

      Assert.That (result, Is.False);
      Assert.That (id, Is.Null);
    }
  }
}
