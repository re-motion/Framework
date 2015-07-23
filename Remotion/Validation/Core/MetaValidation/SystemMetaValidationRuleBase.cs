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
using FluentValidation.Validators;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Validation.MetaValidation
{
  /// <summary>
  /// Base class for implementations of the <see cref="IMetaValidationRule"/> interface which are inherent to the validation specification of any given any application.
  /// </summary>
  /// <typeparam name="TValidator">The type of the <see cref="IPropertyValidator"/> validated by this meta validator.</typeparam>
  public abstract class SystemMetaValidationRuleBase<TValidator> : MetaValidationRuleBase<TValidator>
      where TValidator: IPropertyValidator
  {
    private readonly IPropertyInformation _propertyInfo;

    protected SystemMetaValidationRuleBase (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      _propertyInfo = propertyInfo;
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInfo; }
    }

    protected MetaValidationRuleValidationResult GetValidationResult (bool isValid)
    {
      if (isValid)
        return MetaValidationRuleValidationResult.CreateValidResult();

      return MetaValidationRuleValidationResult.CreateInvalidResult (
          "'{0}' failed for member '{1}.{2}'.",
          GetType().Name,
          PropertyInfo.DeclaringType.FullName,
          PropertyInfo.Name);
    }
  }
}