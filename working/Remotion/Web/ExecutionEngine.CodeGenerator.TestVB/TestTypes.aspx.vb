Imports Remotion.Web.ExecutionEngine
'  <Parameter name="p2" type="System.String" /> 
'  <Parameter name="ps6a" type="String[,]()" /> - does not work: CodeDOM-style [] before VB-style (), won't get converted to (,)()
'  <Parameter name="ps6b" type="String(,)[]" /> - does not work: CodeDOM tries to split type and array dimensions for local vars and parameters: dim ps6b() as String(,)


'<WxeFunction>
'  <Parameter name="p1" type="String" />
'  <Parameter name="p4" type="String()" />
'  <Parameter name="p5" type="String(,)" />
'  <Parameter name="p6" type="String(,)()" />
'  <Parameter name="p3" type="Nullable (Of Integer)" />
'  <Parameter name="p7" type="List (Of String())" />
'  <Parameter name="p8" type="List (Of String())()" />
'  <Parameter name="p9" type="List (Of nullable (Of Integer)())()" />
'  <Parameter name="ps4" type="String[]" />
'  <Parameter name="ps5" type="String[,]" />
'  <Parameter name="ps6" type="String[,][]" />
'</WxeFunction>
Partial Public Class TestTypes
  Inherits WxePage

  Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    Dim ints1 As Integer() = Nothing
    Dim ints2() As Integer = Nothing
    Dim ints3()() As Integer = Nothing


    Dim s As System.String = Nothing
    Dim i As ab.Integer = Nothing
    Dim sin As [Single] = Nothing

  End Sub

End Class

Class [Single]

End Class

Namespace ab
  Class [Integer]

  End Class
End Namespace