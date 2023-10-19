using System;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Used by <see cref="IBocColumnDefinitionWithRowIconSupport"/> to determine where the row icon
  /// should be shown in the <see cref="BocList"/>.
  /// </summary>
  public enum RowIconMode
  {
    /// <summary>
    /// The first column with the `<see cref="Automatic"/>` option will show the icon unless a column with the `<see cref="Preferred"/>` option exists.
    /// </summary>
    Automatic,

    /// <summary>
    /// Columns with the `<see cref="Disabled"/>` option will never show an icon.
    /// </summary>
    Disabled,

    /// <summary>
    /// The first column with the <see cref="Preferred"/> option will show the icon.
    /// </summary>
    Preferred
  }
}
