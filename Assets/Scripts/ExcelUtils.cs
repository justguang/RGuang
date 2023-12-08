using System.Data;
using System.IO;
using Excel;

public class ExcelUtils
{
    /// <summary>
    /// 读取excel文件内容
    /// </summary>
    /// <param name="filePath">文件路劲</param>
    /// <param name="index">要读取文件中第几标签页</param>
    /// <param name="columnNum">总纵列数</param>
    /// <param name="rowNum">总行数</param>
    static DataRowCollection ReadExcel(string filePath, int index, ref int columnNum, ref int rowNum)
    {
        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelData = ExcelReaderFactory.CreateOpenXmlReader(stream);

        DataSet result = excelData.AsDataSet();
        columnNum = result.Tables[index].Columns.Count;
        rowNum = result.Tables[index].Rows.Count;

        return result.Tables[index].Rows;
    }
}
