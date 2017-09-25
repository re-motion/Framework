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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

// ReSharper disable once CheckNamespace

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  public class GenericTestParameterConverter : JavaScriptConverter
  {
    public static readonly GenericTestParameterConverter Instance = new GenericTestParameterConverter();

    private static readonly Type[] s_supportedTypes = { typeof (GenericTestPageParameterDto) };

    private GenericTestParameterConverter ()
    {
    }

    /// <inheritdoc />
    public override object Deserialize (IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
    {
      var status = (GenericTestPageStatus) (int) dictionary["status"];
      var collection = new GenericTestPageParameterCollection();

      var parameters = (Dictionary<string, object>) dictionary["parameters"];
      foreach (var parameter in parameters)
        collection.Add (parameter.Key, ((ArrayList) parameter.Value).Cast<string>().ToArray());

      return new GenericTestPageParameterDto (status, collection);
    }

    /// <inheritdoc />
    public override IDictionary<string, object> Serialize (object obj, JavaScriptSerializer serializer)
    {
      var information = (GenericTestPageParameterDto) obj;
      return new Dictionary<string, object>
             {
                 { "status", (int) information.Status },
                 { "parameters", information.Parameters.ToDictionary (p => p.Name, p => p.ToArray()) }
             };
    }

    /// <inheritdoc />
    public override IEnumerable<Type> SupportedTypes
    {
      get { return s_supportedTypes; }
    }
  }
}