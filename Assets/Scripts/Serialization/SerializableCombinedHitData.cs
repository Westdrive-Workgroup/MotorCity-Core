using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using Westdrive.Types;
/// <summary>
/// makes a dictionary entry with the hitposition Type
/// </summary>
[ProtoContract]

public class SerializableCombinedHitData : GenericSerializableData
{
    [ProtoMember(1)]
    public  Dictionary<int, List<HitPositionType>> Data { get; set; }

    public SerializableCombinedHitData()
    {
        Data = new Dictionary<int, List<HitPositionType>>();
    }
}
