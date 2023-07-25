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
using System.Runtime.Serialization;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome.Dto
{
  /// <summary>
  /// Represents the root object returned by a request to the Chrome for Testing JSON endpoint.
  /// Contains information about known good Chrome versions and associated chromedriver, including download links for multiple platforms.
  /// </summary>
  /// <remarks>
  /// JSON endpoint URL: https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json
  /// General information: https://chromedriver.chromium.org/downloads
  /// </remarks>
  [DataContract]
  public class ChromeKnownGoodVersionsWithDownloads
  {
    [DataMember(Name = "timestamp", IsRequired = true)]
    public string Timestamp { get; set; } = default!;

    [DataMember(Name = "versions", IsRequired = true)]
    public ChromeKnownGoodVersionWithDownloads[] Versions { get; set; } = default!;
  }
}
