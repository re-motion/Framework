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

namespace Remotion.Tools.Console
{
  /// <summary>
  /// Provides an API for manipulating the Console. This abstracts the <see cref="Console"/> class and thus enables unit testing of
  /// console-dependent code even when there is no console as well as use of multiple consoles at the same time.
  /// </summary>
  public interface IConsoleManager
  {
    /// <summary>
    /// Gets or sets the foreground color of the console.
    /// </summary>
    /// <value>The console's foreground color.</value>
    ConsoleColor ForegroundColor { get; set; }

    /// <summary>
    /// Gets or sets the background color of the console.
    /// </summary>
    /// <value>The console's background color.</value>
    ConsoleColor BackgroundColor { get; set; }
  }
}
