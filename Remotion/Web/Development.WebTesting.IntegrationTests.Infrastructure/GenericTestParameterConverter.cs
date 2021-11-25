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
using System.Text.Json;
using System.Text.Json.Serialization;
using Remotion.Utilities;

// ReSharper disable once CheckNamespace

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  public class GenericTestParameterConverter : JsonConverter<GenericTestPageParameterDto>
  {
    public static readonly GenericTestParameterConverter Instance = new();

    private GenericTestParameterConverter ()
    {
    }

    public override GenericTestPageParameterDto Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      var container = Assertion.IsNotNull(JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ref reader, options));
      var status = (GenericTestPageStatus) container["status"].GetInt32();
      var parameters = container["parameters"].EnumerateObject()
          .ToDictionary(
              p => p.Name,
              p => new GenericTestPageParameter(p.Name, p.Value.EnumerateArray().Select(i => i.GetString()).ToArray()));

      return new(status, parameters);
    }

    public override void Write (Utf8JsonWriter writer, GenericTestPageParameterDto value, JsonSerializerOptions options)
    {
      var container = new Dictionary<string, object>
                      {
                          { "status", (int) value.Status },
                          { "parameters", value.Parameters.ToDictionary(pair => pair.Key, pair => pair.Value.Arguments) },
                      };

      JsonSerializer.Serialize(writer, container, options);
    }
  }
}
