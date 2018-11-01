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
using System.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation.Rules;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  public static class Validator
  {
    public static ValidationLogData Validate (IVisitableDefinition startingPoint, params IRuleSet[] customRuleSets)
    {
      ArgumentUtility.CheckNotNull ("startingPoint", startingPoint);
      ArgumentUtility.CheckNotNull ("customRuleSets", customRuleSets);

      var log = new DefaultValidationLog ();
      Validate (startingPoint, log, customRuleSets);
      return log.GetData();
    }

    public static ValidationLogData Validate (IEnumerable<IVisitableDefinition> startingPoints, params IRuleSet[] customRuleSets)
    {
      ArgumentUtility.CheckNotNull ("startingPoints", startingPoints);
      ArgumentUtility.CheckNotNull ("customRuleSets", customRuleSets);

      var log = new DefaultValidationLog ();
      Validate (startingPoints, log, customRuleSets);
      return log.GetData();
    }

        public static void Validate (IVisitableDefinition startingPoint, IValidationLog log, params IRuleSet[] customRuleSets)
    {
      ArgumentUtility.CheckNotNull ("startingPoint", startingPoint);
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("customRuleSets", customRuleSets);

      Validate (new[] { startingPoint }, log, customRuleSets);
    }

    public static void Validate (IEnumerable<IVisitableDefinition> startingPoints, IValidationLog log, params IRuleSet[] customRuleSets)
    {
      ArgumentUtility.CheckNotNull ("startingPoints", startingPoints);
      ArgumentUtility.CheckNotNull ("log", log);
      ArgumentUtility.CheckNotNull ("customRuleSets", customRuleSets);

      var visitor = CreateValidatingVisitor (log, customRuleSets);
      foreach (IVisitableDefinition startingPoint in startingPoints)
        startingPoint.Accept (visitor);
    }

    private static ValidatingVisitor CreateValidatingVisitor (IValidationLog log, IRuleSet[] customRuleSets)
    {
      ValidatingVisitor visitor = new ValidatingVisitor (log);
      InstallDefaultRules (visitor);

      foreach (IRuleSet ruleSet in customRuleSets)
        ruleSet.Install (visitor);
      return visitor;
    }

    private static void InstallDefaultRules (ValidatingVisitor visitor)
    {
      foreach (Type t in AssemblyTypeCache.GetTypes (Assembly.GetExecutingAssembly()))
      {
        if (!t.IsAbstract && typeof (IRuleSet).IsAssignableFrom (t) && t.Namespace == typeof (IRuleSet).Namespace)
        {
          IRuleSet ruleSet = (IRuleSet) Activator.CreateInstance (t);
          ruleSet.Install (visitor);
        }
      }
    }
  }
}
