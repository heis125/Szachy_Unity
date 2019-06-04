using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Pionek : Bazowa_Figura
{
    private bool Pierwszy_Ruch = true;

    public override void Zaladuj(Color kolor_Druzyny, Color32 kolor_Figury, Menager_Figur nowyMenager_Figur)
    {
        base.Zaladuj(kolor_Druzyny, kolor_Figury, nowyMenager_Figur);

        Pierwszy_Ruch = true;

        _Ruch = kolor == Color.white ? new Vector3Int(0, 1, 1) : new Vector3Int(0, -1, -1);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Pionek");
    }
    public override void Ruch()
    {
        base.Ruch();
        Pierwszy_Ruch = false;

        Sprawdz_Promocje();
    }
    private bool Dzialania (int cel_x, int cel_y, Okreslenie_Pola cel)
    {
        Okreslenie_Pola stan = Okreslenie_Pola.None;
        stan = _CurrentCell.plansza.Sprawdzenie_Pola(cel_x, cel_y, this);
        if (stan == cel)
        {
            _Podswietlone_Pola.Add(_CurrentCell.plansza.AllCells[cel_x, cel_y]);
            return true;
        }
        return false;

    }

    public override void Sprawdzenie_drogi()
    {
        int aktualne_x = _CurrentCell.Pozycja_Planszy.x;
        int aktualne_y = _CurrentCell.Pozycja_Planszy.y;

        Dzialania(aktualne_x - _Ruch.z, aktualne_y + _Ruch.z, Okreslenie_Pola.Wrog);

        if(Dzialania(aktualne_x,aktualne_y+_Ruch.y, Okreslenie_Pola.Wolne))
        {
            if(Pierwszy_Ruch)
            {
                Dzialania(aktualne_x, aktualne_y + (_Ruch.y * 2), Okreslenie_Pola.Wolne);
            }
        }
        Dzialania(aktualne_x + _Ruch.z, aktualne_y + _Ruch.z, Okreslenie_Pola.Wrog);
    }

    public override void Reset()
    {
        base.Reset();
        Pierwszy_Ruch = true;
    }
    private void Sprawdz_Promocje()
    {
        int aktualne_x = _CurrentCell.Pozycja_Planszy.x;
        int aktualne_y = _CurrentCell.Pozycja_Planszy.y;

        Okreslenie_Pola pole = _CurrentCell.plansza.Sprawdzenie_Pola(aktualne_x, aktualne_y + _Ruch.y, this);
        if(pole== Okreslenie_Pola.Za_Plansza)
        {
            Color kolor_Figury = GetComponent<Image>().color;
            _Menager_Figur.Promowanie_Figury(this, _CurrentCell, kolor, kolor_Figury);
        }
    }
}
