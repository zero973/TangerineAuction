namespace TangerineAuction.Infrastructure.Email;

internal sealed class SmtpOptions
{
    
    public string Host { get; init; } = string.Empty;
    
    public int Port { get; init; }
    
    public string UserName { get; init; } = string.Empty;
    
    public string Password { get; init; } = string.Empty;
    
    public string FromEmail { get; init; } = string.Empty;
    
    public string FromName { get; init; } = string.Empty;
    
    public bool UseSsl { get; init; } = true;
    
}