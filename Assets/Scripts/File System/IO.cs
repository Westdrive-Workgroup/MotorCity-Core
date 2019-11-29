using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
#if UNITY_STANDALONE_WIN
using Newtonsoft.Json;
#endif
using Westdrive.Types;
using ProtoBuf;
using UnityEngine.UI;
/// <summary>
/// Manages Load and Save Functions
/// </summary>
namespace Westdrive
{

    public class IO
    {
        public  delegate void AsyncReadResult<T>(T data);
        public static async void SaveToFileAsync(string path, GenericSerializableData data, string identifier = "", string fileExtension =".bin")
        {
            try
            {

                WestdriveSettings.CheckIO = IOState.pending;
                using (var file = File.Create(path + "-" + identifier + fileExtension))
                {
                    WestdriveSettings.CheckIO = IOState.working;

                    await Task.Run(() =>
                    {
                        Serializer.Serialize(file, data);
                        file.Flush();
                    });
                    
                    file.Close();
                }

                WestdriveSettings.CheckIO = IOState.ready;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                throw;

            }
        }

        public static List<FileInfo> FilesInDirectory(string path, string extension = "")
        {
            List<FileInfo> files = new List<FileInfo>();
            DirectoryInfo directory = new DirectoryInfo(path);//Assuming Test is your Folder
            if (extension != "")
            {
                FileInfo[] Files = directory.GetFiles("*" + extension);
                foreach (FileInfo file in Files)
                {
                    files.Add(file);
                }
            }
            else
            {
                FileInfo[] Files = directory.GetFiles();
                foreach (FileInfo file in Files)
                {
                    files.Add(file);
                }
            }
            return files;
        }
        public static async Task LoadAsync<T>(string filename, AsyncReadResult<T> callBack)
        {

            T data;
           
            WestdriveSettings.CheckIO = IOState.pending;
            WestdriveSettings.CheckIO = IOState.working;
            using (var file = File.OpenRead(filename))
            {
                data = await Task.Run(() =>
                {
                    return data = Serializer.Deserialize<T>(file);
                });
                file.Close();
            }
            if (callBack != null)
                callBack(data);
            WestdriveSettings.CheckIO = IOState.ready;
            

        }
        //Converts Raw data into Json format
        public static string ConvertToJSON(GenericSerializableData rawData)
        {
            string JSONString = Newtonsoft.Json.JsonConvert.SerializeObject(rawData);
            return JSONString;
        }
        public static T ConvertFromJSON<T>(string JSONString)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(JSONString); 
        }


        // Blue print for futhure methods
        //public static void WriteJSONToFile(string Path, string JSONString)
        //{

        //}
        //public async void TrackerLoadFromFileAsync(string path)
        //{

        //}

        //public async void TrackerConvertToJason()
        //{
        //}

        //public async void TrackerWriteJasonAsync()
        //{

        //}
    }
    //initializes the IniTools
    public static class Settings
    {
        public static void SetConfigFile(string path)
        {
            IniTools.setPath(path);
        }
        public static string ReadValue(string section, string key)
        {
            return IniTools.IniReadValue(section,key);
            
        }
        public static void WriteValue(string section, string key, string value)
        {
            IniTools.IniWriteValue(section, key,value);
        }
        
    }
}

