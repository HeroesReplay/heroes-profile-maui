using System;
using Heroes.ReplayParser;

namespace HeroesProfile.Core.Models
{
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
}