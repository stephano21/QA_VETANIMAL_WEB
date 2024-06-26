﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using Org.BouncyCastle.Asn1.Ocsp;
using RestSharp;
using VET_ANIMAL.WEB.Engines;
using VET_ANIMAL.WEB.Models;
using VET_ANIMAL.WEB.Servicios;

namespace VET_ANIMAL.WEB.Controllers
{
    public class TestingController : Controller
    {
        private readonly IConfiguration configuration;

        private RestClient _apiClient;
        private RestClient _appAutogoClient;
        private static Logger _log = LogManager.GetLogger("Account");

        private string responseContent { get; }
        private AccountService _AccountService;
        private readonly ExcelFormatsHandler excelHandler = new ExcelFormatsHandler();
        private readonly GemboxReportingEngine _reportingEngine = new GemboxReportingEngine();




        public TestingController(IConfiguration configuration)
        {
            this.configuration = configuration;
            _apiClient = new RestClient(configuration["APIClient"]);//RestClient(baseURL);
            //_apiClient.ThrowOnAnyError = true;
            //_apiClient.Timeout = 120000;
            //_apiClient.UseUtf8Json();
            _AccountService = new AccountService(configuration);
        }

        // GET: TestingController
        public ActionResult Testing()
        {
            TempData["menu"] = "";
            return View();
        }

        public async Task<ActionResult> Examenes(int? año, int? mes)
        {
            try
            {
                string tokenValue = Request.Cookies["token"];
                var client = new RestClient(configuration["APIClient"]);

                var request = new RestRequest("/api/Reportes/ContarCasosTotales", Method.Get);
                request.AddHeader("Authorization", $"Bearer {tokenValue}");

                // Agregar filtros de año y/o mes si están presentes
                if (año.HasValue && mes.HasValue)
                {
                    request.AddParameter("año", año.Value);
                    request.AddParameter("mes", mes.Value);
                }
                else if (año.HasValue)
                {
                    request.AddParameter("año", año.Value);
                }
                else if (mes.HasValue)
                {
                    request.AddParameter("mes", mes.Value);
                }

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    var content = response.Content;
                    var responseData = JsonConvert.DeserializeObject<ResponseDatas>(content);

                    // Si deseas manejar los datos de una manera específica, puedes hacerlo aquí antes de devolverlos
                    // Por ejemplo, podrías convertirlos a un formato específico o procesarlos de alguna manera

                    return Json(responseData);
                }
                else
                {
                    // Manejar el error según sea necesario
                    return BadRequest($"Error al obtener la cantidad de casos por enfermedad. Estado de la respuesta: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Manejar el error según sea necesario
                return BadRequest($"Error al obtener la cantidad de casos por enfermedad: {ex.Message}");
            }
        }


        [HttpGet]
        public ActionResult ResultadosComparativos()
        {
            try
            {
                FichaTEST model = new FichaTEST();
                string tokenValue = Request.Cookies["token"];
                var client = new RestClient(configuration["APIClient"]);
                var request = new RestRequest("/api/consulta/GetAllResultados", Method.Get);
                request.AddParameter("Authorization", string.Format("Bearer " + tokenValue), ParameterType.HttpHeader);

                var response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = response.Content;
                    List<FichaComparativa> FichaComparativa= System.Text.Json.JsonSerializer.Deserialize<List<FichaComparativa>>(content);
                    model.FichaComparativa = FichaComparativa;

                    if (FichaComparativa != null)
                    {
                        // Calcular la cantidad total de registros
                        model.TotalRegistros = FichaComparativa.Count;

                        // Calcular la cantidad de registros con resultadoFinal igual a "1"
                        model.ResultadoAcertado = FichaComparativa.Count(f => f.resultadoFinal == "1");

                        // Calcular la cantidad de registros con resultadoFinal igual a "0"
                        model.ResultadoErroneo = FichaComparativa.Count(f => f.resultadoFinal == "0");
                        return Json(model);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Registros no encontrados" });
                    }
                }
                else
                {
                    return Json(new { Mensaje = $"Error al obtener la lista de clientes. Código de estado: {response.StatusCode}" });
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones aquí y registrar información de depuración si es necesario
                return Json(new { Mensaje = $"Error: {ex.Message}" });
            }

        }





        // GET: TestingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TestingController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestingController/Create
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

        // GET: TestingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TestingController/Edit/5
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

        // GET: TestingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TestingController/Delete/5
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
