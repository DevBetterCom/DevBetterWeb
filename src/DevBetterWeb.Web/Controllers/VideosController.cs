﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevBetterWeb.Core;
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

    public VideosController(IMapper mapper, GetAllVideosService getAllVideosService)
    {
      _mapper = mapper;
      _getAllVideosService = getAllVideosService;
    }

    [HttpPost("list")]
    public async Task<IActionResult> ListAsync([FromForm] DataTableParameterModel dataTableParameterModel)
    {
      var draw = dataTableParameterModel.Draw;
      var length = dataTableParameterModel.Length;
      int pageSize = length != null ? Convert.ToInt32(length) : 0;
      var startIndex = Convert.ToInt32(dataTableParameterModel.Start);
      var start = startIndex == 0 ? 1 : (startIndex / pageSize)+1;

      var request = new GetAllVideosRequest("me", start, pageSize);
      var response = await _getAllVideosService
        .ExecuteAsync(request);

      var videoList = new List<VideoModel>();
      if (response.Data?.Data != null)
      {
        videoList = _mapper.Map<List<VideoModel>>(response.Data.Data);
      }

      var jsonData = new { draw = draw, recordsFiltered = response.Data == null ? 0 : response.Data.Total, recordsTotal = response.Data == null ? 0 : response.Data.Total, data = videoList };

      return Ok(jsonData);
    }
  }

}
