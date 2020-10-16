﻿using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DomainEvents;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.User
{
    public class UserLinksUpdateModel
    {

        [ValidUrl]
        public string? LinkedInUrl { get; set; }
        [ValidUrl]
        public string? TwitterUrl { get; set; }
        [ValidUrl]
        public string? GithubUrl { get; set; }
        [ValidUrl]
        public string? BlogUrl { get; set; }
        [ValidUrl]
        public string? TwitchUrl { get; set; }
        [ValidUrl]
        public string? YouTubeUrl { get; set; }
        [ValidUrl]
        public string? OtherUrl { get; set; }

        public UserLinksUpdateModel()
        {

        }

        public UserLinksUpdateModel(Member member)
        {

            BlogUrl = member.BlogUrl;
            TwitchUrl = member.TwitchUrl;
            YouTubeUrl = member.YouTubeUrl;
            TwitterUrl = member.TwitterUrl;
            GithubUrl = member.GitHubUrl;
            LinkedInUrl = member.LinkedInUrl;
            OtherUrl = member.OtherUrl;
        }

    }
}
