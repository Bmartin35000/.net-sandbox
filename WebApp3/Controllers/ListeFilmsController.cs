using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp3.Data;

namespace WebApp3.Controllers
{
    [Authorize]
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
            if (id == null) {
                return NotFound();
            }
            var filmsUtilisateur = _contexte.FilmsUtilisateur.Where(x => x.IdUtilisateur == id); // table FilmUser
            var modele = filmsUtilisateur.Select(x => new ModeleVueFilm // join table Film & FilmUser
            {
                IdFilm = x.IdFilm,
                Titre = x.Film.Titre,
                Annee = x.Film.Annee,
                Vu = x.Vu,
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
            _logger.LogInformation(filmUtilisateur.ToString());

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

        public async Task<IActionResult> Delete(int idFilm) {
            _logger.LogInformation("Delete");

            string userId = await RecupererIdUtilisateurCourant();
            if (userId == null) {
                return NotFound();
            }
            FilmUtilisateur fu = _contexte.FilmsUtilisateur.Where(x => x.IdFilm == idFilm && x.IdUtilisateur == userId).ToList().FirstOrDefault();
            _logger.LogInformation(fu.ToString());

            _contexte.FilmsUtilisateur.Remove(fu);
            await _contexte.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            string userId = await RecupererIdUtilisateurCourant();
            if (userId == null) {
                return NotFound();
            }
            List<FilmUtilisateur> filmsUtilisateur = _contexte.FilmsUtilisateur.Where(x => x.IdFilm == id && x.IdUtilisateur == userId).ToList();
            
            if (filmsUtilisateur == null || filmsUtilisateur.Count() == 0) {
                return NotFound();
            } else {
                FilmUtilisateur filmUtilisateur = filmsUtilisateur.First();
                return View(filmUtilisateur);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Vu,Note,IdFilm,IdUtilisateur")] FilmUtilisateur filmUtilisateur)
        {
            if (ModelState.IsValid)
            {
                _contexte.FilmsUtilisateur.Update(filmUtilisateur);
                await _contexte.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filmUtilisateur);
        }
    }
}
