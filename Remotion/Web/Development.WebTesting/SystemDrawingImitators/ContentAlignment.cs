// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

/// <summary>
/// same as in System.Drawing.ContentAlignment as there are no windows dependencies in this enum
/// </summary>
public enum ContentAlignment
{
  /// <summary>Content is vertically aligned at the top, and horizontally aligned on the left.</summary>
  TopLeft = 1,
  /// <summary>Content is vertically aligned at the top, and horizontally aligned at the center.</summary>
  TopCenter = 2,
  /// <summary>Content is vertically aligned at the top, and horizontally aligned on the right.</summary>
  TopRight = 4,
  /// <summary>Content is vertically aligned in the middle, and horizontally aligned on the left.</summary>
  MiddleLeft = 16, // 0x00000010
  /// <summary>Content is vertically aligned in the middle, and horizontally aligned at the center.</summary>
  MiddleCenter = 32, // 0x00000020
  /// <summary>Content is vertically aligned in the middle, and horizontally aligned on the right.</summary>
  MiddleRight = 64, // 0x00000040
  /// <summary>Content is vertically aligned at the bottom, and horizontally aligned on the left.</summary>
  BottomLeft = 256, // 0x00000100
  /// <summary>Content is vertically aligned at the bottom, and horizontally aligned at the center.</summary>
  BottomCenter = 512, // 0x00000200
  /// <summary>Content is vertically aligned at the bottom, and horizontally aligned on the right.</summary>
  BottomRight = 1024, // 0x00000400
}
