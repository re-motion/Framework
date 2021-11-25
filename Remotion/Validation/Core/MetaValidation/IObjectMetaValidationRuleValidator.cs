using System;
using System.Collections.Generic;
using Remotion.Validation.RuleCollectors;

namespace Remotion.Validation.MetaValidation
{
  /// <summary>
  /// Implementations of the <see cref="IObjectMetaValidationRuleValidator"/> interface can be used to validate the consistency of a 
  /// set of <see cref="IAddingObjectValidationRuleCollector"/>s.
  /// </summary>
  /// <seealso cref="ObjectMetaValidationRuleValidator"/>
  public interface IObjectMetaValidationRuleValidator
  {
    IEnumerable<MetaValidationRuleValidationResult> Validate (IAddingObjectValidationRuleCollector[] addingObjectValidationRulesCollectors);
  }
}
