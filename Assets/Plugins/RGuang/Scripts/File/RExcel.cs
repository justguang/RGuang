using System.Data;
using System.IO;
using Excel;

namespace RGuang
{
    /// <summary>
    /// 读取excel
    /// 
    /// </summary>
    public static class RExcel
    {
        /// <summary>
        /// 读取excel文件内容
        /// </summary>
        /// <param name="filePath">文件路劲</param>
        /// <param name="tableCount">表格数</param>
        public static DataSet ReadExcel(string filePath, ref int tableCount)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelData = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelData.AsDataSet();
            tableCount = result.Tables.Count;
            return result;
        }


        /// <summary>
        /// 读取excel文件内容
        /// </summary>
        /// <param name="filePath">文件路劲</param>
        /// <param name="index">要读取文件中第几标签页</param>
        /// <param name="columnNum">总纵列数</param>
        /// <param name="rowNum">总行数</param>
        public static DataRowCollection ReadExcel(string filePath, int index, ref int columnNum, ref int rowNum, ref int tableCount)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelData = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelData.AsDataSet();
            tableCount = result.Tables.Count;
            columnNum = result.Tables[index].Columns.Count;
            rowNum = result.Tables[index].Rows.Count;

            return result.Tables[index].Rows;
        }

    }

}
