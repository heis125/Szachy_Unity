using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Skoczek : Bazowa_Figura
{
    public override void Zaladuj(Color kolor_Druzyny, Color32 kolor_Figury, Menager_Figur nowyMenager_Figur)
    {
        base.Zaladuj(kolor_Druzyny, kolor_Figury, nowyMenager_Figur);

        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Skoczek");
    }

    private void Stworz_Ruch(int kierunek)
    {
        int aktualne_x = _CurrentCell.Pozycja_Planszy.x;
        int aktualne_y = _CurrentCell.Pozycja_Planszy.y;

        //lewo
        Dzialania(aktualne_x - 2, aktualne_y + (1 * kierunek));
        // górne lewo
        Dzialania(aktualne_x - 1, aktualne_y + (2 * kierunek));
        //górne prawo
        Dzialania(aktualne_x + 1, aktualne_y + (2 * kierunek));
        //prawo
        Dzialania(aktualne_x + 2, aktualne_y + (1 * kierunek));
    }
    public override void Sprawdzenie_drogi()
    {
        //gorna czesc ruchu
        Stworz_Ruch(1);
        //dolna czesc ruchu
        Stworz_Ruch(-1);

    }
    private void Dzialania(int cel_x, int cel_y)
    {
        Okreslenie_Pola stan = Okreslenie_Pola.None;
        stan = _CurrentCell.plansza.Sprawdzenie_Pola(cel_x, cel_y, this);

        if (stan != Okreslenie_Pola.Przyjaciel && stan != Okreslenie_Pola.Za_Plansza)
            _Podswietlone_Pola.Add(_CurrentCell.plansza.AllCells[cel_x, cel_y]);
 
    }
}
