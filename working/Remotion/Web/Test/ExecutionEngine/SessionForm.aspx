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
<%@ Page language="c#" Codebehind="SessionForm.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Web.Test.ExecutionEngine.SessionForm" smartNavigation="False"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html>
  <head>
    <title>ClientForm</title>
<!--
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
-->
<remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>
<!--<body MS_POSITIONING="FlowLayout">-->
  </head>
<body>
<script language="javascript" type="text/javascript" >
<!--
/*  function Window_OnError(msg, url, linenumber)
    {
     alert (msg + ',' + url + ',' + linenumber);
     document.write ('<p>' + msg + ',' + url + ',' + linenumber + '<\/p>');
    }
    window.onerror= Window_OnError;
*/
//-->
</script>
    <form id=Form method=post runat="server">
<table style="WIDTH:100%; HEIGHT:100%">
<tr>
<td >
    <p><asp:label id="FunctionTokenLabel" runat="server">Token</asp:label>, 
    <asp:label id="PostBackIDLabel" runat="server">PostBackID</asp:label>,
    <asp:label id="ViewStateTokenLabel" runat="server">ViewStateToken</asp:label></p>
    <remotion:WebButton id="PostBackButton" runat="server" Text="PostBack"></remotion:WebButton><remotion:WebButton id="OpenSelfButton" runat="server" Text="Open Self"></remotion:WebButton> 
    <asp:linkbutton id="LinkButton1" runat="server">LinkButton</asp:linkbutton>
    <a id="LinkButton2" href="#" onclick="__doPostBack('LinkButton1',''); return false;">LinkButton 2</a>
<p>
<asp:button id="Button1" runat="server" Text="Button"></asp:button>
</p><p>
<remotion:webbutton id="Button1Button" runat="server" Text="Button 1" UseSubmitBehavior="False"></remotion:webbutton>
</p><p>
<remotion:webbutton id="Submit1Button" runat="server" Text="Submit 1"></remotion:webbutton>
</p><p>
<remotion:webbutton id="ExecuteButton" runat="server" Text="Execute"></remotion:webbutton>
</p><p>
<remotion:webbutton id="ExecuteNoRepostButton" runat="server" Text="Execute, No Repost"></remotion:webbutton>
</p><p>
<remotion:webbutton id="Button2Button" runat="server" Text="Button 2" UseSubmitBehavior="False"></remotion:webbutton>
</p><p>
<asp:ImageButton id="ImageButton" runat="server"></asp:ImageButton> (Image Button) <asp:Label runat="server" ID="ImageButtonLabel" EnableViewState="False"/>
</p>
      <p><remotion:WebButton id="OpenSampleFunctionButton" runat="server" Text="Open Sample Function"></remotion:WebButton><br>
<remotion:WebButton id="OpenSampleFunctionWithPermanentUrlButton" runat="server" Text="Open Sample Function with Permant URL"></remotion:WebButton><br>
<remotion:WebButton id="OpenSampleFunctionInNewWindowButton" runat="server" Text="Open Sample Function in New Window"></remotion:WebButton><br>
<remotion:WebButton id="OpenSampleFunctionWithPermanentUrlInNewWindowButton" runat="server" Text="Open Sample Function with Permanent URL in New Window"></remotion:WebButton><br>

<remotion:WebButton id="OpenSessionFunctionButton" runat="server" Text="Open Session Function"></remotion:WebButton><br>
<remotion:WebButton id="OpenSessionFunctionWithPermanentUrlButton" runat="server" Text="Open Session Function with Permanent URL"></remotion:WebButton><br>
<remotion:WebButton id="OpenSessionFunctionInNewWindowButton" runat="server" Text="Open Session Function in New Window"></remotion:WebButton><br>
<remotion:WebButton id="OpenSessionFunctionWithPermanentUrlInNewWindowButton" runat="server" Text="Open Session Function with Permanent URL in New Window"></remotion:WebButton><br>

<remotion:webbutton id="OpenSampleFunctionByRedirectButton" runat="server" Text="Open Sample Function by Redirect"></remotion:webbutton><br>
<remotion:webbutton id="OpenSampleFunctionByRedirectDoNotReturnButton" runat="server" Text="Open Sample Function by Redirect, do not return to Caller"></remotion:webbutton><br>
<remotion:webbutton id="OpenSampleFunctionWithPermanentUrlByRedirectButton" runat="server" Text="Open Sample Function with Permanent URL by Redirect"></remotion:webbutton><br>
<remotion:webbutton id="OpenSampleFunctionWithPermanentUrlByDoNotReturnRedirectButton" runat="server" Text="Open Sample Function with Permanent URL by Redirect, do not return to Caller"></remotion:webbutton><br>

<remotion:WebButton id="ContextOpenSampleFunctionButton" runat="server" Text="WxeContext: Open Sample Function"></remotion:WebButton><br>
<remotion:WebButton id="ContextOpenSampleFunctionInNewWindowButton" runat="server" Text="WxeContext: Open Sample Function in New Window"></remotion:WebButton><br>
<remotion:WebButton id="ContextOpenSampleFunctionWithPermanentUrlButton" runat="server" Text="WxeContext: Open Sample Function with Permanent URL"></remotion:WebButton><br>
<remotion:WebButton id="ContextOpenSampleFunctionWithPermanentUrlInNewWindowButton" runat="server" Text="WxeContext: Open Sample Function with Permanent URL in New Window"></remotion:WebButton><br>
Permalink this: <asp:HyperLink id="CurrentFunctionPermaLink" runat="server"></asp:HyperLink><br>Permalink 
Sample: <asp:HyperLink id="SampleFunctionPermaLink" runat="server"></asp:HyperLink></p>
<p><a href="javascript:alert('script in href');">script in href</a></p>
<p><a href="#" onclick="alert('script in onclick'); return false;">script in onclick</a></p>
<input type="submit" value="testvalue" name="testname">
</td>
</tr>
</table>
</form>
    <script language="javascript" type="text/javascript" >
<!--
    function Page_Abort ()
    {
    }
    
    function Page_Load ()
    { 
      /*
      // IE Only
      var windowTop = 25 + 24 + 24 + 24; // Title bar, menu bar, stanarddbuttons bar, address bar
      var windowBottom = 24;
      var windowLeft = 4;
      var windowRight = 4;
      
      _width = document.body.offsetWidth + windowLeft + windowRight;
      _height = document.body.offsetHeight + windowTop + windowBottom;
      _left = window.screenLeft - windowLeft;
      _top = window.screenTop - windowTop;

      var newWidth = 300;
      var newHeight = 150;
      window.resizeTo (newWidth, newHeight);
      
      var newLeft = ((screen.width - document.body.clientWidth - windowLeft - windowRight) / 2)
      var newTop = ((screen.height - document.body.clientHeight - windowTop - windowBottom) / 2);      
      window.moveTo (newTop, _top);
      */
    }
    
    function Page_BeforeUnload ()
    {
    }
    
    function Page_Unload ()
    {
      /*
      window.resizeTo (_width, _height);
      window.moveTo (_left, _top);
      */
    }
//-->    
    </script>
  </body>
</html>
