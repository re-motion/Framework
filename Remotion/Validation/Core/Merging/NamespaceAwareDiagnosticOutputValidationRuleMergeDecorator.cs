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
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Merging
{
  /// <summary>
  /// Extends the <see cref="DiagnosticOutputValidationRuleMergeDecorator"/> to log the <see cref="Type.FullName"/>.
  /// </summary>
  public class NamespaceAwareDiagnosticOutputValidationRuleMergeDecorator : DiagnosticOutputValidationRuleMergeDecorator
  {
    public NamespaceAwareDiagnosticOutputValidationRuleMergeDecorator (
        IValidationRuleCollectorMerger validationRuleCollectorMerger,
        IValidatorFormatter validatorFormatter,
        ILogManager logManager)
        : base(validationRuleCollectorMerger, validatorFormatter, logManager)
    {
    }

    protected override string GetTypeName (Type type)
    {
      return type.GetFullNameChecked();
    }
  }
}