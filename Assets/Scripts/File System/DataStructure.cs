using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using ProtoBuf;

/// <summary>
/// Manages Load and saving from payload
/// </summary>
public class DataStructure
{
    
    private Payload record;
    private Dictionary<string, Dictionary<int, PositionRotationType>> Data;
    public DataStructure()
    {
        
        record = new Payload();
        Data = new Dictionary<string, Dictionary<int, PositionRotationType>>();
        
    }

    public void Store(string ID, Dictionary<int, PositionRotationType> Payload)
    {
        Data.Add(ID,Payload);
    }

   //Saves the Payload
    public async void SaveAsync(string filename)
    {
      
        try
        {
            
            WestdriveSettings.CheckIO = IOState.pending;
            using (var file = File.Create(filename))
            {
                WestdriveSettings.CheckIO = IOState.working;
                record.Data = this.Data;
                await Task.Run(() =>
                {
                    Serializer.Serialize(file, record);
                });
                file.Flush();
                file.Close();
            }  
           
            WestdriveSettings.CheckIO = IOState.ready;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            throw;

        }
    }

    //Loads the the payload
    public async void LoadAsync(string filename)
    {
        
        
        try
        {
            WestdriveSettings.CheckIO = IOState.pending;
            WestdriveSettings.CheckIO = IOState.working;
            using (var file = File.OpenRead(filename))
            {
                await Task.Run(() =>
                {
                    return record = Serializer.Deserialize<Payload>(file);
                });
                file.Close();
            } 
            WestdriveSettings.CheckIO = IOState.ready;
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
            throw;

        }

    }

    //does nothing  
    public void Load(string FileName)
    {

    }
    //gets the Record
    public Payload getData()
    {
        return record;
    }
}
