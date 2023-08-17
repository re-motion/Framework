using Remotion.Mixins;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.Web.UnitTests.Domain
{
  [BindableObject]
  public class HostTypeForTypeWithString
  {
    public static HostTypeForTypeWithString Create ()
    {
      return ObjectFactory.Create<HostTypeForTypeWithString>(true, ParamList.Empty);
    }

    public TypeWithString[] Strings { get; set; }

    protected HostTypeForTypeWithString ()
    {
    }
  }
}
