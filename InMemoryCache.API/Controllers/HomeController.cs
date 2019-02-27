﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InMemoryCache.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace InMemoryCache.API.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IInMemoryCache _memory;
        public HomeController(IInMemoryCache memory)
        {
            _memory = memory;
        }

        [HttpGet]
        public void Get()
        {
        }

        [HttpGet("{mykey}")]
        public ActionResult<string> Get([FromRoute] string mykey)
        {
            return _memory.Get(mykey)?.ToString();
        }

        [HttpPost("{mykey}")]
        public async Task Post([FromRoute] string mykey)
        {
            var body = await GetBody();
            _memory.Set(mykey, body);
        }

        private async Task<string> GetBody()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.ASCII))
            {
                return await reader.ReadToEndAsync();
            }            
        }

        [HttpPut("{mykey}")]
        public async Task Put([FromQuery] string mykey)
        {
            var body = await GetBody();

            _memory.Set(mykey, body);
        }

        [HttpDelete("{mykey}")]
        public void Delete([FromRoute] string mykey)
        {
            _memory.Del(mykey);
        }

        [HttpGet]
        [Route("/DbSize")]
        public int DbSize()
        {
            return _memory.DbSize();
        }
    }
}