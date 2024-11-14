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
using System.Configuration;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary> This exception is thrown when the execution engine needs to manipulate the ASP.NET execution control flow. </summary>
  /// <remarks> 
  /// This exception is derived from <see cref="ConfigurationException"/> to allow the exception to bubble through the ASP.NET infrastructure 
  /// (i.e. Page.ProcessRequest) without being logged in the ASP.NET performance counters (.e.g. Total Errors).
  /// </remarks>
  public abstract class WxeInfrastructureException : ConfigurationException
  {
    protected WxeInfrastructureException (string message)
#pragma warning disable 612,618
        : base(message)
#pragma warning restore 612,618
    {
    }
  }
}
