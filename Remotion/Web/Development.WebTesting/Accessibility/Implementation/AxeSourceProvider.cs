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
using System.IO;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Accessibility.Implementation
{
  /// Default implementation of <see cref="IAxeSourceProvider" />
  public class AxeSourceProvider : IAxeSourceProvider
  {
    private const string c_resourceName = "axe.min.js";

    public AxeSourceProvider ()
    {
    }

    public string GetSource ()
    {
      var assembly = Assembly.GetExecutingAssembly();

      using (var stream = assembly.GetManifestResourceStream (typeof (IAxeSourceProvider), c_resourceName))
      using (var reader = new StreamReader (stream))
      {
        Assertion.IsNotNull (reader, c_resourceName);

        return reader.ReadToEnd();
      }
    }
  }
}