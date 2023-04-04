using System.Web.UI;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  [ImplementationFor(typeof(IBocListValidationSummaryBlockRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocListValidationSummaryBlockRenderer : RendererBase<BocList>, IBocListValidationSummaryBlockRenderer
  {
    private readonly BocListCssClassDefinition _cssClasses;
    private readonly IFallbackNavigationUrlProvider _fallbackNavigationUrlProvider;

    public BocListValidationSummaryBlockRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        BocListCssClassDefinition cssClasses,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull("fallbackNavigationUrlProvider", fallbackNavigationUrlProvider);

      _cssClasses = cssClasses;
      _fallbackNavigationUrlProvider = fallbackNavigationUrlProvider;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public void Render (BocListRenderingContext renderingContext, BocListValidationFailureRepository validationFailureRepository)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      var unhandledValidationFailures = validationFailureRepository.GetUnhandledValidationFailures();
      if (unhandledValidationFailures.Count == 0)
        return;

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.ValidationSummary);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Ul);
      foreach (var unhandledValidationFailure in unhandledValidationFailures)
      {
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Li);
        if (unhandledValidationFailure.ValidatedProperty != null)
        {
          renderingContext.Writer.WriteEncodedText($"{unhandledValidationFailure.ValidatedProperty.DisplayName}: {unhandledValidationFailure.ErrorMessage}");
        }
        else
        {
          renderingContext.Writer.WriteEncodedText(unhandledValidationFailure.ErrorMessage);
        }
        renderingContext.Writer.RenderEndTag();
      }
      renderingContext.Writer.RenderEndTag();

      renderingContext.Writer.RenderEndTag();
    }
  }
}
