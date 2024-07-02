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
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class RecursiveExceptionReportGenerator : IReportGenerator
  {
    private readonly Exception _exception;

    public RecursiveExceptionReportGenerator (Exception exception)
    {
      ArgumentUtility.CheckNotNull("exception", exception);

      _exception = exception;
    }

    public XElement GenerateXml ()
    {
      return GenerateExceptionElement(_exception);
    }

    private XElement GenerateExceptionElement (Exception exception)
    {
      if (exception == null)
        return null;

      return new XElement(
          "Exception",
          new XAttribute("type", exception.GetType()),
          new XElement("Message", new XCData(exception.Message)),
          new XElement("StackTrace", new XCData(exception.StackTrace)),
          GenerateExceptionElement(exception.InnerException));
    }
  }
}
