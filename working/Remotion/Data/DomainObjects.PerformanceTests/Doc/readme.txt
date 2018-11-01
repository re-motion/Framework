How to run performance tests
============================

- Performance tests are run in NUnit 2.1. 
- See Console.Out for test results.
- Never use a debug build for performance tests. Use only release builds!
- Never run performance tests with a debugger attached. Always start NUnit in a separate process.
- Reference system:
  Hardware: Dell Dimension 5000 (Intel Pentium 4, 3,2 GHz, 2GB RAM), AT-VIE-DEV-9
  Software: Windows 2003 Enterprise Server, SQL Server 2005 Release Version (locally), .NET 1.1 SP1