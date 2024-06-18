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
  public class MemberModifierUtilityTest
  {
    private MemberModifierUtility _memberModifierUtility;

    [SetUp]
    public void SetUp ()
    {
      _memberModifierUtility = new MemberModifierUtility();
    }

    [Test]
    public void IsOverriddenMember_Method ()
    {
      var baseMethodInfo = typeof (BaseClassWithProperty).GetMethod ("DoSomething");
      var subMethodInfo = typeof (ClassWithProperty).GetMethod ("DoSomething");

      Assert.That (_memberModifierUtility.IsOverriddenMember (baseMethodInfo), Is.EqualTo (false));
      Assert.That (_memberModifierUtility.IsOverriddenMember (subMethodInfo), Is.EqualTo (true));
    }

    [Test]
    public void IsOverriddenMember_Property ()
    {
      var basePropertyInfo = typeof (BaseClassWithProperty).GetProperty ("PropertyName");
      var subPropertyInfo = typeof (ClassWithProperty).GetProperty ("PropertyName");

      Assert.That (_memberModifierUtility.IsOverriddenMember (basePropertyInfo), Is.EqualTo (false));
      Assert.That (_memberModifierUtility.IsOverriddenMember (subPropertyInfo), Is.EqualTo (true));
    }

    [Test]
    public void GetMemberModifiers_PublicMethod ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("PublicMethod")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public"));
    }

    [Test]
    public void GetMemberModifiers_ProtectedProperty ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("ProtectedProperty", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("protected"));
    }

    [Test]
    public void GetMemberModifiers_ProtectedInternalEvent ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("ProtectedInternalEvent", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("protected internal"));
    }

    [Test]
    public void GetMemberModifiers_InternalField ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("InternalField", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("internal"));
    }

    [Test]
    public void GetMemberModifiers_PrivateField ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("_privateField", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("private"));
    }

    [Test]
    public void GetMemberModifiers_PublicVirtualMethod ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("PublicVirtualMethod")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public virtual"));
    }

    [Test]
    public void GetMemberModifiers_PublicAbstractMethod ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("PublicAbstractMethod")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public abstract"));
    }

    [Test]
    public void GetMemberModifiers_PublicAbstractAndOverriddenMethod ()
    {
      var memberInfo = typeof (SubModifierTestClass).GetMember ("PublicAbstractMethod")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public abstract override"));
    }

    [Test]
    public void GetMemberModifiers_PublicOverriddenAndSealedMethod ()
    {
      var memberInfo = typeof (SubModifierTestClass).GetMember ("PublicVirtualMethod")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public sealed override"));
    }

    [Test]
    public void GetMemberModifiers_ExplicitInterfaceWithParams ()
    {
      var methodInfo =
          typeof (MemberSignatureTestClass).GetMethod ("MixinXRef.UnitTests.TestDomain.IExplicitInterface.Version",
                                                       BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

      var output = _memberModifierUtility.GetMemberModifiers (methodInfo);
      
      Assert.That (output, Is.EqualTo (""));
    }

    [Test]
    public void GetMemberModifiers_NestedType ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("NestedClass")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo (new TypeModifierUtility().GetTypeModifiers(typeof(MemberModifierTestClass.NestedClass))));
    }

    [Test]
    public void GetMemberModifiers_InterfaceImplementation ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("Dispose")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public"));
    }

    [Test]
    public void GetMemberModifiers_ReadonlyField ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("_readonlyField")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public readonly"));
    }

    [Test]
    public void GetMemberModifiers_StaticField ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("_staticField")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public static"));
    }
  }
}