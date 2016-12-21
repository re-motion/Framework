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
using FluentValidation.Validators;
using Remotion.Utilities;

namespace Remotion.Validation.MetaValidation
{
  /// <summary>
  /// Implementation of <seealso cref="IMetaValidationRule"/> which uses a <see cref="Delegate"/> to validate a set of <see cref="IPropertyValidator"/>s.
  /// </summary>
  /// <typeparam name="TValidator">The type of the <see cref="IPropertyValidator"/> to which the validation is constrained.</typeparam>
  public class DelegateMetaValidationRule<TValidator> : MetaValidationRuleBase<TValidator> 
    where TValidator : IPropertyValidator
  {
    private readonly Func<IEnumerable<TValidator>, MetaValidationRuleValidationResult> _metaValidationRule;

    public DelegateMetaValidationRule (Func<IEnumerable<TValidator>, MetaValidationRuleValidationResult> metaValidationRule)
    {
      ArgumentUtility.CheckNotNull ("metaValidationRule", metaValidationRule);
      
      _metaValidationRule = metaValidationRule;
    }

    public override IEnumerable<MetaValidationRuleValidationResult> Validate (IEnumerable<TValidator> validationRules)
    {
      yield return _metaValidationRule (validationRules);
    }
  }
}