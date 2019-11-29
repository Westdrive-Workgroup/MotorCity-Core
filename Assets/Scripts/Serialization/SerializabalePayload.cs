//#if INCLUDE_ONLY_CODE_GENERATION
//using ZeroFormatter;
using UnityEngine;
using System.Collections.Generic;
using ProtoBuf;
using Westdrive.Types;
/// <summary>
/// makes a dictionary entry with the position and rotation
/// </summary>
[ProtoContract]
public class Payload : GenericSerializableData
{
    [ProtoMember(1)]
    public Dictionary<string, Dictionary<int, PositionRotationType>> Data { get; set; }

    public Payload()
    {
        Data = new Dictionary<string, Dictionary<int, PositionRotationType>>();
    }

   

}
//#endif
