<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ExcelPasswordHashGenrate.aspx.cs" Inherits="ExcelPasswordHashGenrate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Excel Password Hash Generator</title>

    <style type="text/css">
        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 40px;
        }

        .container {
            width: 600px;
            margin: 50px auto;
            background: #ffffff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.15);
        }

        h2 {
            margin-top: 0;
            color: #333333;
        }

        .upload-control {
            margin: 20px 0;
        }

        .btn {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 5px;
            cursor: pointer;
            font-size: 15px;
        }

        .btn:hover {
            background-color: #0056b3;
        }

        .message {
            display: block;
            margin-top: 20px;
            color: #333333;
        }

        .error {
            color: red;
        }

        .success {
            color: green;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="container">

            <h2>Excel Password Hash Generator</h2>

            <p>
                Upload an Excel file. The <strong>Password</strong> column will be
                converted to SHA-256 hash.
            </p>

            <div class="upload-control">
                <asp:FileUpload
                    ID="fuExcel"
                    runat="server"
                    accept=".xlsx,.xls" />
            </div>

            <div>
                <asp:Button
                    ID="btnGenerateHash"
                    runat="server"
                    Text="Generate Password Hash"
                    CssClass="btn"
                    OnClick="btnGenerateHash_Click" />
            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="message">
            </asp:Label>

        </div>

    </form>
</body>
</html>