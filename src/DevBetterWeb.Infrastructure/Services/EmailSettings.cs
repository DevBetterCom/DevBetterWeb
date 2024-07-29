using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBetterWeb.Infrastructure.Services;
public class EmailSettings
{
	public string ApplicationName { get; set; } = string.Empty;
	public string DefaultFromEmail { get; set; } = string.Empty;
}
