using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp3.Data;

namespace WebApp3.Controllers
{
    public class ListeFilmsController : Controller {
        private readonly ApplicationDbContext _contexte;
        private readonly UserManager<Utilisateur> _gestionnaire; // service de .NET Identity pour user connecté
        private readonly ILogger<ListeFilmsController> _logger; 
        
        public ListeFilmsController(ApplicationDbContext contexte,
            UserManager<Utilisateur> gestionnaire, ILogger<ListeFilmsController> logger)
        {
            _logger=logger;
            _contexte = contexte;
            _gestionnaire= gestionnaire;
        }

        private Task<Utilisateur> GetCurrentUserAsync() => _gestionnaire.GetUserAsync(HttpContext.User);

        [HttpGet]
        public async Task<string> RecupererIdUtilisateurCourant() {
            Utilisateur utilisateur = await GetCurrentUserAsync();
            return utilisateur?.Id;
        }

        // GET: Film
        public async Task<IActionResult> Index()
        {
            var id = await RecupererIdUtilisateurCourant();
            var filmsUtilisateur = _contexte.FilmsUtilisateur.Where(x => x.IdUtilisateur == id); // table FilmUser
            var modele = filmsUtilisateur.Select(x => new ModeleVueFilm // join table Film & FilmUser
            {
                IdFilm = x.IdFilm,
                Titre = x.Film.Titre,
                Annee = x.Film.Annee,
                Vu = x.Vu,
                PresentDansListe = true,
                Note = x.Note
            }).ToList();
            
            return View(modele);
        }

         // Create: Film
        public async Task<IActionResult> Create(int? idFilm)
        {
            var fu = new FilmUtilisateur();
            fu.IdFilm = idFilm.GetValueOrDefault();   
            return View(fu);
        }

        // POST: Film/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Vu,Note,IdFilm")] FilmUtilisateur filmUtilisateur)
        {
            Utilisateur user = await GetCurrentUserAsync();
            filmUtilisateur.IdUtilisateur = user.Id;
            filmUtilisateur.User = user;

            filmUtilisateur.Film = _contexte.Films.Where(x => x.Id == filmUtilisateur.IdFilm).ToList().FirstOrDefault();

            if (ModelState.IsValid)
            {
                _contexte.Add(filmUtilisateur);
                await _contexte.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filmUtilisateur);
        }

        public async Task<IActionResult> Delete(ModeleVueFilm item) {
            string userId = await RecupererIdUtilisateurCourant();
            _logger.LogInformation("Delete");
            _logger.LogInformation(item.ToString());

            FilmUtilisateur fu = _contexte.FilmsUtilisateur.Where(x => x.IdFilm == item.IdFilm && x.IdUtilisateur == userId).ToList().FirstOrDefault();
            _logger.LogInformation("fu.ToString()");
            _logger.LogInformation(fu.ToString());

            _contexte.Remove(fu);
            return RedirectToAction(nameof(Index));
        }
    }
}
