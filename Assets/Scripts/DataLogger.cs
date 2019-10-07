using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataLogger
{
    public static void Save(string fileName, string data, bool append)
    {
        Save(fileName, data, append, Application.persistentDataPath);
    }
    public static void Save(string fileName, string data, bool append,string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        string path = directory + "/" + fileName + ".csv";
        if (!File.Exists(path))
        {
            FileStream file = File.Create(path);
            file.Close();
        }
        using (TextWriter writer = new StreamWriter(path, append: append))
        {
            writer.WriteLine(data+",");
        }
    }
    public static string Read(string fileName)
    {
        return Read(fileName, Application.persistentDataPath);
    }
    public static string Read(string fileName,string directory)
    {
        string path = directory+ "/" + fileName + ".csv";
        if (!File.Exists(path))
        {
            throw new System.IO.FileNotFoundException("Attempting to read a file that does not exist.");
        }
        return File.ReadAllText(path);
    }
    public static string[] ReadArray(string fileName)
    {
        return ReadArray(fileName,Application.persistentDataPath);
    }
    public static string[] ReadArray(string fileName,string path)
    {
        string contents = Read(fileName,path);
        string[] separated = contents.Split(',');
        for (int i = 0; i < separated.Length; i++)
        {
            separated[i] = separated[i].Trim();
        }
        return separated;
    }

}