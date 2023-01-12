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
    Success = DataParser.ReplayParseResult.Success,
    ComputerPlayerFound = DataParser.ReplayParseResult.ComputerPlayerFound,
    Incomplete = DataParser.ReplayParseResult.Incomplete,
    PtrRegion = DataParser.ReplayParseResult.PTRRegion,
    TryMeMode = DataParser.ReplayParseResult.TryMeMode,
    Exception = DataParser.ReplayParseResult.Exception,
    PreAlphaWipe = DataParser.ReplayParseResult.PreAlphaWipe,
    FileSizeTooLarge = DataParser.ReplayParseResult.FileSizeTooLarge,
    FileNotFound = DataParser.ReplayParseResult.FileNotFound,
    UnexpectedResult = DataParser.ReplayParseResult.UnexpectedResult,
    CustomGame = 1000
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