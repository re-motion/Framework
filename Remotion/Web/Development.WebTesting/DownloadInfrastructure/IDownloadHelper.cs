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
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure
{
  /// <summary>
  /// Helper for web tests which must deal with downloads.
  /// </summary>
  public interface IDownloadHelper
  {
    /// <summary>
    /// Handles the download of a file which has been triggered by an earlier action. 
    /// </summary>
    /// <param name="fileName">FileName the downloaded File is expected to have. Must not be <see langword="null" /> or empty.</param>
    /// <param name="downloadStartedTimeout">
    /// Optional parameter specifying the maximum time for the server to respond to the download request.
    /// If <see langword="null" />, the configured <see cref="WebTestConfigurationSection.DownloadStartedTimeout"/> is used.
    /// </param>
    /// <param name="downloadUpdatedTimeout">
    /// Optional parameter specifying the maximum wait between the server sending chunks of data to the client. 
    /// If <see langword="null" />, the configured <see cref="WebTestConfigurationSection.DownloadUpdatedTimeout"/> is used.
    /// </param>
    /// <exception cref="DownloadResultNotFoundException">
    /// <para>Thrown if it takes longer than the <paramref name="downloadStartedTimeout"/> until a file gets created in the download directory</para>
    /// <para>- or -</para>
    /// <para>the server does not send new chunks of data to the client longer than the <paramref name="downloadUpdatedTimeout"/></para>
    /// <para>- or -</para>
    /// <para>the expected file cannot be identified in the download directory</para>
    /// <para>- or -</para>
    /// <para>the download directory is in an unrecoverable state due to other programs writing into the download directory at the same time.</para>
    /// </exception>
    [NotNull]
    IDownloadedFile HandleDownloadWithExpectedFileName ([NotNull] string fileName, TimeSpan? downloadStartedTimeout = null, TimeSpan? downloadUpdatedTimeout = null);

    /// <summary>
    /// Handles the download of a file which has been triggered by an earlier action. 
    /// </summary>
    /// <param name="downloadUpdatedTimeout">
    /// Optional parameter specifying the maximum time for the server to respond to the download request or wait between sending chunks of data to the client. 
    /// If <see langword="null" />, the configured <see cref="WebTestConfigurationSection.DownloadUpdatedTimeout"/> is used.
    /// </param>
    /// <param name="downloadStartedTimeout">
    /// Optional parameter specifying the maximum time for the server to respond to the download request or wait between sending chunks of data to the client. 
    /// If <see langword="null" />, the configured <see cref="WebTestConfigurationSection.DownloadUpdatedTimeout"/> is used.
    /// </param>
    /// <exception cref="DownloadResultNotFoundException">
    /// <para>Thrown if it takes longer than the <paramref name="downloadStartedTimeout"/> until a file gets created in the download directory</para>
    /// <para>- or -</para>
    /// <para>the server does not send new chunks of data to the client longer than the <paramref name="downloadUpdatedTimeout"/></para>
    /// <para>- or -</para>
    /// <para>there is none or more than one newly found file in the download directory</para>
    /// <para>- or -</para>
    /// <para>the download directory is in an unrecoverable state due to other programs writing into the download directory at the same time.</para>
    /// </exception>
    [NotNull]
    IDownloadedFile HandleDownloadWithDetectedFileName (TimeSpan? downloadStartedTimeout = null, TimeSpan? downloadUpdatedTimeout = null);

    /// <summary>
    /// Cleans the download directory from all files downloaded in the current browser session. Noted that this API is meant to be called from the Infrastructure (<see cref="WebTestHelper"/>.<see cref="WebTestHelper.OnTearDown"/>), not user code.
    /// </summary>
    void DeleteFiles ();
  }
}