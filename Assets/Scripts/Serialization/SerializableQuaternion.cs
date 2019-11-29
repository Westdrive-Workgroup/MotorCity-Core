//using UnityEngine;
//using System;
//using System.Collections;
/// <summary>
/// Since unity doesn't flag the Quaternion as serializable, we
/// need to create our own version. This one will automatically convert
/// between Quaternion and SerializableQuaternion
/// </summary>
#if INCLUDE_ONLY_CODE_GENERATION
using ZeroFormatter;

namespace UnityEngine
{
    [ZeroFormattable]
    public struct Quaternion
    {
        [Index(0)] public float x;
        [Index(1)] public float y;
        [Index(2)] public float z;
        [Index(3)] public float w;

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}
#endif
[ProtoBuf.ProtoContract]
public class PBQuaternion
{
    [ProtoBuf.ProtoIgnore]
    public UnityEngine.Quaternion quaternion = new UnityEngine.Quaternion();
    public PBQuaternion() { }
    public PBQuaternion(UnityEngine.Quaternion source)
    {
        quaternion = source;
    }
    [ProtoBuf.ProtoMember(1, Name = "x")]
    public float X
    {
        get { return quaternion.x; }
        set { quaternion.x = value; }
    }
    [ProtoBuf.ProtoMember(2, Name = "y")]
    public float Y
    {
        get { return quaternion.y; }
        set { quaternion.y = value; }
    }
    [ProtoBuf.ProtoMember(3, Name = "z")]
    public float Z
    {
        get { return quaternion.z; }
        set { quaternion.z = value; }
    }
    [ProtoBuf.ProtoMember(4, Name = "w")]
    public float w
    {
        get { return quaternion.w; }
        set { quaternion.w = value; }
    }
    public static implicit operator UnityEngine.Quaternion(PBQuaternion i)
    {
        return i.quaternion;
    }
    public static implicit operator PBQuaternion(UnityEngine.Quaternion i)
    {
        return new PBQuaternion(i);
    }
}
