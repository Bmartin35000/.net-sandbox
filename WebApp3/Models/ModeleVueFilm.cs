public class ModeleVueFilm
{
 public int IdFilm { get; set; }
 public string Titre { get; set; }
 public int Annee { get; set; }
 public bool Vu { get; set; }
 public int? Note { get; set; }
 public override string ToString(){
    return "Titre "+Titre;
 }
}