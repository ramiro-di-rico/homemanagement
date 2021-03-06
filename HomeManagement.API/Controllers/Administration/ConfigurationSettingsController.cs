﻿using HomeManagement.Api.Core;
using HomeManagement.API.Filters;
using HomeManagement.Business.Contracts;
using HomeManagement.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace HomeManagement.API.Controllers.Administration
{
    [Authorization(Constants.Roles.Admininistrator)]
    [EnableCors("SiteCorsPolicy")]
    [Produces("application/json")]
    [Route("api/ConfigurationSettings")]
    public class ConfigurationSettingsController : Controller
    {
        private readonly IConfigurationSettingsService configurationSettingsService;

        public ConfigurationSettingsController(IConfigurationSettingsService configurationSettingsService)
        {
            this.configurationSettingsService = configurationSettingsService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var settings = configurationSettingsService.GetConfigs();

            return Ok(settings);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ConfigurationSettingModel model)
        {
            if(ModelState.IsValid)
            {
                var result = configurationSettingsService.Save(model);

                if (result.IsSuccess) return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = configurationSettingsService.Delete(id);
            if (result.IsSuccess) return Ok();
            else return BadRequest();
        }
    }
}