using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Manga2Epub.epubBuild {

    class DeleteDir {
        /// <summary>
        /// 删除文件夹及其所有内容的函数
        /// </summary>
        /// <param name="file"></param>
        public static void delete(string file) {
            try {
                //去除文件夹和子文件的只读属性
                //去除文件夹的只读属性
                System.IO.DirectoryInfo fileInfo = new DirectoryInfo(file);
                fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;

                //去除文件的只读属性
                System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);

                //判断文件夹是否还存在
                if (Directory.Exists(file)) {
                    foreach (string f in Directory.GetFileSystemEntries(file)) {
                        if (File.Exists(f)) {
                            //如果有子文件删除文件
                            File.Delete(f);
                            //Console.WriteLine(f);
                        } else {
                            //循环递归删除子文件夹
                            delete(f);
                        }
                    }
                    //删除空文件夹
                    Directory.Delete(file);
                    //Console.WriteLine(file);
                }
            } catch (Exception ex) {    // 异常处理
                Console.WriteLine(ex.Message.ToString());// 异常信息
            }
        }
    }
}
