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
using System.Reflection.Emit;
using NUnit.Framework;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  [Obsolete]
  public class TypeNameTemplateResolverTest
  {
    private static readonly byte[] s_referencePublicKey =
        new byte[]
        {
            0x00, 0x24, 0x00, 0x00, 0x04, 0x80, 0x00, 0x00, 0x94, 0x00, 0x00, 0x00, 0x06, 0x02, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x52, 0x53, 0x41,
            0x31, 0x00, 0x04, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0xD7, 0xF6, 0x9D, 0xE8, 0xE5, 0xD2, 0xDD, 0xAC, 0x74, 0x67, 0x98, 0x4E, 0x71, 0x23,
            0x03, 0x6A, 0x2D, 0x2A, 0xC6, 0xBB, 0x6D, 0xA2, 0x91, 0x3D, 0xDD, 0xD7, 0x0B, 0xA8, 0x40, 0xA5, 0x61, 0x35, 0x16, 0x8F, 0x34, 0xDF, 0xBC,
            0x75, 0xDA, 0xB2, 0xEC, 0xD8, 0x5D, 0x28, 0x05, 0x5B, 0xB5, 0x1E, 0xF4, 0xB2, 0x9B, 0x81, 0x1D, 0x5D, 0x1B, 0x37, 0x6F, 0x07, 0xB4, 0x45,
            0x9C, 0x55, 0x2F, 0xDB, 0x2A, 0xBE, 0x4D, 0x96, 0xAA, 0xF7, 0xD7, 0x27, 0xA0, 0xB9, 0x72, 0x24, 0xB3, 0x59, 0x22, 0x9B, 0x7D, 0x22, 0x41,
            0x2D, 0x7C, 0xE6, 0xB5, 0x0B, 0x85, 0x2C, 0xC6, 0x35, 0x2F, 0xE5, 0xB4, 0x11, 0x29, 0x7E, 0x87, 0x73, 0xA3, 0xB2, 0x89, 0x26, 0x72, 0xEB,
            0xB1, 0x85, 0xA6, 0x1A, 0x39, 0x4B, 0xF0, 0x9A, 0xBD, 0xF8, 0xDA, 0x52, 0xCA, 0x7A, 0xB1, 0x9A, 0x28, 0x10, 0x23, 0x40, 0x23, 0xBA
        };

    private static readonly byte[] s_referencePublicKeyToken = new byte[] { 0xc5, 0xc1, 0x12, 0x52, 0x69, 0x50, 0x89, 0xf8 };

    private Assembly _referenceAssembly;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      _referenceAssembly = CreateReferenceAssembly (new Version (2, 4, 6, 8), s_referencePublicKey);
      Assert.That (_referenceAssembly.GetName().GetPublicKeyToken(), Is.EqualTo (s_referencePublicKeyToken));
    }

    [Test]
    public void GetTypeName_WithVersion ()
    {
      var result = TypeNameTemplateResolver.ResolveToTypeName ("Name, Version = <version>", _referenceAssembly);
      Assert.That (result, Is.EqualTo ("Name, Version = 2.4.6.8"));
    }

    [Test]
    public void GetTypeName_WithVersionAndKeyToken ()
    {
      const string typeName = "Name, Version = <version>, PublicKeyToken = <publicKeyToken>";
      var result = TypeNameTemplateResolver.ResolveToTypeName (typeName, _referenceAssembly);
      Assert.That (result, Is.EqualTo ("Name, Version = 2.4.6.8, PublicKeyToken = c5c11252695089f8"));
    }

    [Test]
    public void GetTypeName_WithUnsignedAssembly ()
    {
      var referenceAssembly = CreateReferenceAssembly (new Version(), null);

      const string typeName = "Name, Version = <version>, PublicKeyToken = <publicKeyToken>";
      var result = TypeNameTemplateResolver.ResolveToTypeName (typeName, referenceAssembly);
      Assert.That (result, Is.EqualTo ("Name, Version = 0.0.0.0, PublicKeyToken = null"));
    }

    [Test]
    public void ResolveType ()
    {
      const string typeName = "Remotion.UnitTests.ServiceLocation.TypeNameTemplateResolverTest, Remotion.UnitTests, Version = <version>";
      var result = TypeNameTemplateResolver.ResolveToType (typeName, typeof (TypeNameTemplateResolverTest).Assembly);
      Assert.That (result, Is.SameAs (typeof (TypeNameTemplateResolverTest)));
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException), ExpectedMessage = "Could not load type 'Badabing' from assembly 'Remotion, "
       + "Version=.*, Culture=neutral, PublicKeyToken=.*'\\.", MatchType = MessageMatch.Regex)]
    public void ResolveType_WithInvalidTypeName ()
    {
      TypeNameTemplateResolver.ResolveToType ("Badabing", typeof (TypeNameTemplateResolverTest).Assembly);
    }

    private Assembly CreateReferenceAssembly (Version version, byte[] publicKey)
    {
      var name = new AssemblyName (typeof (TypeNameTemplateResolverTest).Name + "_ReferenceAssembly");
      name.Version = version;
      name.SetPublicKey (publicKey);

      // TODO .NET 4: Should actually be RunAndCollect
      return AppDomain.CurrentDomain.DefineDynamicAssembly (name, AssemblyBuilderAccess.ReflectionOnly);
    }

  }
}