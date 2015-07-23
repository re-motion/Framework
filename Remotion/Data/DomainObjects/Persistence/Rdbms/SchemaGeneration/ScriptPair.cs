using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="ScriptPair"/> holds a setup- and a teardown-script of a relational database.
  /// </summary>
  public struct ScriptPair
  {
    private readonly string _setUpScript;
    private readonly string _tearDownScript;

    public ScriptPair (string setUpScript, string tearDownScript)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("setUpScript", setUpScript);
      ArgumentUtility.CheckNotNullOrEmpty ("tearDownScript", tearDownScript);

      _setUpScript = setUpScript;
      _tearDownScript = tearDownScript;
    }

    public string SetUpScript
    {
      get { return _setUpScript; }
    }

    public string TearDownScript
    {
      get { return _tearDownScript; }
    }
  }
}