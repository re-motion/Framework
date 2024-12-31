// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

/// <summary>
/// Dummy class that should eventually imitate the behaviour of System.Drawing.Graphics
/// </summary>
public class StringFormat
{
  public StringFormat (StringFormatFlags flags, StringTrimming trimming)
  {
    FormatFlags = flags;
    Trimming = trimming;
  }

  public StringFormatFlags FormatFlags { get; set; }
  public StringTrimming Trimming { get; set; }
  public static StringFormat GenericDefault { get; set; } = new(0, StringTrimming.Character);
}
