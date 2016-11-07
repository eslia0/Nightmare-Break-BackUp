public enum ClientPacketId
{
    None = 0,
    Create,
    Delete,
    Login,
    Logout,
    GameClose,
}

public enum ServerPacketId
{
    None = 0,
    CreateResult,
    DeleteResult,
    LoginResult,
    Match,
}

public enum P2PPacketId
{
    None = 0,
    ConnectionCheck,
    ConnectionAnswer,
    CreateUnit,
    CharacterState,
}