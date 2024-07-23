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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Utilities
{
  [TestFixture]
  public class MixinTypeCloserTest
  {
    [Test]
    public void Initialization_GenericTypeDefinition ()
    {
      Assert.That(
          () => new MixinTypeCloser(typeof(GenericTargetClass<>)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The target class must not contain generic parameters.",
                  "targetClass"));
    }

    [Test]
    public void GetClosedMixinType_NonGenericMixin ()
    {
      var instantiator = new MixinTypeCloser(typeof(BaseType3));
      Type t = instantiator.GetClosedMixinType(typeof(object));
      Assert.That(t, Is.EqualTo(typeof(object)));
    }

    [Test]
    public void GetClosedMixinType_BindToConstraints ()
    {
      var instantiator = new MixinTypeCloser(typeof(BaseType3));
      Type t = instantiator.GetClosedMixinType(typeof(BT3Mixin6<,>));
      Assert.That(t, Is.EqualTo(typeof(BT3Mixin6<IBT3Mixin6TargetCallDependencies, IBT3Mixin6NextCallDependencies>)));
    }

    [Test]
    public void GetClosedMixinType_BindToTargetType ()
    {
      var instantiator = new MixinTypeCloser(typeof(BaseType3));
      Type t = instantiator.GetClosedMixinType(typeof(BT3Mixin3<,>));
      Assert.That(t, Is.EqualTo(typeof(BT3Mixin3<BaseType3, IBaseType33>)));
    }

    [Test]
    public void GetClosedMixinType_BindToTargetParameter ()
    {
      var instantiator = new MixinTypeCloser(typeof(GenericTargetClass<string>));
      Type t = instantiator.GetClosedMixinType(typeof(GenericMixin<>));
      Assert.That(t, Is.EqualTo(typeof(GenericMixin<string>)));
    }

    [Test]
    public void GetClosedMixinType_UnmappablePosition ()
    {
      var instantiator = new MixinTypeCloser(typeof(BaseType1));
      Assert.That(
          () => instantiator.GetClosedMixinType(typeof(GenericMixin<>)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo(
                  "Cannot bind generic parameter 'T' of mixin "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixin`1[T]' to generic parameter number 0 of target type "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1': The target type does not have so many parameters."));
    }

    [Test]
    public void GetClosedMixinType_PositionalAfterFirstNonPositional ()
    {
      var instantiator = new MixinTypeCloser(typeof(GenericTargetClass<string>));
      Assert.That(
          () => instantiator.GetClosedMixinType(typeof(GenericMixinWithPositionalAfterTargetBoundParameter<,>)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo(
                  "Type parameter 'T2' of mixin "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithPositionalAfterTargetBoundParameter`2[T1,T2]' applied to target class "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericTargetClass`1[System.String]' has a BindToGenericTargetParameterAttribute, but it is not at "
                  + "the front of the generic parameters. The type parameters with BindToGenericTargetParameterAttributes must all be at the front, before any "
                  + "other generic parameters."));
    }

    [Test]
    public void GetClosedMixinType_PositionalAndTargetBound ()
    {
      var instantiator = new MixinTypeCloser(typeof(GenericTargetClass<string>));
      Assert.That(
          () => instantiator.GetClosedMixinType(typeof(GenericMixinWithPositionalAndTargetBoundParameter<>)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo(
                  "Type parameter 'T' of mixin "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithPositionalAndTargetBoundParameter`1[T]' has more than one binding specification."));
    }

    [Test]
    public void GetClosedMixinType_PositionalAndConstraintBound ()
    {
      var instantiator = new MixinTypeCloser(typeof(GenericTargetClass<string>));
      Assert.That(
          () => instantiator.GetClosedMixinType(typeof(GenericMixinWithPositionalAndConstraintBoundParameter<>)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo(
                  "Type parameter 'T' of mixin "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithPositionalAndConstraintBoundParameter`1[T]' has more than one binding specification."));
    }

    [Test]
    public void GetClosedMixinType_MixinWithUnsatisfiableConstraintsThrows ()
    {
      var instantiator = new MixinTypeCloser(typeof(BaseType3));
      Assert.That(
          () => instantiator.GetClosedMixinType(typeof(GenericMixinWithUnsatisfiableConstraints<>)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo(
                  "The generic mixin "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithUnsatisfiableConstraints`1[T]' applied to class "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType3' cannot be automatically closed because the constraints of its type parameter 'T' cannot "
                  + "be unified into a single type. The generic type parameter has inconclusive constraints 'Remotion.Mixins.UnitTests.Core.TestDomain.IUnsatisfiableInterface' "
                  + "and 'System.ICloneable', which cannot be unified into a single type."));
    }

    [Test]
    public void GetClosedMixinType_MixinWithoutBindingInformationThrows ()
    {
      var instantiator = new MixinTypeCloser(typeof(BaseType3));
      Assert.That(
          () => instantiator.GetClosedMixinType(typeof(GenericMixinWithoutBindingInformation<>)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo(
                  "The generic mixin "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithoutBindingInformation`1[T]' applied to class "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType3' cannot be automatically closed because its type parameter 'T' does not have any binding information. "
                  + "Apply the BindToTargetTypeAttribute, BindToConstraintsAttribute, or BindToGenericTargetParameterAttribute to the type parameter or specify the parameter's "
                  + "instantiation when configuring the mixin for the target class."));
    }

    [Test]
    public void GetClosedMixinType_BindToInvalidTargetType ()
    {
      var instantiator = new MixinTypeCloser(typeof(BaseType1));
      Assert.That(
          () => instantiator.GetClosedMixinType(typeof(BT3Mixin3<,>)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo(
                  "Cannot close the generic mixin type "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.BT3Mixin3`2[TTarget,TNext]' applied to class 'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1' - "
                  + "the inferred type arguments violate the generic parameter constraints. Specify the arguments manually, modify the parameter binding "
                  + "specification, or relax the constraints. GenericArguments[0], 'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1', on "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.BT3Mixin3`2[TTarget,TNext]' violates the constraint of type 'TTarget'."));
    }

    [Test]
    public void GetClosedMixinType_BindToTargetAndConstraints ()
    {
      var instantiator = new MixinTypeCloser(typeof(BaseType1));
      Assert.That(
          () => instantiator.GetClosedMixinType(typeof(GenericMixinWithDoubleBindingInformation<>)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo(
                  "Type parameter 'T' of mixin "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithDoubleBindingInformation`1[T]' has more than one binding specification."));
    }
  }
}
