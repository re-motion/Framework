using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Remotion.Mixins.XRef
{
  public class XRefInAppDomainRunner
  {
    public int Run (string[] talkBackArgs, XRefArguments xRefArgs)
    {
      // Create new application domain and run cross referencer
      var appDomain = AppDomain.CurrentDomain;

      var setupInformation = appDomain.SetupInformation;
      setupInformation.ApplicationBase = xRefArgs.AssemblyDirectory;

      if (!string.IsNullOrEmpty(xRefArgs.AppBaseDirectory))
      {
        setupInformation.ApplicationBase = xRefArgs.AppBaseDirectory;

        var appBaseDirectory = xRefArgs.AppBaseDirectory;
        if (!appBaseDirectory.EndsWith("\\"))
          appBaseDirectory += "\\";

        if (xRefArgs.AssemblyDirectory.StartsWith(appBaseDirectory))
        {
          var relativeSearchPath = xRefArgs.AssemblyDirectory.Remove(0, appBaseDirectory.Length);
          if (!string.IsNullOrEmpty(relativeSearchPath))
          {
            setupInformation.PrivateBinPath = relativeSearchPath;
          }
        }
        else
        {
          throw new ArgumentException("Input directory is not a sub directory of application base directory!");
        }
      }

      if (!string.IsNullOrEmpty(xRefArgs.AppConfigFile))
      {
        if (!File.Exists(xRefArgs.AppConfigFile))
          throw new ArgumentException(string.Format("Supplied app-config file '{0}' does not exist.", xRefArgs.AppConfigFile));

        setupInformation.ConfigurationFile = xRefArgs.AppConfigFile;

        // The PrivateBinPath needs to be read manually from the config because for some reason it does not via automatic setup.
        var privateBinPath = GetPrivateBinPathFromConfig(xRefArgs.AppConfigFile);
        if (!string.IsNullOrEmpty(privateBinPath))
        {
          if (string.IsNullOrEmpty(setupInformation.PrivateBinPath))
            setupInformation.PrivateBinPath = privateBinPath;
          else
            setupInformation.PrivateBinPath = setupInformation.PrivateBinPath + ";" + privateBinPath;
        }
      }

      XRef.Run(xRefArgs);
      return 1;
    }

    private string GetPrivateBinPathFromConfig (string appConfigFile)
    {
      var configXml = XDocument.Load(appConfigFile);
      var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
      xmlNamespaceManager.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");
      var probing = configXml.XPathSelectElement("/configuration/runtime/asm:assemblyBinding/asm:probing", xmlNamespaceManager);
      if (probing == null)
        return null;
      return probing.Attribute("privatePath").Value;
    }
  }
}
