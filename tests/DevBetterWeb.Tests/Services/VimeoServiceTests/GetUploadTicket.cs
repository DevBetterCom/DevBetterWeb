using System.Threading.Tasks;
using DevBetterWeb.Infrastructure.Services;
using Xunit;

namespace DevBetterWeb.Tests.Services.VimeoServiceTests
{
  public class GetUploadTicket
  {
    [Fact]
    public async Task ReturnsUploadTicketAsync()
    {
      var vimeoCredential = new VimeoCredential("pvt token");
      var vimeoService = new VimeoService(vimeoCredential);
      var uploadTicket = await vimeoService.GetUploadTicketAsync();

      Assert.False(string.IsNullOrEmpty(uploadTicket.TicketId));
    }
  }
}
