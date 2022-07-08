using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Suite
{
    Hearts,
    Diamond,
    Club, 
    Spade,
    None
}

[System.Serializable]
public enum Number
{
    Ace,
    //One, 
    //Two, 
    //Three, 
    //Four, 
    //Five,
    //Six,
    //Seven,
    //Eight, 
    Nine, 
    Ten,  
    Jack, 
    Queen, 
    King 
} 

[System.Serializable]
public class GameData
{
    public int NumberOfPlayers;
    public int NumberOfRounds;
    public int MaxNumberOfRounds;
    public int currentRound;

    public int currentActivePlayerID;
    public Suite currentHandSuite;
    public int firstHandPlayer;
    public int NumberofHands;

    public Suite trumpSuite;
    public int trumpCard;
    public Team leadTeam;

    public int minBetAount; 
    public int trumpBet;
    public bool trumpCalled; 

    public Team teamWon;

    public int LocalPlayerID; 

}

[System.Serializable]
public enum Team
{ 
    None,
    A, 
    B
}


[System.Serializable]
public class TeamData
{
    public Team team;
    public string teamName;
    public List<PlayerData> players;
    public int Score;
    public int RoundsWon; 
}


[System.Serializable]
public class PlayerData
{
    public string PlayerName;
    public int PlayerID;
    public List<int> Hand; 
    public int Score;
    public Team team;
    public bool hasBet;
    public bool hasPassed;
    public int PartnerID; 
    //public int trumpCallerPlayer; 
}

[System.Serializable]
public class HandData
{
    //public int[] players = new int[4]; 
    public List<int> players = new List<int>(); 

}

[System.Serializable] 
public class Round
{
    public int RoundNumber; 
    public bool TrumpRevealed;
    public Team TrumpTeam;
    public int TrumpPlayer;
    public int NumberOfPlayersBetorPassed;
    public bool RoundStarted; 
    public List<HandData> hand;
    public int currentHand;
    public int firstPlayer; 
    
}