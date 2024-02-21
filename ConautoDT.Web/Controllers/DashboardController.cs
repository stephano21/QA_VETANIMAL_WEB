﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using VET_ANIMAL.WEB.Engines;
using VET_ANIMAL.WEB.Servicios;

namespace VET_ANIMAL.WEB.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration configuration;

        private RestClient _apiClient;
        private RestClient _appAutogoClient;
        private static Logger _log = LogManager.GetLogger("Account");

        private string responseContent { get; }
        private AccountService _AccountService;
        private readonly ExcelFormatsHandler excelHandler = new ExcelFormatsHandler();
        private readonly GemboxReportingEngine _reportingEngine = new GemboxReportingEngine();




        public DashboardController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _apiClient = new RestClient(configuration["APIClient"]);//RestClient(baseURL);
            //_apiClient.ThrowOnAnyError = true;
            //_apiClient.Timeout = 120000;
            //_apiClient.UseUtf8Json();
            _AccountService = new AccountService(configuration);
        }
        

        public ActionResult Index()
        {
            TempData["menu"] = "";
            
                return View();
            
        }

        public async Task<ActionResult> Indexxx()
        {
            string tokenValue = Request.Cookies["token"];
            var client = new RestClient(configuration["APIClient"]);

            var request = new RestRequest("/api/Reportes/ContarCasosPorEnfermedad", Method.Get);
            request.AddHeader("Authorization", $"Bearer {tokenValue}");

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var content = response.Content;
                var casosPorEnfermedad = JsonConvert.DeserializeObject<Dictionary<string, int>>(content);

                // Devolver los datos como JSON
                return Json(casosPorEnfermedad);
            }
            else
            {
                // Manejar el error según sea necesario
                return BadRequest($"Error al obtener la cantidad de casos por enfermedad. Estado de la respuesta: {response.StatusCode}");
            }
        }

        // GET: DashboardController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DashboardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DashboardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashboardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DashboardController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashboardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DashboardController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
