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
using Remotion.Mixins.Validation;
using Remotion.Tools.Console;
using Remotion.Utilities;

namespace Remotion.Development.Mixins.Validation
{
  public static class ConsoleDumper
  {
    public static void DumpValidationResults (IEnumerable<ValidationResult> results)
    {
      ArgumentUtility.CheckNotNull ("results", results);

      foreach (ValidationResult result in results)
      {
        if (result.TotalRulesExecuted == 0)
        {
          //Console.ForegroundColor = ConsoleColor.DarkGray;
          //Console.WriteLine ("No rules found for {0} '{1}'", result.Definition.GetType ().Name, result.Definition.FullName);
        }
        else if (result.TotalRulesExecuted != result.Successes.Count)
        {
          using (ConsoleUtility.EnterColorScope (ConsoleColor.Gray, null))
          {
            Console.WriteLine (
                "{0} '{1}', {2} rules executed",
                result.ValidatedDefinition.GetType().Name,
                result.ValidatedDefinition.FullName,
                result.TotalRulesExecuted);
            DumpContext (result);
          }
        }
        DumpResultList ("unexpected exceptions", result.Exceptions, ConsoleColor.White, ConsoleColor.DarkRed);
        // DumpResultList ("successes", result.Successes, ConsoleColor.Green, null);
        DumpResultList ("warnings", result.Warnings, ConsoleColor.Yellow, null);
        DumpResultList ("failures", result.Failures, ConsoleColor.Red, null);
      }
    }

    private static void DumpContext (ValidationResult result)
    {
      string contextString = result.GetDefinitionContextPath();
      if (contextString.Length > 0)
        Console.WriteLine ("Context: " + contextString);
    }

    private static void DumpResultList<T> (string title, List<T> resultList, ConsoleColor foreColor, ConsoleColor? backColor) where T : IDefaultValidationResultItem
    {
      if (resultList.Count > 0)
      {
        using (ConsoleUtility.EnterColorScope (foreColor, backColor))
        {
          Console.WriteLine ("  {0} - {1}", title, resultList.Count);
          foreach (T resultItem in resultList)
            Console.WriteLine ("    {0} ({1})", resultItem.Message, resultItem.RuleName);
        }
      }
    }
  }
}
