using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Westdrive.Types;
using ProtoBuf;
/// <summary>
/// makes a dictionary entry with the hitposition type
/// </summary>
[ProtoContract]
public class SerializableHitData : GenericSerializableData
{
    [ProtoMember(1)]
    public  Dictionary<int, HitPositionType> Data { get; set; }

    public SerializableHitData()
    {
        Data = new Dictionary<int, HitPositionType>();
    }
}
