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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;

namespace Remotion.Web.Development.WebTesting.Configuration.IConfiguration
{
  /// <summary>
  /// DTO object for <see cref="IWebTestHostingSettings"/>, which is deserialized by the .NET configuration infrastructure.
  /// </summary>
  public record WebTestHostingSettingsDto : IWebTestHostingSettings
  {
    /// <inheritdoc />
    [Required]
    public required string Name { get; init; }

    /// <inheritdoc />
    [Required]
    public required string Type { get; init; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Parameters { get; init; } = ReadOnlyDictionary<string, string>.Empty;
  }
}
