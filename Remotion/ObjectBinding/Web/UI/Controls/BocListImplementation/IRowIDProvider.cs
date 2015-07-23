using System;
using System.Collections;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation
{
  /// <summary>
  /// Defines the API for retrieving the row-ID for a <see cref="BocListRow"/> and the <see cref="BocListRow"/> for its row-ID.
  /// </summary>
  public interface IRowIDProvider
  {
    string GetControlRowID (BocListRow row);
    string GetItemRowID (BocListRow row);
    BocListRow GetRowFromItemRowID (IList rows, string rowID);
    void AddRow (BocListRow row);
    void RemoveRow (BocListRow row);
  }
}