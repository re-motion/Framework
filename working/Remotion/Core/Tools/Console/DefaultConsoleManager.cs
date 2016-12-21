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
  /// Provides a default implementation of the <see cref="IConsoleManager"/> interface for the console represented by the <see cref="Console"/>
  /// class.
  /// </summary>
  public class DefaultConsoleManager : IConsoleManager
  {
    /// <summary>
    /// The single instance of this class.
    /// </summary>
    public static readonly DefaultConsoleManager Instance = new DefaultConsoleManager();

    private DefaultConsoleManager ()
    {
    }

    /// <summary>
    /// Gets or sets the foreground color of the console.
    /// </summary>
    /// <value>The console's foreground color. Returns <see cref="ConsoleColor.Gray"/> if there is no console attached to the current process.</value>
    public ConsoleColor ForegroundColor
    {
      get { return System.Console.ForegroundColor; }
      set { System.Console.ForegroundColor = value; }
    }

    /// <summary>
    /// Gets or sets the background color of the console.
    /// </summary>
    /// <value>The console's background color. Returns <see cref="ConsoleColor.Black"/> if there is no console attached to the current process.</value>
    public ConsoleColor BackgroundColor
    {
      get { return System.Console.BackgroundColor; }
      set { System.Console.BackgroundColor = value; }
    }
  }
}
