using System.Collections.Generic;
using DevBetterWeb.Web.Models;

namespace DevBetterWeb.Web.Endpoints;

public class VideoListDataTableResponse
{
	public string? Draw { get; set; }
	public decimal CurrentPage { get; set; }
	public int RecordsFiltered { get; set; }
	public int RecordsTotal { get; set; }
	public List<ArchiveVideoDto>? Data { get; set; }

	public VideoListDataTableResponse(string? draw, decimal currentPage, int recordsFiltered, int recordsTotal, List<ArchiveVideoDto>? data)
	{
		Draw = draw;
		CurrentPage = currentPage;
		RecordsFiltered = recordsFiltered;
		RecordsTotal = recordsTotal;
		Data = data;
	}
}
