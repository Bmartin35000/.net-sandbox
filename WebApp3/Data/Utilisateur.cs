public class Utilisateur : Microsoft.AspNetCore.Identity.IdentityUser
{
   public Utilisateur() : base()
   {
      ListeFilms = new HashSet<FilmUtilisateur>();
   }
   
 public string Prenom { get; set; }
 public virtual ICollection<FilmUtilisateur> ListeFilms { get; set; }
}