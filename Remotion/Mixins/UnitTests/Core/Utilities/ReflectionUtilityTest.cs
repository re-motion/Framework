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
using Remotion.Mixins.Samples.UsesAndExtends.Core;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.UnitTests.Core.Utilities.TestDomain.AssociatedMethods;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Utilities
{
  [TestFixture]
  public class ReflectionUtilityTest
  {
    class Base
    {
      public void Foo (int i) { }
      public virtual void Bar (int i) { }

      public int FooP { get { return 0; } set { } }
      public virtual int BarP { get { return 0; } set { } }

      public event Func<int> FooE;
      public virtual event Func<int> BarE;
    }

    class Derived : Base
    {
      public virtual new void Foo (int i) { }
      public override void Bar (int i) { }
      public void Baz (int i) { }

      public virtual new int FooP { get { return 0; } set { } }
      public override int BarP { get { return 0; } set { } }
      public int BazP { get { return 0; } set { } }

      public virtual new event Func<int> FooE;
      public override event Func<int> BarE;
      public event Func<int> BazE;
    }

    private class Mixin1 : Mixin<Derived>
    {
    }

    private class Mixin2 : Mixin<Derived, Mixin2.IDerived>
    {
      public interface IDerived
      {
      }
    }

    [Test]
    public void IsNewSlotMember()
    {
      Assert.That (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetMethod ("Foo")), Is.True);
      Assert.That (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetProperty ("FooP")), Is.True);
      Assert.That (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetEvent ("FooE")), Is.True);

      Assert.That (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetMethod ("Bar")), Is.False);
      Assert.That (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetProperty ("BarP")), Is.False);
      Assert.That (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetEvent ("BarE")), Is.False);

      Assert.That (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetMethod ("Baz")), Is.False);
      Assert.That (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetProperty ("BazP")), Is.False);
      Assert.That (ReflectionUtility.IsNewSlotMember (typeof (Derived).GetEvent ("BazE")), Is.False);
    }

    [Test]
    public void IsVirtualMember ()
    {
      Assert.That (ReflectionUtility.IsVirtualMember (typeof (Derived).GetMethod ("Foo")), Is.True);
      Assert.That (ReflectionUtility.IsVirtualMember (typeof (Derived).GetProperty ("FooP")), Is.True);
      Assert.That (ReflectionUtility.IsVirtualMember (typeof (Derived).GetEvent ("FooE")), Is.True);

      Assert.That (ReflectionUtility.IsVirtualMember (typeof (Derived).GetMethod ("Bar")), Is.True);
      Assert.That (ReflectionUtility.IsVirtualMember (typeof (Derived).GetProperty ("BarP")), Is.True);
      Assert.That (ReflectionUtility.IsVirtualMember (typeof (Derived).GetEvent ("BarE")), Is.True);

      Assert.That (ReflectionUtility.IsVirtualMember (typeof (Derived).GetMethod ("Baz")), Is.False);
      Assert.That (ReflectionUtility.IsVirtualMember (typeof (Derived).GetProperty ("BazP")), Is.False);
      Assert.That (ReflectionUtility.IsVirtualMember (typeof (Derived).GetEvent ("BazE")), Is.False);
    }

    public class BlaAttribute : Attribute { }

    interface IInterface
    {
      void Explicit ();
    }

    class ClassWithAllVisibilityMethods : IInterface
    {
      public void Public () { }
      protected void Protected () { }
      protected internal void ProtectedInternal () { }
      internal void Internal () { }
      private void Private () { }

      void IInterface.Explicit () { }
    }

    [Test]
    public void IsPublicOrProtected ()
    {
      BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      Assert.That (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("Public", bf)), Is.True);
      Assert.That (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("Protected", bf)), Is.True);
      Assert.That (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("ProtectedInternal", bf)), Is.True);
      Assert.That (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("Internal", bf)), Is.False);
      Assert.That (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("Private", bf)), Is.False);
      Assert.That (ReflectionUtility.IsPublicOrProtected (typeof (ClassWithAllVisibilityMethods).GetMethod ("Remotion.Mixins.UnitTests.Core.Utilities.ReflectionUtilityTest.IInterface.Explicit", bf)), Is.False);
    }

    [Test]
    public void IsPublicOrProtectedOrExplicitInterface ()
    {
      BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      Assert.That (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("Public", bf)), Is.True);
      Assert.That (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("Protected", bf)), Is.True);
      Assert.That (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("ProtectedInternal", bf)), Is.True);
      Assert.That (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("Internal", bf)), Is.False);
      Assert.That (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("Private", bf)), Is.False);
      Assert.That (ReflectionUtility.IsPublicOrProtectedOrExplicit (typeof (ClassWithAllVisibilityMethods).GetMethod ("Remotion.Mixins.UnitTests.Core.Utilities.ReflectionUtilityTest.IInterface.Explicit", bf)), Is.True);
    }

    [Test]
    public void IsAssemblySigned_Assembly ()
    {
      Assert.That (ReflectionUtility.IsAssemblySigned (typeof (object).Assembly), Is.True);
      Assert.That (ReflectionUtility.IsAssemblySigned (typeof (Uri).Assembly), Is.True);
      Assert.That (ReflectionUtility.IsAssemblySigned (typeof (Mixin).Assembly), Is.True);

      Assert.That (ReflectionUtility.IsAssemblySigned (typeof (ReflectionUtilityTest).Assembly), Is.False);
    }

    [Test]
    public void IsAssemblySigned_Name ()
    {
      Assert.That (ReflectionUtility.IsAssemblySigned (typeof (object).Assembly.GetName()), Is.True);
      Assert.That (ReflectionUtility.IsAssemblySigned (typeof (Uri).Assembly.GetName ()), Is.True);
      Assert.That (ReflectionUtility.IsAssemblySigned (typeof (Mixin).Assembly.GetName ()), Is.True);

      Assert.That (ReflectionUtility.IsAssemblySigned (typeof (ReflectionUtilityTest).Assembly.GetName ()), Is.False);
    }

    [Test]
    public void IsAssemblySigned_StringName ()
    {
      string fullName = typeof (object).Assembly.FullName;
      Assert.That (ReflectionUtility.IsAssemblySigned (new AssemblyName (fullName)), Is.True);

      fullName = typeof (ReflectionUtilityTest).Assembly.FullName;
      Assert.That (ReflectionUtility.IsAssemblySigned (new AssemblyName (fullName)), Is.False);
    }

    [Test]
    public void GetAssociatedMethods_MethodInfo ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetMethod ("Method");
      var associatedMethods = ReflectionUtility.GetAssociatedMethods (member);
      Assert.That (associatedMethods, Is.EqualTo (new[] { member }));
    }

    [Test]
    public void GetAssociatedMethods_PropertyInfo ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetProperty ("Property");
      var associatedMethods = ReflectionUtility.GetAssociatedMethods (member);
      Assert.That (associatedMethods, Is.EquivalentTo (new[] { member.GetGetMethod (), member.GetSetMethod () }));
    }

    [Test]
    public void GetAssociatedMethods_PropertyInfo_Protected ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetProperty ("ProtectedProperty", BindingFlags.NonPublic | BindingFlags.Instance);
      var associatedMethods = ReflectionUtility.GetAssociatedMethods (member);
      Assert.That (associatedMethods, Is.EquivalentTo (new[] { member.GetGetMethod (true), member.GetSetMethod (true) }));
    }

    [Test]
    public void GetAssociatedMethods_EventInfo ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetEvent ("Event");
      var associatedMethods = ReflectionUtility.GetAssociatedMethods (member);
      Assert.That (associatedMethods, Is.EquivalentTo (new[] { member.GetAddMethod(), member.GetRemoveMethod () }));
    }

    [Test]
    public void GetAssociatedMethods_EventInfo_Protected ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetEvent ("ProtectedEvent", BindingFlags.NonPublic | BindingFlags.Instance);
      var associatedMethods = ReflectionUtility.GetAssociatedMethods (member);
      Assert.That (associatedMethods, Is.EquivalentTo (new[] { member.GetAddMethod (true), member.GetRemoveMethod (true) }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Associated methods can only be retrieved for methods, properties, and events.")]
    public void GetAssociatedMethods_InvalidMemberInfoKind ()
    {
      var member = typeof (ClassWithAllKindsOfMembers).GetConstructor (Type.EmptyTypes);
      ReflectionUtility.GetAssociatedMethods (member);
    }

    [Test]
    public void IsReachableFromSignedAssembly_True ()
    {
      Assert.That (ReflectionUtility.IsReachableFromSignedAssembly (typeof (object)), Is.True);
      Assert.That (ReflectionUtility.IsReachableFromSignedAssembly (typeof (EquatableMixin<>)), Is.True);
    }

    [Test]
    public void IsReachableFromSignedAssembly_False ()
    {
      Assert.That (ReflectionUtility.IsReachableFromSignedAssembly (typeof (NullMixin)), Is.False);
    }

    [Test]
    public void IsReachableFromSignedAssembly_False_GenericArgument ()
    {
      Assert.That (ReflectionUtility.IsReachableFromSignedAssembly (typeof (EquatableMixin<NullMixin>)), Is.False);
    }

    [Test]
    public void IsRangeReachableFromSignedAssembly_True ()
    {
      Assert.That (ReflectionUtility.IsRangeReachableFromSignedAssembly (new[] { typeof (object), typeof (EquatableMixin<>) }), Is.True);
    }

    [Test]
    public void IsRangeReachableFromSignedAssembly_False ()
    {
      Assert.That (ReflectionUtility.IsRangeReachableFromSignedAssembly (new[] { typeof (object), typeof (EquatableMixin<NullMixin>) }), Is.False);
    }

    [Test]
    public void IsMixinType ()
    {
      Assert.That (ReflectionUtility.IsMixinType (typeof (Mixin<>)), Is.True);
      Assert.That (ReflectionUtility.IsMixinType (typeof (Mixin<,>)), Is.True);
      Assert.That (ReflectionUtility.IsMixinType (typeof (Mixin<Derived>)), Is.True);
      Assert.That (ReflectionUtility.IsMixinType (typeof (Mixin<Derived,Mixin2.IDerived>)), Is.True);
      Assert.That (ReflectionUtility.IsMixinType (typeof (Mixin1)), Is.True);
      Assert.That (ReflectionUtility.IsMixinType (typeof (Mixin2)), Is.True);
      Assert.That (ReflectionUtility.IsMixinType (typeof (IInitializableMixin)), Is.False);
      Assert.That (ReflectionUtility.IsMixinType (typeof (Object)), Is.False);
      Assert.That (ReflectionUtility.IsMixinType (typeof (Derived)), Is.False);
    }
  }
}
