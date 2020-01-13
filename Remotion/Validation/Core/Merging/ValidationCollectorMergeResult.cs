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
using System.Linq;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleCollectors;

namespace Remotion.Validation.Merging
{
  public class ValidationCollectorMergeResult
  {
    private readonly IReadOnlyCollection<IAddingPropertyValidationRuleCollector> _collectedPropertyValidationRules;
    private readonly IReadOnlyCollection<IAddingObjectValidationRuleCollector> _collectedObjectValidationRules;
    private readonly ILogContext _logContext;

    public ValidationCollectorMergeResult (
        IEnumerable<IAddingPropertyValidationRuleCollector> collectedPropertyValidationRules,
        IEnumerable<IAddingObjectValidationRuleCollector> collectedObjectValidationRules,
        ILogContext logContext)
    {
      ArgumentUtility.CheckNotNull ("collectedPropertyValidationRules", collectedPropertyValidationRules);
      ArgumentUtility.CheckNotNull ("collectedObjectValidationRules", collectedObjectValidationRules);
      ArgumentUtility.CheckNotNull ("logContext", logContext);

      _collectedPropertyValidationRules = collectedPropertyValidationRules.ToList().AsReadOnly();
      _collectedObjectValidationRules = collectedObjectValidationRules.ToList().AsReadOnly();
      _logContext = logContext;
    }

    public IReadOnlyCollection<IAddingPropertyValidationRuleCollector> CollectedPropertyValidationRules
    {
      get { return _collectedPropertyValidationRules; }
    }

    public IReadOnlyCollection<IAddingObjectValidationRuleCollector> CollectedObjectValidationRules
    {
      get { return _collectedObjectValidationRules; }
    }

    public ILogContext LogContext
    {
      get { return _logContext; }
    }
  }
}