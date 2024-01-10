using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  public static class FluentControlSelectorExtensionsForIntegrationTests
  {
    public static FluentControlSelector<DropDownListSelector, DropDownListControlObject> DropDownLists (this IControlHost host)
    {
      return new FluentControlSelector<DropDownListSelector, DropDownListControlObject>(host, new DropDownListSelector());
    }

    public static FluentControlSelector<ScopeSelector, ScopeControlObject> Scopes (this IControlHost host)
    {
      return new FluentControlSelector<ScopeSelector, ScopeControlObject>(host, new ScopeSelector());
    }

    public static FluentControlSelector<TextBoxSelector, TextBoxControlObject> TextBoxes (this IControlHost host)
    {
      return new FluentControlSelector<TextBoxSelector, TextBoxControlObject>(host, new TextBoxSelector());
    }

    public static FluentControlSelector<WebButtonSelector, WebButtonControlObject> WebButtons (this IControlHost host)
    {
      return new FluentControlSelector<WebButtonSelector, WebButtonControlObject>(host, new WebButtonSelector());
    }
  }
}
