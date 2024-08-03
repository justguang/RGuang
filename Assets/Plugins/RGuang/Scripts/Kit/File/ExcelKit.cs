using System.Data;
using System.IO;
using Excel;

namespace RGuang.Kit
{
    /// <summary>
    /// 读取excel
    /// 
    /// </summary>
    public static class ExcelKit
    {

        /// <summary>
        /// 读取excel文件内容：
        /// 返回DataSet数据
        /// </summary>
        /// <param name="filePath">文件路劲</param>
        /// <param name="tableCount">表格总数</param>
        public static DataSet ReadExcel(string filePath, ref int tableCount)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelData = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelData.AsDataSet();
            tableCount = result.Tables.Count;
            return result;
        }


        /// <summary>
        /// 读取excel文件内容：
        /// 通过表的索引，返回一个DataRowCollection表数据对象
        /// </summary>
        /// <param name="filePath">文件路劲</param>
        /// <param name="sheetIndex">要读取excel文件表格索引</param>
        /// <param name="columnNum">总纵列数</param>
        /// <param name="rowNum">总行数</param>
        /// <param name="tableCount">表格总数</param>
        public static DataRowCollection ReadExcel(string filePath, int sheetIndex, ref int columnNum, ref int rowNum, ref int tableCount)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelData = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelData.AsDataSet();
            tableCount = result.Tables.Count;
            columnNum = result.Tables[sheetIndex].Columns.Count;
            rowNum = result.Tables[sheetIndex].Rows.Count;

            return result.Tables[sheetIndex].Rows;
        }

        /// <summary>
        /// 读取excel文件内容：
        /// 通过表的名字，返回一个DataRowCollection表数据对象
        /// </summary>
        /// <param name="filePath">文件路劲</param>
        /// <param name="sheetName">要读取excel文件表格名</param>
        /// <param name="columnNum">总纵列数</param>
        /// <param name="rowNum">总行数</param>
        /// <param name="tableCount">表格总数</param>
        public static DataRowCollection ReadExcel(string filePath, string sheetName, ref int columnNum, ref int rowNum, ref int tableCount)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelData = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelData.AsDataSet();
            tableCount = result.Tables.Count;
            columnNum = result.Tables[sheetName].Columns.Count;
            rowNum = result.Tables[sheetName].Rows.Count;

            return result.Tables[sheetName].Rows;
        }


    }

}
