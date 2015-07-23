using System;
using System.Collections.Specialized;
using System.Linq;
using Remotion.Configuration;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.Web.Test
{
  public class StubSecurityProvider : ExtendedProviderBase, ISecurityProvider
  {
    private readonly AccessType[] _accessTypes = Enum.GetValues(typeof (GeneralAccessTypes)).Cast<Enum>().Select (e => AccessType.Get ((Enum) e)).ToArray();

    public StubSecurityProvider ()
        : this ("Stub", new NameValueCollection())
    {
    }

    public StubSecurityProvider (string name, NameValueCollection config)
        : base (name, config)
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