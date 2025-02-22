using System.Collections;
using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack;
using TMPro;
using UnityEngine;

public class hp : MonoBehaviour
{
    public Character chSc;
    public TextMeshProUGUI txt;
    void Start()
    {
        txt.text = "HP: " + chSc.hp;
    }

    void Update()
    {
        txt.text = "HP: " + chSc.hp;
    }
}
