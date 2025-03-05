// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
namespace Remotion.Web.Development.WebTesting.ScreenshotCreation;

/// <summary>
/// Always returns <see cref="EmptyCursorInformation"/>.<see cref="EmptyCursorInformation.Instance"/> for <see cref="GetCursorInformation"/>.
/// </summary>
public class EmptyCursorInformationProvider : ICursorInformationProvider
{
  public static readonly EmptyCursorInformationProvider Instance = new();

  private EmptyCursorInformationProvider ()
  {
  }

  /// <inheritdoc />
  public ICursorInformation GetCursorInformation ()
  {
    return EmptyCursorInformation.Instance;
  }
}
