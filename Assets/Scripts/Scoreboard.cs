using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour
{
    public TextMesh text;
    public int score = 0;

    public void addpoints(int number)
    {
        score = score + number;
        updatescore();
    }
    public void updatescore()
    {
        text.text = "Score:" + score;
    }
}
