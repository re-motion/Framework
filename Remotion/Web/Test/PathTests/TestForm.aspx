<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page language="c#" Codebehind="TestForm.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Web.Test.PathTests.TestForm" %>
<%@ Import Namespace="Remotion.Web.Utilities" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Path Tests</title>
  </head>
  <body>
    <form id="Form1" method="post" runat="server">
      <table>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~")                </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~")                 %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~/")               </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~/")                %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~/test")           </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~/test")            %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~/.")              </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~/.")               %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~/./path")         </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~/./path")          %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~/path/.")         </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~/path/.")          %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~/path1/../path2") </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~/path1/../path2")  %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~/../path2")       </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~/../path2")        %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~/~")              </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~/~")               %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~/~/")             </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~/~/")              %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~/~/test")         </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~/~/test")          %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("~\\test")          </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "~\\test")           %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("/test/path")       </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "/test/path")        %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("\\test\\path")     </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "\\test\\path")      %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("/test\\path")      </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "/test\\path")       %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("\\test/path")      </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "\\test/path")       %>"</td></tr>
        <tr><td>UrlUtility.GetAbsoluteUrl ("")                 </td><td>"<%= UrlUtility.GetAbsoluteUrl (Context, "")                  %>"</td></tr>

        <tr><td>&nbsp;</td><td></td></tr>

        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~")                </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~")                 %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~/")               </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~/")                %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~/test")           </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~/test")            %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~/.")              </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~/.")               %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~/./path")         </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~/./path")          %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~/path/.")         </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~/path/.")          %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~/path1/../path2") </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~/path1/../path2")  %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~/../path2")       </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~/../path2")        %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~/~")              </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~/~")               %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~/~/")             </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~/~/")              %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~/~/test")         </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~/~/test")          %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("~\\test")          </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "~\\test")           %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("/test/path")       </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "/test/path")        %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("\\test\\path")     </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "\\test\\path")      %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("/test\\path")      </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "/test\\path")       %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("\\test/path")      </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "\\test/path")       %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("")                 </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "")                  %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("test/path")        </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "test/path")         %>"</td></tr>
        <tr><td>UrlUtility.ResolveUrlCaseSensitive ("test\\path")       </td><td>"<%= UrlUtility.ResolveUrlCaseSensitive (Context, "test\\path")        %>"</td></tr>

        <tr><td>&nbsp;</td><td></td></tr>

        <tr><td>HttpResponse.ApplyAppPathModifier ("~")                </td><td>"<%= Response.ApplyAppPathModifier ("~")                 %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~/")               </td><td>"<%= Response.ApplyAppPathModifier ("~/")                %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~/test")           </td><td>"<%= Response.ApplyAppPathModifier ("~/test")            %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~/.")              </td><td>"<%= Response.ApplyAppPathModifier ("~/.")               %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~/./path")         </td><td>"<%= Response.ApplyAppPathModifier ("~/./path")          %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~/path/.")         </td><td>"<%= Response.ApplyAppPathModifier ("~/path/.")          %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~/path1/../path2") </td><td>"<%= Response.ApplyAppPathModifier ("~/path1/../path2")  %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~/../path2")       </td><td>"<%= Response.ApplyAppPathModifier ("~/../path2")        %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~/~")              </td><td>"<%= Response.ApplyAppPathModifier ("~/~")               %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~/~/")             </td><td>"<%= Response.ApplyAppPathModifier ("~/~/")              %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~/~/test")         </td><td>"<%= Response.ApplyAppPathModifier ("~/~/test")          %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("~\\test")          </td><td>"<%= Response.ApplyAppPathModifier ("~\\test")           %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("/test/path")       </td><td>"<%= Response.ApplyAppPathModifier ("/test/path")        %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("\\test\\path")     </td><td>"<%= Response.ApplyAppPathModifier ("\\test\\path")      %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("/test\\path")      </td><td>"<%= Response.ApplyAppPathModifier ("/test\\path")       %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("\\test/path")      </td><td>"<%= Response.ApplyAppPathModifier ("\\test/path")       %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("")                 </td><td>"<%= Response.ApplyAppPathModifier ("")                  %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("test/path")        </td><td>"<%= Response.ApplyAppPathModifier ("test/path")         %>"</td></tr>
        <tr><td>HttpResponse.ApplyAppPathModifier ("test\\path")       </td><td>"<%= Response.ApplyAppPathModifier ("test\\path")        %>"</td></tr>

        <tr><td>&nbsp;</td><td></td></tr>

        <tr><td>Control.ResolveUrl ("~")                </td><td>"<%= ResolveUrl ("~")                 %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~/")               </td><td>"<%= ResolveUrl ("~/")                %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~/test")           </td><td>"<%= ResolveUrl ("~/test")            %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~/.")              </td><td>"<%= ResolveUrl ("~/.")               %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~/./path")         </td><td>"<%= ResolveUrl ("~/./path")          %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~/path/.")         </td><td>"<%= ResolveUrl ("~/path/.")          %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~/path1/../path2") </td><td>"<%= ResolveUrl ("~/path1/../path2")  %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~/../path2")       </td><td>"<%= ResolveUrl ("~/../path2")        %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~/~")              </td><td>"<%= ResolveUrl ("~/~")               %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~/~/")             </td><td>"<%= ResolveUrl ("~/~/")              %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~/~/test")         </td><td>"<%= ResolveUrl ("~/~/test")          %>"</td></tr>
        <tr><td>Control.ResolveUrl ("~\\test")          </td><td>"<%= ResolveUrl ("~\\test")           %>"</td></tr>
        <tr><td>Control.ResolveUrl ("/test/path")       </td><td>"<%= ResolveUrl ("/test/path")        %>"</td></tr>
        <tr><td>Control.ResolveUrl ("\\test\\path")     </td><td>"<%= ResolveUrl ("\\test\\path")      %>"</td></tr>
        <tr><td>Control.ResolveUrl ("/test\\path")      </td><td>"<%= ResolveUrl ("/test\\path")       %>"</td></tr>
        <tr><td>Control.ResolveUrl ("\\test/path")      </td><td>"<%= ResolveUrl ("\\test/path")       %>"</td></tr>
        <tr><td>Control.ResolveUrl ("")                 </td><td>"<%= ResolveUrl ("")                  %>"</td></tr>
        <tr><td>Control.ResolveUrl ("test/path")        </td><td>"<%= ResolveUrl ("test/path")         %>"</td></tr>
        <tr><td>Control.ResolveUrl ("test\\path")       </td><td>"<%= ResolveUrl ("test\\path")        %>"</td></tr>

        <tr><td>&nbsp;</td><td></td></tr>

        <tr><td>Control.ResolveClientUrl ("~")                </td><td>"<%= ResolveClientUrl ("~")                 %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~/")               </td><td>"<%= ResolveClientUrl ("~/")                %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~/test")           </td><td>"<%= ResolveClientUrl ("~/test")            %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~/.")              </td><td>"<%= ResolveClientUrl ("~/.")               %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~/./path")         </td><td>"<%= ResolveClientUrl ("~/./path")          %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~/path/.")         </td><td>"<%= ResolveClientUrl ("~/path/.")          %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~/path1/../path2") </td><td>"<%= ResolveClientUrl ("~/path1/../path2")  %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~/../path2")       </td><td>"<%= ResolveClientUrl ("~/../path2")        %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~/~")              </td><td>"<%= ResolveClientUrl ("~/~")               %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~/~/")             </td><td>"<%= ResolveClientUrl ("~/~/")              %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~/~/test")         </td><td>"<%= ResolveClientUrl ("~/~/test")          %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("~\\test")          </td><td>"<%= ResolveClientUrl ("~\\test")           %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("/test/path")       </td><td>"<%= ResolveClientUrl ("/test/path")        %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("\\test\\path")     </td><td>"<%= ResolveClientUrl ("\\test\\path")      %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("/test\\path")      </td><td>"<%= ResolveClientUrl ("/test\\path")       %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("\\test/path")      </td><td>"<%= ResolveClientUrl ("\\test/path")       %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("")                 </td><td>"<%= ResolveClientUrl ("")                  %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("test/path")        </td><td>"<%= ResolveClientUrl ("test/path")         %>"</td></tr>
        <tr><td>Control.ResolveClientUrl ("test\\path")       </td><td>"<%= ResolveClientUrl ("test\\path")        %>"</td></tr>

      </table>
    </form>
  </body>
</html>
