using System;
using Remotion.Utilities;

namespace Remotion.Security.Development
{
  /// <summary>
  /// Allows for easy overriding of the <see cref="ISecurityProvider"/> in development scenarios.
  /// </summary>
  /// <remarks>
  /// Use <see cref="SetCustomSecurityProvider"/> to specify an <see cref="ISecurityProvider"/>
  /// and <see cref="ResetCustomSecurityProvider"/> to switch it back to a null object.
  /// </remarks>
  public class FakeSecurityProvider : ISecurityProvider
  {
    private static readonly ISecurityProvider s_defaultSecurityProvider = new NullSecurityProvider();
    private ISecurityProvider? _customSecurityProvider;

    public FakeSecurityProvider ()
    {
    }

    public void SetCustomSecurityProvider (ISecurityProvider provider)
    {
      ArgumentUtility.CheckNotNull("provider", provider);

      _customSecurityProvider = provider;
    }

    public void ResetCustomSecurityProvider ()
    {
      _customSecurityProvider = null;
    }

    public bool IsNull => (_customSecurityProvider ?? s_defaultSecurityProvider).IsNull;

    public AccessType[] GetAccess (ISecurityContext context, ISecurityPrincipal principal) => (_customSecurityProvider ?? s_defaultSecurityProvider).GetAccess(context, principal);
  }
}
