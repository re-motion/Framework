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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class IntroductionTest : CodeGenerationBaseTest
  {
    private const BindingFlags c_nonPublicFlags = BindingFlags.NonPublic | BindingFlags.Instance;

    [Test]
    public void IntroducedInterfacesAreImplementedViaDelegation ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      var bt1AsMixedIface = bt1 as IBT1Mixin1;
      Assert.That (bt1AsMixedIface, Is.Not.Null);
      Assert.That (bt1AsMixedIface.IntroducedMethod (), Is.EqualTo ("BT1Mixin1.IntroducedMethod"));
    }

    [Test]
    public void MixinCanImplementMethodsExplicitly ()
    {
      var bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithExplicitImplementation));
      var explicito = bt1 as IExplicit;
      Assert.That (explicito, Is.Not.Null);
      Assert.That (explicito.Explicit (), Is.EqualTo ("XXX"));
    }

    [Test]
    public void MixinCanIntroduceGenericInterface ()
    {
      var bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingGenericInterface<>));
      var generic = bt1 as IGeneric<BaseType1>;
      Assert.That (generic, Is.Not.Null);
      Assert.That (generic.Generic (bt1), Is.EqualTo ("Generic"));
    }

    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      var bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingInheritedInterface));
      Assert.That (((IMixinIII1) bt1).Method1 (), Is.EqualTo ("MixinIntroducingInheritedInterface.Method1"));
      Assert.That (((IMixinIII2) bt1).Method1 (), Is.EqualTo ("MixinIntroducingInheritedInterface.Method1"));
      Assert.That (((IMixinIII2) bt1).Method2 (), Is.EqualTo ("MixinIntroducingInheritedInterface.Method2"));
      Assert.That (((IMixinIII3) bt1).Method3 (), Is.EqualTo ("MixinIntroducingInheritedInterface.Method3"));
      Assert.That (((IMixinIII4) bt1).Method4 (), Is.EqualTo ("MixinIntroducingInheritedInterface.Method4"));
      Assert.That (((IMixinIII4) bt1).Method2 (), Is.EqualTo ("MixinIntroducingInheritedInterface.Method2"));
    }

    [Test]
    public void MixinImplementingFullPropertiesWithPartialIntroduction ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (MixinImplementingFullPropertiesWithPartialIntroduction)).EnterScope())
      {
        var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
        MethodInfo[] allMethods = bt1.GetType ().GetMethods (c_nonPublicFlags | BindingFlags.DeclaredOnly);
        string[] allMethodNames = Array.ConvertAll (allMethods, mi => mi.Name);
        Assert.That (allMethodNames, Has.Member("Remotion.Mixins.UnitTests.Core.TestDomain.InterfaceWithPartialProperties.get_Prop1"));
        Assert.That (allMethodNames, Has.Member("Remotion.Mixins.UnitTests.Core.TestDomain.InterfaceWithPartialProperties.set_Prop2"));

        Assert.That (allMethodNames, Has.No.Member("Remotion.Mixins.UnitTests.Core.TestDomain.InterfaceWithPartialProperties.set_Prop1"));
        Assert.That (allMethodNames, Has.No.Member("Remotion.Mixins.UnitTests.Core.TestDomain.InterfaceWithPartialProperties.get_Prop2"));
      }
    }

    [Test]
    public void ExplicitlyNonIntroducedInterface ()
    {
      object o = CreateMixedObject<NullTarget> (typeof (MixinNonIntroducingSimpleInterface));
      Assert.That (o is ISimpleInterface, Is.False);
      Assert.That (Mixin.Get<MixinNonIntroducingSimpleInterface> (o), Is.InstanceOf (typeof (ISimpleInterface)));
    }

    [Test]
    public void ImplicitlyNonIntroducedInterface ()
    {
      var o = CreateMixedObject<ClassImplementingSimpleInterface> (typeof (MixinImplementingSimpleInterface));
      Assert.That (o, Is.InstanceOf (typeof (ISimpleInterface)));
      Assert.That (o.Method (), Is.EqualTo ("ClassImplementingSimpleInterface.Method"));
    }

    [Test]
    public void MultipleSimilarInterfaces ()
    {
      object o = CreateMixedObject<NullTarget> (typeof (MixinIntroducingInterfacesImplementingEachOther<>));
      Assert.That (o is IList<NullTarget>, Is.True);
      Assert.That (o is ICollection<NullTarget>, Is.True);
      Assert.That (o is IEnumerable<NullTarget>, Is.True);
      Assert.That (o is IList, Is.True);
      Assert.That (o is ICollection, Is.True);
      Assert.That (o is IEnumerable, Is.True);
    }

    [Test]
    public void IntroducedMemberVisibilites_Public ()
    {
      Type t = CreateMixedType (typeof (NullTarget), typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodInfo methodInfo = t.GetMethod ("MethodWithPublicVisibility", BindingFlags.Public | BindingFlags.Instance);
      PropertyInfo propertyInfo = t.GetProperty ("PropertyWithPublicVisibility", BindingFlags.Public | BindingFlags.Instance);
      EventInfo eventInfo = t.GetEvent ("EventWithPublicVisibility", BindingFlags.Public | BindingFlags.Instance);

      Assert.That (methodInfo, Is.Not.Null);
      Assert.That (propertyInfo, Is.Not.Null);
      Assert.That (eventInfo, Is.Not.Null);
    }

    [Test]
    public void IntroducedMemberVisibilites_Private ()
    {
      Type t = CreateMixedType (typeof (NullTarget), typeof (MixinIntroducingMembersWithDifferentVisibilities));
      MethodInfo methodInfo = t.GetMethod (
          typeof (IMixinIntroducingMembersWithDifferentVisibilities) + ".MethodWithDefaultVisibility", c_nonPublicFlags);
      PropertyInfo propertyInfo = t.GetProperty (
          typeof (IMixinIntroducingMembersWithDifferentVisibilities) + ".PropertyWithDefaultVisibility",
          c_nonPublicFlags);
      EventInfo eventInfo = t.GetEvent (
          typeof (IMixinIntroducingMembersWithDifferentVisibilities) + ".EventWithDefaultVisibility", c_nonPublicFlags);

      Assert.That (methodInfo, Is.Not.Null);
      Assert.That (propertyInfo, Is.Not.Null);
      Assert.That (eventInfo, Is.Not.Null);

      Assert.That (methodInfo.IsPrivate);
      Assert.That (propertyInfo.GetGetMethod (true).IsPrivate);
      Assert.That (propertyInfo.GetSetMethod (true).IsPrivate);
      Assert.That (eventInfo.GetAddMethod (true).IsPrivate);
      Assert.That (eventInfo.GetRemoveMethod (true).IsPrivate);
    }

    [Test]
    public void IntroducedSpecialNameAttribute ()
    {
      Type t = CreateMixedType (typeof (NullTarget), typeof (MixinAddingSpecialNameMethod));

      var specialNameMethod = t.GetMethod (typeof (IMixinAddingSpecialNameMethod).FullName + ".MethodWithSpecialName", c_nonPublicFlags);
      Assert.That (specialNameMethod.IsSpecialName, Is.True);

      var publicSpecialNameMethod = t.GetMethod ("PublicMethodWithSpecialName");
      Assert.That (publicSpecialNameMethod.IsSpecialName, Is.True);
    }

    [Test]
    public void IntroducedMemberAttributes_Method ()
    {
      Type t = CreateMixedType (typeof (NullTarget), typeof (MixinIntroducingMembersWithDifferentVisibilities));
      
      MethodInfo methodInfo = t.GetMethod ("MethodWithPublicVisibility", BindingFlags.Public | BindingFlags.Instance);
      var methodAttribute = (IntroducedMemberAttribute) methodInfo.GetCustomAttributes (typeof (IntroducedMemberAttribute), false).Single ();

      Assert.That (methodAttribute.Mixin, Is.SameAs (typeof (MixinIntroducingMembersWithDifferentVisibilities)));
      Assert.That (methodAttribute.MixinMemberName, Is.EqualTo ("MethodWithPublicVisibility"));
      Assert.That (methodAttribute.IntroducedInterface, Is.SameAs (typeof (IMixinIntroducingMembersWithDifferentVisibilities)));
      Assert.That (methodAttribute.InterfaceMemberName, Is.EqualTo ("MethodWithPublicVisibility"));
    }

    [Test]
    public void IntroducedMemberAttributes_Property ()
    {
      Type t = CreateMixedType (typeof (NullTarget), typeof (MixinIntroducingMembersWithDifferentVisibilities));

      PropertyInfo propertyInfo = t.GetProperty ("PropertyWithPublicVisibility", BindingFlags.Public | BindingFlags.Instance);
      var propertyAttribute = (IntroducedMemberAttribute) propertyInfo.GetCustomAttributes (typeof (IntroducedMemberAttribute), false).Single ();

      Assert.That (propertyAttribute.Mixin, Is.SameAs (typeof (MixinIntroducingMembersWithDifferentVisibilities)));
      Assert.That (propertyAttribute.MixinMemberName, Is.EqualTo ("PropertyWithPublicVisibility"));
      Assert.That (propertyAttribute.IntroducedInterface, Is.SameAs (typeof (IMixinIntroducingMembersWithDifferentVisibilities)));
      Assert.That (propertyAttribute.InterfaceMemberName, Is.EqualTo ("PropertyWithPublicVisibility"));
    }

    [Test]
    public void IntroducedMemberAttributes_Event ()
    {
      Type t = CreateMixedType (typeof (NullTarget), typeof (MixinIntroducingMembersWithDifferentVisibilities));

      EventInfo eventInfo = t.GetEvent ("EventWithPublicVisibility", BindingFlags.Public | BindingFlags.Instance);
      var eventAttribute = (IntroducedMemberAttribute) eventInfo.GetCustomAttributes (typeof (IntroducedMemberAttribute), false).Single ();

      Assert.That (eventAttribute.Mixin, Is.SameAs (typeof (MixinIntroducingMembersWithDifferentVisibilities)));
      Assert.That (eventAttribute.MixinMemberName, Is.EqualTo ("EventWithPublicVisibility"));
      Assert.That (eventAttribute.IntroducedInterface, Is.SameAs (typeof (IMixinIntroducingMembersWithDifferentVisibilities)));
      Assert.That (eventAttribute.InterfaceMemberName, Is.EqualTo ("EventWithPublicVisibility"));
    }

    [Test]
    public void IntroducedMemberAttributes_Explicit ()
    {
      Type t = CreateMixedType (typeof (NullTarget), typeof (MixinWithExplicitImplementation));

      MethodInfo methodInfo = t.GetMethod (typeof (IExplicit).FullName + ".Explicit", c_nonPublicFlags);
      var methodAttribute = (IntroducedMemberAttribute) methodInfo.GetCustomAttributes (typeof (IntroducedMemberAttribute), false).Single ();

      Assert.That (methodAttribute.Mixin, Is.SameAs (typeof (MixinWithExplicitImplementation)));
      Assert.That (methodAttribute.MixinMemberName, Is.EqualTo (typeof (IExplicit).FullName + ".Explicit"));
      Assert.That (methodAttribute.IntroducedInterface, Is.SameAs (typeof (IExplicit)));
      Assert.That (methodAttribute.InterfaceMemberName, Is.EqualTo ("Explicit"));
    }
  }
}
