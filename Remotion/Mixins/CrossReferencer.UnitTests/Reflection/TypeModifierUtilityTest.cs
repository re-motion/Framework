// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System.Reflection;
using MixinXRef.Formatting;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class TypeModifierUtilityTest
  {
    private TypeModifierUtility _typeModifierUtility;

    [SetUp]
    public void SetUp ()
    {
      _typeModifierUtility = new TypeModifierUtility();
    }


    [Test]
    public void GetTypeModifiers_Visibility()
    {
      Assert.That (_typeModifierUtility.GetTypeModifiers(typeof(TypeModifierTestClass)), Is.EqualTo ("public"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass).GetNestedType ("ProtectedClass", BindingFlags.Instance | BindingFlags.NonPublic)), Is.EqualTo ("protected"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass.ProtectedInternalClass)), Is.EqualTo ("protected internal"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass.InternalClass)), Is.EqualTo ("internal"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass).GetNestedType ("PrivateClass", BindingFlags.Instance | BindingFlags.NonPublic)), Is.EqualTo ("private"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TopLevelInternalClass)), Is.EqualTo ("internal"));
    }

    [Test]
    public void GetTypeModifiers_Sealed ()
    {
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (PublicSealedClass)), Is.EqualTo ("public sealed"));
      // struct is sealed by default
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass.PublicStruct)), Is.EqualTo ("public sealed"));
    }

    [Test]
    public void GetTypeModifiers_Abstract ()
    {
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (PublicAbstractClass)), Is.EqualTo ("public abstract"));      
    }
  }
}