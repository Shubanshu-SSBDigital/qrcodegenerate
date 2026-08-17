using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;

public partial class GenerateQrCode : System.Web.UI.Page
{
    protected void btnGenerateCsv_Click(object sender, EventArgs e)
    {
        GenerateData(outputExcel: false);
    }

    protected void btnGenerateExcel_Click(object sender, EventArgs e)
    {
        GenerateData(outputExcel: true);
    }

    private void GenerateData(bool outputExcel)
    {
        //int totalRecords = 30000;
        //int startSerial = 1;


        int totalRecords = 500;
        int startSerial = 1;

        HashSet<string> usedQRText = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<string> usedQRValue = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        LoadExistingQRCodes(usedQRText, usedQRValue);

        List<QRData> qrList = new List<QRData>();
        Random rand = new Random();

        for (int i = 0; i < totalRecords; i++)
        {
            int serialNo = startSerial + i;

            string qrText = GenerateUniqueQRText(usedQRText, rand);
            string qrValue = GenerateUniqueQRValue(usedQRValue);

            qrList.Add(new QRData
            {
                //SerialNo = serialNo.ToString("D5"),
                SerialNo = serialNo.ToString("D3"),

                QRText = qrText,
                QRValue = qrValue
            });
        }

        if (outputExcel)
            ExportToExcel(qrList);
        else
            ExportToCsv(qrList);       
    }


    private void ExportToCsv(List<QRData> qrList)
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("SerialNo,QRText,QRValue");

        foreach (var item in qrList)
        {
            csv.AppendLine("'" + item.SerialNo + "," + item.QRText + "," + item.QRValue);
        }

        string fileName = "QRCodeMaster_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";

        Response.Clear();
        Response.ContentType = "text/csv";
        Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
        Response.Write(csv.ToString());
        Response.End();
    }

    private void ExportToExcel(List<QRData> qrList)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (ExcelPackage excel = new ExcelPackage())
        {
            var ws = excel.Workbook.Worksheets.Add("QRCodeMaster");

           

            ws.Cells[1, 1].Value = "SerialNo";
            ws.Cells[1, 2].Value = "QRText";
            ws.Cells[1, 3].Value = "QRValue";

            ws.Row(1).Style.Font.Bold = true;

            int row = 2;

            foreach (var item in qrList)
            {
                ws.Cells[row, 1].Value = item.SerialNo;
                ws.Cells[row, 2].Value = item.QRText;
                ws.Cells[row, 3].Value = item.QRValue;
                row++;
            }

            ws.Cells.AutoFitColumns();

            string fileName = "QRCodeMaster_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);

            Response.BinaryWrite(excel.GetAsByteArray());
            Response.End();
        }
    }


    private void LoadExistingQRCodes(HashSet<string> usedQRText, HashSet<string> usedQRValue)
    {
        string connStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection con = new SqlConnection(connStr))
        {
            string query = "SELECT QRText, QRValue FROM QRCodeMaster";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        if (!dr.IsDBNull(0))
                            usedQRText.Add(dr.GetString(0).Trim());

                        if (!dr.IsDBNull(1))
                            usedQRValue.Add(dr.GetString(1).Trim());
                    }
                }
            }
        }
    }


    private string GenerateUniqueQRText(HashSet<string> usedSet, Random rand)
    {
        string qrText;

        do { qrText = "A" + GenerateRandomNumber(10, rand); }
        while (usedSet.Contains(qrText));

        usedSet.Add(qrText);
        return qrText;
    }

    private string GenerateUniqueQRValue(HashSet<string> usedSet)
    {
        string qrValue;

        do { qrValue = GenerateRandomHex(16); }
        while (usedSet.Contains(qrValue));

        usedSet.Add(qrValue);
        return qrValue;
    }

    private string GenerateRandomNumber(int length, Random rand)
    {
        StringBuilder sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
            sb.Append(rand.Next(0, 10));

        return sb.ToString();
    }

    private string GenerateRandomHex(int length)
    {
        byte[] buffer = new byte[length / 2];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(buffer);
        }

        StringBuilder sb = new StringBuilder(length);

        foreach (byte b in buffer)
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }
}

public class QRData
{
    public string SerialNo { get; set; }
    public string QRText { get; set; }
    public string QRValue { get; set; }
}
