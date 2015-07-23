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
using System.Linq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Mixins.Implementation;
using Remotion.Validation.Mixins.UnitTests.Implementation.TestDomain;

namespace Remotion.Validation.Mixins.UnitTests.Implementation
{
  [TestFixture]
  public class MixedInvolvedTypeProviderDecoratorTest
  {
    private IInvolvedTypeProvider _mixedInvolvedTypeProvider;

    [SetUp]
    public void SetUp ()
    {
      var compoundValidationTypeFilter = new CompoundValidationTypeFilter (
          new IValidationTypeFilter[] { new LoadFilteredValidationTypeFilter(), new MixedLoadFilteredValidationTypeFilter() });
      _mixedInvolvedTypeProvider = new MixedInvolvedTypeProviderDecorator (
          new InvolvedTypeProvider (compoundValidationTypeFilter),
          compoundValidationTypeFilter);
    }

    [Test]
    public void GetAffectedType_WithMixinHierarchy ()
    {
      var result = _mixedInvolvedTypeProvider.GetTypes (typeof (DerivedConcreteTypeForMixin)).SelectMany (t => t).ToList();

      Assert.That (result[0], Is.EqualTo (typeof (IBaseConcreteTypeForMixin)));
      Assert.That (result[1], Is.EqualTo (typeof (BaseConcreteTypeForMixin)));
      Assert.That (result[2], Is.EqualTo (typeof (IDerivedConcreteTypeForMixin)));
      Assert.That (result[3], Is.EqualTo (typeof (DerivedConcreteTypeForMixin)));
      Assert.That (result[4], Is.EqualTo (typeof (IBaseBaseIntroducedFromMixinForDerivedType1)));
      Assert.That (result[5], Is.EqualTo (typeof (IBaseIntroducedFromMixinForDerivedTypeA1)));
      Assert.That (result[6], Is.EqualTo (typeof (IBaseIntroducedFromMixinForDerivedTypeB1)));
      Assert.That (result[7], Is.EqualTo (typeof (IIntroducedFromMixinForBaseType)));
      Assert.That (result[8], Is.EqualTo (typeof (IIntroducedFromMixinForDerivedType1)));
      Assert.That (result[9], Is.EqualTo (typeof (IIntroducedFromMixinForDerivedType2)));
      Assert.That (result[10].Name, Is.StringStarting ("DerivedConcreteTypeForMixin_AssembledTypeProxy_"));
      Assert.That (result[11], Is.EqualTo (typeof (BaseMixinForDerivedType)));
      Assert.That (result[12], Is.EqualTo (typeof (MixinForBaseType)));
      Assert.That (result[13], Is.EqualTo (typeof (MixinForDerivedType1)));
      Assert.That (result[14], Is.EqualTo (typeof (MixinForDerivedType2)));
    }
  }
}