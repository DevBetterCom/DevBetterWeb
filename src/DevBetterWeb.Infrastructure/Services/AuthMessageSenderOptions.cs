namespace DevBetterWeb.Infrastructure.Services;

public class AuthMessageSenderOptions
{
  public string? SmtpServer { get; set; }
  public int SmtpPort { get; set; } = 587;
  public string? SmtpUsername { get; set; }
  public string? SmtpPassword { get; set; }
}
