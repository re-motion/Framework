using System;
using System.Web;
using Remotion.Utilities;

namespace Remotion.Development.Web.ResourceHosting
{
  /// <summary>
  /// used for mapping file name extensions to asp.net handlers
  /// </summary>
  public class FileExtensionHandlerMapping
  {
    public static FileExtensionHandlerMapping[] Default
    {
      get
      {
        return new[]
               {
                   new FileExtensionHandlerMapping ("jpg", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping ("jpeg", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping ("png", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping ("gif", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping ("js", ResourceVirtualPathProvider.StaticFileHandler),
                   new FileExtensionHandlerMapping ("css", ResourceVirtualPathProvider.StaticFileHandler),
               };
      }
    }

    private readonly string _extension;
    private readonly IHttpHandler _handler;

    public FileExtensionHandlerMapping (string extension, IHttpHandler handler)
    {
      ArgumentUtility.CheckNotNull ("extension", extension);
      ArgumentUtility.CheckNotNull ("handler", handler);

      _extension = extension;
      _handler = handler;
    }

    public string Extension
    {
      get { return _extension; }
    }

    public IHttpHandler Handler
    {
      get { return _handler; }
    }
  }
}