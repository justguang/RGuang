using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;

namespace RGuang.Operation
{
    public static class Dynamic
    {
        /// <summary>
        /// 动态读取指定路径下所有脚本文件，编译并加载
        /// </summary>
        /// <param name="filePath">指定路径</param>
        /// <param name="loadTypeCallback">编译加载成功的回调</param>
        /// <param name="errorCallback">编译加载失败的回调</param>
        /// <param name="fileNamePrefix">加载的文件名字前几个字符限制要求，默认空</param>
        /// <param name="fileNameSuffix">加载的文件名字后缀几个字符限制要求，默认空</param>
        public static void DynamicLoadCSharpScripts(string filePath, 
                                                Action<Type[]> loadTypeCallback, Action<object> errorCallback,
                                                string fileNamePrefix = null, string fileNameSuffix = null)
        {
            // 创建CSharpCodeProvider对象
            CSharpCodeProvider provider = new CSharpCodeProvider();

            // 编译参数
            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.GenerateInMemory = true;  // 在内存中生成编译结果

            //引入需要的程序集
            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParams.ReferencedAssemblies.Add("System.Core.dll");
            //compilerParams.ReferencedAssemblies.Add("UnityEngine.CoreModule.dll");

            var dir = new DirectoryInfo(filePath);
            var fileArr = dir.GetFiles();
            List<string> fullNameLst = new List<string>();
            for (int i = 0; i < fileArr.Length; i++)
            {
                string tmpFullName = fileArr[i].FullName;
                if (string.IsNullOrEmpty(fileNamePrefix) == false && fileArr[i].Name.StartsWith(fileNamePrefix) == false)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(fileNameSuffix) == false && fileArr[i].Name.EndsWith(fileNameSuffix) == false)
                {
                    continue;
                }
                fullNameLst.Add(tmpFullName);
            }

            CompilerResults resultArr = provider.CompileAssemblyFromFile(compilerParams, fullNameLst.ToArray());


            if (resultArr.Errors.HasErrors)
            {
                //出现异常
                for (int i = 0; i < resultArr.Errors.Count; i++)
                {
                    errorCallback?.Invoke(resultArr.Errors[i].ErrorText);
                }
            }
            else
            {
                // 获取编译后的类型
                var typeArr = resultArr.CompiledAssembly.GetTypes();
                loadTypeCallback?.Invoke(typeArr);

                /*
                for (int i = 0; i < typeArr.Length; i++)
                {
                    // 实例化对象
                    object instance = Activator.CreateInstance(typeArr[i]);
                }
                */

            }
        }

    }

}

