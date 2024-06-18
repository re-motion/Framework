using System.Xml.Linq;

namespace Remotion.Mixins.XRef.Report
{
  public interface IXmlReportGenerator
  {
    XDocument GenerateXmlDocument ();
  }
}
