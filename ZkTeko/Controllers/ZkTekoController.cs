using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using ZkTeko.Classes;
using ZkTeko.Models;

namespace ZkTeko.Controllers
{
    public class ZkTekoController : ApiController
    {
        [HttpGet]
        [Route("api/ZkTeko/GetTerminals")]
        public List<ZkTekoTerminal> GetTerminals()
        {
            if(!Request.Headers.Contains("ApiKey"))
                throw new Exception("API Key not found in header");
            var apiKey = Request.Headers.GetValues("ApiKey").FirstOrDefault();
            if (!IsApiKeyValid(apiKey))
                throw new Exception("API Kay is invalid");

            return GetTerminalList();
        }

        [HttpGet]
        [Route("api/ZkTeko/GetEmployees")]
        public GetEmployeesResponse GetEmployees(string act,string terminal_id)
        {
            if(!Request.Headers.Contains("ApiKey"))
                throw new Exception("API Key not found in header");
            var apiKey = Request.Headers.GetValues("ApiKey").FirstOrDefault();

            if (!IsApiKeyValid(apiKey))
                throw new Exception("API Kay is invalid");
            var d = new FingerPrintDevice(false);

            var terminalInfo = GetTerminalList().FirstOrDefault(x=>x.TerminalId == terminal_id);

            if(terminalInfo == null)
                throw new Exception("Terminal not found");
            var res = new GetEmployeesResponse();

            if (d.Connect(terminalInfo.IpAddress, Convert.ToInt32(terminalInfo.Port)))
            {
                try
                {
                    res.data = d.GetEmployees();
                }
                finally
                {
                    d.Disconnect();
                }
            }
            else
                throw new Exception("CannotConnectToDevice");

            return res;
        }

        [HttpGet]
        [Route("api/ZkTeko/GetPunches")]
        public GetPunchesResponse GetPunches(string act,string date,string terminal_id)
        {
            if(!Request.Headers.Contains("ApiKey"))
                throw new Exception("API Key not found in header");
            var apiKey = Request.Headers.GetValues("ApiKey").FirstOrDefault();
            if (!IsApiKeyValid(apiKey))
                throw new Exception("API Kay is invalid");
            var d = new FingerPrintDevice(false);
            var terminalInfo = GetTerminalList().FirstOrDefault(x=>x.TerminalId == terminal_id);

            if(terminalInfo == null)
                throw new Exception("Terminal not found");
            var res = new GetPunchesResponse();
            if (d.Connect(terminalInfo.IpAddress, Convert.ToInt32(terminalInfo.Port)))
            {
                try
                {
                    res.data = new List<Punches>();
                    var data = d.GetDataFromDevice();
                    foreach (var record in data)
                    {
                        if (record.Date.ToString("YYYY-MM-DD") == date)
                        {
                            res.data.Add(new Punches()
                            {
                                date = record.Date.ToString("YYYY-MM-DD"),
                                time = record.Date.ToString("hh:mm:ss"),
                                employees_id = record.IdUser,
                            });
                        }
                    }
                }
                finally
                {
                    d.Disconnect();
                }
            }
            else
                throw new Exception("CannotConnectToDevice");
            return res;
        }

        private List<ZkTekoTerminal> GetTerminalList()
        {
            var res = new List<ZkTekoTerminal>();
            var section = (NameValueCollection)ConfigurationManager.GetSection("appSettings");
            var countOfDevices = Convert.ToInt32(section["deviceCount"]);

            for (int i = 1; i <= countOfDevices; i++)
            {
                var x = section[$"DEVICE_{i}"].Split(':');
                res.Add(new ZkTekoTerminal() {TerminalId = $"{i}", IpAddress = x[0],Port = x[1]});
            }

            return res;
        }

        private bool IsApiKeyValid(string apiKey)
        {
            var section = (NameValueCollection)ConfigurationManager.GetSection("appSettings");

            var apiKeyInConfig = section["apiKey"];

            return apiKeyInConfig == apiKey;
        }
    }
}