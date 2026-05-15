<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GenerateQrCode2.aspx.cs" Inherits="GenerateQrCode2" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Generate QR CSV</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: center; margin-top: 50px;">
            <asp:Button ID="btnGenerateCsv" runat="server" Text="Generate CSV File"
                OnClick="btnGenerateCsv_Click"
                Style="padding: 10px 20px; font-size: 16px;" />

            <asp:Button ID="btnGenerateExcel"
                runat="server"
                Text="Generate Excel"
                OnClick="btnGenerateExcel_Click"
                Style="padding: 10px 20px; font-size: 16px;" />

        </div>
    </form>
</body>
</html>