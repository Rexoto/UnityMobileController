public class Packet { public Packet() { } }

public class JoystickPacket : Packet
{
    public float x { get; set; }
    public float y { get; set; }
}

public class AccelerometerPacket : Packet
{
    public float x { get; set; }
}

public class GyroscopePacket : Packet
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float w { get; set; }
}

public class ButtonPacket : Packet
{
    public string name { get; set; }
}

public class UIPacket : Packet
{
    public string name { get; set; }
    public bool enabled { get; set; }
}

public class PlayerPacket : Packet
{
    public string name { get; set; }
    public float r { get; set; }
    public float g { get; set; }
    public float b { get; set; }
}

public class ScenePacket : Packet
{
    public string name { get; set; }
}

public class OrientationPacket : Packet
{
    public string name { get; set; }
}