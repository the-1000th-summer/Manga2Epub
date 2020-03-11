# Manga2Epub GUI

The GUI version of the [Eromanga2epub (Python)][https://cangku.moe/archives/121282]。

## 编译提示

1. 额外安装的Nuget包：Fody, Costura.Fody（用于辅助打包成独立的exe文件）, DotNetZip（用于压缩文件）, WindowsAPICodePack-Core, WindowsAPICodePath-Shell（用于支持打开文件夹）
2. 注意`epubBuild/FileTemplate.cs`文件中的带空行的字符串。空行实际上有空格缩进，这些空格对创建正确格式的文件有重要作用。在使用IDE重构时请注意这个问题。

## 注意事项

1. 在生成Epub的过程中会产生末尾有`(output)`字样的文件夹，此文件夹在生成Epub结束后会自动被删除。
2. 程序在生成Epub时会忽略末尾有`(output)`字样的文件夹。
3. 若手动中止任务，程序会删除所有末尾有`(output)`字样的文件夹。
4. 目前程序只支持自动识别漫画名和作者名。