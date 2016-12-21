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
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class OverrideTest : CodeGenerationBaseTest
  {
    [Test]
    public void OverrideMixinMethod ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers));
      var comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.That (comAsIAbstractMixin, Is.Not.Null);
      Assert.That (comAsIAbstractMixin.ImplementedMethod (), Is.EqualTo ("MixinWithAbstractMembers.ImplementedMethod-ClassOverridingMixinMembers.AbstractMethod-25"));
    }

    [Test]
    public void OverrideMixinProperty ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers));
      var comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.That (comAsIAbstractMixin, Is.Not.Null);
      Assert.That (comAsIAbstractMixin.ImplementedProperty (), Is.EqualTo ("MixinWithAbstractMembers.ImplementedProperty-ClassOverridingMixinMembers.AbstractProperty"));
    }

    [Test]
    public void OverrideMixinEvent ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers));
      var comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.That (comAsIAbstractMixin, Is.Not.Null);
      Assert.That (comAsIAbstractMixin.ImplementedEvent (), Is.EqualTo ("MixinWithAbstractMembers.ImplementedEvent"));
    }

    [Test]
    public void DoubleOverride ()
    {
      ClassOverridingSingleMixinMethod com = CreateMixedObject<ClassOverridingSingleMixinMethod> (typeof (MixinOverridingClassMethod));
      var comAsIAbstractMixin = com as IMixinOverridingClassMethod;
      Assert.That (comAsIAbstractMixin, Is.Not.Null);
      Assert.That (comAsIAbstractMixin.AbstractMethod (25), Is.EqualTo ("ClassOverridingSingleMixinMethod.AbstractMethod-25"));
      Assert.That (com.OverridableMethod (13), Is.EqualTo ("MixinOverridingClassMethod.OverridableMethod-13"));
    }

    [Test]
    public void ClassOverridingInheritedMixinMethod ()
    {
      ClassOverridingInheritedMixinMethod coimm = ObjectFactory.Create<ClassOverridingInheritedMixinMethod> (ParamList.Empty);
      var mixin = Mixin.Get<MixinWithInheritedMethod> (coimm);
      Assert.That (mixin.InvokeInheritedMethods (), Is.EqualTo ("ClassOverridingInheritedMixinMethod.ProtectedInheritedMethod-"
                                                                + "ClassOverridingInheritedMixinMethod.ProtectedInternalInheritedMethod-"
                                                                + "ClassOverridingInheritedMixinMethod.PublicInheritedMethod"));
    }

    [Test]
    public void ClassWithProtectedOverrider ()
    {
      ClassOverridingMixinMembersProtected com = CreateMixedObject<ClassOverridingMixinMembersProtected> (typeof (MixinWithAbstractMembers));
      var comAsIAbstractMixin = com as IMixinWithAbstractMembers;

      Assert.That (comAsIAbstractMixin, Is.Not.Null);
      Assert.That (comAsIAbstractMixin.ImplementedMethod (), Is.EqualTo ("MixinWithAbstractMembers.ImplementedMethod-ClassOverridingMixinMembersProtected.AbstractMethod-25"));
      Assert.That (comAsIAbstractMixin.ImplementedProperty (), Is.EqualTo ("MixinWithAbstractMembers.ImplementedProperty-ClassOverridingMixinMembersProtected.AbstractProperty"));
      Assert.That (comAsIAbstractMixin.ImplementedEvent (), Is.EqualTo ("MixinWithAbstractMembers.ImplementedEvent"));
    }
  }
}
