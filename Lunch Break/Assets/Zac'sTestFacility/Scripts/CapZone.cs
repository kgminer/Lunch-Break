using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CapZone : MonoBehaviour
{

    int teamInd = 0;
    public string team = "Team0";

    List<Collider> Cappers = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (!Cappers.Contains(other))
            Cappers.Add(other);

        if (other.gameObject.tag != team)
            CountCappers();

        //Debug.Log("Cap triggered, " + other.gameObject.tag + " entered. Cap belongs to " + team);
    }

    private void OnTriggerExit(Collider other)
    {
        Cappers.Remove(other);
        CountCappers();
    }

    private void CountCappers()
    {
        int[] teamcount = { 0, 0, 0, 0, 0 };

        foreach(Collider capper in Cappers)
        {
            if (capper.gameObject.tag == "Team1")
                teamcount[1]++;
            if (capper.gameObject.tag == "Team2")
                teamcount[2]++;
            if (capper.gameObject.tag == "Team3")
                teamcount[3]++;
            if (capper.gameObject.tag == "Team4")
                teamcount[4]++;
        }

        int max = teamcount.Max();
        int contestingTeam = teamcount.ToList().IndexOf(max);

        // favor current cappoint holder; contestant must have more cappers
        if (teamcount[contestingTeam] > teamcount[teamInd])
        {
            teamInd = contestingTeam;

            if (teamInd == 1)
                team = "Team1";
            if (teamInd == 2)
                team = "Team2";
            if (teamInd == 3)
                team = "Team3";
            if (teamInd == 4)
                team = "Team4";
        }
    }
}
