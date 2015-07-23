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
using Remotion.Collections;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  public class DefaultValidationLog : IValidationLog
  {
    private readonly Stack<ValidationResult> _currentData = new Stack<ValidationResult> ();
    private readonly SimpleDataStore<object, object> _contextStore = new SimpleDataStore<object, object> ();

    private readonly ValidationLogData _data = new ValidationLogData();

    public void ValidationStartsFor (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      var validationResult = new ValidationResult (definition);
      _currentData.Push (validationResult);
    }

    public void ValidationEndsFor (IVisitableDefinition definition)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);
      if (_currentData.Count == 0)
      {
        string message = string.Format ("Validation of definition {0}/{1} cannot be ended, because it wasn't started.", definition.GetType ().Name,
            definition.FullName);
        throw new InvalidOperationException (message);
      }
      else
      {
        ValidationResult currentResult = _currentData.Peek();
        // Only compare the full name rather than creating a new ID - it's more performant, and it's only a safety check anyway
        if (currentResult.ValidatedDefinition.FullName != definition.FullName)
        {
          string message = string.Format (
              "Cannot end validation for {0} while {1} is validated.", 
              definition.FullName, 
              currentResult.ValidatedDefinition.FullName);
          throw new InvalidOperationException (message);
        }

        _currentData.Pop();
        _data.Add (currentResult);
      }
    }

    private ValidationResult GetCurrentResult ()
    {
      if (_currentData.Count == 0)
      {
        throw new InvalidOperationException ("Validation has not been started.");
      }
      return _currentData.Peek ();
    }

    public void Succeed (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult().Successes.Add (new ValidationResultItem(rule.RuleName, rule.Message));
    }

    public void Warn (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult ().Warnings.Add (new ValidationResultItem(rule.RuleName, rule.Message));
    }

    public void Fail (IValidationRule rule)
    {
      ArgumentUtility.CheckNotNull ("rule", rule);
      GetCurrentResult ().Failures.Add (new ValidationResultItem (rule.RuleName, rule.Message));
    }

    public void UnexpectedException (IValidationRule rule, Exception ex)
    {
      GetCurrentResult ().Exceptions.Add (new ValidationExceptionResultItem (rule.RuleName, ex));
    }

    public IDataStore<object, object> ContextStore
    {
      get { return _contextStore; }
    }

    public ValidationLogData GetData ()
    {
      return _data;
    }

    public void MergeIn (IValidationLog log)
    {
      _data.Add (log.GetData());
    }
  }
}
