// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
namespace Remotion.Web.Development.WebTesting;

public interface IBrowserSession
{
  BrowserWindow Window { get; }

  BrowserWindow FindWindow (string windowLocator);
}
