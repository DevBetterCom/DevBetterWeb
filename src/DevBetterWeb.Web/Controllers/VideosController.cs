using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.Specs;
using DevBetterWeb.Vimeo.Services.VideoServices;
using DevBetterWeb.Web.Models;
using DevBetterWeb.Web.Models.Vimeo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DevBetterWeb.Web.Controllers
{
  [Route("videos")]
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
  [ApiController]
  public class VideosController : Controller
  {
    private readonly IMapper _mapper;
    private readonly GetAllVideosService _getAllVideosService;
    private readonly IRepository<ArchiveVideo> _repository;

    public VideosController(IMapper mapper, IRepository<ArchiveVideo> repository, GetAllVideosService getAllVideosService)
    {
      _mapper = mapper;
      _getAllVideosService = getAllVideosService;
      _repository = repository;
    }

    [HttpPost("list")]
    public async Task<IActionResult> ListAsync([FromForm] DataTableParameterModel dataTableParameterModel)
    {
      var draw = dataTableParameterModel.Draw;
      var length = dataTableParameterModel.Length;
      int pageSize = length != null ? Convert.ToInt32(length) : 0;
      var startIndex = Convert.ToInt32(dataTableParameterModel.Start);
      var start = startIndex == 0 ? 1 : (startIndex / pageSize)+1;

      var spec = new ArchiveVideoByPageSpec(start, pageSize);
      var archiveVideos = await _repository.ListAsync(spec);
      var archiveVideosDto = _mapper.Map<List<VideoModel>>(archiveVideos);      

      var jsonData = new { draw = draw, recordsFiltered = archiveVideos == null ? 0 : archiveVideos.Count, recordsTotal = archiveVideos == null ? 0 : archiveVideos.Count, data = archiveVideosDto };

      return Ok(jsonData);
    }

    // TODO: need to add authorization and add this call on console application.
    [AllowAnonymous]
    [HttpPost("add-video-info")]
    public async Task<IActionResult> AddVideoInfoAsync([FromBody] ArchiveVideoDto archiveVideoDto)
    {
      var archiveVideo = _mapper.Map<ArchiveVideo>(archiveVideoDto);

      archiveVideo = await _repository.AddAsync(archiveVideo);

      return Ok(archiveVideo);
    }
  }

}
