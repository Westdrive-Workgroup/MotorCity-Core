using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
/// <summary>
/// defines the type of hit
/// </summary>
[ProtoContract]
public class HitPositionType {
    [ProtoMember(1,Name = "centerHitPostion")]
    public PBVector3 centerHitPostion { get; set; }
    [ProtoMember(2,Name = "boxHitPostion")]
    public PBVector3 boxHitPostion { get; set; }
    [ProtoMember(3,Name = "cameraPosition")]
    public PBVector3 cameraPosition { get; set; }
    [ProtoMember(4,Name = "cameraRotation")]
    public PBQuaternion cameraRotation { get; set; }

    public HitPositionType()
    {
        this.centerHitPostion = new Vector3();
        this.boxHitPostion = new Vector3();
        this.cameraPosition = new Vector3();
        this.cameraRotation = new Quaternion();
    }
}
