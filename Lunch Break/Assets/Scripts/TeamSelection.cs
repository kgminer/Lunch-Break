using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelection : MonoBehaviour
{
    private static int team;

    public static void SetTeam(int teamNumber)
    {
        team = teamNumber;
    }
    
    public static int GetTeam()
    {
        return team;
    }
}
