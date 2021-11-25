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
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.UnitTests.TestDomain;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class AttributeBasedValidationRuleCollectorTest
  {
    private Type _validatedType;
    private IAddingPropertyValidationRuleCollector[] _addingPropertyValidationRuleCollector;
    private IPropertyMetaValidationRuleCollector[] _propertyMetaValidationRuleCollector;
    private IRemovingPropertyValidationRuleCollector[] _removingPropertyValidationRuleCollector;
    private AttributeBasedValidationRuleCollector _collector;

    [SetUp]
    public void SetUp ()
    {
      _validatedType = typeof(Customer);
      _addingPropertyValidationRuleCollector = new IAddingPropertyValidationRuleCollector[0];
      _propertyMetaValidationRuleCollector = new IPropertyMetaValidationRuleCollector[0];
      _removingPropertyValidationRuleCollector = new IRemovingPropertyValidationRuleCollector[0];

      _collector = new AttributeBasedValidationRuleCollector(
          _validatedType,
          _addingPropertyValidationRuleCollector,
          _propertyMetaValidationRuleCollector,
          _removingPropertyValidationRuleCollector);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_collector.ValidatedType, Is.SameAs(_validatedType));
      Assert.That(_collector.AddedPropertyRules, Is.EqualTo(_addingPropertyValidationRuleCollector));
      Assert.That(_collector.PropertyMetaValidationRules, Is.EqualTo(_propertyMetaValidationRuleCollector));
      Assert.That(_collector.RemovedPropertyRules, Is.EqualTo(_removingPropertyValidationRuleCollector));
    }
  }
}