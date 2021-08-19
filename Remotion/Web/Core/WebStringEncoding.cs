namespace Remotion.Web
{
  /// <summary>
  /// Defines the different encodings that a <see cref="WebString"/> can be encoded in.
  /// </summary>
  public enum WebStringEncoding
  {
    /// <summary>
    /// An unencoded <see cref="WebString"/> will be HTML encoded.
    /// </summary>
    Html,
    
    /// <summary>
    /// An unencoded <see cref="WebString"/> will be HTML encoded and line break characters will be replaced by &lt;br /&gt; tags.
    /// </summary>
    HtmlWithTransformedLineBreaks,
    
    /// <summary>
    /// An unencoded <see cref="WebString"/> will be HTML attribute encoded.
    /// </summary>
    Attribute
  }
}