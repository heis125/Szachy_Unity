using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menager_Figur : MonoBehaviour
{
    [HideInInspector]
    public bool Czy_krol_zyje = true;

    public GameObject Prefab_Figury;
    private List<Bazowa_Figura> Biale = null;
    private List<Bazowa_Figura> Czarne = null;
    private List<Bazowa_Figura> Promowane = new List<Bazowa_Figura>();

    private string[] Kolejnosc_Figur = new string[16]
    {
        "P", "P", "P", "P", "P", "P", "P", "P",
        "W", "S", "G", "D", "K", "G", "S", "W"
    };

    public Dictionary<string, Type> Spis_Figur = new Dictionary<string, Type>()
    {
        {"P", typeof(Pionek)},  //1
        {"W", typeof(Wieza)}, //2
        {"S", typeof(Skoczek)}, //3
        {"G", typeof(Goniec)}, // 4
        {"D", typeof(Dama)},    //5
        {"K", typeof(Krol)} //6

    };
    public void Zaladuj(Board board)
    {
        // tworzenie białych figur
        Biale = Stworz_Figure(Color.white, new Color32(80, 124, 159, 255), board);
        //tworzenie czarnych figur
        Czarne = Stworz_Figure(Color.black, new Color32(210, 95, 64, 255), board);

        //ustawianie figur
        Ustaw_Figury(1, 0, Biale, board);
        Ustaw_Figury(6, 7, Czarne, board);
        indeksy(Biale);
        indeksy(Czarne);

        //Biełe zaczynaja 
        Zmiana_Strony(Color.black);

    }

    private Bazowa_Figura Stworz_F(Type typ_figury)
    {
        //Tworzenie nowego obiektu 
        GameObject nowy_Obiekt_Figury = Instantiate(Prefab_Figury);
        nowy_Obiekt_Figury.transform.SetParent(transform);

        //Skiala i pozycja 
        nowy_Obiekt_Figury.transform.localScale = new Vector3(1, 1, 1);
        nowy_Obiekt_Figury.transform.localRotation = Quaternion.identity;

        Bazowa_Figura _nowa_Figura = (Bazowa_Figura)nowy_Obiekt_Figury.AddComponent(typ_figury);

        return _nowa_Figura;
    }
    private List<Bazowa_Figura> Stworz_Figure(Color teamColor,Color32 spriteColor,Board board)
    {
        List<Bazowa_Figura> nowa_Figura = new List<Bazowa_Figura>();

        for (int i = 0; i < Kolejnosc_Figur.Length; i++) 
        {

            // załadowanie typu i stworzenie obiektu
            string key = Kolejnosc_Figur[i];
            Type typ_Figury =Spis_Figur[key];

            // przechowywanie nowej figury
            Bazowa_Figura _nowa_Figura = Stworz_F(typ_Figury);
            nowa_Figura.Add(_nowa_Figura);

            //ustawienie figury
            _nowa_Figura.Zaladuj(teamColor, spriteColor, this);
        }
        return nowa_Figura;
    }

    private void Ustaw_Figury(int pionki, int krolewscy, List<Bazowa_Figura> figury, Board board)
    {
        for (int i = 0; i < 8; i++)
        {
            //ustawienie pionków
            figury[i].Miejsce(board.AllCells[i, pionki]);

            // ustawienie lini królewskiejh ;P
            figury[i+8].Miejsce(board.AllCells[i, krolewscy]);


        }
    }

    public void indeksy(List<Bazowa_Figura> figury)
    {
        for (int i = 0; i < 8; i++)
                 figury[i].a = 1;
        figury[8].a = 2;
        figury[9].a = 3;
        figury[10].a = 4;
        figury[11].a = 5;
        figury[12].a = 6;
        figury[13].a = 4;
        figury[14].a = 3;
        figury[15].a = 2;
    }

    private void Aktywnosc_Figur(List<Bazowa_Figura> wszystkie, bool czy)
    {
        foreach (Bazowa_Figura figura in wszystkie)
            figura.enabled = czy;
    }

    public void Zmiana_Strony(Color kolor)
    {
        if(!Czy_krol_zyje)
        {
            //reset figur
            Reset_Figur();

            //wskrzeszanie króla
            Czy_krol_zyje = true;

            kolor = Color.black;
        }
        bool czy_czarne = kolor == Color.white ? true : false;

        // ustawienie aktywnosci 
        Aktywnosc_Figur(Biale, !czy_czarne);
        Aktywnosc_Figur(Czarne, czy_czarne);

        //Promowany 
        foreach(Bazowa_Figura figura in Promowane)
        {
            bool czyCzarny = figura.kolor != Color.white ? true : false;
            bool czy_Druzyna = czyCzarny == true ? czy_czarne : !czy_czarne;
                figura.enabled = czy_Druzyna;
        }
        // ruch komputera
        if(kolor == Color.white)
        {
            int[] ruch = minimax(2, true, Czarne); // 0 nie ważne 1 które pole 2 ktora figura

            Czarne[ruch[2]].Pole_Ataku = Czarne[ruch[2]]._Podswietlone_Pola[ruch[1]];
            Czarne[ruch[2]].Ruch();
            Zmiana_Strony(Color.black);

        }
        
    }
    protected int[] minimax(int glebokosc, bool czy_Max, List<Bazowa_Figura> aktualna)
    {
        int temp;
        int[] takie = new int[3] { 0,0,0} ;
        List<Cell> Pozycja_przed_zmiana = new List<Cell>();

        if (glebokosc <= 0)
            return takie;
        if(czy_Max)
         {
             
             for (int i = 0; i < 16; i++)
             {
                Pozycja_przed_zmiana[i] = aktualna[i]._CurrentCell;
                aktualna[i].Sprawdzenie_drogi();
                int j = 0;
                int a = 0;
                if (aktualna[i].czy_ma_jakikolwiek_ruch)
                {
                    foreach(Cell cell in aktualna[i]._Podswietlone_Pola)
                    {
                        a = a + 1;
                    }
                    while (j<a)
                    {
                        aktualna[i]._CurrentCell = aktualna[i]._Podswietlone_Pola[j];
                        if (mini > minimax(glebokosc - 1, false, Biale)[0])
                        {
                            mini = minimax(glebokosc - 1, false, Biale)[0];
                            takie[1] = j;
                            takie[2] = i;
                            aktualna[takie[2]].Pole_Ataku = aktualna[takie[2]]._Podswietlone_Pola[1];
                        }
                        if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura != null)
                        {
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 1)
                            {
                                if (takie[0] < takie[0] + 10)
                                {
                                    takie[0] = 10;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 2)
                            {
                                if (takie[0] < takie[0] + 30)
                                {
                                    takie[0] = 30;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 3)
                            {
                                if (takie[0] < takie[0] + 30)
                                {
                                    takie[0] = 30;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 4)
                            {
                                if (takie[0] < takie[0] + 50)
                                {
                                    takie[0] = 50;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 5)
                            {
                                if (takie[0] < takie[0] + 90)
                                {
                                    takie[0] = 90;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 9)
                            {
                                if (takie[0] < takie[0] + 900)
                                {
                                    takie[0] = 900;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }

                        }
                        
                    }


            
                            j++;
                       
                    
                }

             }


        }
         else
         {
            for (int i = 0; i < 16; i++)
            {
                Pozycja_przed_zmiana[i] = aktualna[i]._CurrentCell;
                aktualna[i].Sprawdzenie_drogi();
                int j = 0;
                int a = 0;
                if (aktualna[i].czy_ma_jakikolwiek_ruch)
                {
                    foreach (Cell cell in aktualna[i]._Podswietlone_Pola)
                    {
                        a = a + 1;
                    }
                    while (j < a)
                    {
                        aktualna[i]._CurrentCell = aktualna[i]._Podswietlone_Pola[j];
                        if (max < minimax(glebokosc - 1, true, Czarne)[0])
                        {
                            max = minimax(glebokosc - 1, true, Czarne)[0];
                            takie[1] = j;
                            takie[2] = i;
                            aktualna[takie[2]].Pole_Ataku = aktualna[takie[2]]._Podswietlone_Pola[1];
                        }
                        if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura != null)
                        {
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 1)
                            {
                                if (takie[0] > takie[0] - 10)
                                {
                                    takie[0] = -10;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 2)
                            {
                                if (takie[0] > takie[0] - 30)
                                {
                                    takie[0] = -30;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 3)
                            {
                                if (takie[0] > takie[0] - 30)
                                {
                                    takie[0] = -30;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 4)
                            {
                                if (takie[0] > takie[0] - 50)
                                {
                                    takie[0] = - 50;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 5)
                            {
                                if (takie[0] > takie[0] - 90)
                                {
                                    takie[0] = -90;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }
                            if (aktualna[i]._Podswietlone_Pola[j].Aktualna_Figura.a == 5)
                            {
                                if (takie[0] > takie[0] - 900)
                                {
                                    takie[0] = -900;
                                    takie[1] = j;
                                    takie[2] = i;
                                }
                            }

                        }

                    }



                    j++;


                }

            }
        }
        for(int i =0; i <16; i ++)
        {
            aktualna[i]._CurrentCell = Pozycja_przed_zmiana[i];
        }
        return takie;
    }


    public void Reset_Figur()
    {
        foreach(Bazowa_Figura figura in Promowane)
        {
            figura.Zabij();
            Destroy(figura.gameObject);
 
        }
        Promowane.Clear();
        foreach (Bazowa_Figura figura in Biale)
            figura.Reset();

        foreach (Bazowa_Figura figura in Czarne)
            figura.Reset();

    }

    public void Promowanie_Figury(Pionek pionek, Cell cell, Color kolor_Druzyny, Color kolor_Figury)
    {
        // zabic pionka 
        pionek.Zabij();
        //stworzenie nowego 
        Bazowa_Figura Promowany = Stworz_F(typeof(Dama));
        Promowany.Zaladuj(kolor_Druzyny,kolor_Figury,this);
        // postawic 
        Promowany.Miejsce(cell);
        // dodac
        Promowane.Add(Promowany);
    }
   
}

