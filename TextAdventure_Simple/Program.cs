/// <summary>
///  Text Adventure Simple
///  Raaka versio 
/// </summary>

const int PAIKKOJEN_MAARA = 6;
const int ESINEIDEN_MAARA = 3;
const int ILMANSUUNTIEN_MAARA = 6;

// Alkumääritykset

// Paikat kuvattuna ja talletettu string arrayhyn. 
string[] paikan_kuvaus = new string[PAIKKOJEN_MAARA]
    {
        "Paikka 0", // Paikkaa ei ole, ensimmäinen paikka on paikan_kuvaus[1]
        "Olet omassa huoneessasi Draculan linnassa. On aamu ja näit hänet viimeksi eilen, saatettuaan sinut tänne kammioon. Pohjoiseen on ovi.",
        "Olet käytävällä, jossa muutamien taulujen lisäksi ei näytäisi olevan mitään muuta.",
        "Olet vieras huoneessa, joka näyttää samalta kuin sinun oma huoneesi.",
        "Olet oudossa pienessä huoneessa. Se näyttäisi olevan täysin tyhjä.",
        "ALAKERTA."
    };

// Ilmansuunnat mihin yksittäisestä paikasta pääsee liikkumaan.
// Ilmansuunnilla on 6 mahdollisuutta POHOISEEN, ITÄÄN, ETELÄ, LÄNSI, YLÖS, ALAS
// Ensimmäinen array indexi viittaa paikkaan (joten 0 paikkaa ei ole) ja toinen indexi on itse 6 suuntaa
// Jos luku on 0 suuntaan ei pääse. Jos luku on -1 suuntaan ei pääse ilman että tekee jotain (esim. avaa oven lukon). Yli 0 olevat numerot kertovat mikä on 
//      kyseiseen ilmansuuntaan olevan paikan numero
int[][] paikan_ilmansuunnat = new int[PAIKKOJEN_MAARA][];

paikan_ilmansuunnat[0] = new int[ILMANSUUNTIEN_MAARA] { 0, 0, 0, 0, 0, 0 }; // Paikkaa ei ole, kuten ei ole ilmansuuntiakaan
paikan_ilmansuunnat[1] = new int[ILMANSUUNTIEN_MAARA] { -1, 0, 0, 0, 0, 0 }; // Aloitus huone lukittu ovi pohjoiseen
paikan_ilmansuunnat[2] = new int[ILMANSUUNTIEN_MAARA] { 0, 4, 1, 3, 0, 0 };  // Käytävä
paikan_ilmansuunnat[3] = new int[ILMANSUUNTIEN_MAARA] { 0, 2, 0, 0, 0, 0 };  // Vierashuone
paikan_ilmansuunnat[4] = new int[ILMANSUUNTIEN_MAARA] { 0, 0, 0, 2, 0, -1 }; // Outo pieni huone jossa salaportaat -> löytyy vasta kun on tutkittu huone ja painettu nappia
paikan_ilmansuunnat[5] = new int[ILMANSUUNTIEN_MAARA] { 0, 0, 0, 0, 4, 0 };  // ALAKERTA

// Esineet kuvattuna ja talletettu string arrayhyn. 
string[] esineen_kuvaus = new string[ESINEIDEN_MAARA]
    {
        "Esine 0", // Esinettä ei ole
        "Avain",
        "Veitsi"
    };

// Esineiden sijainti. Kertoo paikan missä esine sijaitsee.
// Jos sijainti on -1, se on piilossa jossain. Jos sijainti on 0 se on itsellään.
int[] esineen_sijainti = new int[ESINEIDEN_MAARA];

esineen_sijainti[0] = -100; // Esinettä ei ole
esineen_sijainti[1] = 1;
esineen_sijainti[2] = 3;

// Käyttäjän komentotila

// Mistä paikasta aloitetaan
int nykyinen_paikka = 1;

// Määritetään poistumismuuttuja
bool exit = true;

// Määritetään paikan 1 ovi -> onko auki vai ei
bool paikka1Ovi = false;

// Määritetään paikan 4 salanappi -> onko löydetty
bool paikka4nappi = false;

// Määritetään paikan 4 salanappi -> onko painettu
bool paikka4nappipainettu = false;

