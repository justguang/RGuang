## 简单自用功能工具

## 主页:https://gitee.com/justguang

## 导入unity：
    下载发行版RGuang中的unitypackage导入到项目中 
	或者通过[git URL]下载安装 ：
	需要支持 git 包路径查询参数的 unity 版本（Unity >= 2019.3.4f1，Unity >= 2020.1a21）。
	可以添加https://gitee.com/justguang/RGuang.git?path=Assets/Plugins/RGuang到包管理器。
	
	或者加 "com.justguang.rguang" : "https://gitee.com/justguang/RGuang.git?path=Assets/Plugins/RGuang" 到Packages/manifest.json。
	

	
##

	

## 导入错误或可能出现的异常：
	RGuang包中有引入第三方dll库文件，路径在【Plugins/RGuang/Scripts/Kit/File/Dll/】文件夹下。
	引入的第三方dll库文件有：
	EPPlus.dll
	Excel.dll
	I18N.CJK.dll
	I18N.dll
	I18N.MidEast.dll
	I18N.Other.dll
	I18N.Rare.dll
	I18N.West.dll
	ICSharpCode.SharpZipLib.dll
	
	导入请检查项目中与此包有重复的dll库文件。
	

