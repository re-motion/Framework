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
using System.Linq;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.IntegrationTests.TestDomain.ComponentA.ValidationCollectors
{
  public class EmployeeValidationRuleCollector1 : ValidationRuleCollectorBase<Employee>
  {
    public EmployeeValidationRuleCollector1 ()
    {
      var propertyInfo = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty<Employee, string>(e => e.CV));
      AddRule<string>(propertyInfo, e => ((Employee)e).CVLines)
          .SetValidator(
              _ => new PredicateValidator(
                  (_, value, _) => ((string[])value).All(l => l.Length <= 25),
                  new InvariantValidationMessage("A line exceeds the maximum length of 25 characters.")));

      AddRule(e => e.Salary)
          .SetValidator(_ => new NotEqualValidator(0m, new InvariantValidationMessage("Conditional Message Text: Kein Gehalt definiert")));
      AddRule(e => e.Salary)
          .SetCondition(e => e.Salary > 0)
          .SetValidator(_ => new GreaterThanValidator(500m, new InvariantValidationMessage("Conditional Message Text: Gehalt zu gering")));
    }
  }
}