// Kierros
do
{
    // Hae paikan kuvaus
    Console.WriteLine(paikan_kuvaus[nykyinen_paikka]);

    // Hae ilmansuunnat mihin voi kulkee
    string ilmansuunnat = "Voit kulkea: ";
    for (int i = 0; i < ILMANSUUNTIEN_MAARA; i++)
    {
        if (paikan_ilmansuunnat[nykyinen_paikka][i] == 0) // Paikkaan ei voida koskaan kulkea
        { }
        else if (paikan_ilmansuunnat[nykyinen_paikka][i] < 0) // Jos paikka on -1, sinne ei pääse ilman muuta toiminto (esim. lukittu ovi)
        { }
        else
        {
            if (i == 0) ilmansuunnat += "POHJOISEEN ";
            else if (i == 1) ilmansuunnat += "ITÄÄN ";
            else if (i == 2) ilmansuunnat += "ETELÄÄN ";
            else if (i == 3) ilmansuunnat += "LÄNTEEN ";
            else if (i == 4) ilmansuunnat += "YLÖS ";
            else if (i == 5) ilmansuunnat += "ALAS ";
            else { };
        }
    }
    Console.WriteLine(ilmansuunnat);

    // Hae paikan esineet, jos on
    string esineet = "Täällä on esineet: ";
    for (int i = 0; i < ESINEIDEN_MAARA; i++)
    {
        if (esineen_sijainti[i] == nykyinen_paikka) esineet += esineen_kuvaus[i] + " ";
        else { };
    }
    Console.WriteLine(esineet);

    // Tee satunnaisten tapahtumien tarkistus paikalle
    int ticks = System.Environment.TickCount;
    Random rnd = new Random(ticks);
    int nix = rnd.Next(1, 100);

    // Satunnainen kissa liikkuu
    bool kissa = false;
    if (nix < 20)
    {
        Console.WriteLine("Täällä liikkuu kissa.");
        kissa = true;
    }

    // Näytetään "komentotulkki" ja odotetaan käyttäjän komentoa
    Console.Write("> ");
    string komento = Console.ReadLine();

    // Jaetaan käyttäjän komento yksi sanaisiin ja kaksi sanaisiin (erotusmerkkinä välilyönti)
    string[] komennot = komento.Split(' ');

    // Käyttäjän komento on yksi sanainen
    if (komennot.Length == 1)
    {
        komennot[0] = komennot[0].ToUpper();

        // Exit komento
        if (komennot[0].StartsWith("EXI")) exit = false;

        // Inventaario komento
        else if (komennot[0].StartsWith("INV"))
        {
            Console.WriteLine("Sinulla on tavarat: ");
            for (int i = 0; i < esineen_sijainti.Length; i++)
            {
                if (esineen_sijainti[i] == 0)
                {
                    Console.WriteLine(esineen_kuvaus[i]);
                }
                else { }
            }
        }


        else if (komennot[0].StartsWith("POH"))
        {
            if (paikan_ilmansuunnat[nykyinen_paikka][0] == 0)
            {
                Console.WriteLine("Et voi kulkea siihen suuntaan.");
            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][0] == -1)
            {
                if (nykyinen_paikka == 1 || paikan_ilmansuunnat[nykyinen_paikka][0] == -1)
                {
                    Console.WriteLine("Ovi on lukossa.");
                }
            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][0] > 0)
            {
                nykyinen_paikka = paikan_ilmansuunnat[nykyinen_paikka][0];
            }
            else
            { }

        }

        else if (komennot[0].StartsWith("ITÄ"))
        {
            if (paikan_ilmansuunnat[nykyinen_paikka][1] == 0)
            {
                Console.WriteLine("Et voi kulkea siihen suuntaan.");
            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][1] == -1)
            {

            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][1] > 0)
            {
                nykyinen_paikka = paikan_ilmansuunnat[nykyinen_paikka][1];
            }
            else
            { }

        }

        else if (komennot[0].StartsWith("ETE"))
        {
            if (paikan_ilmansuunnat[nykyinen_paikka][2] == 0)
            {
                Console.WriteLine("Et voi kulkea siihen suuntaan.");
            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][2] == -1)
            {

            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][2] > 0)
            {
                nykyinen_paikka = paikan_ilmansuunnat[nykyinen_paikka][2];
            }
            else
            { }

        }

        else if (komennot[0].StartsWith("LÄN"))
        {
            if (paikan_ilmansuunnat[nykyinen_paikka][3] == 0)
            {
                Console.WriteLine("Et voi kulkea siihen suuntaan.");
            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][3] == -1)
            {

            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][3] > 0)
            {
                nykyinen_paikka = paikan_ilmansuunnat[nykyinen_paikka][3];
            }
            else
            { }
        }

        else if (komennot[0].StartsWith("YLÖ"))
        {
            if (paikan_ilmansuunnat[nykyinen_paikka][4] == 0)
            {
                Console.WriteLine("Et voi kulkea siihen suuntaan.");
            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][4] == -1)
            {

            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][4] > 0)
            {
                nykyinen_paikka = paikan_ilmansuunnat[nykyinen_paikka][4];
            }
            else
            { }
        }

        else if (komennot[0].StartsWith("ALA"))
        {
            if (paikan_ilmansuunnat[nykyinen_paikka][5] == 0)
            {
                Console.WriteLine("Et voi kulkea siihen suuntaan.");
            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][5] == -1)
            {

            }
            else if (paikan_ilmansuunnat[nykyinen_paikka][5] > 0)
            {
                nykyinen_paikka = paikan_ilmansuunnat[nykyinen_paikka][5];
            }
            else
            { }
        }

        // Käsky mitä ei ymmärretä
        else { Console.WriteLine("En ymmärrä käskyäsi."); }

    }

    // Käyttäjän komento on kaksi sanainen
    else if (komennot.Length == 2)
    {
        komennot[0] = komennot[0].ToUpper();
        komennot[1] = komennot[1].ToUpper();

        // Ota komento ja sen käsittely
        if (komennot[0] == "OTA")
        {
            // Komento OTA AVAIN ja esine 1 (avain) on tässä sijainnissa
            if (komennot[1] == "AVA" && esineen_sijainti[1] == nykyinen_paikka)
            {
                Console.WriteLine("Otit avaimen.");
                esineen_sijainti[1] = 0; // Määritetään avain on itsellään (pistetään taskuun)
            }
            else if (komennot[1] == "AVA") { Console.WriteLine("Täällä ei ole avainta."); } // Ei ole avainta mitä ottaa
            // Komento OTA Veitsi ja esine 2 (veitsi) on tässä sijainnissa
            else if (komennot[1] == "VEI" && esineen_sijainti[2] == nykyinen_paikka)
            {
                Console.WriteLine("Otit veitsen.");
                esineen_sijainti[2] = 0; // Määritetään avain on itsellään (pistetään taskuun)
            }
            else if (komennot[1] == "KIS" || kissa == true) { Console.WriteLine("Yritit ottaa kissan kiinni mut se on niin nopea. Ei onnistu!"); } // Hassu kissa tapahtuma
            else Console.WriteLine("Haluat ottaa MITÄ?");

        }

        // Avaa komento ja sen käsittely
        else if (komennot[0] == "AVA")
        {
            // Komento AVAA OVI ja paikan sijainti = 1 sekä esine 1 (avain) on otettu taskuun : Ovea ei ole avattu aiemmin
            if (komennot[1] == "OVI" && nykyinen_paikka == 1 && esineen_sijainti[1] == 0 && paikka1Ovi == false)
            {
                Console.WriteLine("Avain kävi oveen ja sait sen auki.");
                paikka1Ovi = true; // Ovi avattu paikassa 1
                paikan_ilmansuunnat[1][0] = 2; // Muutetaan paikan 1 kulkeminen pohjoiseen tästä lähtien "auki" eli suoraan paikkaan 2
                paikan_kuvaus[1] = "Olet omassa huoneessasi Draculan linnassa. On aamu ja näit hänet viimeksi eilen, saatettuaan sinut tänne kammioon. Pohjoiseen oleva ovi on nyt auki.";
            }
            // Komento AVAA OVI ja paikan sijainti = 1 sekä esine 1 (avain) on otettu taskuun : Ovea on avattu aiemmin
            if (komennot[1] == "OVI" && nykyinen_paikka == 1 && esineen_sijainti[1] == 0 && paikka1Ovi == true)
            {
                Console.WriteLine("Olet jo avannut oven ja se nyt pysyvästi auki.");
            }
            else if (komennot[1] == "OVI") { Console.WriteLine("Täällä ei ole ovea mitä avata."); } // Avataan ovi jota ei ole
            else if (komennot[1] == "KIS" || kissa == true) { Console.WriteLine("Yrität siis avata kissan. Liian raakaa, ei onnistu!"); } // Hassu kissa tapahtuma
            else Console.WriteLine("Haluat avata MITÄ?"); // Avataan jotain ihmeellistä
        }

        // Pudota komento ja sen käsittely
        else if (komennot[0] == "PUD")
        {
            // Komento PUDOTA AVAIN ja esine 1 (avain) pudotetaan nykyiseen sijaintiin
            if (komennot[1] == "AVA" && esineen_sijainti[1] == 0)
            {
                Console.WriteLine("Pudotit avaimen.");
                esineen_sijainti[1] = nykyinen_paikka; // Esineen sijainti määritellään nykyiseen paikkaan
            }
            // Komento PUDOTA VEITSI ja esine 2 (veitsi) pudotetaan nykyiseen sijaintiin
            if (komennot[1] == "VEI" && esineen_sijainti[2] == 0)
            {
                Console.WriteLine("Pudotit veitsen.");
                esineen_sijainti[2] = nykyinen_paikka; // Esineen sijainti määritellään nykyiseen paikkaan
            }
            else if (komennot[1] == "AVA" && esineen_sijainti[1] != 0) { Console.WriteLine("Sinulla ei ole avainta jota pudottaa."); } // Pudotetaan esinettä jota ei ole
            else if (komennot[1] == "KIS" || kissa == true) { Console.WriteLine("Yrität siis pudottaa kissan. Kunhan nyt saisit sen edes kiini, ei onnistu!"); } // Hassu kissa tapahtuma
            else Console.WriteLine("Pudotat MITÄ?"); // Pudotetaan jotain ihmeellistä
        }

        // Tutki komento ja sen käsittely
        else if (komennot[0] == "TUT")
        {
            // 
            if (komennot[1] == "HUO" && nykyinen_paikka == 4)
            {
                Console.WriteLine("Tutkit pientä outoa huonetta ja löydät seinästä pienen napin.");
                paikka4nappi = true;
            }
            else if (komennot[1] == "HUO" && nykyinen_paikka != 4) { Console.WriteLine("Tutkit huonetta mutta et näe mitään epätavallista."); }
            else if (komennot[1] == "AVA" && esineen_sijainti[1] == 0) { Console.WriteLine("Avain on ihan normaali ja käy omaan huoneesi oveen."); }
            else if (komennot[1] == "KIS" || kissa == true) { Console.WriteLine("Tutkit kissaa ja se on musta."); } // Hassu kissa tapahtuma
            else Console.WriteLine("Tutkit MITÄ?"); // Tutkitaan jotain ihmeellistä
        }

        // Paina komento ja sen käsittely
        else if (komennot[0] == "PAI")
        {
            if (komennot[1] == "NAP" && nykyinen_paikka == 4 && paikka4nappi == true && paikka4nappipainettu == false)
            {
                Console.WriteLine("Painat nappia seinästä ja yht'äkkiä kuuluu kova ääni. Pian lattiasta lähtee salaportaat alaspäin.");
                paikka4nappipainettu = true;
                paikan_ilmansuunnat[4][5] = 5;
                paikan_kuvaus[4] = "Olet oudossa pienessä huoneessa. Täällä on salaiset portaat alaspäin";
            }
            else if (komennot[1] == "NAP" && nykyinen_paikka == 4 && paikka4nappi == true && paikka4nappipainettu == true)
            {
                Console.WriteLine("Olet jo painanut nappia. Näyttäisi siltä mitään ei tapahdu enää.");
            }
            else if (komennot[1] == "NAP") { Console.WriteLine("Täällä ei ole nappia mitä painaa."); }
            else Console.WriteLine("Painat MITÄ?"); // Tutkitaan jotain ihmeellistä
        }

        // Käsky mitä ei ymmärretä
        else { Console.WriteLine("En ymmärrä käskyäsi."); }


    }

    // Käyttäjä antanut enemmän kuin kaksi sanaa MITÄS NYT !
    else
    { }


} while (exit); // Poistutaan



