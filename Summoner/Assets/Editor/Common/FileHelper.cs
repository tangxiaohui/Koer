using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileHelper
{
    public static string GetFileContent(string path)
    {
        string str = string.Empty;
        if(!IsFileExists(path))
        {
            Debug.LogError(path);
            return str;
        }

        ArrayList list = fnLoadFile(path);
        for(int i = 0; i < list.Count; ++i)
        {
            str += list[i];
            str += "\n";
        }
        return str;
    }

    public static bool IsFileExists(string path)
    {
        return File.Exists(path);
    }

    public static ArrayList fnLoadFile(string sPath)
    {
        StreamReader t_sStreamReader = null; // 使用流的形式读取                                 
        t_sStreamReader = File.OpenText(sPath);
        string t_sLine; // 每行的内容
        ArrayList t_aArrayList = new ArrayList(); // 容器
        while ((t_sLine = t_sStreamReader.ReadLine()) != null)
        {
            t_aArrayList.Add(t_sLine); // 将每一行的内容存入数组链表容器中
        }
        t_sStreamReader.Close(); // 关闭流

        t_sStreamReader.Dispose(); // 销毁流

        return t_aArrayList; // 将数组链表容器返回
    }

    public static string GetFirstContent(string path)
    {
        string str = string.Empty;
        if (!IsFileExists(path))
        {
            Debug.LogError(path);
            return str;
        }

        ArrayList list = fnLoadFile(path);
        if(list.Count > 0)
        {
            str =  (string)(list[0]);
        }

        return str;
    }

    public static void fnCreateFile(string sPath,string content)
    {
        StreamWriter t_sStreamWriter; // 文件流信息
        FileInfo t_fFileInfo = new FileInfo(sPath);
        if (!t_fFileInfo.Exists)
        {
            t_sStreamWriter = t_fFileInfo.CreateText();  // 如果此文件不存在则创建
        }
        else
        {
            t_sStreamWriter = t_fFileInfo.AppendText(); // 如果此文件存在则打开
        }
        t_sStreamWriter.WriteLine(content); // 以行的形式写入信息 
        t_sStreamWriter.Close(); //关闭流
        t_sStreamWriter.Dispose(); // 销毁流
    }
}
