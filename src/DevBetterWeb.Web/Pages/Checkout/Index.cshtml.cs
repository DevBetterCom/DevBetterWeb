using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DevBetterWeb.Web.Pages.Checkout;

public class IndexModel : PageModel
{
	public bool IsReCaptchaValid { get; set; }
	public bool HasCaptchaBeenCompleted { get; set; }

	[FromServices]
	public IConfiguration Configuration { get; set; }

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
		string secretKey = Configuration["googleReCaptcha:SecretKey"];
		var apiUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}";

		var httpResponse = await httpClient.GetAsync(apiUrl);
		var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

		var validationResult = JsonConvert.DeserializeObject<ReCaptchaValidationResult>(jsonResponse);

		return validationResult.Success;
	}

}

public class ReCaptchaValidationResult
{
	[JsonProperty("success")]
	public bool Success { get; set; }

	[JsonProperty("error-codes")]
	public List<string> ErrorCodes { get; set; }
}
