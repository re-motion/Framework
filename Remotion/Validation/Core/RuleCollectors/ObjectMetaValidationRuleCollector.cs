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
using System.Text;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.MetaValidation;

namespace Remotion.Validation.RuleCollectors
{
  /// <summary>
  /// Default implementation of the <see cref="IObjectMetaValidationRuleCollector"/> interface.
  /// </summary>
  public sealed class ObjectMetaValidationRuleCollector : IObjectMetaValidationRuleCollector
  {
    public static ObjectMetaValidationRuleCollector Create<TValidatedType> (Type collectorType)
    {
      ArgumentUtility.CheckNotNull("collectorType", collectorType);

      return new ObjectMetaValidationRuleCollector(TypeAdapter.Create(typeof(TValidatedType)), collectorType);
    }

    public ITypeInformation ValidatedType { get; }
    public Type CollectorType { get; }
    private readonly List<IObjectMetaValidationRule> _metaValidationRules;

    public ObjectMetaValidationRuleCollector (ITypeInformation validatedType, Type collectorType)
    {
      ArgumentUtility.CheckNotNull("validatedType", validatedType);
      ArgumentUtility.CheckNotNull("collectorType", collectorType); // TODO RM-5906: Add type check for IComponentValidationCollector

      ValidatedType = validatedType;
      CollectorType = collectorType;
      _metaValidationRules = new List<IObjectMetaValidationRule>();
    }

    public IEnumerable<IObjectMetaValidationRule> MetaValidationRules
    {
      get { return _metaValidationRules.AsReadOnly(); }
    }

    public void RegisterMetaValidationRule (IObjectMetaValidationRule metaValidationRule)
    {
      ArgumentUtility.CheckNotNull("metaValidationRule", metaValidationRule);

      _metaValidationRules.Add(metaValidationRule);
    }

    public override string ToString ()
    {
      var sb = new StringBuilder(GetType().Name);
      sb.Append(": ");
      sb.Append(ValidatedType.GetFullNameSafe());

      return sb.ToString();
    }
  }
}
