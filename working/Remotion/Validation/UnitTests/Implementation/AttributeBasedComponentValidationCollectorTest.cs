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
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class AttributeBasedComponentValidationCollectorTest
  {
    private Type _validatedType;
    private AddingComponentPropertyRule[] _addingPropertyRules;
    private AddingComponentPropertyMetaValidationRule[] _addingMetaPropertyRules;
    private RemovingComponentPropertyRule[] _removingPropertyRegistration;
    private AttributeBasedComponentValidationCollector _collector;

    [SetUp]
    public void SetUp ()
    {
      _validatedType = typeof (Customer);
      _addingPropertyRules = new AddingComponentPropertyRule[0];
      _addingMetaPropertyRules = new AddingComponentPropertyMetaValidationRule[0];
      _removingPropertyRegistration = new RemovingComponentPropertyRule[0];

      _collector = new AttributeBasedComponentValidationCollector (
          _validatedType,
          _addingPropertyRules,
          _addingMetaPropertyRules,
          _removingPropertyRegistration);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_collector.ValidatedType, Is.SameAs (_validatedType));
      Assert.That (_collector.AddedPropertyRules, Is.EqualTo (_addingPropertyRules));
      Assert.That (_collector.AddedPropertyMetaValidationRules, Is.EqualTo (_addingMetaPropertyRules));
      Assert.That (_collector.RemovedPropertyRules, Is.EqualTo (_removingPropertyRegistration));
    }
  }
}