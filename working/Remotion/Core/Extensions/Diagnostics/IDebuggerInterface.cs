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
using System.Diagnostics;

namespace Remotion.Diagnostics
{
  /// <summary>
  /// Provides an API for classes interfacing with the debugger. This functionality is also provided by the <see cref="Debugger"/> class, but 
  /// <see cref="IDebuggerInterface"/> is implemented as an interface and allows alternative implementations for testing, custom debuggers, or other
  /// extensibility.
  /// </summary>
  public interface IDebuggerInterface
  {
    bool IsAttached { get; }
    string DefaultCategory { get; }

    void Break ();
    void Launch();

    void Log (int level, string category, string message);
    bool IsLogging ();
  }
}