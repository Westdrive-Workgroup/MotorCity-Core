
/// <summary>
/// Since unity doesn't flag the Vector3 as serializable, we
/// need to create our own version. This one will automatically convert
/// between Vector3 and SerializableVector3
/// </summary>


[ProtoBuf.ProtoContract]
public class PBVector3
{
    [ProtoBuf.ProtoIgnore]
    public UnityEngine.Vector3 vector3 = new UnityEngine.Vector3();
    public PBVector3() { }
    public PBVector3(UnityEngine.Vector3 source)
    {
        vector3 = source;
    }
    [ProtoBuf.ProtoMember(1, Name = "x")]
    public float X
    {
        get { return vector3.x; }
        set { vector3.x = value; }
    }
    [ProtoBuf.ProtoMember(2, Name = "y")]
    public float Y
    {
        get { return vector3.y; }
        set { vector3.y = value; }
    }
    [ProtoBuf.ProtoMember(3, Name = "z")]
    public float Z
    {
        get { return vector3.z; }
        set { vector3.z = value; }
    }
    public static implicit operator UnityEngine.Vector3(PBVector3 i)
    {
        return i.vector3;
    }
    public static implicit operator PBVector3(UnityEngine.Vector3 i)
    {
        return new PBVector3(i);
    }
}
