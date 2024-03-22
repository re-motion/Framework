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
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Mixins.Implementation
{
  ///<summary>
  /// Implements the <see cref="IValidationRuleCollectorValidator"/> interface an verifies that the <see cref="IValidationRuleCollector.ValidatedType"/> of an
  /// <see cref="IValidationRuleCollector"/> instance is not a mixin.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IValidationRuleCollectorValidator), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class CheckNoMixinValidationRuleCollectorValidator : IValidationRuleCollectorValidator
  {
    public CheckNoMixinValidationRuleCollectorValidator ()
    {
    }

    public void CheckValid (IValidationRuleCollector collector)
    {
      ArgumentUtility.CheckNotNull("collector", collector);

      if (Remotion.Mixins.Utilities.ReflectionUtility.IsMixinType(collector.ValidatedType))
      {
        throw new NotSupportedException(
              string.Format(
                  "Validation rules for type '{0}' are not supported. If validation rules should be defined for mixins, "
                  +"please ensure to apply the rules to 'ITargetInterface' or 'IIntroducedInterface' instead.",
                  collector.ValidatedType.GetFullNameSafe()));
      }
    }
  }
}
