namespace HeroesProfile.UI.Core.Models;

public enum SessionState
{
    None,
    BattleLobby,
    StormSave,
    StormReplay
}

public enum ParseType
{
    BattleLobby,
    StormSave,
    StormReplay
}

public enum ProcessStatus
{
    Pending,
    Success,
    Duplicate,
    Error,
    NotSupported
}

public enum UploadStatus
{
    None,
    Success,
    InProgress,
    UploadError,
    Duplicate,
    AiDetected,
    CustomGame,
    PtrRegion,
    Incomplete,
    TooOld,
}