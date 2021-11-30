using System;
using System.Collections.Generic;
using Remotion.Utilities;
using Remotion.Validation.Validators;

namespace Remotion.Validation.MetaValidation
{
  /// <summary>
  /// Implementation of <seealso cref="IObjectMetaValidationRule"/> which uses a <see cref="Delegate"/> to validate a set of <see cref="IObjectValidator"/>s.
  /// </summary>
  /// <typeparam name="TValidator">The type of the <see cref="IObjectValidator"/> to which the validation is constrained.</typeparam>
  public class DelegateObjectMetaValidationRule<TValidator> : ObjectMetaValidationRuleBase<TValidator>
      where TValidator : IObjectValidator
  {
    private readonly Func<IEnumerable<TValidator>, MetaValidationRuleValidationResult> _metaValidationRule;

    public DelegateObjectMetaValidationRule (Func<IEnumerable<TValidator>, MetaValidationRuleValidationResult> metaValidationRule)
    {
      ArgumentUtility.CheckNotNull("metaValidationRule", metaValidationRule);

      _metaValidationRule = metaValidationRule;
    }

    public override IEnumerable<MetaValidationRuleValidationResult> Validate (IEnumerable<TValidator> validationRules)
    {
      yield return _metaValidationRule(validationRules);
    }
  }
}
