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
using JetBrains.Annotations;
using OpenQA.Selenium.Chrome;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Edge
{
  /// <summary>
  /// Extends <see cref="ChromeOptions"/> provided by Selenium with additional data used by the web testing infrastructure.
  /// </summary>
  public class ExtendedEdgeOptions : ChromeOptions
  {
    /// <summary>
    /// The user directory of this <see cref="ChromeOptions"/>.
    /// </summary>
    [CanBeNull]
    public string UserDirectory { get; set; }
  }
}