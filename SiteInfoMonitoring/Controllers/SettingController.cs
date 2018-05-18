﻿using Calabonga.Portal.Config;
using SiteInfoMonitoring.Core.Parsers;
using SiteInfoMonitoring.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteInfoMonitoring.Controllers
{
    public class SettingController : Controller
    {
        private readonly IConfigService<CurrentAppSettings> _configService;

        public SettingController(IConfigService<CurrentAppSettings> configService)
        {
            configService.Reload();
            _configService = configService;
        }

        [NoCache]
        public ActionResult Index()
        {
            if (IsAdminUser())
            {
                var settings = _configService.Config;
                return View(settings);
            }
            else
            {
                return Redirect("/Home/Index");
            }
        }

        public bool IsAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = new XmlParser().GetUsers().FirstOrDefault(u => u.Login == User.Identity.Name);
                if (user != null)
                {
                    if (user.Role == Core.Enums.RolesEnum.admin)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}