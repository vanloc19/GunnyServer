using Newtonsoft.Json;
using System;
using System.IO;

namespace Bussiness
{
    public class MarshalBussiness : BaseBussiness
    {
        /// <summary>
        /// Zip compress data
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] Compress(byte[] src)
        {
            return Compress(src, 0, src.Length);
        }

        /// <summary>
        /// Zip compress data with offset and length.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] Compress(byte[] src, int offset, int length)
        {
            MemoryStream ms = new MemoryStream();
            Stream s = new zlib.ZOutputStream(ms, 9);
            s.Write(src, offset, length);
            s.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// Zip uncompress data.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public byte[] Uncompress(byte[] src)
        {
            return Uncompress(src, 0);
        }
        public byte[] Uncompress(byte[] src, int offset)
        {
            MemoryStream md = new MemoryStream();
            Stream d = new zlib.ZOutputStream(md);
            d.Write(src, offset, src.Length);
            d.Close();
            return md.ToArray();
        }        

        private string dataPatch = @"datas\";

        public T LoadDataJsonFile<T>(string filename, bool isEncrypt = false)
        {
            if (!File.Exists(dataPatch + filename + ".json"))
                return default(T);

            try
            {
                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(dataPatch + filename + @".json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    T obj = (T)serializer.Deserialize(file, typeof(T));
                    return obj;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("LoadDataJsonFile: " + ex);
            }
            return default(T);
        }

        public bool SaveDataJsonFile<T>(T instance, string filename, bool isEncrypt = false)
        {
            try
            {
                if (!Directory.Exists(dataPatch))
                {
                    Directory.CreateDirectory(dataPatch);
                }
               
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(dataPatch + filename + @".json"))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented
                    };
                    serializer.Serialize(file, instance);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("SaveDataJsonFile: " + ex);
            }
            return false;
        }
        public bool SaveLogJsonFile<T>(T instance, string filename, bool isEncrypt = false)
        {
            var logpath = dataPatch + @"\logs\";
            try
            {               
                if (!Directory.Exists(logpath))
                {
                    Directory.CreateDirectory(logpath);
                }
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(logpath + filename + @".json"))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented
                    };
                    serializer.Serialize(file, instance);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("SaveLogJsonFile: " + ex);
            }
            return false;
        }
    }
}
