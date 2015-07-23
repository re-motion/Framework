using System;
using System.Linq;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class StubSecurityProvider : ISecurityProvider
  {
    private readonly AccessType[] _accessTypes = Enum.GetValues(typeof (GeneralAccessTypes)).Cast<Enum>().Select (e => AccessType.Get ((Enum) e)).ToArray();

    public StubSecurityProvider ()
    {
    }

    public bool IsNull
    {
      get { return false; }
    }

    public AccessType[] GetAccess (ISecurityContext context, ISecurityPrincipal principal)
    {
      return _accessTypes;
    }
  }
}