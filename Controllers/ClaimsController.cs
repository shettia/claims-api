using ClaimsAPI.Models;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private IWebHostEnvironment _env;
        public ClaimsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        [Route("{date}")]
        public JsonResult GetClaimsByDate(DateTime date)
        
        {
            String dataPath = Path.Combine(_env.ContentRootPath, "Data");
            List<Claim> claims = new List<Claim>();

            List<Member> members = new List<Member>();

            using (var reader = new StreamReader(Path.Combine(dataPath, "Claim.csv")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                claims = csv.GetRecords<Claim>().ToList();
            }

            using (var reader = new StreamReader(Path.Combine(dataPath, "Member.csv")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                members = csv.GetRecords<Member>().ToList();
            }

            if (claims.Count == 0 || members.Count == 0)
            {
                return new JsonResult(new { });
            }

            var result = (from c in claims
                          join m in members on c.MemberID equals m.MemberID
                          where c.ClaimDate <= date
                          select new
                          {
                              MemberID = m.MemberID,
                              FirstName = m.FirstName,
                              LastName = m.LastName,
                              ClaimAmount = c.ClaimAmount
                          }).ToList();

            return new JsonResult(result);
        }
    }
}
