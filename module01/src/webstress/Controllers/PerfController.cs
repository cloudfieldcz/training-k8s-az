using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace WebStress.Controllers
{
    [Route("api/[controller]")]
    public class PerfController : Controller
    {
        [HttpGet]
        [Route("/")]
        public async Task<IActionResult> Probe()
        {
            DateTime start = DateTime.Now;

            await calculateAsync(1);

            var ts = DateTime.Now - start;

            return Ok(string.Format("execution time: {0}ms", ts.TotalMilliseconds));
        }

        [HttpGet]
        [Route("/perf")]
        public async Task<IActionResult> Perf()
        {
            DateTime start = DateTime.Now;

            await calculateAsync(Repeats);

            var ts = DateTime.Now - start;

            return Ok(string.Format("execution time: {0}ms", ts.TotalMilliseconds));
        }

        private async Task calculateAsync(int rounds)
        {
            await Task.Run(() => calculate(rounds));
        }

        private void calculate(int rounds)
        {
            {
                var sha1 = System.Security.Cryptography.SHA1.Create();

                byte[] hash = new byte[32];
                System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(hash);
                for (int i = 0; i < (256 * rounds); i++)
                {
                    hash = sha1.ComputeHash(hash);
                }
            }
        }

        public int Repeats
        {
            get
            {
                int ret = 1024;
                var tmp = Environment.GetEnvironmentVariable("CYCLECOUNT");
                var b = int.TryParse(tmp, out ret);
                if (!b || ret <= 0)
                {
                    ret = 1024;
                }
                return ret;
            }
        }

    }
}
