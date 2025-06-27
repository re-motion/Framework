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
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;

namespace Remotion.Web.Development.WebTesting.BrowserLog;

/// <summary>
/// Instructs the <see cref="WebTestHelper"/>'s browser log check to ignore any <see cref="BrowserLogEntry"/> whose <see cref="BrowserLogEntry.Level"/> is lower than the
/// <see cref="MinimumLevel"/>. 
/// </summary>
/// <remarks>
/// Use this on test methods, the classes to which they belong, or the whole assembly.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class BrowserLogMinimumLevelAttribute : WebTestAttribute
{
  public const string PropertyKey = "MinimumBrowserLogLevel";

  public BrowserLogMinimumLevelAttribute (LogLevel minimumLevel)
  {
    MinimumLevel = minimumLevel;
  }

  public LogLevel MinimumLevel { get; }

  public override void ApplyValue (IDictionary<string, object> dictionary)
  {
    ArgumentNullException.ThrowIfNull(dictionary);

    dictionary[PropertyKey] = MinimumLevel;
  }
}
