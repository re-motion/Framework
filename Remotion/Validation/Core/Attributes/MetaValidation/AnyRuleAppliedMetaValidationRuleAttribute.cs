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
using System.Reflection;
using Remotion.Utilities;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.MetaValidation.Rules.Custom;

namespace Remotion.Validation.Attributes.MetaValidation
{
  /// <summary>
  /// Apply the <see cref="AnyRuleAppliedMetaValidationRuleAttribute"/> to a property to ensure that there is at least one validation rule 
  /// specified. This is used if a domain library provides a property that obviously requires some constraints, but the exact usage can vary 
  /// from project to project.
  /// </summary>
  public class AnyRuleAppliedMetaValidationRuleAttribute : AddingMetaValidationRuleAttributeBase
  {
    public override IMetaValidationRule GetMetaValidationRule (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      
      return new AnyRuleAppliedMetaValidationRule (property);
    }
  }
}