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
using System.Reflection;
using NUnit.Framework;

namespace Remotion.UnitTests
{
  [TestFixture]
  public class PermanentGuidAttributeTest
  {
    [PermanentGuid ("BA2F9118-6516-4F47-9AF1-58632B5F9F9E")]
    private class TheClass
    {
#pragma warning disable 169
      [PermanentGuid ("823BAC5E-2685-4535-8B12-F2C7BF943FDC")]
      private object _field;
#pragma warning restore 169

      [PermanentGuid ("A9EC3641-CBEE-41D6-8867-F202CDF66660")]
      public object Property { get; set; }

      [PermanentGuid ("C60DA8F6-F3C0-4B00-AFB5-298B4B929408")]
      public void Method ()
      {
      }
    }

    private class DerivedClass : TheClass
    {
    }

    [PermanentGuid ("467BB753-98AC-4F60-90BA-FF5A02A45649")]
    private interface ITheInterface
    {
    }

    [PermanentGuid ("E6A34024-80B8-4774-9AD8-E8367D5D0387")]
    private struct TheStruct
    {
    }

    [PermanentGuid ("B6EA85C3-1179-4BFA-9D85-15F081348759")]
    private enum TheEnum
    {
      [PermanentGuid ("3F50B378-390F-44E8-86F6-61AAC722A19E")]
      Value
    }

    [Test]
    public void TypeIsClass ()
    {
      var type = typeof (TheClass);
      var attribute = type.GetCustomAttribute<PermanentGuidAttribute>();

      Assert.That (attribute.Value, Is.EqualTo (new Guid ("BA2F9118-6516-4F47-9AF1-58632B5F9F9E")));
    }

    [Test]
    public void TypeIsInterface ()
    {
      var type = typeof (ITheInterface);
      var attribute = type.GetCustomAttribute<PermanentGuidAttribute>();

      Assert.That (attribute.Value, Is.EqualTo (new Guid ("467BB753-98AC-4F60-90BA-FF5A02A45649")));
    }

    [Test]
    public void TypeIsStruct ()
    {
      var type = typeof (TheStruct);
      var attribute = type.GetCustomAttribute<PermanentGuidAttribute>();

      Assert.That (attribute.Value, Is.EqualTo (new Guid ("E6A34024-80B8-4774-9AD8-E8367D5D0387")));
    }

    [Test]
    public void TypeIsEnum ()
    {
      var type = typeof (TheEnum);
      var attribute = type.GetCustomAttribute<PermanentGuidAttribute>();

      Assert.That (attribute.Value, Is.EqualTo (new Guid ("B6EA85C3-1179-4BFA-9D85-15F081348759")));
    }

    [Test]
    public void Property ()
    {
      var type = typeof (TheClass);
      var value = type.GetProperty ("Property", BindingFlags.Instance | BindingFlags.Public);
      var attribute = value.GetCustomAttribute<PermanentGuidAttribute>();

      Assert.That (attribute.Value, Is.EqualTo (new Guid ("A9EC3641-CBEE-41D6-8867-F202CDF66660")));
    }

    [Test]
    public void Field ()
    {
      var type = typeof (TheClass);
      var value = type.GetField ("_field", BindingFlags.Instance | BindingFlags.NonPublic);
      var attribute = value.GetCustomAttribute<PermanentGuidAttribute>();

      Assert.That (attribute.Value, Is.EqualTo (new Guid ("823BAC5E-2685-4535-8B12-F2C7BF943FDC")));
    }

    [Test]
    public void Method ()
    {
      var type = typeof (TheClass);
      var value = type.GetMethod ("Method", BindingFlags.Instance | BindingFlags.Public);
      var attribute = value.GetCustomAttribute<PermanentGuidAttribute>();

      Assert.That (attribute.Value, Is.EqualTo (new Guid ("C60DA8F6-F3C0-4B00-AFB5-298B4B929408")));
    }

    [Test]
    public void EnumValue ()
    {
      var type = typeof (TheEnum);
      var value = type.GetField ("Value", BindingFlags.Static | BindingFlags.Public);
      var attribute = value.GetCustomAttribute<PermanentGuidAttribute>();

      Assert.That (attribute.Value, Is.EqualTo (new Guid ("3F50B378-390F-44E8-86F6-61AAC722A19E")));
    }

    [Test]
    public void DerivedClass_DoesNotInherit ()
    {
      var type = typeof (DerivedClass);
      var attribute = type.GetCustomAttribute<PermanentGuidAttribute>();

      Assert.That (attribute, Is.Null);
    }
  }
}