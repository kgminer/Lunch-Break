using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CapZone : MonoBehaviour
{

    int teamInd = 0;
    public string team = "uncapped";

    List<Collider> Cappers = new List<Collider>();

    public string GetTeam()
    {
        return team;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.tag != "Team1" || other.gameObject.tag != "Team2" || other.gameObject.tag != "Team3")
         //   return;

        if (!Cappers.Contains(other))
            Cappers.Add(other);

        if (other.gameObject.tag != team)
            CountCappers();
    }

    private void OnTriggerExit(Collider other)
    {
       // if (other.gameObject.tag != "Team1" || other.gameObject.tag != "Team2" || other.gameObject.tag != "Team3")
        //    return;

        Cappers.Remove(other);
        CountCappers();
    }

    private void CountCappers()
    {
        int[] teamcount = { 0, 0, 0, 0 };

        foreach(Collider capper in Cappers)
        {
            if (capper.gameObject.tag == "scienceGeek")
                teamcount[1]++;
            if (capper.gameObject.tag == "bookWorm")
                teamcount[2]++;
            if (capper.gameObject.tag == "jocks")
                teamcount[3]++;
        }

        int max = teamcount.Max();
        int contestingTeam = teamcount.ToList().IndexOf(max);

        // favor current cappoint holder; contestant must have more cappers
        if (teamcount[contestingTeam] > teamcount[teamInd])
        {
            teamInd = contestingTeam;

            if (teamInd == 1)
                team = "scienceGeek";
            if (teamInd == 2)
                team = "bookWorm";
            if (teamInd == 3)
                team = "jocks";
        }
    }

    public string GetOwner()
    {
        return team;
    }
}
