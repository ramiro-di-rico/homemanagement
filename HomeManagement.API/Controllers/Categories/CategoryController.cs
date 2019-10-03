﻿using HomeManagement.API.Business;
using HomeManagement.API.Extensions;
using HomeManagement.API.Filters;
using HomeManagement.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeManagement.API.Controllers.Categories
{
    [Authorization]
    [EnableCors("SiteCorsPolicy")]
    [Produces("application/json")]
    [Route("api/Category")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var email = HttpContext.GetEmailClaim();

            return Ok(categoryService.GetActive());
        }

        [HttpGet("active")]
        public IActionResult GetActiveCategories()
        {
            var email = HttpContext.GetEmailClaim();

            return Ok(categoryService.GetActive());
        }

        [HttpPost]
        public IActionResult Post([FromBody]CategoryModel category)
        {
            if (category == null) return BadRequest();

            categoryService.Add(category);

            return Ok();
        }

        [HttpPut]
        public IActionResult Put([FromBody]CategoryModel category)
        {
            if (category == null) return BadRequest();

            if (!(category.Id > 0)) return BadRequest();

            categoryService.Update(category);

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var result = categoryService.Delete(id);

            if (result.Result.Equals(Result.Error)) return BadRequest(result.Errors);

            return Ok();
        }
    }
}