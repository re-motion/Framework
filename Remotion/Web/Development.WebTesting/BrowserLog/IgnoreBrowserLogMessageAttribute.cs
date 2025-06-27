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
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;

namespace Remotion.Web.Development.WebTesting.BrowserLog;

/// <summary>
/// Instructs the <see cref="WebTestHelper"/>'s browser log check to ignore any <see cref="BrowserLogEntry"/> whose <see cref="BrowserLogEntry.Message"/> matches the
/// <see cref="Regex"/> in <see cref="Pattern"/>. 
/// </summary>
/// <remarks>
/// Use this on test methods, the classes to which they belong, or the whole assembly.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class IgnoreBrowserLogMessageAttribute : WebTestAttribute
{
  public const string PropertyKey = "IgnoreBrowserLogMessage";

  public IgnoreBrowserLogMessageAttribute ([NotNull] [RegexPattern] string pattern)
  {
    ArgumentUtility.CheckNotNullOrEmpty(nameof(pattern), pattern);

    Pattern = pattern;
  }

  public IgnoreBrowserLogMessageAttribute ([NotNull] [RegexPattern] string pattern, [NotNull] [ItemCanBeNull] object?[] templateArgs)
  {
    ArgumentUtility.CheckNotNullOrEmpty(nameof(pattern), pattern);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(templateArgs), templateArgs);

    pattern = string.Format(pattern, templateArgs);

    Pattern = pattern;
  }

  public string Pattern { get; }

  public override void ApplyValue (IDictionary<string, object> dictionary)
  {
    ArgumentNullException.ThrowIfNull(dictionary);

    if (!dictionary.TryGetValue(PropertyKey, out var value) || value is not IList<Regex> patterns)
    {
      patterns = new List<Regex>();
      dictionary[PropertyKey] = patterns;
    }

    patterns.Add(new Regex(Pattern));
  }
}
