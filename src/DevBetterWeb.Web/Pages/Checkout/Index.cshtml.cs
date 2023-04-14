using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DevBetterWeb.Web.Pages.Checkout;

public class IndexModel : PageModel
{
	public bool IsReCaptchaValid { get; set; }
	public bool HasCaptchaBeenCompleted { get; set; }

	[FromServices]
	public IConfiguration Configuration { get; set; } = default!;

	[FromServices]
	public ILogger<IndexModel> Logger { get; set; } = default!;

  public void OnGet()
  {
  }

	public async Task<IActionResult> OnPostAsync()
	{
		var recaptchaResponse = Request.Form["captchaInput"].ToString() ?? string.Empty;
		IsReCaptchaValid = await ValidateReCaptchaAsync(recaptchaResponse);
		HasCaptchaBeenCompleted = true;

		return Page();
	}

	private async Task<bool> ValidateReCaptchaAsync(string recaptchaResponse)
	{
		if (string.IsNullOrEmpty(recaptchaResponse)) return false;

		using var httpClient = new HttpClient();
		string secretKey = Configuration["googleReCaptcha:SecretKey"]!;
		var apiUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}";

		var httpResponse = await httpClient.GetAsync(apiUrl);
		var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

		var validationResult = JsonConvert.DeserializeObject<ReCaptchaValidationResult>(jsonResponse);

		if (!validationResult!.Success)
		{
			// Log or handle error codes
			foreach (var errorCode in validationResult.ErrorCodes)
			{
				// Log the error codes using your _logger instance
				Logger.LogError($"reCAPTCHA error: {errorCode}");
			}
		}

		return validationResult!.Success;
	}

}

public class ReCaptchaValidationResult
{
	[JsonProperty("success")]
	public bool Success { get; set; }

	[JsonProperty("challenge_ts")]
	public DateTime ChallengeTimestamp { get; set; }

	[JsonProperty("hostname")]
	public string Hostname { get; set; } = default!;

	[JsonProperty("error-codes")]
	public List<string> ErrorCodes { get; set; } = default!;
}
