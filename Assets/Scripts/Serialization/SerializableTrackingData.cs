using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using Westdrive.Types;
/// <summary>
/// makes a dictionary entry with the position and rotation
/// </summary>
[ProtoContract]
public class TrackingData : GenericSerializableData
{
    [ProtoMember(1)]
    public  Dictionary<int, PositionRotationType> Data { get; set; }

    public TrackingData()
    {
        Data = new Dictionary<int, PositionRotationType>();
    }
}
