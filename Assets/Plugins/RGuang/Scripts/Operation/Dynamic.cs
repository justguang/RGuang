using System;
using System.Collections.Generic;
using System.IO;

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
        /// <param name="addReferencedAssemblies">
        /// 编译需要引入的程序集
        /// eg:new string[]
        /// {
        ///     "UnityEngine.CoreModule.dll",
        ///     typeof(TEnum.BuffEnum).Assembly.Location,
        ///     typeof(BUFFAttribute).Assembly.Location
        /// }
        /// </param>
        public static void DynamicLoadCSharpScripts(string filePath,
                                                        Action<Type[]> loadTypeCallback, Action<object> errorCallback,
                                                        string fileNamePrefix = null, string fileNameSuffix = null, params string[] addReferencedAssemblies)
        {


            // 创建CSharpCodeProvider对象
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();

            // 编译参数
            System.CodeDom.Compiler.CompilerParameters compilerParams = new System.CodeDom.Compiler.CompilerParameters();
            compilerParams.GenerateInMemory = true;  // 在内存中生成编译结果

            //引入需要的程序集
            compilerParams.ReferencedAssemblies.AddRange(new string[] { "mscorlib.dll", "System.Core.dll" });
            if (addReferencedAssemblies != null && addReferencedAssemblies.Length > 0)
            {
                compilerParams.ReferencedAssemblies.AddRange(addReferencedAssemblies);
            }

            //compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            //compilerParams.ReferencedAssemblies.Add("System.Core.dll");
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


            System.CodeDom.Compiler.CompilerResults resultArr = provider.CompileAssemblyFromFile(compilerParams, fullNameLst.ToArray());


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

