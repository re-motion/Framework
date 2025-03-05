// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
namespace Remotion.Web.Development.WebTesting.ScreenshotCreation;

/// <summary>
/// Defines an API for retrieving the <see cref="ICursorInformation"/> for the current mouse state.
/// </summary>
public interface ICursorInformationProvider
{
  ICursorInformation GetCursorInformation ();
}
