using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
/// <summary>
/// Returns position and rotation of object
/// </summary>
[ProtoContract]
public class PositionRotationType
{
    [ProtoMember(1,Name = "postion")]
    public PBVector3 position { get; set; }
    [ProtoMember(2,Name = "rotation")]
    public PBQuaternion rotaion { get; set; }

    public PositionRotationType()
    {
        this.position = new Vector3();
        this.rotaion = new Quaternion();
    }
   

}