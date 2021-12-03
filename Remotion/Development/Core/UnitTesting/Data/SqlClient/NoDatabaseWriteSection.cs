using System;
using System.Linq;

namespace Remotion.Development.UnitTesting.Data.SqlClient
{
  /// <summary>
  /// A section that ensures that no database writes have been made while the section was open.
  /// </summary>
  /// <remarks>
  /// This is done by comparing the last used timestamp values before and after the section.
  /// As such, only changes to tables that contain a rowversion will be detected.
  /// Also, UPDATE statements that have no effective changes will also count as a write.
  /// </remarks>
  public class NoDatabaseWriteSection : IDisposable
  {
    private readonly DatabaseAgent _databaseAgent;
    private readonly byte[] _initialRowVersion;

    public NoDatabaseWriteSection (DatabaseAgent databaseAgent, byte[] initialRowVersion)
    {
      _databaseAgent = databaseAgent;
      _initialRowVersion = initialRowVersion;
    }

    public void Dispose ()
    {
      var version = _databaseAgent.GetLastUsedTimestamp();
      if (!version.SequenceEqual(_initialRowVersion))
        throw new InvalidOperationException("Database changed while NoDatabaseWriteSection was open.");
    }
  }
}
