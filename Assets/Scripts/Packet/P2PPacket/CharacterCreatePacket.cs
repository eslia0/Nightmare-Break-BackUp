public class CharacterStateDataPacket : IPacket<CharacterStateData>
{
    public class CharacterStateDataSerializer : Serializer
    {

        public bool Serialize(CharacterStateData data)
        {
            bool ret = true;
            ret &= Serialize(data.state);
            ret &= Serialize(data.direction);
            ret &= Serialize(data.hor);
            ret &= Serialize(data.ver);
            ret &= Serialize(data.posX);
            ret &= Serialize(data.posY);
            ret &= Serialize(data.posZ);
            return ret;
        }

        public bool Deserialize(ref CharacterStateData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;
            byte state = 0;
            bool direction = true;
            float ver = 0;
            float hor = 0;
            float posX = 0;
            float posY = 0;
            float posZ = 0;

            ret &= Deserialize(ref state);
            ret &= Deserialize(ref direction);
            ret &= Deserialize(ref ver);
            ret &= Deserialize(ref hor);
            ret &= Deserialize(ref posX);
            ret &= Deserialize(ref posY);
            ret &= Deserialize(ref posZ);
            element.state = state;
            element.direction = direction;
            element.ver = ver;
            element.hor = hor;
            element.posX = posX;
            element.posY = posY;
            element.posZ = posZ;

            return ret;
        }
    }

    CharacterStateData m_data;

    public CharacterStateDataPacket(CharacterStateData data) // 데이터로 초기화(송신용)
    {
        m_data = data;
    }

    public CharacterStateDataPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        m_data = new CharacterStateData();
        CharacterStateDataSerializer serializer = new CharacterStateDataSerializer();
        serializer.SetDeserializedData(data);
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        CharacterStateDataSerializer serializer = new CharacterStateDataSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public CharacterStateData GetData() // 데이터 얻기(수신용)
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)P2PPacketId.CharacterState;
    }
}

public class CharacterStateData
{
    public byte state;
    public bool direction;
    public float ver;
    public float hor;
    public float posX;
    public float posY;
    public float posZ;

    public CharacterStateData()
    {
        state = 0;
        direction = true;
        ver = 0;
        hor = 0;
        posX = 0;
        posY = 0;
        posZ = 0;
    }

    public CharacterStateData(byte newState, bool newDirection, float newVer, float newHor, float newX, float newY, float newZ)
    {
        state = newState;
        direction = newDirection;
        ver = newVer;
        hor = newHor;
        posX = newX;
        posY = newY;
        posZ = newZ;
    }
}