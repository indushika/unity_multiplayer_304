using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class GameManager : MonoBehaviour
{
    public List<CardData> Cards;
    public List<PlayerData> Players;
    public List<Round> Rounds;
    public List<TeamData> Teams;
    public GameData gameData;

    public PhotonManager photonManager; 

    [SerializeField]
    private List<CardData> ShuffledCards;

    [SerializeField]
    private List<int> ShuffledCardIndexes;

    private Team newTeamAssign = Team.A;

    public int numberOfRounds; 
    public bool shuffledDone; 

    void Start()
    {

        InitiateGameData();

        //Shuffle();
    }


    void Update()
    {
        
    } 


    public void InitiateTeams()
    {
        for (int i = 0; i < Teams.Count; i++)
        {
            Teams[i].players = new List<PlayerData>();
        }
    }

    public void InitiateGameData()
    {
        gameData = new GameData();
        gameData.NumberOfPlayers = 0;
        gameData.NumberOfRounds = 0;
        gameData.MaxNumberOfRounds = 7;
        gameData.currentRound = 0;
        gameData.currentActivePlayerID = 0;
        gameData.firstHandPlayer = 0;
        gameData.NumberofHands = 6; 
        gameData.minBetAount = 199;
        gameData.trumpBet = 0;


        InitiateRoundData(gameData.currentRound, false, Team.None, 0,0,false,0,0);
    }

    public void InitiateRoundData(int roundNumber, bool trumpRevealed, Team leadTeam, int trumpPlayer, int numberOfPlayersBet, bool roundStarted, int currentHand, int firstPlayer)
    {
        //Rounds.Add
        Round round = new Round();
        round.RoundNumber = roundNumber;
        round.TrumpRevealed = trumpRevealed;
        round.TrumpTeam = leadTeam;
        round.TrumpPlayer = trumpPlayer;
        round.NumberOfPlayersBetorPassed = numberOfPlayersBet;
        round.RoundStarted = roundStarted;
        round.currentHand = currentHand;
        round.firstPlayer = firstPlayer; 

        List<HandData> hand = new List<HandData>();

        hand.Add(InitiateHandData()); 

        round.hand = hand;

        AddRoundData(round); 
    } 

    public HandData InitiateHandData()
    {
        HandData handData = new HandData();
        for (int i = 0; i < 4; i++)
        {
            //handData.players[i] = -1;
            handData.players.Add(-1); 
        }
        return handData; 
    }

    public void AddRoundData(Round roundData)
    {
        Rounds.Add(roundData); 
    }

    public void UpdateRoundData(int trumpPlayer)
    {
        Rounds[gameData.currentRound].TrumpTeam = Players[trumpPlayer].team;
        Rounds[gameData.currentRound].TrumpPlayer = trumpPlayer;
    }

    #region Round Data Fucntions
    public bool RoundTrumpRevealed
    {
        get { return Rounds[gameData.currentRound].TrumpRevealed; }
        set { Rounds[gameData.currentRound].TrumpRevealed = value; }
    }

    public Team RoundTrumpTeam
    {
        get { return Rounds[gameData.currentRound].TrumpTeam; }
        set { Rounds[gameData.currentRound].TrumpTeam = value; }
    }

    public int RoundTrumpPlayer
    {
        get { return Rounds[gameData.currentRound].TrumpPlayer; }
        set { Rounds[gameData.currentRound].TrumpPlayer = value; }
    }

    public int RoundNumberPlayerBetorPassed
    {
        get { return Rounds[gameData.currentRound].NumberOfPlayersBetorPassed; }
        set { Rounds[gameData.currentRound].NumberOfPlayersBetorPassed = value; }
    }

    public bool RoundStarted
    {
        get { return Rounds[gameData.currentRound].RoundStarted; }
        set { Rounds[gameData.currentRound].RoundStarted = value; }
    }

    public int RoundCurrentHand
    {
        get { return Rounds[gameData.currentRound].currentHand; }
        set { Rounds[gameData.currentRound].currentHand = value; }
    }

    public void RoundAddCardtoHand(int playerID, int cardIndex)
    {
        Rounds[gameData.currentRound].hand[RoundCurrentHand].players[playerID] = cardIndex; 
    }

    public int RoundGetCardfromHand(int playerID)
    {
        int cardIndex;
        cardIndex = Rounds[gameData.currentRound].hand[RoundCurrentHand].players[playerID];
        return cardIndex; 
    }

    public void AddNewHand(HandData handData)
    {
        Rounds[gameData.currentRound].hand.Add(handData);
        RoundCurrentHand++; 
    }

    public int RoundFirstPlayer
    {
        get { return Rounds[gameData.currentRound].firstPlayer; }
        set { Rounds[gameData.currentRound].firstPlayer = value; }
    }
    #endregion

    public void InitiatePlayerData(string playerName, int PlayerID)
    {
        PlayerData player = new PlayerData();
        List<int> Hand = new List<int>(); 

        player.PlayerName = playerName;
        player.PlayerID = PlayerID; 
        player.Hand = Hand;
        player.PartnerID = AssignPartner(PlayerID);

        Players.Add(player);
        AssignTeam(Players[Players.IndexOf(player)]);


        gameData.NumberOfPlayers += 1;

    }

    public int AssignPartner(int PlayerID)
    {
        int partnerID=0; 

        switch (PlayerID)
        {
            case 0:
                partnerID = 2;
                break;
            case 1:
                partnerID = 3;
                break;
            case 2:
                partnerID = 0;
                break;
            case 3:
                partnerID = 1;
                break;
                
        }

        return partnerID; 
    }

    public void AssignTeam(PlayerData Player)
    {
        //Choose Team 
        //Assign Team 
        switch (newTeamAssign)
        {
            case Team.None:
                newTeamAssign = Team.A; 
                break;
            case Team.A:
                newTeamAssign = Team.B;
                break;
            case Team.B:
                newTeamAssign = Team.A;
                break;
            default:
                break;
        }

        Player.team = newTeamAssign;
        for (int i = 0; i < Teams.Count; i++)
        {
            if (Teams[i].team==newTeamAssign)
            {
                Teams[i].players.Add(Player);
            }
        }


        //for (int i = 0; i < Teams.Count; i++)
        //{
        //    if (Teams[i].players.Count < 2)
        //    {
        //        Teams[i].players.Add(Player);
        //        Player.team = Teams[i].team;
        //        break;
        //    }

            //}
    }



    #region Deprecated Functions 

    public void Shuffle()
    {
        //player 3 gets the option to shuffle 

        ShuffledCards = new List<CardData>();
        for (int i = 0; i < Cards.Count; i++)
        {
            ShuffledCards.Add(Cards[i]); 

        }

        int n = Cards.Count;
        while (n>1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            CardData value = ShuffledCards[k];
            ShuffledCards[k] = ShuffledCards[n];
            ShuffledCards[n] = value;
        }
        
        //update other players shuffled cards through photon  
    }

    public void Distribute()
    {
        int k = Cards.Count / 2;

        while (k > 0 && ShuffledCards.Count > 0)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                k--;
                CardData card = ShuffledCards[k];
                //hand changed from "CardData" to "Int"
                //Players[i].Hand.Add(card);
                ShuffledCards.Remove(card);
            }
        }
    }


    #endregion


    /// <summary>
    /// Master Client Functions 
    public void ShuffleCards()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            ShuffledCardIndexes.Add(i);
        }

        int n = Cards.Count;
        while (n > 0)
        {
            n--;
            int k = Random.Range(0, n + 1);
            int value = ShuffledCardIndexes[k];
            ShuffledCardIndexes[k] = ShuffledCardIndexes[n];
            ShuffledCardIndexes[n] = value;
        }

        SendShuffledCardsToAllPlayers();
    }

    public void SendShuffledCardsToAllPlayers()
    {
        for (int i = 0; i < ShuffledCardIndexes.Count; i++)
        {
            //if (i == ShuffledCardIndexes.Count - 1)

            //    photonManager.ShuffledCard(ShuffledCardIndexes[i], true);

            //else

            photonManager.ShuffledCard(ShuffledCardIndexes[i], false);

        }
        photonManager.ShuffledCard(-1, true);

    }

    /// </summary>


    public void AddToShuffledCardsFromMasterClient(int CardIndex)
    {
        ShuffledCardIndexes.Add(CardIndex); 
    }

    public void DistributeCards()
    {
        int k = Cards.Count / 2;
        //if (RoundStarted)
        //{
        //    k = Cards.Count;
        //}

        while (k > 0)   // && ShuffledCardIndexes.Count > 0
        {
            for (int i = 0; i < Players.Count; i++)
            {
                k--;
                int cardIndex = ShuffledCardIndexes[k];

                Players[i].Hand.Add(cardIndex);
                ShuffledCardIndexes.Remove(cardIndex);
            }
        } 
    } 

    public void PickATrumpCard(int cardIndex, int trumpPlayerID)
    {
        gameData.trumpCard = cardIndex;
        gameData.trumpSuite = Cards[cardIndex].suite;
        gameData.leadTeam = Players[trumpPlayerID].team;

        Players[trumpPlayerID].hasBet = true;
        //Players[gameData.LocalPlayerID].trumpCallerPlayer = trumpPlayerID; 

        UpdateRoundData(trumpPlayerID); 
    }

    public void PassUpdate(int trumpPlayerID)
    {
        Players[trumpPlayerID].hasPassed = true;
    }

    public void CallBet(int BetAmount)
    {
        gameData.trumpBet = BetAmount;
        gameData.trumpCalled = true;
        gameData.minBetAount = BetAmount + 10;
    }

    public PlayerData GetLocalPlayerData(int playerID)
    {
        PlayerData player;
        player = Players[playerID];
        return player; 
    } 

    public bool isTrumpCalled()
    {
        bool trumpCalled;
        trumpCalled = gameData.trumpCalled;
        return trumpCalled;
    }

    public void StartRound()
    {
        //distribute the rest of the cards 
    } 

    public void AddCardtoActiveHand(int playerID, int cardIndex)
    {
        RoundAddCardtoHand(playerID, cardIndex);
        //gameData.currentHandSuite = Cards[cardIndex].suite; 
    }

    public void UpdateCurentHandSuite(int cardIndex)
    {
        gameData.currentHandSuite = Cards[cardIndex].suite; 
    }

    public void UpdateRoundTrumpRevealed(bool isTrumpRevealed)
    {
        RoundTrumpRevealed = isTrumpRevealed; 
    }

    public void OnHandEnd()
    {
        ///determine which team has leading card 
        bool isTrumpCardinHand=false;
        int totalHandScore=0;
        int leadingScore = 0;
        int leadingCardIndex = 0;
        int leadingPlayerID = 0;
        int leadingTeamScore = 0;

        gameData.currentHandSuite = Suite.None;

        //loop through the hand and check for trump cards 
        for (int i = 0; i < Players.Count; i++)
        {
            int cardIndex = RoundGetCardfromHand(i);

            //while looping add total scores 
            totalHandScore = totalHandScore + Cards[cardIndex].Points; 

            if (Cards[cardIndex].suite==gameData.trumpSuite)
            {
                isTrumpCardinHand = true;
            }
        }

        //if hand has trump card
        if (isTrumpCardinHand)
        {
            //loop, if the trump suite, add score to the temp variable  
            for (int i = 0; i < Players.Count; i++)
            {
                int cardIndex = RoundGetCardfromHand(i);

                //check if the temp variable score is greater than the leadingscorevariable, if so add
                if (Cards[cardIndex].suite == gameData.trumpSuite)
                {
                    int tempScore = Cards[cardIndex].Points;
                    if (tempScore>leadingScore)
                    {
                        leadingScore = tempScore;
                        //add the cardIndex as leadingcardIndex 
                        leadingCardIndex = cardIndex; 
                    }
                }
            }

        }
        else
        {
            //loop, add score to the temp variable  
            for (int i = 0; i < Players.Count; i++)
            {
                int cardIndex = RoundGetCardfromHand(i);

                int tempScore = Cards[cardIndex].Points;

                //check if the temp variable score is greater than the leadingscorevariable, if so add
                if (tempScore > leadingScore)
                {
                    leadingScore = tempScore;
                    //add the cardIndex as leadingcardIndex 
                    leadingCardIndex = cardIndex;
                }
            }
        }


        //check for the playerID leadingcardindex belongs to in the hand 
        leadingPlayerID = Rounds[gameData.currentRound].hand[RoundCurrentHand].players.IndexOf(leadingCardIndex);

        //add score to player 
        Players[leadingPlayerID].Score = totalHandScore;

        //loop through teams and check which team player belongs to
        for (int i = 0; i < Teams.Count; i++)
        {
            int tempTeamScore = 0;
            tempTeamScore = Teams[i].Score;
            //add the score to the team score 
            if (Teams[i].team==Players[leadingPlayerID].team)
            {
                Teams[i].Score = tempTeamScore + totalHandScore; 
            }

            //check which team score is higher, and add that team to the leading team 
            if (Teams[i].Score > leadingTeamScore)
            {
                leadingTeamScore = Teams[i].Score;
                gameData.leadTeam = Teams[i].team; 
            }

        }


        //update active player and first hand player 
        gameData.firstHandPlayer = leadingPlayerID;
        gameData.currentActivePlayerID = leadingPlayerID;

        //initiate new hand and add to the current rounds hands 
        HandData newHand = InitiateHandData();
        AddNewHand(newHand); 

    } 

    public void UpdateRound()
    {
        int firstPlayer = RoundFirstPlayer;

        ResetGameData();
        ResetPlayerHand(); 
        //switch the players by one 
        if (firstPlayer==3)
        {
            firstPlayer = 0;
        }
        else
        {
            firstPlayer++;
        }

        gameData.currentActivePlayerID = firstPlayer;

        InitiateRoundData(gameData.currentRound, false, Team.None, 0, 0, false, 0, firstPlayer);

    }

    public void UpdatePlayerData()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].hasBet = false;
            Players[i].hasPassed = false;
        }
    }

    public void UpdateTeamWon()
    {
        gameData.leadTeam = GetLeadingTeam();
        switch (gameData.leadTeam)
        {
            case Team.A:
                Teams[0].RoundsWon++; 
                break;
            case Team.B:
                Teams[1].RoundsWon++;
                break;
            default:
                break;
        }
    }

    public Team GetLeadingTeam()
    {
        Team trumpTeam = RoundTrumpTeam;
        Team leadingTeam = RoundTrumpTeam;

        switch (trumpTeam)
        {
            case Team.A:
                if (Teams[0].Score >= gameData.trumpBet)
                {
                    leadingTeam = Team.A;
                }
                else
                {
                    leadingTeam = Team.B;
                }
                break;
            case Team.B:
                if (Teams[1].Score >= gameData.trumpBet)
                {
                    leadingTeam = Team.B;
                }
                else
                {
                    leadingTeam = Team.A;
                }
                break;
            default:
                leadingTeam = trumpTeam;
                break;
        }
        return leadingTeam; 
    }

    public void ResetTeamScore()
    {
        for (int i = 0; i < Teams.Count; i++)
        {
            Teams[i].Score = 0;
        }
    }

    public void ResetGameData()
    {
        gameData.NumberOfRounds++;
        gameData.currentRound++;
        gameData.currentActivePlayerID = 0;
        gameData.firstHandPlayer = 0;
        gameData.trumpCard = 0;
        gameData.minBetAount = 199;
        gameData.trumpBet = 0;
        gameData.trumpCalled = false;

    }

    public void ResetPlayerHand()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].Hand.Clear(); 
        }
    } 

    public void EndGame()
    {
        //check which team won most hands 
        int roundsWon=0;
        int leadingTeamIndex=0; 

        for (int i = 0; i < Teams.Count; i++)
        {
            int tempRoundsWon;
            tempRoundsWon = Teams[i].RoundsWon;
            if (tempRoundsWon>roundsWon)
            {
                roundsWon = tempRoundsWon;
                leadingTeamIndex = i; 
            }
        }

        gameData.teamWon = Teams[leadingTeamIndex].team; 
    }
}
