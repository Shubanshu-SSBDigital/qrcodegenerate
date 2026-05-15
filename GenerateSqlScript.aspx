<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GenerateSqlScript.aspx.cs" Inherits="GenerateSqlScript" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Generate SQL Script</title>
</head>
<body>
    <form method="post" runat="server" id="form1">
        <div style="text-align:center; margin-top:50px;">
            <asp:Button ID="btnGenerateScript" runat="server" Text="Generate SQL Insert Script" OnClick="btnGenerateScript_Click" 
                        Style="padding:10px 20px; font-size:16px;" />
            <br /><br />
            <asp:Label ID="lblMessage" runat="server" Style="color:Green; font-size:16px;"></asp:Label>
        </div>
    </form>
</body>
</html>