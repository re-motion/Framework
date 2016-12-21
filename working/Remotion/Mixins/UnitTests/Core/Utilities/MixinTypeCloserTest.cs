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
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Utilities
{
  [TestFixture]
  public class MixinTypeCloserTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The target class must not contain generic parameters.\r\n"
        + "Parameter name: targetClass")]
    public void Initialization_GenericTypeDefinition ()
    {
      new MixinTypeCloser (typeof (GenericTargetClass<>));
    }

    [Test]
    public void GetClosedMixinType_NonGenericMixin()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType3));
      Type t = instantiator.GetClosedMixinType (typeof (object));
      Assert.That (t, Is.EqualTo (typeof (object)));
    }

    [Test]
    public void GetClosedMixinType_BindToConstraints ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType3));
      Type t = instantiator.GetClosedMixinType (typeof (BT3Mixin6<,>));
      Assert.That (t, Is.EqualTo (typeof (BT3Mixin6<IBT3Mixin6TargetCallDependencies, IBT3Mixin6NextCallDependencies>)));
    }

    [Test]
    public void GetClosedMixinType_BindToTargetType ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType3));
      Type t = instantiator.GetClosedMixinType (typeof (BT3Mixin3<,>));
      Assert.That (t, Is.EqualTo (typeof (BT3Mixin3<BaseType3, IBaseType33>)));
    }

    [Test]
    public void GetClosedMixinType_BindToTargetParameter ()
    {
      var instantiator = new MixinTypeCloser (typeof (GenericTargetClass<string>));
      Type t = instantiator.GetClosedMixinType (typeof (GenericMixin<>));
      Assert.That (t, Is.EqualTo (typeof (GenericMixin<string>)));
    }

    [Test]
    [ExpectedException (typeof(ConfigurationException), ExpectedMessage = "Cannot bind generic parameter 'T' of mixin "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixin`1[T]' to generic parameter number 0 of target type "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1': The target type does not have so many parameters.")]
    public void GetClosedMixinType_UnmappablePosition ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType1));
      instantiator.GetClosedMixinType (typeof (GenericMixin<>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Type parameter 'T2' of mixin "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithPositionalAfterTargetBoundParameter`2[T1,T2]' applied to target class "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericTargetClass`1[System.String]' has a BindToGenericTargetParameterAttribute, but it is not at "
        + "the front of the generic parameters. The type parameters with BindToGenericTargetParameterAttributes must all be at the front, before any "
        + "other generic parameters.")]
    public void GetClosedMixinType_PositionalAfterFirstNonPositional ()
    {
      var instantiator = new MixinTypeCloser (typeof (GenericTargetClass<string>));
      instantiator.GetClosedMixinType (typeof (GenericMixinWithPositionalAfterTargetBoundParameter<,>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Type parameter 'T' of mixin "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithPositionalAndTargetBoundParameter`1[T]' has more than one binding specification.")]
    public void GetClosedMixinType_PositionalAndTargetBound ()
    {
      var instantiator = new MixinTypeCloser (typeof (GenericTargetClass<string>));
      instantiator.GetClosedMixinType (typeof (GenericMixinWithPositionalAndTargetBoundParameter<>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Type parameter 'T' of mixin "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithPositionalAndConstraintBoundParameter`1[T]' has more than one binding specification.")]
    public void GetClosedMixinType_PositionalAndConstraintBound ()
    {
      var instantiator = new MixinTypeCloser (typeof (GenericTargetClass<string>));
      instantiator.GetClosedMixinType (typeof (GenericMixinWithPositionalAndConstraintBoundParameter<>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The generic mixin "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithUnsatisfiableConstraints`1[T]' applied to class "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType3' cannot be automatically closed because the constraints of its type parameter 'T' cannot "
        + "be unified into a single type. The generic type parameter has inconclusive constraints 'System.ICloneable' "
        + "and 'System.Runtime.Serialization.ISerializable', which cannot be unified into a single type.")]
    public void GetClosedMixinType_MixinWithUnsatisfiableConstraintsThrows ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType3));
      instantiator.GetClosedMixinType (typeof (GenericMixinWithUnsatisfiableConstraints<>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The generic mixin "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithoutBindingInformation`1[T]' applied to class "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType3' cannot be automatically closed because its type parameter 'T' does not have any binding "
        + "information. Apply the BindToTargetTypeAttribute, BindToConstraintsAttribute, or BindToGenericTargetParameterAttribute to the type parameter or specify the parameter's "
        + "instantiation when configuring the mixin for the target class.")]
    public void GetClosedMixinType_MixinWithoutBindingInformationThrows ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType3));
      instantiator.GetClosedMixinType (typeof (GenericMixinWithoutBindingInformation<>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Cannot close the generic mixin type "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.BT3Mixin3`2[TTarget,TNext]' applied to class 'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1' - "
        + "the inferred type arguments violate the generic parameter constraints. Specify the arguments manually, modify the parameter binding "
        + "specification, or relax the constraints. GenericArguments[0], 'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType1', on "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.BT3Mixin3`2[TTarget,TNext]' violates the constraint of type 'TTarget'.")]
    public void GetClosedMixinType_BindToInvalidTargetType ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType1));
      instantiator.GetClosedMixinType (typeof (BT3Mixin3<,>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Type parameter 'T' of mixin "
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithDoubleBindingInformation`1[T]' has more than one binding specification.")]
    public void GetClosedMixinType_BindToTargetAndConstraints ()
    {
      var instantiator = new MixinTypeCloser (typeof (BaseType1));
      instantiator.GetClosedMixinType (typeof (GenericMixinWithDoubleBindingInformation<>));
    }
  }
}
