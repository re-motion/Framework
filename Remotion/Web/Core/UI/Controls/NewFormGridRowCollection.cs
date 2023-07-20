using System;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{
  public class NewFormGridRowCollection : ControlItemCollection
  {
    private static readonly Type[] s_supportedTypes = { typeof(NewFormGridRow) };

    private readonly NewFormGrid _formGrid;

    public NewFormGridRowCollection (NewFormGrid formGrid)
        : base(formGrid, s_supportedTypes)
    {
      _formGrid = formGrid;
    }

    protected override void OnInsertComplete (int index, object? value)
    {
      if (value != null)
        _formGrid.Controls.Add((Control)value);
    }

    public new NewFormGridRow[] ToArray ()
    {
      return (NewFormGridRow[])InnerList.ToArray(typeof(NewFormGridRow));
    }

    //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
    protected internal new NewFormGridRow this [int index]
    {
      get { return (NewFormGridRow)List[index]!; }
      set { List[index] = value; }
    }
  }
}
