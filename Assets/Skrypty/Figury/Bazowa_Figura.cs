using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Bazowa_Figura : EventTrigger
{
    [HideInInspector]
    public Color kolor = Color.clear;
    public int a;
    public Cell _OriginalCell = null;
    public Cell _CurrentCell = null;
    public bool czy_ma_jakikolwiek_ruch = false;
    protected RectTransform _RectTransform = null;
    protected Menager_Figur _Menager_Figur;

    public Cell Pole_Ataku = null;

    protected Vector3Int _Ruch = Vector3Int.one;
    public List<Cell> _Podswietlone_Pola = new List<Cell>();

    public virtual void Zaladuj(Color kolor_Druzyny, Color32 kolor_Figury, Menager_Figur nowyMenager_Figur)
    {
        _Menager_Figur = nowyMenager_Figur;

        kolor = kolor_Druzyny;
        GetComponent<Image>().color = kolor_Figury;
        _RectTransform = GetComponent<RectTransform>();
    }

    public void Miejsce(Cell newCell)
    {
        //rzeczy do Cell
        _CurrentCell = newCell;
        _OriginalCell = newCell;
        _CurrentCell.Aktualna_Figura = this;

        //rzeczy do objektu
        transform.position = newCell.transform.position;
        gameObject.SetActive(true);
    }

    public virtual void Reset()
    {
        Zabij();
        Miejsce(_OriginalCell);
    }

    public virtual void Zabij()
    {
        // Czyszczenie pola
        _CurrentCell.Aktualna_Figura = null;
        // usuwanie figury
        gameObject.SetActive(false);

    }

    #region WykonywanieRuchu
    private void Tworzenie_Ruchu(int kierunek_x, int kierunek_y, int ruch)
    {
        // pozycja 
        int aktualne_x = _CurrentCell.Pozycja_Planszy.x;
        int aktualne_y = _CurrentCell.Pozycja_Planszy.y;


        //sprawdzenie wszystkich pól 
        for (int i = 1; i <= ruch; i++)
        {

            aktualne_x += kierunek_x;
            aktualne_y += kierunek_y;

            // Ustawianie stanu 
            Okreslenie_Pola okreslenie_Pola = Okreslenie_Pola.None;
            okreslenie_Pola = _CurrentCell.plansza.Sprawdzenie_Pola(aktualne_x, aktualne_y, this);
          
            //jesli wrog zapisz i zerwij petle
            if (okreslenie_Pola == Okreslenie_Pola.Wrog)
            {

                _Podswietlone_Pola.Add(_CurrentCell.plansza.AllCells[aktualne_x, aktualne_y]);
                czy_ma_jakikolwiek_ruch = true;
                break;
            }
            // jesli nie jest wolne zerwij petle
            if (okreslenie_Pola != Okreslenie_Pola.Wolne)
                break;

            // pozostałe dodajemy do listy 
            _Podswietlone_Pola.Add(_CurrentCell.plansza.AllCells[aktualne_x, aktualne_y]);
            czy_ma_jakikolwiek_ruch = true;


        }

    }
    public virtual void Sprawdzenie_drogi()
    {
        Tworzenie_Ruchu(1, 0, _Ruch.x);
        Tworzenie_Ruchu(-1, 0, _Ruch.x);

        Tworzenie_Ruchu(0, 1, _Ruch.y);
        Tworzenie_Ruchu(0, -1, _Ruch.y);

        Tworzenie_Ruchu(1, 1, _Ruch.z);
        Tworzenie_Ruchu(-1, 1, _Ruch.z);

        Tworzenie_Ruchu(-1, -1, _Ruch.z);
        Tworzenie_Ruchu(1, -1, _Ruch.z);
    }

    protected void Pokaz_Pola()
    {
        foreach (Cell cell in _Podswietlone_Pola)
            cell._OutlineImage.enabled = true;
    }
    public void Wyczysc_Pola()
    {
        foreach (Cell cell in _Podswietlone_Pola)
            cell._OutlineImage.enabled = false;
        _Podswietlone_Pola.Clear();
    }

    public virtual void Ruch()
    {
        // jeśli jest wróg, usuwamy go 
        Pole_Ataku.Usun_Figure();
        // czyszczenie aktualnego polozenia
        _CurrentCell.Aktualna_Figura = null;

        //zamiana pól 
        _CurrentCell = Pole_Ataku;
        _CurrentCell.Aktualna_Figura = this;

        //Ruch na plansz 
        transform.position = _CurrentCell.transform.position;
        Pole_Ataku = null;
    }
    #endregion

    #region Eventy

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        Sprawdzenie_drogi();
        Pokaz_Pola();


    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        // podążanie za kursorem
        transform.position += (Vector3)eventData.delta;

        foreach(Cell cell in _Podswietlone_Pola)
        {
            if(RectTransformUtility.RectangleContainsScreenPoint(cell._RectTransform,Input.mousePosition))
            {
                Pole_Ataku = cell;
                break;
            }
            Pole_Ataku = null;
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
      base.OnEndDrag(eventData);
      Wyczysc_Pola();

         if(!Pole_Ataku)
              {
                  transform.position = _CurrentCell.gameObject.transform.position;
                  return;
              }
            Ruch();

            _Menager_Figur.Zmiana_Strony(kolor);
    }
    #endregion
}
