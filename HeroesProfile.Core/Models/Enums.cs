using System;

using Heroes.ReplayParser;

namespace HeroesProfile.Core.Models;

public enum SessionState
{
    None,
    BattleLobby,
    StormSave,
    StormReplay
}

public enum ParseResult
{
    Success = Heroes.StormReplayParser.StormReplayParseResult.Success,    
    Incomplete = Heroes.StormReplayParser.StormReplayParseResult.Incomplete,
    PtrRegion = Heroes.StormReplayParser.StormReplayParseResult.PTRRegion,
    TryMeMode = Heroes.StormReplayParser.StormReplayParseResult.TryMeMode,    
    Exception = Heroes.StormReplayParser.StormReplayParseResult.Exception,
    PreAlphaWipe = Heroes.StormReplayParser.StormReplayParseResult.PreAlphaWipe,
    FileSizeTooLarge = Heroes.StormReplayParser.StormReplayParseResult.FileSizeTooLarge,
    FileNotFound = Heroes.StormReplayParser.StormReplayParseResult.FileNotFound,
    UnexpectedResult = Heroes.StormReplayParser.StormReplayParseResult.UnexpectedResult
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
    Pending,
    Success,
    UploadError,
    Duplicate,
    AiDetected,
    CustomGame,
    PtrRegion,
    Incomplete,
    TooOld,
}