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
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation
{
  public class ValidatingVisitor : IDefinitionVisitor
  {
    private readonly IValidationLog _validationLog;

    private readonly List<IValidationRule<TargetClassDefinition>> _targetClassRules = new();
    private readonly List<IValidationRule<MixinDefinition>> _mixinRules = new();
    private readonly List<IValidationRule<InterfaceIntroductionDefinition>> _interfaceIntroductionRules = new();
    private readonly List<IValidationRule<NonInterfaceIntroductionDefinition>> _nonIntroductedInterfaceRules = new();
    private readonly List<IValidationRule<MethodIntroductionDefinition>> _methodIntroductionRules = new();
    private readonly List<IValidationRule<PropertyIntroductionDefinition>> _propertyIntroductionRules = new();
    private readonly List<IValidationRule<EventIntroductionDefinition>> _eventIntroductionRules = new();
    private readonly List<IValidationRule<MethodDefinition>> _methodRules = new();
    private readonly List<IValidationRule<PropertyDefinition>> _propertyRules = new();
    private readonly List<IValidationRule<EventDefinition>> _eventRules = new();
    private readonly List<IValidationRule<RequiredTargetCallTypeDefinition>> _requiredTargetCallTypeRules = new();
    private readonly List<IValidationRule<RequiredNextCallTypeDefinition>> _requiredNextCallTypeRules = new();
    private readonly List<IValidationRule<RequiredMixinTypeDefinition>> _requiredMixinTypeRules = new();
    private readonly List<IValidationRule<RequiredMethodDefinition>> _requiredMethodRules = new();
    private readonly List<IValidationRule<TargetCallDependencyDefinition>> _targetCallDependencyRules = new();
    private readonly List<IValidationRule<NextCallDependencyDefinition>> _nextCallDependencyRules = new();
    private readonly List<IValidationRule<MixinDependencyDefinition>> _mixinDependencyRules = new();
    private readonly List<IValidationRule<ComposedInterfaceDependencyDefinition>> _composedInterfaceDependencyRules = new();
    private readonly List<IValidationRule<AttributeDefinition>> _attributeRules = new();
    private readonly List<IValidationRule<AttributeIntroductionDefinition>> _attributeIntroductionRules = new();
    private readonly List<IValidationRule<NonAttributeIntroductionDefinition>> _nonAttributeIntroductionRules = new();
    private readonly List<IValidationRule<SuppressedAttributeIntroductionDefinition>> _suppressedAttributeIntroductionRules = new();

    public ValidatingVisitor (IValidationLog validationLog)
    {
      ArgumentNullException.ThrowIfNull(validationLog);
      _validationLog = validationLog;
    }

    public IList<IValidationRule<TargetClassDefinition>> TargetClassRules
    {
      get { return _targetClassRules; }
    }

    public IList<IValidationRule<MixinDefinition>> MixinRules
    {
      get { return _mixinRules; }
    }

    public IList<IValidationRule<InterfaceIntroductionDefinition>> InterfaceIntroductionRules
    {
      get { return _interfaceIntroductionRules; }
    }

    public IList<IValidationRule<NonInterfaceIntroductionDefinition>> NonIntroductedInterfaceRules
    {
      get { return _nonIntroductedInterfaceRules; }
    }

    public IList<IValidationRule<MethodIntroductionDefinition>> MethodIntroductionRules
    {
      get { return _methodIntroductionRules; }
    }

    public IList<IValidationRule<PropertyIntroductionDefinition>> PropertyIntroductionRules
    {
      get { return _propertyIntroductionRules; }
    }

    public IList<IValidationRule<EventIntroductionDefinition>> EventIntroductionRules
    {
      get { return _eventIntroductionRules; }
    }

    public IList<IValidationRule<MethodDefinition>> MethodRules
    {
      get { return _methodRules; }
    }

    public IList<IValidationRule<PropertyDefinition>> PropertyRules
    {
      get { return _propertyRules; }
    }

    public IList<IValidationRule<EventDefinition>> EventRules
    {
      get { return _eventRules; }
    }

    public IList<IValidationRule<RequiredTargetCallTypeDefinition>> RequiredTargetCallTypeRules
    {
      get { return _requiredTargetCallTypeRules; }
    }

    public IList<IValidationRule<RequiredNextCallTypeDefinition>> RequiredNextCallTypeRules
    {
      get { return _requiredNextCallTypeRules; }
    }

    public IList<IValidationRule<RequiredMixinTypeDefinition>> RequiredMixinTypeRules
    {
      get { return _requiredMixinTypeRules; }
    }

    public IList<IValidationRule<RequiredMethodDefinition>> RequiredMethodRules
    {
      get { return _requiredMethodRules; }
    }

    public IList<IValidationRule<TargetCallDependencyDefinition>> TargetCallDependencyRules
    {
      get { return _targetCallDependencyRules; }
    }

    public IList<IValidationRule<NextCallDependencyDefinition>> NextCallDependencyRules
    {
      get { return _nextCallDependencyRules; }
    }

    public IList<IValidationRule<MixinDependencyDefinition>> MixinDependencyRules
    {
      get { return _mixinDependencyRules; }
    }

    public IList<IValidationRule<AttributeDefinition>> AttributeRules
    {
      get { return _attributeRules; }
    }

    public IList<IValidationRule<AttributeIntroductionDefinition>> AttributeIntroductionRules
    {
      get { return _attributeIntroductionRules; }
    }

    public IList<IValidationRule<NonAttributeIntroductionDefinition>> NonAttributeIntroductionRules
    {
      get { return _nonAttributeIntroductionRules; }
    }

    public IList<IValidationRule<SuppressedAttributeIntroductionDefinition>> SuppressedAttributeIntroductionRules
    {
      get { return _suppressedAttributeIntroductionRules; }
    }

    public void Visit (TargetClassDefinition targetClass)
    {
      ArgumentNullException.ThrowIfNull(targetClass);
      CheckRules(_targetClassRules, targetClass);
    }

    public void Visit (MixinDefinition mixin)
    {
      ArgumentNullException.ThrowIfNull(mixin);
      CheckRules(_mixinRules, mixin);
    }

    public void Visit (InterfaceIntroductionDefinition interfaceIntroduction)
    {
      ArgumentNullException.ThrowIfNull(interfaceIntroduction);
      CheckRules(_interfaceIntroductionRules, interfaceIntroduction);
    }

    public void Visit (NonInterfaceIntroductionDefinition nonIntroductionDefinition)
    {
      ArgumentNullException.ThrowIfNull(nonIntroductionDefinition);
      CheckRules(_nonIntroductedInterfaceRules, nonIntroductionDefinition);
    }

    public void Visit (MethodIntroductionDefinition methodIntroduction)
    {
      ArgumentNullException.ThrowIfNull(methodIntroduction);
      CheckRules(_methodIntroductionRules, methodIntroduction);
    }

    public void Visit (PropertyIntroductionDefinition propertyIntroduction)
    {
      ArgumentNullException.ThrowIfNull(propertyIntroduction);
      CheckRules(_propertyIntroductionRules, propertyIntroduction);
    }

    public void Visit (EventIntroductionDefinition eventIntroduction)
    {
      ArgumentNullException.ThrowIfNull(eventIntroduction);
      CheckRules(_eventIntroductionRules, eventIntroduction);
    }

    public void Visit (MethodDefinition method)
    {
      ArgumentNullException.ThrowIfNull(method);
      CheckRules(_methodRules, method);
    }

    public void Visit (PropertyDefinition property)
    {
      ArgumentNullException.ThrowIfNull(property);
      CheckRules(_propertyRules, property);
    }

    public void Visit (EventDefinition eventDefinition)
    {
      ArgumentNullException.ThrowIfNull(eventDefinition);
      CheckRules(_eventRules, eventDefinition);
    }

    public void Visit (RequiredTargetCallTypeDefinition requiredTargetCallType)
    {
      ArgumentNullException.ThrowIfNull(requiredTargetCallType);
      CheckRules(_requiredTargetCallTypeRules, requiredTargetCallType);
    }

    public void Visit (RequiredNextCallTypeDefinition requiredNextCallType)
    {
      ArgumentNullException.ThrowIfNull(requiredNextCallType);
      CheckRules(_requiredNextCallTypeRules, requiredNextCallType);
    }

    public void Visit (RequiredMixinTypeDefinition requiredMixinType)
    {
      ArgumentNullException.ThrowIfNull(requiredMixinType);
      CheckRules(_requiredMixinTypeRules, requiredMixinType);
    }

    public void Visit (RequiredMethodDefinition requiredMethod)
    {
      ArgumentNullException.ThrowIfNull(requiredMethod);
      CheckRules(_requiredMethodRules, requiredMethod);
    }

    public void Visit (TargetCallDependencyDefinition dependency)
    {
      ArgumentNullException.ThrowIfNull(dependency);
      CheckRules(_targetCallDependencyRules, dependency);
    }

    public void Visit (NextCallDependencyDefinition dependency)
    {
      ArgumentNullException.ThrowIfNull(dependency);
      CheckRules(_nextCallDependencyRules, dependency);
    }

    public void Visit (MixinDependencyDefinition dependency)
    {
      ArgumentNullException.ThrowIfNull(dependency);
      CheckRules(_mixinDependencyRules, dependency);
    }

    public void Visit (ComposedInterfaceDependencyDefinition dependency)
    {
      ArgumentNullException.ThrowIfNull(dependency);
      CheckRules(_composedInterfaceDependencyRules, dependency);
    }

    public void Visit (AttributeDefinition attribute)
    {
      ArgumentNullException.ThrowIfNull(attribute);
      CheckRules(_attributeRules, attribute);
    }

    public void Visit (AttributeIntroductionDefinition attributeIntroduction)
    {
      ArgumentNullException.ThrowIfNull(attributeIntroduction);
      CheckRules(_attributeIntroductionRules, attributeIntroduction);
    }

    public void Visit (NonAttributeIntroductionDefinition nonAttributeIntroduction)
    {
      ArgumentNullException.ThrowIfNull(nonAttributeIntroduction);
      CheckRules(_nonAttributeIntroductionRules, nonAttributeIntroduction);
    }

    public void Visit (SuppressedAttributeIntroductionDefinition suppressedAttributeIntroduction)
    {
      ArgumentNullException.ThrowIfNull(suppressedAttributeIntroduction);
      CheckRules(_suppressedAttributeIntroductionRules, suppressedAttributeIntroduction);
    }

    private void CheckRules<TDefinition> (IEnumerable<IValidationRule<TDefinition>> rules, TDefinition definition) where TDefinition : IVisitableDefinition
    {
      _validationLog.ValidationStartsFor(definition);
      foreach (IValidationRule<TDefinition> rule in rules)
      {
        try
        {
          rule.Execute(this, definition, _validationLog);
        }
        catch (Exception ex)
        {
          _validationLog.UnexpectedException(rule, ex);
        }
      }
      _validationLog.ValidationEndsFor(definition);
    }
  }
}
