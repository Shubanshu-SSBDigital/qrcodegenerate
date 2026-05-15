using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

public partial class Index2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        string path = Server.MapPath("~/uploads");

        GenerateFiles(path);

    
        pnlDownload.Visible = true;
    }

   
    public void GenerateFiles(string path)
    {
        int totalRecords = 5000;

        Directory.CreateDirectory(path);

        string csvPath = Path.Combine(path, "qrcode_data.csv");
        string excelPath = Path.Combine(path, "qrcode_data.xlsx");

        Random random = new Random();
        HashSet<string> uniqueSet = new HashSet<string>();

        using (StreamWriter writer = new StreamWriter(csvPath, false, Encoding.UTF8))
        {
           
            writer.WriteLine("srno,qrvalue,qrtext");

            for (int i = 0; i < totalRecords; i++)
            {
                string qrvalue;

               
                do
                {
                    qrvalue = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
                }
                while (!uniqueSet.Add(qrvalue));

               
                string qrtext = "A" + random.Next(1000000000, int.MaxValue).ToString();

              
                string srno = (i + 1).ToString("D7");

                
                writer.WriteLine(string.Format("{0},\"{1}\",\"{2}\"",
                    srno, qrvalue, qrtext));
            }
        }

        
        File.Copy(csvPath, excelPath, true);
    }
   
    protected void btnDownloadCSV_Click(object sender, EventArgs e)
    {
        DownloadFile("qrcode_data.csv");
    }

 
    protected void btnDownloadExcel_Click(object sender, EventArgs e)
    {
        DownloadFile("qrcode_data.xlsx");
    }

    private void DownloadFile(string fileName)
    {
        string filePath = Server.MapPath("~/uploads/" + fileName);

        if (File.Exists(filePath))
        {
            Response.Clear();
            Response.ContentType = "application/octet-stream";

           
            string unique = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            Response.AddHeader("Content-Disposition",
                "attachment; filename=" + unique + "_" + fileName);

            Response.WriteFile(filePath);
            Response.End();
        }
    }
}