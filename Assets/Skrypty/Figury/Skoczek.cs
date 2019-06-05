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
        wartosc = 350;
    }

    private bool Stworz_Ruch(int kierunek, bool sprawdzanie_czy_mat, List<PossibleMove> possibleMoves)
    {
        int aktualne_x = _CurrentCell.Pozycja_Planszy.x;
        int aktualne_y = _CurrentCell.Pozycja_Planszy.y;

        //lewo
        if (Dzialania(aktualne_x - 2, aktualne_y + (1 * kierunek)))
            if (possibleMoves != null)
                possibleMoves.Add(new PossibleMove(aktualne_x, aktualne_y, aktualne_x - 2, aktualne_y + (1 * kierunek)));
        // górne lewo
        if (Dzialania(aktualne_x - 1, aktualne_y + (2 * kierunek)))
            if (possibleMoves != null)
                possibleMoves.Add(new PossibleMove(aktualne_x, aktualne_y, aktualne_x - 1, aktualne_y + (2 * kierunek)));
        //górne prawo
        if (Dzialania(aktualne_x + 1, aktualne_y + (2 * kierunek)))
            if (possibleMoves != null)
                possibleMoves.Add(new PossibleMove(aktualne_x, aktualne_y, aktualne_x + 1, aktualne_y + (2 * kierunek)));
        //prawo
        if (Dzialania(aktualne_x + 2, aktualne_y + (1 * kierunek)))
            if (possibleMoves != null)
                possibleMoves.Add(new PossibleMove(aktualne_x, aktualne_y, aktualne_x + 2, aktualne_y + (1 * kierunek)));

        return false;
    }
    public override bool Sprawdzenie_drogi(bool sprawdzanie_czy_mat = false, List<PossibleMove> possibleMoves = null)
    {
        Wyczysc_Pola();
        //gorna czesc ruchu
        Stworz_Ruch(1, sprawdzanie_czy_mat, possibleMoves);
        //dolna czesc ruchu
        Stworz_Ruch(-1, sprawdzanie_czy_mat, possibleMoves);

        return false;

    }
    private bool Dzialania(int cel_x, int cel_y)
    {
        Okreslenie_Pola stan = Okreslenie_Pola.None;
        stan = _CurrentCell.plansza.Sprawdzenie_Pola(cel_x, cel_y, this);

        if (stan != Okreslenie_Pola.Przyjaciel && stan != Okreslenie_Pola.Za_Plansza)
        {
            _Podswietlone_Pola.Add(_CurrentCell.plansza.AllCells[cel_x, cel_y]);
            return true; //zwraca gdy można tam wykonać ruch
        }
        return false;
 
    }
}
