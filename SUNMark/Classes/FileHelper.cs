using Microsoft.AspNetCore.Http;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SUNMark.Classes
{
    public static class FileHelper
    {
        public static byte[] ConvertIFormFileToBytes(IFormFile fileUpload)
        {
            try
            {
                if (fileUpload != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        fileUpload.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public static bool FileUpload(string filePath, string fullFilePathWithName, byte[] fileBytes)
        {
            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                File.WriteAllBytes(fullFilePathWithName, fileBytes);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable ReadXLSXFile(string filePath)
        {
            DataTable dt = new DataTable();
            try
            {
                Workbook workbook = new Workbook();
                workbook.LoadFromFile(filePath);
                Worksheet sheet = workbook.Worksheets[0];
                dt = sheet.ExportDataTable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;

        }

    }
}
