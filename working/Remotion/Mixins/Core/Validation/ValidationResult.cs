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
using Remotion.FunctionalProgramming;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  public struct ValidationResult
  {
    private readonly IVisitableDefinition _validatedDefinition;

    private readonly List<ValidationResultItem> _successes;
    private readonly List<ValidationResultItem> _warnings;
    private readonly List<ValidationResultItem> _failures;
    private readonly List<ValidationExceptionResultItem> _exceptions;

    public ValidationResult (IVisitableDefinition validatedDefinition)
    {
      ArgumentUtility.CheckNotNull ("validatedDefinition", validatedDefinition);

      _validatedDefinition = validatedDefinition;

      _successes = new List<ValidationResultItem> ();
      _warnings = new List<ValidationResultItem> ();
      _failures = new List<ValidationResultItem> ();
      _exceptions = new List<ValidationExceptionResultItem> ();
    }

    public IVisitableDefinition ValidatedDefinition
    {
      get { return _validatedDefinition; }
    }

    public string GetDefinitionContextPath()
    {
      return string.Join (" -> ", ValidatedDefinition.Parent.CreateSequence (d => d.Parent).Select (d => d.FullName));
    }

    public int TotalRulesExecuted
    {
      get { return _successes.Count + _warnings.Count + _failures.Count + _exceptions.Count; }
    }

    public List<ValidationExceptionResultItem> Exceptions
    {
      get { return _exceptions; }
    }

    public List<ValidationResultItem> Failures
    {
      get { return _failures; }
    }

    public List<ValidationResultItem> Warnings
    {
      get { return _warnings; }
    }

    public List<ValidationResultItem> Successes
    {
      get { return _successes; }
    }
  }
}
