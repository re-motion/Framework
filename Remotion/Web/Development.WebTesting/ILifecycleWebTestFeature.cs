using System;

namespace Remotion.Web.Development.WebTesting;

/// <summary>
/// Defines a contract for web test components that require explicit setup 
/// and teardown logic during the test execution lifecycle.
/// </summary>
public interface ILifecycleWebTestFeature : IDisposable
{
  /// <summary>
  /// Prepares the feature for use. This must be called after construction 
  /// but before any other members are accessed.
  /// </summary>
  void Initialize ();
}
