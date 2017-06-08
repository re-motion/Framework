using System;
using System.Collections.Generic;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.SecurityManager.PerformanceTests
{
  public sealed class SimpleSecurityContext : ISecurityContext
  {
    private readonly string _class;
    private readonly string _owner;
    private readonly string _ownerGroup;
    private readonly string _ownerTenant;
    private readonly bool _isStateless;
    private readonly Dictionary<string, EnumWrapper> _states;
    private readonly EnumWrapper[] _abstractRoles;

    public SimpleSecurityContext (
        string @class,
        string owner,
        string ownerGroup,
        string ownerTenant,
        bool isStateless,
        Dictionary<string, EnumWrapper> states,
        EnumWrapper[] abstractRoles)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("class", @class);
      ArgumentUtility.CheckNotNull ("states", states);
      ArgumentUtility.CheckNotNull ("abstractRoles", abstractRoles);

      _class = @class;
      _owner = StringUtility.EmptyToNull (owner);
      _ownerGroup = StringUtility.EmptyToNull (ownerGroup);
      _ownerTenant = StringUtility.EmptyToNull (ownerTenant);
      _isStateless = isStateless;
      _states = states;
      _abstractRoles = abstractRoles;
    }

    public string Class
    {
      get { return _class; }
    }

    public string Owner
    {
      get { return _owner; }
    }

    public string OwnerGroup
    {
      get { return _ownerGroup; }
    }

    public string OwnerTenant
    {
      get { return _ownerTenant; }
    }

    public IEnumerable<EnumWrapper> AbstractRoles
    {
      get { return _abstractRoles; }
    }

    public EnumWrapper GetState (string propertyName)
    {
      return _states[propertyName];
    }

    public bool ContainsState (string propertyName)
    {
      return _states.ContainsKey (propertyName);
    }

    public bool IsStateless
    {
      get { return _isStateless; }
    }

    public int GetNumberOfStates ()
    {
      return _states.Count;
    }
  }
}