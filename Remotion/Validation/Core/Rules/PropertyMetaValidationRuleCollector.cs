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
using System.Linq.Expressions;
using System.Text;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.MetaValidation;

namespace Remotion.Validation.Rules
{
  /// <summary>
  /// Default implementation of the <see cref="IPropertyMetaValidationRuleCollector"/> interface.
  /// </summary>
  public sealed class PropertyMetaValidationRuleCollector : IPropertyMetaValidationRuleCollector
  {
    public static PropertyMetaValidationRuleCollector Create<TValidatedType, TProperty> (Expression<Func<TValidatedType, TProperty>> expression, Type collectorType)
    {
      var propertyInfo = MemberInfoFromExpressionUtility.GetProperty (expression);

      return new PropertyMetaValidationRuleCollector (PropertyInfoAdapter.Create (propertyInfo), collectorType);
    }

    public IPropertyInformation Property { get; }
    public Type CollectorType { get; }
    private readonly List<IMetaValidationRule> _metaValidationRules;

    public PropertyMetaValidationRuleCollector (IPropertyInformation property, Type collectorType)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      ArgumentUtility.CheckNotNull ("collectorType", collectorType); // TODO RM-5906: Add type check for IComponentValidationCollector

      Property = property;
      CollectorType = collectorType;
      _metaValidationRules = new List<IMetaValidationRule> ();
    }

    public IEnumerable<IMetaValidationRule> MetaValidationRules
    {
      get { return _metaValidationRules.AsReadOnly(); }
    }

    public void RegisterMetaValidationRule (IMetaValidationRule metaValidationRule)
    {
      ArgumentUtility.CheckNotNull ("metaValidationRule", metaValidationRule);

      _metaValidationRules.Add (metaValidationRule);
    }

    public override string ToString ()
    {
      var sb = new StringBuilder (GetType ().Name);
      sb.Append (": ");
      sb.Append (Property.DeclaringType != null ? Property.DeclaringType.FullName + "#" : string.Empty);
      sb.Append (Property.Name);

      return sb.ToString ();
    }

  }
}