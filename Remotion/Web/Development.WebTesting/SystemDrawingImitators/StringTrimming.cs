// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Remotion.Web.Development.WebTesting.SystemDrawingImitators;

/// <summary>
///  Specifies how to trim characters from a string that does not completely fit into a layout shape.
/// </summary>
public enum StringTrimming
{
    /// <summary>
    ///  Specifies no trimming.
    /// </summary>
    None,

    /// <summary>
    ///  Specifies that the string is broken at the boundary of the last character
    ///  that is inside the layout rectangle. This is the default.
    /// </summary>
    Character,

    /// <summary>
    ///  Specifies that the string is broken at the boundary of the last word that is inside the layout rectangle.
    /// </summary>
    Word,

    /// <summary>
    ///  Specifies that the string is broken at the boundary of the last character that is inside
    ///  the layout rectangle and an ellipsis (...) is inserted after the character.
    /// </summary>
    EllipsisCharacter,

    /// <summary>
    ///  Specifies that the string is broken at the boundary of the last word that is inside the
    ///  layout rectangle and an ellipsis (...) is inserted after the word.
    /// </summary>
    EllipsisWord
}
