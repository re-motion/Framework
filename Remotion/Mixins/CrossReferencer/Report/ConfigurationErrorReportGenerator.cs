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
using System.Xml.Linq;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class ConfigurationErrorReportGenerator : IReportGenerator
  {
    private readonly ErrorAggregator<Exception> _errorAggregator;

    public ConfigurationErrorReportGenerator(ErrorAggregator<Exception> errorAggregator)
    {
      ArgumentUtility.CheckNotNull ("errorAggregator", errorAggregator);

      _errorAggregator = errorAggregator;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "ConfigurationErrors",
          from exception in _errorAggregator.Exceptions
          select new RecursiveExceptionReportGenerator (exception).GenerateXml()
          );
    }
  }
}
