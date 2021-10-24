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
using Microsoft.Extensions.Options;

namespace DevBetterWeb.Web.Controllers
{
  [Route("videos")]
  [Authorize(Roles = AuthConstants.Roles.ADMINISTRATORS_MEMBERS_ALUMNI)]
  [ApiController]
  public class VideosController : Controller
  {
    private readonly string _expectedApiKey;
    private readonly IMapper _mapper;
    private readonly GetAllVideosService _getAllVideosService;
    private readonly IRepository<ArchiveVideo> _repository;

    public VideosController(IMapper mapper,
      IRepository<ArchiveVideo> repository,
      GetAllVideosService getAllVideosService,
      IOptions<ApiSettings> apiSettings)
    {
      _mapper = mapper;
      _getAllVideosService = getAllVideosService;
      _repository = repository;
      _expectedApiKey = apiSettings.Value.ApiKey;
    }

    [HttpPost("list")]
    public async Task<IActionResult> ListAsync([FromForm] DataTableParameterModel dataTableParameterModel)
    {
      var draw = dataTableParameterModel.Draw;
      var length = dataTableParameterModel.Length;
      int pageSize = length != null ? Convert.ToInt32(length) : 0;
      var startIndex = Convert.ToInt32(dataTableParameterModel.Start);

      var spec = new ArchiveVideoByPageSpec(startIndex, pageSize);
      var totalRecords = await _repository.CountAsync();
      var archiveVideos = await _repository.ListAsync(spec);
      var archiveVideosDto = _mapper.Map<List<ArchiveVideoDto>>(archiveVideos);      

      var jsonData = new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = archiveVideosDto };

      return Ok(jsonData);
    }

    [AllowAnonymous]
    [HttpPost("add-video-info")]
    public async Task<IActionResult> AddVideoInfoAsync([FromBody] ArchiveVideoDto archiveVideoDto)
    {
      var apiKey = Request.Headers[Constants.ConfigKeys.ApiKey];

      if (apiKey != _expectedApiKey)
      {
        return Unauthorized();
      }

      var archiveVideo = _mapper.Map<ArchiveVideo>(archiveVideoDto);

      archiveVideo = await _repository.AddAsync(archiveVideo);

      return Ok(archiveVideo);
    }
  }

}
