namespace TwitterSDK.Examples;

public abstract class AuthClient
{
	public abstract Task<IAuthHeader> GetAuthHeaderAsync(string? url, string? method);
}
