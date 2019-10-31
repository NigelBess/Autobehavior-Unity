using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class DataLogger
{

    public static void Save(Path path, string data, bool append)
    {
        path.Create();
        using (TextWriter writer = new StreamWriter(path.ToString(), append: append))
        {
            writer.WriteLine(data);
        }
    }
    public static void Save(Path path, string[] data, bool append)
    {
        path.Create();
        if (!append)
        {
            using (TextWriter writer = new StreamWriter(path.ToString(), append: false))
            {
                writer.Write(string.Empty);
            }
        }
        using (TextWriter writer = new StreamWriter(path.ToString(), append: true))
        {
            for (int i = 0; i < data.Length; i++)
            {
                writer.WriteLine(data[i]+",");
            }
            
        }
    }
    public static string Read(Path path)
    {
        if (!path.Exists())
        {
            throw new System.IO.FileNotFoundException("Attempting to read a file that does not exist.");
        }
        return File.ReadAllText(path.ToString());
    }
    public static string[] ReadArray(Path path)
    {
        string contents = Read(path);
        string[] separated = contents.Split(',');
        for (int i = 0; i < separated.Length; i++)
        {
            separated[i] = separated[i].Trim();
        }
        return separated;
    }
    public class Path
    {
        private const string defaultExtension = "csv";
        public string fileName = "";
        public string extension = "";
        public string directory = "";
        public Path(string directory,string fileName, string extension)
        {
            Initialize(fileName, extension, directory);
        }
        public Path(string fileName, string extension)
        {
            Initialize(fileName, extension, Application.persistentDataPath);
        }
        public Path(string fileName)
        {
            Initialize(fileName, defaultExtension, Application.persistentDataPath);
        }

        private void Initialize(string fileName, string extension, string directory)
        {
            this.fileName = fileName;
            this.extension = extension;
            this.directory = directory;
        }
        public void Create()
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string path = this.ToString();
            if (!File.Exists(path))
            {
                FileStream file = File.Create(path);
                file.Close();
            }
        }
        public bool Exists()
        {
            if (!Directory.Exists(directory))
            {
                return false;
            }
            string path = this.ToString();
            if (!File.Exists(path))
            {
                return false;
            }
            return true;
        }
        public override string ToString()
        {
            return directory + "/" + fileName + "." + extension;
        }
    }
}
