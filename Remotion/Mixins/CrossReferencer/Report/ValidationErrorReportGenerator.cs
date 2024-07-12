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
using System.Xml.Linq;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class ValidationErrorReportGenerator : IReportGenerator
  {
    private readonly ErrorAggregator<ValidationException> _errorAggregator;

    public ValidationErrorReportGenerator (ErrorAggregator<ValidationException> errorAggregator)
    {
      ArgumentUtility.CheckNotNull("errorAggregator", errorAggregator);

      _errorAggregator = errorAggregator;
    }

    public XElement GenerateXml ()
    {
      var validationErrors = new XElement("ValidationErrors");

      foreach (var exception in _errorAggregator.Exceptions)
      {
        var topLevelExceptionElement = new RecursiveExceptionReportGenerator(exception).GenerateXml();
        var validationLog = exception.ValidationLogData;

        topLevelExceptionElement.Add(
            new XElement(
                "ValidationLog",
                new XAttribute("number-of-rules-executed", validationLog?.NumberOfRulesExecuted ?? -1),
                new XAttribute("number-of-failures", validationLog?.NumberOfFailures ?? 0),
                new XAttribute("number-of-unexpected-exceptions", validationLog?.NumberOfUnexpectedExceptions ?? -1),
                new XAttribute("number-of-warnings", validationLog?.NumberOfWarnings ?? -1),
                new XAttribute("number-of-successes", validationLog?.NumberOfSuccesses ?? -1))
        );
        validationErrors.Add(topLevelExceptionElement);
      }

      return validationErrors;
    }
  }
}
