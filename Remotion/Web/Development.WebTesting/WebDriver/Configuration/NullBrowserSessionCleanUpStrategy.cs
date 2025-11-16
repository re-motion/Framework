// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration;

/// <summary>
/// Null object pattern implementation for <see cref="IBrowserSessionCleanUpStrategy"/>.
/// </summary>
public class NullBrowserSessionCleanUpStrategy : IBrowserSessionCleanUpStrategy
{
  public static readonly NullBrowserSessionCleanUpStrategy Instance = new();

  private NullBrowserSessionCleanUpStrategy ()
  {
  }

  /// <inheritdoc />
  public void CleanUp ()
  {
  }
}
