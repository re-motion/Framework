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
using System.Collections.ObjectModel;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation
{
  /// <summary>
  /// Holds the data written to an <see cref="IValidationLog"/>.
  /// </summary>
  public class ValidationLogData
  {
    private readonly List<ValidationResult> _results = new List<ValidationResult>();

    private int _failures = 0;
    private int _warnings = 0;
    private int _exceptions = 0;
    private int _successes = 0;

    public int ResultCount
    {
      get { return _results.Count; }
    }

    public void Add (ValidationResult result)
    {
       _results.Add(result);
      _failures += result.Failures.Count;
      _warnings += result.Warnings.Count;
      _exceptions += result.Exceptions.Count;
      _successes += result.Successes.Count;
    }

    public void Add (ValidationLogData data)
    {
      foreach (ValidationResult mergedResult in data.GetResults())
      {
        ValidationResult? activeResult = FindMatchingResult(mergedResult.ValidatedDefinition);
        if (activeResult == null)
        {
          activeResult = new ValidationResult(mergedResult.ValidatedDefinition);
          Add(activeResult.Value);
        }

        foreach (ValidationResultItem resultItem in mergedResult.Successes)
        {
          activeResult.Value.Successes.Add(resultItem);
          ++_successes;
        }
        foreach (ValidationResultItem resultItem in mergedResult.Failures)
        {
          activeResult.Value.Failures.Add(resultItem);
          ++_failures;
        }
        foreach (ValidationResultItem resultItem in mergedResult.Warnings)
        {
          activeResult.Value.Warnings.Add(resultItem);
          ++_warnings;
        }
        foreach (ValidationExceptionResultItem resultItem in mergedResult.Exceptions)
        {
          activeResult.Value.Exceptions.Add(resultItem);
          ++_exceptions;
        }
      }
    }

    public ReadOnlyCollection<ValidationResult> GetResults ()
    {
      return _results.AsReadOnly();
    }

    public int GetNumberOfFailures ()
    {
      return _failures;
    }

    public int GetNumberOfWarnings ()
    {
      return _warnings;
    }

    public int GetNumberOfSuccesses ()
    {
      return _successes;
    }

    public int GetNumberOfUnexpectedExceptions ()
    {
      return _exceptions;
    }

    public int GetNumberOfRulesExecuted ()
    {
      return _successes + _warnings + _failures + _exceptions;
    }

    private ValidationResult? FindMatchingResult (IVisitableDefinition validatedDefinition)
    {
      foreach (var result in GetResults())
      {
        if (result.ValidatedDefinition == validatedDefinition)
          return result;
      }
      return null;
    }

  }
}
