﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Runtime.Serialization.Json;
using System.Text;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Accessibility.AxeJsonResultDtos;

namespace Remotion.Web.Development.WebTesting.Accessibility.Implementation
{
  /// Default implementation of <see cref="IAxeResultParser"/>.
  public class AxeResultParser : IAxeResultParser
  {
    public AxeResultParser ()
    {
    }

    /// <inheritdoc />
    public AxeResult Parse (string rawJson)
    {
      ArgumentUtility.CheckNotNullOrEmpty("rawJson", rawJson);

      var serializer = new DataContractJsonSerializer(typeof(AxeResult));
      using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(rawJson)))
      {
        return (AxeResult)Assertion.IsNotNull(serializer.ReadObject(stream), "Axe result could not be deserialized.");
      }
    }
  }
}
