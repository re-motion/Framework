namespace Remotion.Web
{
  /// <summary>
  /// Defines the kind of a <see cref="WebString"/> which determines if it is encoded when rendered.
  /// </summary>
  public enum WebStringType
  {
    /// <summary>
    /// The <see cref="WebString"/> contains plain text, which will be encoded before being rendered.
    /// </summary>
    PlainText,
    
    /// <summary>
    /// The <see cref="WebString"/> contains already encoded text, which will be rendered as is.
    /// </summary>
    Encoded
  }
}