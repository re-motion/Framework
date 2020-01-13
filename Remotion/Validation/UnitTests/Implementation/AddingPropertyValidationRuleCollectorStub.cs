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
using System.Collections.Generic;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Implementation
{
  public class AddingPropertyValidationRuleCollectorStub : IAddingPropertyValidationRuleCollector
  {
    private class Collector : IValidationRuleCollector<DomainType>
    {
      public Type ValidatedType => throw new NotImplementedException();

      public IReadOnlyCollection<IAddingPropertyValidationRuleCollector> AddedPropertyRules => throw new NotImplementedException();

      public IReadOnlyCollection<IPropertyMetaValidationRuleCollector> PropertyMetaValidationRules => throw new NotImplementedException();

      public IReadOnlyCollection<IRemovingPropertyValidationRuleCollector> RemovedPropertyRules => throw new NotImplementedException();

      public IReadOnlyCollection<IAddingObjectValidationRuleCollector> AddedObjectRules => throw new NotImplementedException();

      public IReadOnlyCollection<IObjectMetaValidationRuleCollector> ObjectMetaValidationRules => throw new NotImplementedException();

      public IReadOnlyCollection<IRemovingObjectValidationRuleCollector> RemovedObjectRules => throw new NotImplementedException();
    }

    public class DomainType
    {
      public string DomainProperty { get; set; }
    }

    private readonly List<IPropertyValidator> _validators = new List<IPropertyValidator>();

    public AddingPropertyValidationRuleCollectorStub ()
    {
    }

    public IEnumerable<IPropertyValidator> Validators => _validators;

    public void SetCondition<TValidatedType> (Func<TValidatedType, bool> predicate) => throw new NotImplementedException();

    public void RegisterValidator (Func<PropertyValidationRuleInitializationParameters, IPropertyValidator> validatorFactory)
      => _validators.Add (validatorFactory (new PropertyValidationRuleInitializationParameters (new InvariantValidationMessage ("Fake Message"))));

    public string RuleSet => throw new NotImplementedException();

    public Type CollectorType => typeof (Collector);

    public IPropertyInformation Property
    {
      get { return PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty ((DomainType _) => _.DomainProperty)); }
    }

    public bool IsRemovable => throw new NotImplementedException();


    public void SetRemovable ()
    {
      throw new NotImplementedException();
    }

    public void ApplyRemoveValidatorRegistrations (IPropertyValidatorExtractor propertyValidatorExtractor)
    {
      throw new NotImplementedException();
    }

    public void ApplyCondition (Func<object, bool> predicate)
    {
      throw new NotImplementedException();
    }

    public IValidationRule CreateValidationRule (IValidationMessageFactory validationMessageFactory)
    {
      throw new NotImplementedException();
    }
  }
}