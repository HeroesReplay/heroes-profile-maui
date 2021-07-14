using System;
using Heroes.ReplayParser;

namespace HeroesProfile.Core.Models
{
    public enum SessionState
    {
        None,
        BattleLobby,
        StormSave,
        StormReplay
    }

    [Flags]
    public enum ParseResult
    {
        PtrRegion = DataParser.ReplayParseResult.PTRRegion,
        ComputerPlayerFound = DataParser.ReplayParseResult.ComputerPlayerFound,
        TryMeMode = DataParser.ReplayParseResult.TryMeMode,
        Exception = DataParser.ReplayParseResult.Exception,
        PreAlphaWipe = DataParser.ReplayParseResult.PreAlphaWipe,
        FileSizeTooLarge = DataParser.ReplayParseResult.FileSizeTooLarge,
        Incomplete = DataParser.ReplayParseResult.Incomplete,
        FileNotFound = DataParser.ReplayParseResult.FileNotFound,
        UnexpectedResult = DataParser.ReplayParseResult.UnexpectedResult,
        Success = DataParser.ReplayParseResult.Success,
        CustomGame
    }

    public enum ParseType
    {
        All,
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
}