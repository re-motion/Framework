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
using System.Reflection;
using FluentValidation.Validators;
using Remotion.Utilities;

namespace Remotion.Validation.MetaValidation.Rules.Custom
{
  public class RemotionMaxLengthMetaValidationRule : MetaValidationRuleBase<LengthValidator>
  {
    private readonly int _maxLength;
    private readonly PropertyInfo _propertyInfo;

    public RemotionMaxLengthMetaValidationRule (PropertyInfo propertyInfo, int maxLength)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      _maxLength = maxLength;
      _propertyInfo = propertyInfo;
    }

    public int MaxLength
    {
      get { return _maxLength; }
    }

    public override IEnumerable<MetaValidationRuleValidationResult> Validate (IEnumerable<LengthValidator> validationRules)
    {
      var rules = validationRules.ToArray();

      if (!rules.Any())
      {
        yield return
            MetaValidationRuleValidationResult.CreateInvalidResult (
                "'{0}' failed for property '{1}.{2}': No max-length validation rules defined.",
                GetType().Name,
                _propertyInfo.ReflectedType.FullName,
                _propertyInfo.Name);
      }
      else if (rules.Where (r => r.Max > _maxLength).Any())
      {
        yield return
            MetaValidationRuleValidationResult.CreateInvalidResult (
                "'{0}' failed for property '{1}.{2}': Max-length validation rule value '{3}' exceeds meta validation rule max-length value of '{4}'.",
                GetType().Name,
                _propertyInfo.ReflectedType.FullName,
                _propertyInfo.Name,
                rules.Max(r=>r.Max),
                _maxLength);
      }
      else
        yield return MetaValidationRuleValidationResult.CreateValidResult();
    }
  }
}