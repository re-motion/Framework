// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure;

/// <summary>
/// Implements the null object pattern for <see cref="IDownloadHelper"/>.
/// </summary>
public class NullDownloadHelper : IDownloadHelper
{
  public static readonly NullDownloadHelper Instance = new();

  private NullDownloadHelper ()
  {
  }

  public IDownloadedFile HandleDownloadWithExpectedFileName (string fileName, TimeSpan? downloadStartedTimeout, TimeSpan? downloadUpdatedTimeout)
  {
    throw new NotSupportedException();
  }

  public IDownloadedFile HandleDownloadWithDetectedFileName (TimeSpan? downloadStartedTimeout, TimeSpan? downloadUpdatedTimeout)
  {
    throw new NotSupportedException();
  }

  public void DeleteFiles ()
  {
  }
}
