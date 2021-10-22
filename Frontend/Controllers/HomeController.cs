using Frontend.Models;
using Frontend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            if (TempData.ContainsKey("Message"))
                ViewBag.Message = TempData["Message"].ToString();

            var TopPlayer = await BackendService.GetBalanceAsync();
            return View(TopPlayer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(PlayerView model)
        {
            var player = await BackendService.PostPlayerAsync(model);

            if (player != null)
            {
                ViewBag.Message = "Usuário cadastrado com sucesso! ID: " + player.PlayerId + " Nome: " + player.Name;
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Erro ao cadastrar o usuário, tente novamente.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PlayAsync(long PlayerId)
        {
            var player = await BackendService.GetPlayerAsync(PlayerId);

            if (player.PlayerId > 0)
            {
                // Caso venha do Player tem pontuação para ser exibida
                if (TempData.ContainsKey("Win"))
                    ViewBag.Win = TempData["Win"].ToString();

                ViewBag.Games = await BackendService.GetGamesAsync();
                return View(player);
            }

            TempData["Message"] = "Usuário não encontrado, cadastre-se primeiro.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PlayedAsync(GameResultView model)
        {
            // Gerar Win randomico
            Random rnd = new Random();
            model.Win = (long)(rnd.NextDouble() * 200 - 100);

            // Enviar para o endpoint1
            var resultGame = await BackendService.PostGameResultAsync(model);

            if (resultGame)
            {
                TempData["Win"] = model.Win.ToString();
                return RedirectToAction("Play", new { PlayerId = model.PlayerId });
            }

            ViewBag.Message = resultGame;
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
