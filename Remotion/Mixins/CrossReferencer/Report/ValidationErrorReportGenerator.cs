// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Xml.Linq;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;
using IRemotionReflector = MixinXRef.Reflection.RemotionReflector.IRemotionReflector;

namespace MixinXRef.Report
{
  public class ValidationErrorReportGenerator : IReportGenerator
  {
    private readonly ErrorAggregator<Exception> _errorAggregator;
    private readonly IRemotionReflector _remotionReflector;

    public ValidationErrorReportGenerator(ErrorAggregator<Exception> errorAggregator, IRemotionReflector remotionReflector)
    {
      ArgumentUtility.CheckNotNull ("errorAggregator", errorAggregator);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);

      _errorAggregator = errorAggregator;
      _remotionReflector = remotionReflector;
    }

    public XElement GenerateXml ()
    {
      var validationErrors = new XElement ("ValidationErrors");

      foreach (var exception in _errorAggregator.Exceptions)
      {
        var topLevelExceptionElement = new RecursiveExceptionReportGenerator (exception).GenerateXml();
        var validationLog = _remotionReflector.GetValidationLogFromValidationException(exception);

        topLevelExceptionElement.Add (
            new XElement (
                "ValidationLog",
                new XAttribute("number-of-rules-executed", validationLog.GetProperty("NumberOfRulesExecuted")),
                new XAttribute("number-of-failures", validationLog.GetProperty("NumberOfFailures")),
                new XAttribute("number-of-unexpected-exceptions", validationLog.GetProperty("NumberOfUnexpectedExceptions")),
                new XAttribute("number-of-warnings", validationLog.GetProperty("NumberOfWarnings")),
                new XAttribute("number-of-successes", validationLog.GetProperty("NumberOfSuccesses")))
            );
        validationErrors.Add (topLevelExceptionElement);
      }

      return validationErrors;
    }
  }
}