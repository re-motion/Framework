﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Default implementation of the <see cref="ILogContext"/> interface.
  /// </summary>
  public class DefaultLogContext : ILogContext
  {
    private readonly MultiDictionary<IAddingPropertyValidationRuleCollector, PropertyValidatorLogContextInfo> _removingLogEntriesForPropertyValidators;
    private readonly MultiDictionary<IAddingObjectValidationRuleCollector, ObjectValidatorLogContextInfo> _removingLogEntriesForObjectValidators;

    public DefaultLogContext ()
    {
      _removingLogEntriesForPropertyValidators = new MultiDictionary<IAddingPropertyValidationRuleCollector, PropertyValidatorLogContextInfo>();
      _removingLogEntriesForObjectValidators = new MultiDictionary<IAddingObjectValidationRuleCollector, ObjectValidatorLogContextInfo>();
    }

    public void ValidatorRemoved (
        IPropertyValidator removedValidator,
        RemovingPropertyValidatorRegistration[] removingPropertyValidatorRegistrations,
        IAddingPropertyValidationRuleCollector addingPropertyValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull("addingPropertyValidationRuleCollector", addingPropertyValidationRuleCollector);
      ArgumentUtility.CheckNotNull("removingPropertyValidatorRegistrations", removingPropertyValidatorRegistrations);
      ArgumentUtility.CheckNotNull("addingPropertyValidationRuleCollector", addingPropertyValidationRuleCollector);

      var logContextInfo = new PropertyValidatorLogContextInfo(removedValidator, removingPropertyValidatorRegistrations);
      _removingLogEntriesForPropertyValidators[addingPropertyValidationRuleCollector].Add(logContextInfo);
    }

    public IEnumerable<PropertyValidatorLogContextInfo> GetLogContextInfos (IAddingPropertyValidationRuleCollector addingPropertyValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull("addingPropertyValidationRuleCollector", addingPropertyValidationRuleCollector);

      return _removingLogEntriesForPropertyValidators[addingPropertyValidationRuleCollector];
    }

    public void ValidatorRemoved (
        IObjectValidator removedValidator,
        RemovingObjectValidatorRegistration[] removingObjectValidatorRegistrations,
        IAddingObjectValidationRuleCollector addingObjectValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull("addingObjectValidationRuleCollector", addingObjectValidationRuleCollector);
      ArgumentUtility.CheckNotNull("removingObjectValidatorRegistrations", removingObjectValidatorRegistrations);
      ArgumentUtility.CheckNotNull("addingObjectValidationRuleCollector", addingObjectValidationRuleCollector);

      var logContextInfo = new ObjectValidatorLogContextInfo(removedValidator, removingObjectValidatorRegistrations);
      _removingLogEntriesForObjectValidators[addingObjectValidationRuleCollector].Add(logContextInfo);
    }

    public IEnumerable<ObjectValidatorLogContextInfo> GetLogContextInfos (IAddingObjectValidationRuleCollector addingObjectValidationRuleCollector)
    {
      ArgumentUtility.CheckNotNull("addingObjectValidationRuleCollector", addingObjectValidationRuleCollector);

      return _removingLogEntriesForObjectValidators[addingObjectValidationRuleCollector];
    }
  }
}
