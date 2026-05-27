using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;

public partial class GenerateQrCode2 : System.Web.UI.Page
{
    protected void btnGenerateCsv_Click(object sender, EventArgs e)
    {
        GenerateData(false);
    }

    protected void btnGenerateExcel_Click(object sender, EventArgs e)
    {
        GenerateData(true);
    }

    private void GenerateData(bool outputExcel)
    {
        List<QRData> qrList = new List<QRData>();

        string connStr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;

        using (SqlConnection con = new SqlConnection(connStr))
        {
            string query = "SELECT srno, qrvalue, qrtext, qruse, createddate, updatedate FROM qrcodemaster_1";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        qrList.Add(new QRData
                        {
                            srno = dr["srno"].ToString(),
                            qrvalue = dr["qrvalue"].ToString(),
                            qrtext = dr["qrtext"].ToString(),
                            qruse = dr["qruse"].ToString(),
                            createddate = Convert.ToDateTime(dr["createddate"]),
                            updatedate = Convert.ToDateTime(dr["updatedate"])
                        });
                    }
                }
            }
        }

        if (outputExcel)
            ExportToExcel(qrList);
        else
            ExportToCsv(qrList);
    }

  
    private void ExportToCsv(List<QRData> qrList)
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("srno,qrvalue,qrtext,qruse,createddate,updatedate");

        foreach (var item in qrList)
        {
            csv.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}",
                item.srno,
                item.qrvalue,
                item.qrtext,
                item.qruse,
                item.createddate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.updatedate.ToString("yyyy-MM-dd HH:mm:ss")
            ));
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
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        using (OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage())
        {
            var ws = excel.Workbook.Worksheets.Add("QRCodeMaster");

           
            ws.Cells[1, 1].Value = "srno";
            ws.Cells[1, 2].Value = "qrvalue";
            ws.Cells[1, 3].Value = "qrtext";
            ws.Cells[1, 4].Value = "qruse";
            ws.Cells[1, 5].Value = "createddate";
            ws.Cells[1, 6].Value = "updatedate";

            ws.Row(1).Style.Font.Bold = true;

           
            ws.Column(1).Style.Numberformat.Format = "@"; // srno
            ws.Column(2).Style.Numberformat.Format = "@"; // qrvalue
            ws.Column(3).Style.Numberformat.Format = "@"; // qrtext

            int row = 2;

            foreach (var item in qrList)
            {
                ws.Cells[row, 1].Value = item.srno;
                ws.Cells[row, 2].Value = item.qrvalue;
                ws.Cells[row, 3].Value = item.qrtext;
                ws.Cells[row, 4].Value = item.qruse;

                ws.Cells[row, 5].Value = item.createddate;
                ws.Cells[row, 5].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";

                ws.Cells[row, 6].Value = item.updatedate;
                ws.Cells[row, 6].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";

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
            string query = "SELECT qrtext, qrvalue FROM qrcodemaster_1";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        if (!dr.IsDBNull(0))
                            usedQRText.Add(dr["qrtext"].ToString().Trim());

                        if (!dr.IsDBNull(1))
                            usedQRValue.Add(dr["qrvalue"].ToString().Trim());
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

    // ===================== MODEL =====================
    public class QRData
    {
        public string srno { get; set; }
        public string qrtext { get; set; }
        public string qrvalue { get; set; }
        public string qruse { get; set; }
        public DateTime createddate { get; set; }
        public DateTime updatedate { get; set; }
    }
}