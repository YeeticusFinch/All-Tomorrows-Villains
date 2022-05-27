using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlWatch : MonoBehaviour {

    public TextAsset[] songs;
    Dictionary<string, string> nameMap = new Dictionary<string, string>();

    GameObject obj;

    public int playingSong = 0;
    bool stopSongs = false;

    public string[] songNames = {
        "The Following Theme", "Stranger Things Title Screen", "Sonata in C 3rd Movement", "Doctor Who Theme", "Skyrim Title Screen", "Cello Concerto", "Secunda", "Evil Morty", "Ward", "Love the Way You Lie", "Spooky Scary Skeletons", "Seven Nation Army", "Goodbye Moonmen", "All Star but its so Beautiful", "Crab Rave", "Happy Birthday", "Natural", "Heathens", "Rick and Morty Intro", "Waterloo", "Take on Me", "Despacito", "Interstellar Firstep", "Overwatch Theme", "Control", "A Cruel Angel's Thesis Neon Genesis", "Soviet National Anthem", "Beastars OP", "Stairway to Heaven", "Memelovania", "Imagine Communism", "Sweeeeet Child O' Mine", "Kiss of Death", "The Game is On, Sherlock", "Bohemian Rhapsody", "Fade", "For Whom the Bell Tolls", "Cantina", "The Force Awakens - Rey's Theme", "Marble Machine - Wintergatan", "O Fortuna", "Crazy Train", "Thunderstruck", "Charol of the Bells", "Thomas the Dank Engine", "Højt fra træets grønne top", "Teeth", "Maple Leaf Rag", "A Cruel Angel's Meme", "The Other Side of Paradise", "InSaNiTy", "Ghost Fight", "I Am The Doctor", "Old Town Road", "Senorita", "Dance Monkey", "YMCA", "The Swan", "Sweet but Psycho", "Dear Society", "﻿Uragirimono No Requiem (JoJo)", "Giorno's Theme (JoJo)", "A Butterflies Wings (JoJo Rabit)", "Every Girl's A Super Girl (JoJo)", "Clair De Lune", "Jazz In Paris", "Cohen's Masterpiece (Bioshock)", "Out Of The Black", "Moonage DayDream", "Space Oddity", "Changes", "Teeth A", "Teeth B", "Seven Nation Army A", "7 Nation Army B", "Great War - Sabaton", "Hyatt A", "Hyatt B", "Hyatt C", "Cohen A", "Cohen B", "Cohen C", "RWBY", "Criminal", "Crystal Dolphin A", "Crystal Dolphin B", "Brass Knob", "Bang A", "Bang B", "Bang C", "Crab Rave A", "Crab Rave B", "Crab Rave C", "Russian Crab Rave", "Gourmet Race A", "Gourmet Race B", "Your Turn to Roll", "Tubbo's Theme", "Heatwaves", "Feed The Machine", "Super Mario Theme", "Wide Putin", "Pigstep", "Believer", "I Can't Decide", "I Was Made to Love You", "Solar Power", "The Entertainer", "前前前世", "Life Is A Highway", "Sweet Dreams", "Sweet Dreams Kahoot Remix", "Winter Wonderland Jazz Remix",
    };
    public string[] fileNames = {
        "TheFollowingTheme.txt", "StrangerThingsTitleScreen.txt", "SonatainC3rdMovement.txt", "DoctorWhoTheme.txt", "Dragonborn.txt", "CelloConcerto.txt", "Secunda.txt", "EvilMorty.txt", "Ward.txt", "LovetheWayYouLie.txt", "SpookyScarySkeletons.txt", "7NationArmy.txt", "GoodbyeMoonmen.txt", "AllStar.txt", "CrabRave.txt", "HappyBirthday.txt", "Natural.txt", "Heathens.txt", "RickandMorty.txt", "Waterloo.txt", "TakeOnMe.txt", "Despacito.txt", "Firstep.txt", "OverwatchTheme.txt", "Control.txt", "ACruelAngelsThesis.txt", "USSRAnthem.txt", "Beasters.txt", "StairwaytoHeaven.txt", "Memelovania.txt", "ImagineCommunism.txt", "SweetChildofMine.txt", "KissofDeath.txt", "Sherlock.txt", "BohemianRapsody.txt", "Fade.txt", "ForWhomtheBellTolls.txt", "Cantina.txt", "ReysTheme.txt", "MarbleMachine.txt", "Fortuna.txt", "CrazyTrain.txt", "Thunderstruck.txt", "CharolofBells.txt", "ThomastheDankEngine.txt", "DanishChristmas.txt", "Teeth.txt", "MapleLeafRag.txt", "CruelAngelsMeme.txt", "TheOtherSideofParadise.txt", "Insanity.txt", "GhostFight.txt", "IAmTheDoctor.txt", "OldTownRoad.txt", "Senorita.txt", "DanceMonkey.txt", "YMCA.txt", "Swan.txt", "SweetbutPsycho.txt", "DearSociety.txt", "Traitor.txt", "Giorno.txt", "Butterfly.txt", "SuperGirl.txt", "clair.txt", "JazzInParis.txt", "Cohen.txt", "Outofblack.txt", "MoonageDaydream.txt", "SpaceOddity.txt", "Changes.txt", "TeethA.txt", "TeethB.txt", "7NationA.txt", "7NationB.txt", "GreatWar.txt", "HyattA.txt", "HyattB.txt", "HyattC.txt", "CohenA.txt", "CohenB.txt", "CohenC.txt", "theDay.txt", "Criminal.txt", "CrystalA.txt", "CrystalB.txt", "BrassKnob.txt", "BangA.txt", "BangB.txt", "BangC.txt", "CrabA.txt", "CrabB.txt", "CrabC.txt", "RussianCrab.txt", "GourmetA.txt", "GourmetB.txt", "CriticalRole.txt", "Tubbo.txt", "HeatWaves.txt", "FeedTheMachine.txt", "Mario.txt", "WidePutin.txt", "Pigstep.txt", "Believer.txt", "ICantDecide.txt", "IWasMadeToLoveYou.txt", "SolarPower.txt", "Entertainer.txt", "Zenzenzense.txt", "LifeIsAHighway.txt", "SweetDreams.txt", "SweetDreamsKahoot.txt", "WinterWonderland.txt",
    };
    //float[] tmods;
    //float[] yeetDurs;

    int[] table = {
        0, -2, 1760, 1397, 1568, 1319, 1175, 55, 440, 349, 392, 330, 294, 1480, 1109, 988, 47, 370, 278, 247, -7, 6000, 1000, 500, -5, 8000, 2000, -4, 110, 131, 175, 165, 92, 147, 139, -228, 196, 262, 82, 98, 123, -240, -6, 4000, 73, 41, 62, 659, 587, 494, 1500, -3, 784, 698, 523, 554, 880, 740, 1047, 831, 622, 466, 415, 311, 1245, -8, -16, -12, -32, -14, -22, -28, -10, -60, 220, 185, 208, -68, 12000, 3000, -11, -9, -96, -49, -18, 117, 156, 65, 49, 933, 668, 334, 2004, 1336, 501, 167, 77, 59, 233, 664, 332, 1328, 996, 1992, 83, 166, 2093, 1865, 1661, 408, -58, 816, 204, -62, -31, 136, 102, -128, 51, 612, 33, 37, 39, 31, 52, 104, 116, 87, 44, 1600, 800, 400, 200, 600, -30, -36, -46, 1112, 834, 2224, 556, -20, -24, -82, 417, -57, -230, -224, -17, 1560, 390, 780, 585, 195, 1170, -13, 3120, -64, -15, 2340, -136, 932, 1200, 3951, 277, 2637, 3136, 2349, 1976, 2489, 2960, 3520, 381, 127, 254, 1016, 508, 1524, 762, 2032, -19, 828, 621, 414, 207, 1656, 3312, 1242, 276, 2484, 1449, 69, 1248, 416, 1664, 832, -79, 3328, 1872, 624, 312, 468, 1092, 936, -21, 2496, 61, 124, -51, -26, -52, 240, 120, 480, 250, 2400, 300, 150, -48, 450, 900, 225, 75, 1800, 182, 91, 728, 364, 1456, 546, -100, 4186, 3729, 3322, 2794, 2217, 1056, 176, 1408, 528, 704, 352, 1232, -124, 356, 178, 1424,
    };

    int[] table2 = {
        0, -2, 294, 370, 494, 587, 554, 466, 659, 740, 330, 440, 392, -5, -4, -3, -6, 784, 880, -9, -15, -13, 1175, 1109, 988, 1319, 1568, 1760, 1480, 2016, 84, -8, 1344, 504, 336, 1008, 168, 672, 224, -7, -12, -10, 2688, 185, 196, 220, 233, 123, 247, 98, 73, 147, 110, 62, 2704, 1352, 507, 338, 169, 2028, -24, -256, 676, 1014, -68, -225, 523, 175, 87, 49, 1047, 1976, 2093, 1397, 698, 262, 2637, 2349, 349, 165, 131, 5760, 1920, 3840, 960, 1440, 480, 360, 240, -1140, -64, -75, 932, 4, 8, 16, 320, -222, 160, -96, -52, -84, -16, 15360, 55, 82, 65, 44, 41, 1992, 996, -160, 5976, 2988, -41, -43, 3984, 7968, -51, -28, 332, -33, 498, -72, -38, 249, -128, 1494, 15936, 1245, 831, 1500, 250, 500, 1000, 125, 750, 2000, 375, 278, -17, 92, 104, -112, 415, 311, 208, 622, 1750, 139, 156, 117, 77, -32, -26, 933, 116, 69, 720, 60, 930, 450, 120, -124, 52, 59, 1800, -31, 420, 1600, 50, 100, -18, 400, 800, 600, 200, 1200, 29, 39, 37, 33, 2400, -14, 3200, 56, 112, 896, -19, 448, -11, -23, -65, 28, 4000, 3000, 8000, 6000, -21, -22, 875, -30, 1661, 1865, 2217, 2489, 2794, 2960, 3136, 3322, 3520, 3729, 3951, 4186, 277, 124, 248, 31, 496, 372, 992, 93, 1984, 744, 47, 35, 46, 61, 428, 856, 642, 214, 3424, 1284, 1712, 2568, -78, 216, -190, 432, 648, 1296, 1512, 1728, 324, 54, 864, 108, -34, 27, -27, 288, -39, 7000
    };

    int[] table3 = {
        0, 294, 440, 415, 466, 392, 587, -2, 554, 659, 698, 880, 784, -3, 831, 933, 1175, 1109, 1319, 1397, 1760, 1568, 1047, 494, 988, 1480, 1661, 349, 330, 740, 370, 1984, 248, -4, 124, 496, 992, 1488, -6, 62, 744, -32, 868, -33, -12, -9, 73, 147, 55, 98, 196, 82, 87, 110, 116, 165, 175, 139, 220, 247, 278, 104, 123, 185, 208, 92, 500, 250, -8, -62, 1000, -34, -18, -19, -29, -46, 750, 125, 2000, -5, 311, 233, 262, 131, 277, 77, 65, 59, 69, -7, 523, 622, 2976, 372, 1116, 186, -10, 2232, 558, -11, -26, -16, 156, -48, -14, 93, -20, -17, -44, 932, 1245, 1865, 2489, 2217, -28, 668, 334, 167, -15, 2672, -128, 1336, -88, -13, 2004, -104, 1976, 7968, 1992, 5976, 3984, 996, 498, -65, 747, 664, 1328, 1494, 41, 49, 47, 44, 117, 8016, 6012, 4008, 501, 1002, -129, 3006, 375, -24, -25, -30, 329, 288, 108, 18, 144, 576, 1152, 2304, 216, 36, 432, 864, 192, 72, 384, 96, 61, 39, 52, 264, 528, 2112, 1056, 132, 66, 198, 33, 99, 396, 1584, -39, 176, -258, -56, -54, -112, -115, 4000, 6000, 1692, 846, 2538, 423, -480, 224, 448, 896, -57, 672, 336, 112, 168, 56, 1344, -50, -37, 226, 452, 904, -586, 113, 339, -133, -221, 31, 912, 456, 228, 114, -570, 342, -22, 684, 1824, 1368, -64, 171, 37, 1808, 678, -161, -340, 1026, 1032, 688, 172, 43, 344, 3000, 1500, -73, -31, 428, 214, 856, 642, 107, 1712,
    };

    int[] table4 = {
        0, -2, 415, 622, 523, 392, 466, 494, 659, -3, 831, 988, 1661, -7, 1245, 1397, 1047, 933, 698, 784, 1175, 1109, 554, 349, 740, 440, 311, 300, 150, -5, 600, -16, 1200, -8, -6, -4, -9, -12, 450, -14, 77, 104, 208, 110, 116, 277, 82, 52, 124, 247, 294, 329, 147, 165, 98, 123, 131, 92, 87, 233, 262, 370, 65, 69, 73, 59, 196, 139, -17, -36, -79, -84, -76, 587, -11, 880, 1568, 960, 720, 480, 240, 60, 930, 120, 360, 420, -10, -124, 1920, 1440, 156, 175, 3720, -31, 330, 278, 220, 856, 428, 1284, 214, 642, -15, -40, 1712, -38, 321, 107, 185, 117, 8000, 4000, 3000, 375, 250, 1000, 2000, -88, -24, 62, 1500, 500, 6000, -32, 264, 132, 528, 396, 198, 66, 1056, 176, 55, 46, 39, 49, 266, 133, 532, 1760, 2093, 1976, 1865, 1480, 1319, 368, 184, 552, 276, -28, 1472, 736, 1104, -384, -13, -30, 2208, -26, 644, 2576, -62, -256, -238, 932, 868, 434, 651, 217, 1302, 1736, 516, 258, 387, 129, 1032, 774, 520, 260, 195, 780, 2080, 306, 153, 612, 459, 1224, 918, 1836, 2448, -65, -23, -20, -22, 1914, 638, 1276, 319, 957, 320, -270, 80, 640, 1280, 350, 1808, 452, 226, 113, 904, 339, -42, 704, 352, 1408, 88, -64, 2816, 1232, 2112, 58, 41, 47, 130, 230, 115, 920, 690, 345, 460, 1840, 2944, 732, 1464, 2196, 2928, 366, 840, -312, -439, -257, -192, 828, 1656, 414, 138, 207, 37, 834, 1668, 1112, 556, 417, -57, -120,
    };

    int[] table5 = {
        0, -3, 330, 440, 523, 659, 622, 494, 587, -2, 466, 554, 698, 415, 220, 262, 880, 831, 784, 740, 294, 165, 110, 98, 87, 82, 349, 175, 370, -6, 247, 392, 988, 1047, 1175, 1319, 1397, 8000, 750, 250, 1000, 2000, -8, 500, -4, -9, 1500, 6000, 375, 4000, -18, 125, 156, 147, 139, 131, 123, 116, 104, 73, 92, 55, 65, 77, 208, 52, 62, 311, 44, -48, -28, -34, -66, -19, -10, 233, 277, 329, 933, 1760, 2093, 2794, 1865, 1109, 1661, 1480, 1245, 2217, 185, 1976, 2637, 2960, 2349, 3729, 2489, 320, 240, 480, 1280, 640, 1920, 192, -15, -12, 3840, 360, 212, 428, -5, 59, 69, 196, 46, 124, 61, 332, 249, 498, 664, 1328, 1992, 3984, 5976, -90, -14, 756, 189, 378, 567, 1134, -7, -16, -17, -38, -36, -55, -24, -176, 3, 37, 49, 41, 188, 376, 752, 564, -258, -132, -45, 94, -192, -144, 856, 214, 642, 107, -26, 1712, 117, 278, 932, 1568, 252, 504, 1008, 2016, 4032, 3024, 1512, 126, 3528, 336, 3000, 2144, 268, -40, 1072, 134, 536, 804, -73, 1608, -11, -32, 402, -78, 67, 201, 33, -43, 1876, 1728, 216, 432, 864, -13, 648, -64, -72, 3951, 3136, 3520, 2032, 508, 1016, 254, 1524, 381, 762, 127, -22, -20, 47, 31, 58, 248, 496, 992, 1984, 1488, 372, 744, 667, 333, 308, -160, -30, -124, 3322, 4978, 4435, 1125, 75, -212, -216, -168, 4186, 5588, 4699, 7459, -180, -140,
    };

    int[] table6 = {
        73, -3, 87, 82, 110, 131, 123, 0, 440, -2, 392, 349, 330, 294, 587, 466, 523, 659, 262, -7, -6, 220, -4, 698, 147, 175, 165, 880, 784, 1047, 1175, 1397, 933, 1319, 1109, 233, 247, 196, -12, 116, 98, 1760, 250, -48, 500, -5, 1000, 750, 2000, 1500, 125, -35, 35, -8, -11, 63, 375, 83, -24, -9, 37, 44, 41, -32, -28, -16, -15, 139, 55, 554, 59, 277, 208, 185, 156, -19, -13, -92, -62, -61, -191, -60, -162, -14, -78, -66, 494, 740, 988, 4000, -10, -256, 3000, 2250, 6000, 4500, -33, 106, 188, 1125, -189, 831, 300, 1200, 900, 450, 150, 600, 1800, 400, 2400, 622, 311, 415, 370, 8000, 104, 1712, 856, 214, 428, 642, -23, 124, 107, -30, 65, -64, 932, 1568, 92, 376, 94, 752, 1504, 47, 1128, 282, 49, 29, 117, 58, 33, 2093, 1245, 1865, 1976, 324, 162, 648, 486, 81, -18, -126, -124, 77, 278, -96, 564, 3008, -120, -42, 352, 704, 176, 264, 528, 132, -17, -79, 1408, 1440, 360, 180, 720, 45, 135, 30, 120, 90, -80, 1080, -25, 240, 88, 1056, -27, 960, 1920, 2880, 3840, 480, 60, 62, 7200, 7680, -34, 999, 1998, 1332, 3996, 7992, 5994, 666, -151, -56, 6500, -45, -50, -71, -49, -127, -128, -81, 1498, 499, 3496, 6993, 350, -131, -98, -136, -20, 12000, -26, -37, 7000, 2997, -110, -46, -103, -76, -21, -29, -194, -22, 78, 312, 234, 936, 468, 624, 1248, 1092, 69, -160, -68, -72, -158, 875, 39, -67, -59, -65, -192,
    };

    int[] table7 = {
        262, 523, 233, 466, 208, 415, 196, 392, 175, 349, 156, 311, 147, 294, 131, -3, -2, 622, 784, 587, 698, 0, 165, 185, 220, 247, 278, 330, 370, 440, 494, 831, 1245, 1047, 1175, 1397, 1109, 988, 933, 740, 659, 554, 329, -4, 1000, -16, 3000, 2000, 500, 750, 666, 333, 250, 83, -25, 1333, -7, -12, 5333, 2333, 2666, 5000, -6, -5, 116, 104, 98, 87, 77, 73, 65, 139, 124, 92, 82, 69, 123, -9, -8, 4000, 880, 1319, 1568, 1480, 1760, 1661, 1500, 6000, 664, 8000, 1750, 110, 3500, -99, -15,
    };

    public int sampleRate = 44100;
    public float waveLengthInSeconds = 2.0f;

    AudioSource audioSource;
    int timeIndex = 0;

    public void startShit(TextAsset[] songs, GameObject obj)
    {
        this.songs = SortSongs(songs);
        this.obj = obj;
        audioSource = obj.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 50f * GameManager.instance.sound.distMult;
        audioSource.spatialBlend = 1f;
        audioSource.pitch = GameManager.instance.sound.pitch;
        //audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically
        for (int i = 0; i < Mathf.Min(songNames.Length, fileNames.Length); i++)
        {
            nameMap.Add(fileNames[i], songNames[i]);
        }
    }

    int okn = 0;

    float startSeconds = 0;

    public void playSong(int n)
    {
        //okn = 2;
        if (playingSong == 0)
        {
            startSeconds = 0;
            StartCoroutine(playTrack(n, true));
            StartCoroutine(playTrack(n, false));
            if (songNames[n].Substring(songNames[n].Length - 2) == " A")
            {
                if (songNames[n + 1].Substring(songNames[n + 1].Length - 2) == " B")
                {
                    StartCoroutine(playTrack(n + 1, true));
                    StartCoroutine(playTrack(n + 1, false));
                }
                if (songNames[n + 2].Substring(songNames[n + 2].Length - 2) == " C")
                {
                    StartCoroutine(playTrack(n + 2, true));
                    StartCoroutine(playTrack(n + 2, false));
                }
            } else if (songNames[n].Substring(songNames[n].Length - 2) == " B")
            {
                if (songNames[n + 1].Substring(songNames[n + 1].Length - 2) == " C")
                {
                    StartCoroutine(playTrack(n + 1, true));
                    StartCoroutine(playTrack(n + 1, false));
                }
                if (songNames[n - 1].Substring(songNames[n - 1].Length - 2) == " A")
                {
                    StartCoroutine(playTrack(n - 1, true));
                    StartCoroutine(playTrack(n - 1, false));
                }
            } else if (songNames[n].Substring(songNames[n].Length - 2) == " C")
            {
                if (songNames[n - 1].Substring(songNames[n - 1].Length - 2) == " B")
                {
                    StartCoroutine(playTrack(n - 1, true));
                    StartCoroutine(playTrack(n - 1, false));
                }
                if (songNames[n -2].Substring(songNames[n -2].Length - 2) == " A")
                {
                    StartCoroutine(playTrack(n - 2, true));
                    StartCoroutine(playTrack(n - 2, false));
                }
            }
        } else
        {
            StartCoroutine(StopSongs());
        }
    }

    IEnumerator StopSongs()
    {
        stopSongs = true;
        while (playingSong > 0)
            yield return new WaitForSeconds(0.1f);
        stopSongs = false;
    }

    IEnumerator playTrack(int songNumber, bool treble)
    {
        okn++;


        bool playing = true;
        playingSong++;

        float[] tempInitShit = tmodPlusYeetDur(songNumber);

        int freq1 = 0; //Frequency to be played
        int dur1 = 0; //Duration to be played
        int repFreq1 = -1;
        int repDur1 = -1;
        int fin = 0; // frequency index
        int din = 0; // duration index
        float tmod = tempInitShit[0];
        float yeetDur = tempInitShit[1];
        int bfmod = (int)tempInitShit[2];
        float trebleMod = tempInitShit[3];
        float baseMod = tempInitShit[4];
        int freqModTreble = (int)tempInitShit[5];
        int freqModBase = (int)tempInitShit[6];
        bool decoded = false;
        int fancySize = -1;

        tmod *= treble ? trebleMod : baseMod;
        //tmod = 0.2f;
        int freqMod = treble ? freqModTreble : freqModBase* bfmod;
        //freqMod = 1;

        char[][] yeet = { new char[16384], new char[16384]};

        Debug.Log("Loading Song " + songNames[songNumber]);
        #region read file
        string text = songs[songNumber].text;
        int tfsi = 0; // Get start indices in the text
        int tdsi = text.IndexOf("\n", tfsi)+1;
        int bfsi = text.IndexOf("\n", tdsi)+1;
        int bdsi = text.IndexOf("\n", bfsi)+1;
        int fsi = treble ? tfsi : bfsi;
        int dsi = treble ? tdsi : bdsi;
        int fsiMax = dsi-1;
        int dsiMax = treble ? bfsi - 1 : text.Length;

        Debug.Log("tdsi = " + tdsi);
        Debug.Log("bfsi = " + bfsi);
        Debug.Log("bdsi = " + bdsi);

        int i = 0;
        while (fsi < fsiMax)
        { // 38, 0, 1, 38, 39
            //int nextIndex = Math.Min(Math.Min(text.IndexOf(",", fsi), text.IndexOf("\n", fsi)), text.IndexOf("}", fsi));
            if (fsi >= text.Length)
                break;
            int nextIndex = text.IndexOfAny(new char[] { ',', '\n', '}' }, fsi);
            //Debug.Log("fsi = " + fsi);
            //int nextIndex = text.IndexOf(",", fsi);
            if (nextIndex == -1) break;
            //Debug.Log("nextIndex = " + nextIndex);
            //Debug.Log(text.Substring(fsi, nextIndex-fsi));
            yeet[0][i] = (char)int.Parse(text.Substring(fsi, nextIndex-fsi));
            fsi = text.IndexOf(" ", nextIndex);
            if (fsi == -1) break;
            fsi++;
            i++;
        }
        Debug.Log("Freq count = " + i);
        fancySize = i;

        i = 0;
        while (dsi < dsiMax)
        {
            //int nextIndex = Math.Min(Math.Min(text.IndexOf(",", fsi), text.IndexOf("\n", fsi)), text.IndexOf("}", fsi));
            if (dsi >= text.Length)
            {
                //Debug.Log("dsi is too big");
                break;
            }
            int nextIndex = text.IndexOfAny(new char[] { ',', '\n', '}' }, dsi);
            //Debug.Log("dsi = " + dsi);
            //Debug.Log("nextIndex = " + nextIndex);
            if (nextIndex == -1)
            {
                //Debug.Log(text.Substring(dsi));
                yeet[1][i] = (char)int.Parse(text.Substring(dsi));
                i++;
                break;
            }
            //Debug.Log(text.Substring(dsi, nextIndex-dsi));
            yeet[1][i] = (char)int.Parse(text.Substring(dsi, nextIndex-dsi));
            dsi = text.IndexOf(" ", nextIndex);
            if (dsi == -1) break;
            dsi++;
            i++;
        }
        Debug.Log("Dur count = " + i);
        //fancySize = Math.Min(fancySize, i);

        #endregion

        Debug.Log("Finished loading song, fancySize = " + fancySize);

        okn--;

        while (okn > 0)
            yield return new WaitForEndOfFrame();

        if (startSeconds == 0)
            startSeconds = Time.time;
        float timeIndex = startSeconds;

        while (playing)
        {
            #region decomPlay
            int temp;

            if (fin >= fancySize || stopSongs)
            {
                //Debug.Log("Fin is too big!!!");
                playing = false;
            }

            if (decoded == false && fin < fancySize)
            {
                //Debug.Log("Decoding");
                if (repFreq1 == 0) // Stopped repeating
                {
                    repFreq1 = -1;
                    fin++;
                }

                if (repFreq1 < 1)
                {
                    temp = tableValue(yeet[0][fin], songNumber); // get the frequency
                    
                    if (temp >= 0) // if the number is negative, then you gotta repeat
                    {
                        freq1 = temp;
                        fin++;
                    }
                    else
                    {
                        repFreq1 = Mathf.Abs(temp) - 1;
                    }
                }
                if (repDur1 == 0)
                {
                    repDur1 = -1;
                    din++;
                }
                if (repDur1 < 1)
                {
                    temp = 0;
                    temp = tableValue(yeet[1][din], songNumber);
                    
                    if (temp >= 0)
                    {
                        dur1 = temp;
                        din++;
                    }
                    else
                    {
                        repDur1 = Mathf.Abs(temp) - 1;
                    }
                }
                decoded = true;
                //Debug.Log("Finished Decoding");
            }
            if (fin < fancySize)
            {
                //Debug.Log("Yeet Note");
                //Debug.Log(dur1 + " * " + tmod + " = " + Math.Max((int)(dur1 * tmod + 0.5f), 1));
                //dur1 = Math.Max((int)(dur1 * tmod + 0.5f), 1);
                #region yeet note
                decoded = false;
                if (freq1 > 0)
                {
                    //tone(buzzer1, freq, max((int)(dur * yeetDur), 1), 0, 0);
                    int numSamples = (int)(sampleRate * Mathf.Max(dur1 * yeetDur * tmod,1) / 1000f);
                    float[] samples = new float[numSamples];
                    //Debug.Log("Generating wave");
                    for (int ii = 0; ii < samples.Length; ii++)
                        samples[ii] = CreateSharktooth(ii, freq1 * freqMod, sampleRate);
                    //Debug.Log("Playing Tone " + freq1 * freqMod + " for " + Mathf.Max(dur1 * yeetDur * tmod, 1));
                    AudioClip ac = AudioClip.Create("tone", samples.Length, 1, sampleRate, false);
                    ac.SetData(samples, 0);
                    audioSource.PlayOneShot(ac, 0.1f);
                    //yield return new WaitForSeconds(dur1 * yeetDur / 1000f);
                    //audioSource.Stop();
                    //yield return new WaitForSeconds(dur1 * (1 - yeetDur) / 1000f);
                    //yield return new WaitForSeconds(dur1 * tmod/* * yeetDur*/ / 1000f);
                    timeIndex += dur1 * tmod / 1000f;
                    yield return new WaitForSeconds(Mathf.Max(timeIndex - Time.time));
                } else
                {
                    //yield return new WaitForSeconds(dur1 * tmod/* * yeetDur*/ / 1000f);
                    timeIndex += dur1 * tmod / 1000f;
                    yield return new WaitForSeconds(Mathf.Max(0, timeIndex - Time.time));
                }
                //tin++;
                if (repFreq1 > 0)
                {
                    repFreq1--;
                }
                if (repDur1 > 0)
                {
                    repDur1--;
                }

                #endregion
                //yeetNoteTreble(freq1, max((int)(dur1 * tmod), 1));
            }
            #endregion
        }

        playingSong--;
        //Debug.Log("Finished playing song");

        //float d = 0;
        //yield return new WaitForSeconds(d/1000f);
    }

    private int tableValue(char fancyChar, int songNumber)
    {
        if (songNumber < 21)
            return table[(int)fancyChar];
        else if (songNumber < 33)
            return table2[(int)fancyChar];
        else if (songNumber < 47)
            return table3[(int)fancyChar];
        else if (songNumber < 65)
            return table4[(int)fancyChar];
        else if (songNumber < 82)
            return table5[(int)fancyChar];
        else if (songNumber < 111)
            return table6[(int)fancyChar];
        else
            return table7[(int)fancyChar];
    }

    //Creates a sinewave
    public float CreateSine(int timeIndex, float frequency, float sampleRate)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate);
    }

    //Creates a squarewave
    public float CreateSquare(int timeIndex, float frequency, float sampleRate)
    {
        return Mathf.Sign(Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate));
    }

    //Creates a sawtooth wave
    public float CreateSawtooth(int timeIndex, float frequency, float sampleRate)
    {
        return Mathf.Repeat(timeIndex * frequency / sampleRate, 1) * 2f - 1f;
    }

    public float CreateSharktooth(int timeIndex, float frequency, float sampleRate)
    {
        return (Mathf.Sqrt(1f-Mathf.Pow(Mathf.Repeat(timeIndex * frequency / sampleRate, 1) - 1f,2f))) * 2f - 1f;
    }

    public float CreateNoise(int timeIndex, float frequency, float sampleRate)
    {
        return CreateSharktooth(timeIndex, frequency, sampleRate)*(0.1f+1.3f*UnityEngine.Random.value);
    }

    private TextAsset[] SortSongs(TextAsset[] s)
    {
        TextAsset[] r = new TextAsset[s.Length];
        for (int i = 0; i < fileNames.Length; i++)
        {
            //Debug.Log(fileNames[i].Substring(0, fileNames[i].Length-4));
            foreach (TextAsset e in s)
            {
                if (e.name == fileNames[i].Substring(0, fileNames[i].Length - 4))
                {
                    Debug.Log(e.name);
                    r[i] = e;
                    break;
                }
            }
        }
        return r;
    }

    private float[] tmodPlusYeetDur(int songNumber)
    {
        float tmod = 0.2f;
        float yeetDur = 0.5f;
        int bfmod = 2;
        float trebleMod = 1;
        float baseMod = 1;
        int freqModTreble = 1;
        int freqModBase = 1;
        #region if-statements
        if (songNumber == 0)
        {
            tmod = 0.2f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 1)
        {
            tmod = 0.2f;
            yeetDur = 0.9f;
        }
        else if (songNumber == 2)
        {
            tmod = 0.2f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 3)
        {
            tmod = 0.1f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 4)
        {
            tmod = 0.2f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 5)
        {
            tmod = 0.25f;
            yeetDur = 0.9f;
        }
        else if (songNumber == 6)
        {
            tmod = 0.8f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 7)
        {
            tmod = 0.5f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 8)
        {
            tmod = 0.8f;
            yeetDur = 0.9f;
        }
        else if (songNumber == 9)
        {
            tmod = 0.6f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 10)
        {
            tmod = 0.4f;
            yeetDur = 0.3f;
        }
        else if (songNumber == 11)
        {
            tmod = 0.8f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 12)
        {
            tmod = 0.8f;
            yeetDur = 0.8f;
        }
        else if (songNumber == 13)
        {
            tmod = 0.65f;
            yeetDur = 0.8f;
        }
        else if (songNumber == 14)
        {
            tmod = 0.87f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 15)
        {
            tmod = 0.4f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 16)
        {
            tmod = 0.8f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 17)
        {
            tmod = 0.8f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 18)
        {
            tmod = 0.7f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 19)
        {
            tmod = 0.8f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 20)
        {
            tmod = 0.86f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 21)
        {
            tmod = 0.8f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 22)
        {
            tmod = 0.4f;
            yeetDur = 0.8f;
        }
        else if (songNumber == 23)
        {
            tmod = 0.8f;
            yeetDur = 0.8f;
        }
        else if (songNumber == 24)
        {
            tmod = 0.6f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 25)
        {
            tmod = 0.8f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 26)
        {
            tmod = 0.8f;
            yeetDur = 0.8f;
        }
        else if (songNumber == 27)
        {
            tmod = 0.84f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 28)
        {
            tmod = 0.55f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 29)
        {
            tmod = 0.85f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 30)
        {
            tmod = 0.5f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 31)
        {
            tmod = 0.8f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 32)
        {
            tmod = 0.2f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 33)
        {
            tmod = 0.6f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 34)
        {
            tmod = 0.6f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 35)
        {
            tmod = 0.6f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 36)
        {
            tmod = 0.2f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 37)
        {
            tmod = 0.7f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 38)
        {
            tmod = 0.7f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 39)
        {
            tmod = 0.7f;
            yeetDur = 0.4f;
        }
        else if (songNumber == 40)
        {
            tmod = 0.3f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 41)
        {
            tmod = 0.9f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 42)
        {
            tmod = 0.9f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 43)
        {
            tmod = 0.9f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 44)
        {
            tmod = 0.27f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 45)
        {
            tmod = 0.24f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 46)
        {
            tmod = 0.9f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 47)
        {
            tmod = 0.9f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 48)
        {
            tmod = 0.8f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 49)
        {
            tmod = 0.85f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 50)
        {
            tmod = 0.25f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 51)
        {
            tmod = 0.84f;
            yeetDur = 0.4f;
        }
        else if (songNumber == 52)
        {
            tmod = 1;
            yeetDur = 0.65f;
        }
        else if (songNumber == 53)
        {
            tmod = 0.87f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 54)
        {
            tmod = 0.8f;
            yeetDur = 0.8f;
        }
        else if (songNumber == 55)
        {
            tmod = 0.7f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 56)
        {
            tmod = 0.2f;
            yeetDur = 0.75f;
        }
        else if (songNumber == 57)
        {
            tmod = 0.9f;
            yeetDur = 0.9f;
        }
        else if (songNumber == 58)
        {
            tmod = 0.85f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 59)
        {
            tmod = 0.85f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 60)
        {
            tmod = 0.5f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 61)
        {
            tmod = 0.9f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 62)
        {
            tmod = 0.6f;
            yeetDur = 0.76f;
        }
        else if (songNumber == 63)
        {
            tmod = 0.75f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 64)
        {
            tmod = 0.75f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 65)
        {
            tmod = 0.3f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 66)
        {
            tmod = 0.3f;
            yeetDur = 0.8f;
        }
        else if (songNumber == 67)
        {
            tmod = 0.7f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 68)
        {
            tmod = 0.7f;
            yeetDur = 0.75f;
        }
        else if (songNumber == 69)
        {
            tmod = 0.7f;
            yeetDur = 0.8f;
        }
        else if (songNumber == 70)
        {
            tmod = 0.6f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 71)
        {
            tmod = 0.9f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 72)
        {
            tmod = 0.9f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 73 || songNumber == 74)
        {
            tmod = 0.8f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 75)
        {
            tmod = 0.95f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 76 || songNumber == 77 || songNumber == 78)
        {
            tmod = 0.3f;
            yeetDur = 0.5f;
        }
        else if (songNumber == 79 || songNumber == 80 || songNumber == 81)
        {
            tmod = 0.7f;
            yeetDur = 0.8f;
        }
        else if (songNumber == 82)
        {
            tmod = 0.18f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 83)
        {
            tmod = 0.25f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 84 || songNumber == 85)
        {
            tmod = 0.75f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 86)
        {
            tmod = 0.2f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 87 || songNumber == 88 || songNumber == 89)
        {
            tmod = 0.8f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 90 || songNumber == 91 || songNumber == 92)
        {
            tmod = 0.25f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 93)
        {
            tmod = 0.6f;
            yeetDur = 0.8f;
        }
        else if (songNumber == 94 || songNumber == 95)
        {
            tmod = 0.66f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 96)
        {
            tmod = 0.2f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 97)
        {
            tmod = 0.8f;
            yeetDur = 0.9f;
        }
        else if (songNumber == 98)
        {
            tmod = 1.0f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 99)
        {
            tmod = 0.3f;
            yeetDur = 0.4f;
        }
        else if (songNumber == 100)
        {
            tmod = 0.17f;
            yeetDur = 0.4f;
        }
        else if (songNumber == 101)
        {
            tmod = 0.35f;
            yeetDur = 0.4f;
        }
        else if (songNumber == 102)
        {
            tmod = 0.15f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 103)
        {
            tmod = 0.16f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 104 || songNumber == 105)
        {
            tmod = 0.14f;
            yeetDur = 0.7f;
        }
        else if (songNumber == 106 || songNumber == 107)
        {
            tmod = 0.3f;
            yeetDur = 0.6f;
            bfmod = 1;
        }
        else if (songNumber == 108)
        {
            tmod = 0.75f;
            yeetDur = 0.6f;
            bfmod = 2;
        }
        else if (songNumber == 109)
        {
            tmod = 1.2f;
            yeetDur = 0.65f;
        }
        else if (songNumber == 110)
        {
            tmod = 0.4f;
            yeetDur = 0.6f;
        }
        else if (songNumber == 111)
        {
            tmod = 0.25f;
            yeetDur = 0.75f;
        }
        else if (songNumber == 112)
        {
            tmod = 0.15f;
            yeetDur = 0.35f;
        }
        #endregion
        #region more if-statements for treble
        if (songNumber == 6)
        {
            trebleMod = 0.994011976f;
        }
        else if (songNumber == 7)
        {
            freqModTreble = 2;
            trebleMod = 0.9803921569f;
        }
        else if (songNumber == 21)
        {
            trebleMod = 0.9940828402f;
        }
        else if (songNumber == 22)
        {
            trebleMod = 0.9638554217f;
        }
        else if (songNumber == 36)
        {
            trebleMod = 1.0060240964f;
        }
        else if (songNumber == 42)
        {
            trebleMod = 0.9912280702f;
        }
        else if (songNumber == 62)
        {
            trebleMod = 0.9945652174f;
        }
        else if (songNumber == 67)
        {
            freqModTreble = 2;
            trebleMod = 0.9947089947f;
        }
        else if (songNumber == 69)
        {
            trebleMod = 0.9920634921f;
        }
        else if (songNumber == 75)
        {
            trebleMod = 1.008064516f;
        }
        else if (songNumber == 98)
        {
            trebleMod = 0.9973404255f;
        }
        #endregion
        #region more if-statements for base
        if (songNumber == 7 || songNumber == 67)
        {
            freqModBase = 2;
        }
        else if (songNumber == 38)
        {
            baseMod = 1.0909090909f;
        }
        else if (songNumber == 40)
        {
            baseMod = 2.3640661939f;
        }
        else if (songNumber == 41)
        {
            baseMod = 0.9911504425f;
        }
        else if (songNumber == 43)
        {
            baseMod = 0.9941860465f;
        }
        else if (songNumber == 51)
        {
            baseMod = 0.992481203f;
        }
        else if (songNumber == 54)
        {
            baseMod = 0.9923076923f;
        }
        else if (songNumber == 57)
        {
            baseMod = 0.996875f;
        }
        else if (songNumber == 59)
        {
            baseMod = 0.25f;
        }
        else if (songNumber == 64)
        {
            baseMod = 0.9928057554f;
        }
        #endregion
        return new float[] { tmod, yeetDur, bfmod, trebleMod, baseMod, freqModTreble, freqModBase };
    }
}
