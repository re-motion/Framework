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
using System;
using System.Collections.Generic;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Merging
{
  /// <summary>
  /// Implements the <see cref="IValidationCollectorMerger"/> interface as a base class with a template method for the merge operation.
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  public abstract class ValidationCollectorMergerBase : IValidationCollectorMerger
  {
    protected ValidationCollectorMergerBase ()
    {
    }

    protected abstract ILogContext CreateNewLogContext ();
    protected abstract void MergeRules (IEnumerable<ValidationCollectorInfo> collectorGroup, List<IAddingComponentPropertyRule> collectedRules, ILogContext logContext);

    public ValidationCollectorMergeResult Merge (IEnumerable<IEnumerable<ValidationCollectorInfo>> validationCollectorInfos)
    {
      ArgumentUtility.CheckNotNull ("validationCollectorInfos", validationCollectorInfos);

      var logContext = CreateNewLogContext();
      var collectedRules = new List<IAddingComponentPropertyRule>();
      foreach (var validationCollectorGroup in validationCollectorInfos)
        MergeRules (validationCollectorGroup, collectedRules, logContext);

      return new ValidationCollectorMergeResult(collectedRules, logContext);
    }
  }
}