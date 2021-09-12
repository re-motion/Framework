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
<%@ Page Language="c#" CodeBehind="TestForm.aspx.cs" AutoEventWireup="false" Inherits="Remotion.ObjectBinding.Web.Test.Shared.TestForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <title>Test Form</title>
  <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
  <script type="text/javascript">
    function DoAspNetAjaxCall()
    {
      document.getElementById("Result").textContent = "";

      var intValueAsString = document.getElementById("IntField").value;
      intValueAsString = intValueAsString == '' ? 0 : intValueAsString;
      var params = {
        stringValue: document.getElementById("StringField").value,
        intValue: parseInt(intValueAsString)
      };
      executingRequest = Sys.Net.WebServiceProxy.invoke("TestService.asmx", "DoStuff", false, params,
                                          function (result, context, methodName)
                                          {
                                            executingRequest = null;
                                            document.getElementById("Result").textContent = result;
                                          },
                                          function (err, context, methodName)
                                          {
                                            executingRequest = null;
                                            document.getElementById("Result").textContent = err.get_message();
                                          });

    }
    function DoXmlHttpRequestCall()
    {
      document.getElementById("Result").textContent = "";

      var intValueAsString = document.getElementById("IntField").value;
      intValueAsString = intValueAsString == '' ? 0 : intValueAsString;
      var params = {
        stringValue: document.getElementById("StringField").value,
        intValue: parseInt(intValueAsString)
      };

      var request = new XMLHttpRequest();
      request.open('POST', "TestService.asmx/DoStuff", false);
      request.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
      var failedHandler = function() { 
        document.getElementById("Result").textContent = request.responseText;
      }
      request.onload = function() {
        if (request.status >= 200 && request.status <= 299)
          document.getElementById("Result").textContent = JSON.parse(request.response).d;
        else
          failedHandler();
      };
      request.onerror = failedHandler;
      request.send("{ stringValue: '" + params.stringValue + "', intValue: " + params.intValue + "}");
    }
  </script>
</head>
<body>
  <form id="form1" runat="server">
  <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true"
    AsyncPostBackTimeout="3600" />
  <input id="StringField" type="text" placeholder="String to be mirrored | throw"/>
  <input id="IntField" type="text" placeholder="Number" />
  <input type="button" value="ASP.NET Ajax" onclick="DoAspNetAjaxCall()" />
  <input type="button" value="XMLHttpRequest Ajax" onclick="DoXmlHttpRequestCall()" />
  <div id="Result">
  </div>
  </form>
  <script type="text/javascript">
    document.getElementById("StringField").addEventListener("keydown", function (event)
    {
      // re-motion: block event bubbling
      event.stopPropagation();
      if (event.keyCode == 9) // TAB
      {
        DoXmlHttpRequestCall();
      } 
    });
  </script>
</body>
</html>
