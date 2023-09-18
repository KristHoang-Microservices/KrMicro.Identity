namespace KrMicro.Core.CQS.Command.Jwt;

public record GenerateJwtCommandRequest(string userId, string UserName, string Role);