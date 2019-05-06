using System;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public class WebGenericTestPageParameterFactory : GenericTestPageParameterFactory
  {
    private TestConstants TestConstants { get; } = new TestConstants();

    public GenericTestPageParameter CreateTextContentSelector ()
    {
      return new GenericTestPageParameter (
          TestConstants.TextContentSelectorID,
          TestConstants.VisibleTextContentID,
          TestConstants.HiddenTextContentID,
          TestConstants.VisibleHtmlID);
    }

    public GenericTestPageParameter CreateTitleSelector ()
    {
      return new GenericTestPageParameter (
          TestConstants.TitleSelectorID,
          TestConstants.VisibleTitleID,
          TestConstants.HiddenTitleID,
          TestConstants.VisibleHtmlID);
    }

    public GenericTestPageParameter CreateItemIDSelector ()
    {
      return new GenericTestPageParameter (
          TestConstants.ItemIDSelectorID,
          TestConstants.VisibleControlID,
          TestConstants.HiddenControlID,
          TestConstants.VisibleHtmlID);
    }
  }
}