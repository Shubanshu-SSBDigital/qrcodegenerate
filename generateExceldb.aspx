<%@ Page Language="C#" AutoEventWireup="true"
    CodeFile="generateExceldb.aspx.cs"
    Inherits="generateExceldb" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Export QR Code Data</title>
    <style>
        body {
            font-family: Arial;
            margin-top: 50px;
        }

        .container {
            text-align: center;
        }

        .btn {
            padding: 12px 24px;
            font-size: 16px;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            margin: 10px;
        }

        .csv {
            background-color: #198754;
        }

        .excel {
            background-color: #0d6efd;
        }

        .message {
            display: block;
            margin-top: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Export QR Code Master</h2>

            <asp:Button ID="btnCSV" runat="server"
                Text="Download CSV"
                CssClass="btn csv"
                OnClick="btnCSV_Click" />

            <asp:Button ID="btnExcel" runat="server"
                Text="Download Excel"
                CssClass="btn excel"
                OnClick="btnExcel_Click" />

            <asp:Label ID="lblMessage" runat="server"
                CssClass="message" />
        </div>
    </form>
</body>
</html>
