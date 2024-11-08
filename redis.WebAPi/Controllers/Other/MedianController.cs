using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedianController : ControllerBase
    {
        [HttpPost("sendMedianJson")]
        public IActionResult SendMedianJson([FromBody] FolderPathRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Path))
            {
                return BadRequest("·������Ϊ��");
            }

            string baseFolderPath = request.Path;
            if (!Directory.Exists(baseFolderPath))
            {
                return BadRequest($"ָ�����ļ��� {baseFolderPath} ������");
            }

            List<string> resultMessages = new List<string>();
            List<MedianResult> allResults = new List<MedianResult>(); // �洢�����ļ��еļ�����

            try
            {
                // ��ȡ·�����������ļ���
                string[] subDirectories = Directory.GetDirectories(baseFolderPath, "*", SearchOption.TopDirectoryOnly);

                // ���û�����ļ��У���ֱ�Ӵ���ǰ�ļ����µ��ļ�
                if (subDirectories.Length == 0)
                {
                    string[] fileNames = Directory.GetFiles(baseFolderPath, "*.xlsx", SearchOption.TopDirectoryOnly);
                    if (fileNames.Any())
                    {
                        allResults.AddRange(ProcessFiles(fileNames, new DirectoryInfo(baseFolderPath).Name));
                    }
                    else
                    {
                        resultMessages.Add($"��ǰ�ļ��� {baseFolderPath} ��û�� .xlsx �ļ�");
                    }
                }
                else
                {
                    // ���򣬴���ÿ�����ļ���
                    foreach (var subDirectory in subDirectories)
                    {
                        string[] fileNames = Directory.GetFiles(subDirectory, "*.xlsx", SearchOption.TopDirectoryOnly);
                        if (fileNames.Any())
                        {
                            allResults.AddRange(ProcessFiles(fileNames, new DirectoryInfo(subDirectory).Name));
                        }
                    }
                }

                // ���� Excel �ļ�������
                var fileContent = GenerateExcelReport(allResults);
                var fileName = "Median_Report.xlsx";
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "�����ļ���ʱ��������", details = ex.Message });
            }
        }

        // �����ļ����е��ļ�������ÿ���ļ�����λ�����������ļ��е���λ��
        private List<MedianResult> ProcessFiles(string[] fileNames, string folderName)
        {
            List<double> medianList = new List<double>();
            List<MedianResult> results = new List<MedianResult>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            foreach (var fileName in fileNames)
            {
                List<double> data = ExtractData(fileName);
                if (data.Any())
                {
                    double median = CalculateMedian(data);
                    results.Add(new MedianResult
                    {
                        FileName = fileName,
                        Median = median
                    });
                    medianList.Add(median);
                }
            }

            if (medianList.Any())
            {
                double overallMedian = CalculateMedian(medianList);
                results.Add(new MedianResult
                {
                    FileName = $"{folderName} ��λ������λ��",
                    Median = overallMedian
                });
            }
            else
            {
                results.Add(new MedianResult
                {
                    FileName = $"{folderName} û�п�������",
                    Median = 0
                });
            }

            return results;
        }

        // �� Excel �ļ�����ȡ����
        private List<double> ExtractData(string fileName)
        {
            List<double> data = new List<double>();
            FileInfo fileInfo = new FileInfo(fileName);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int startRow = 12; // ��ʼ��
                int rowCount = worksheet.Dimension.Rows;

                for (int row = startRow; row <= rowCount; row++)
                {
                    if (double.TryParse(worksheet.Cells[row, 2].Text, out double value))
                    {
                        data.Add(value);
                    }
                }
            }

            return data;
        }

        // ������λ��
        private double CalculateMedian(List<double> data)
        {
            data.Sort();
            int count = data.Count;
            if (count % 2 == 0)
            {
                return (data[count / 2 - 1] + data[count / 2]) / 2.0;
            }
            else
            {
                return data[count / 2];
            }
        }

        // ���� Excel ���沢����������
        private byte[] GenerateExcelReport(List<MedianResult> results)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("��λ������");

                // ��ӱ�ͷ
                worksheet.Cells[1, 1].Value = "�ļ���";
                worksheet.Cells[1, 2].Value = "��λ��";

                // �������
                int row = 2;
                foreach (var result in results)
                {
                    worksheet.Cells[row, 1].Value = result.FileName;
                    worksheet.Cells[row, 2].Value = result.Median;
                    row++;
                }

                // ��ʽ���п�
                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();

                // ���� Excel �ļ�����
                return package.GetAsByteArray();
            }
        }
    }

    // ���ڽ���ǰ��������ļ���·��
    public class FolderPathRequest
    {
        public string Path { get; set; }
    }

    // ���ڴ洢ÿ���ļ�����λ�����
    public class MedianResult
    {
        public string FileName { get; set; }
        public double Median { get; set; }
    }
}
