using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class PhotonManager : PunBehaviour, IPunTurnManagerCallbacks
{
    public GameData gameData;

    [SerializeField]
    private string VersionName = "0.1";

    [SerializeField]
    private PunTurnManager turnManager;
    [SerializeField]
    private UIController uiController;
    [SerializeField]
    private GameManager gameManager; 

    private bool IsShowingResults;


    //Player Data 
    public int playerID;
    public string playerName;
    public bool isBet =false;
    public bool isPass= false; 

    //Turn Data
    private int shuffleTurn; 
    private int distributeTurn; 
    private int pickTrumpTurn; 
    private int placeBetTurn; 
    private int callNextPlayerTurn;

    public int numberofplayerbetsorpass; 
    public int currentHandPlayerCount;
    public int currentActivePlayerID; 

    //trump data 
    private PhotonPlayer currentTrumpPlayer; 
    private PhotonPlayer currentActivePlayer;
    public bool isTrumpRevealed; 

    private void Awake()
    {

    }

    private void Start()
    {
        this.turnManager.TurnManagerListener = this; 

    }

    private void Update()
    {
        if (!PhotonNetwork.inRoom)
        {
            return;
        }

        ////if (PhotonNetwork.room.PlayerCount > 0)
        ////{
        //    if (this.turnManager.IsOver)
        //    {
        //        return;
        //    }
        //    if (turnManager.Turn > 0)
        //    {
        //        Debug.Log("Turn : " + turnManager.Turn.ToString());
        //    }
        //    else
        //    {
        //        Debug.Log("no turn yet");
        //    }
        ////}

    }

    public void JoinARoom(string playerName)
    {
        this.playerName = playerName;
        PhotonNetwork.player.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings(VersionName);

    }

    public void CreateARoom()
    {

    }

    public void StartTurn()
    {
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("Is Master Client");
            this.turnManager.BeginTurn();
        }
    }

    #region Player Moves

 

    public void ShuffledCard(int shuffledCardIndex, bool isEndTurn)
    {
        this.turnManager.SendMove((byte)shuffledCardIndex, isEndTurn);

    } 

    public void DistributeCards()
    {
        int empty = 1; 
        this.turnManager.SendMove((byte)empty, true);
    }

    public void PlacedCard(int cardIndex)
    {
        this.turnManager.SendMove((byte)cardIndex, true);
    }

    public void PlacedBetAmount(int BetAmount)
    {
        this.turnManager.SendMove((byte)BetAmount, true);
    }

    public void CallOnTheNextPlayer(int NextPlayerID)
    {
        this.turnManager.SendMove((byte)NextPlayerID, true);
    }

    public void EndTurn()
    {
        int empty = 0;
        this.turnManager.SendMove((byte)empty, true); 
    }

    public void UpdateNumberOfPlayerBets(int numberOfPlayersBet)
    {
        this.turnManager.SendMove((byte)numberOfPlayersBet, true);
    }

    public void PlayCard(int CardIndex)
    {
        this.turnManager.SendMove((byte)CardIndex, true);
    }

    public void TrumpRevealed()
    {
        int empty = 0;
        this.turnManager.SendMove((byte)empty, false);
    }
    #endregion 

    #region Photon Callbacks 
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        //PhotonNetwork.JoinRandomRoom();
        Debug.Log("Connected!");


    }

    public override void OnJoinedLobby()
    {
        if (!PhotonNetwork.InRoom)
        {
            RoomOptions roomOptions = new RoomOptions() { isVisible = true, maxPlayers = 6 };
            PhotonNetwork.JoinOrCreateRoom("testRoom", roomOptions, TypedLobby.Default);
            //PhotonNetwork.JoinRandomRoom();

        }
    }

    public void SetUsername(string Username)
    {
        PhotonNetwork.playerName = Username;
    }

    /// <summary>Called when a turn begins (Master Client set a new Turn number).</summary>
    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins() turn: " + turn); 
        IsShowingResults = false;

        switch (turn)
        {
            case 2:
                //enable distribute UI 
                gameManager.gameData.currentHandSuite = Suite.None;
                ResetTrumpVariables();
                uiController.EnableDistributeUI(); 
                break; 
            case 3:
                uiController.EnablePassButton();
                break;
            //send the placed bet amount, automatically at the start of the turn and end the turn 
            case 4:
                if (PhotonNetwork.player==currentTrumpPlayer)
                {
                    if (isBet)
                    {
                        PlacedBetAmount(gameManager.gameData.trumpBet);
                    }
                    else if (isPass)
                    {
                        PhotonNetwork.room.SetTurn(5); 
                    }
                }
                break;
            //update the global variable of how many players bet 
            case 5:
                if (PhotonNetwork.player == currentTrumpPlayer)
                {
                    numberofplayerbetsorpass++;
                    gameManager.RoundNumberPlayerBetorPassed=numberofplayerbetsorpass;
                    UpdateNumberOfPlayerBets(numberofplayerbetsorpass);
                }
                break;
            case 6:
                if (PhotonNetwork.player == currentTrumpPlayer)
                {
                    //check if its the end of the turn or call next player 
                    CheckEndPlayer(playerID); 
                    //uiController.CallOnNextPlayerTurn(isPass); 
                }
                break; 
            //reset the turn back to the picktrump turn
            case 7:
                if (PhotonNetwork.isMasterClient)
                {
                    PhotonNetwork.room.SetTurn(3);
                }
                break;
            case 8:
                uiController.EndTurn();

                //if player is the trump player reveal the trump card in ui 
                if (playerID==gameManager.RoundTrumpPlayer)
                {
                    uiController.OnTrumpRevealed(); 
                }

                //assign the rest of the sprites to hand 
                gameManager.RoundStarted = true;
                gameManager.DistributeCards();
                uiController.AssignSpritesToHand();
                if (PhotonNetwork.isMasterClient)
                {
                    PhotonNetwork.room.SetTurn(9);
                }

                break;
            case 9:
                currentHandPlayerCount++;
                uiController.ActivateCurrentPlayerHand();

                break;
            case 10:
                if (currentActivePlayerID==3)
                {
                    currentActivePlayerID = 0;
                }
                else
                {
                    currentActivePlayerID++;
                }
                gameManager.gameData.currentActivePlayerID = currentActivePlayerID; 

                if (PhotonNetwork.player==currentActivePlayer)
                {
                    if (currentHandPlayerCount > 3)
                    {
                        currentHandPlayerCount = 0;
                        currentActivePlayerID = 0;
                        PhotonNetwork.room.SetTurn(11);
                    }
                    else
                    {
                        PhotonNetwork.room.SetTurn(9);
                    }
                }
                break;
            case 11:
                currentHandPlayerCount = 0;
                currentActivePlayerID = 0;
                //gameManager.gameData.currentActivePlayerID = currentActivePlayerID; 

                gameManager.OnHandEnd();

                //check if the trump is revealed from the previous round and update
                if (isTrumpRevealed && !gameManager.RoundTrumpRevealed)
                {
                    gameManager.UpdateRoundTrumpRevealed(isTrumpRevealed);
                    uiController.OnTrumpRevealed(); 
                }
                uiController.UpdateScoreUI();
                uiController.ClearActiveHandUI();

                //check if current round is less than (number of rounds -1) 
                if (gameManager.gameData.currentRound<(gameManager.gameData.MaxNumberOfRounds-1))
                {
                    //if the round has more hands the turn resets to turn 9 
                    if (gameManager.RoundCurrentHand < gameManager.gameData.NumberofHands)
                    {
                        if (PhotonNetwork.isMasterClient)
                        {
                            PhotonNetwork.room.SetTurn(9);
                        }
                    }
                    else
                    {
                        //round is over 
                        //update round data 
                        gameManager.UpdateTeamWon();
                        uiController.UpdateRoundWonUI();
                        gameManager.ResetTeamScore();
                        uiController.UpdateScoreUI();

                        gameManager.UpdateRound();
                        gameManager.UpdatePlayerData();
                        uiController.ClearPlayerHandSprites();
                        //set room to shuffle 
                        uiController.RestartRoundtoShuffle();
                        uiController.ActivateBettingPanel();
                        if (PhotonNetwork.isMasterClient)
                        {
                            PhotonNetwork.room.SetTurn(1);
                        }
                    }
                }
                //else ends the game and announce final winner 
                else
                {
                    if (PhotonNetwork.isMasterClient)
                    {
                        PhotonNetwork.room.SetTurn(12);
                    }
                }
                break;
            case 12:

                gameManager.EndGame();
                uiController.GameEndUI(); 

                break; 
            default:
                break;
        }
    }

    public void OnTurnCompleted(int obj)
    {
        Debug.Log("OnTurnCompleted: " + obj);

    }

    // when a player moved (but did not finish the turn)
    public void OnPlayerMove(PhotonPlayer photonPlayer, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + photonPlayer + " turn: " + turn + " action: " + move);

        switch (turn)
        {
            case 1:
                Debug.Log("Shuffle");
                //if this is not master client add these to the shuffled list 

                if (!photonPlayer.IsLocal)
                {
                    Debug.Log("Check Photo Player ID: " + photonPlayer.ID); 
                    gameManager.AddToShuffledCardsFromMasterClient((int)(byte)move);
                }
                else
                {
                    if (gameManager.shuffledDone)
                    {
                        PhotonNetwork.room.SetTurn(2);
                    }
                }
                break;

            case 2:
                break;

            case 3:
                break;

            case 4:
                break;
            case 9:
                //trump revealed 
                isTrumpRevealed = true; 
                break; 
            default:
                break;
        }
    }


    //when a player made the last/final move in a turn
    public void OnPlayerFinished(PhotonPlayer photonPlayer, int turn, object move)
    {
        Debug.Log("OnTurnFinished: " + photonPlayer + " turn: " + turn + " action: " + move);
        switch (turn)
        {
            case 2:
                distributeTurn = turn; 
                if (!photonPlayer.IsLocal)
                {
                    gameManager.DistributeCards();
                    uiController.OnDistributeOtherPlayers();
                    Debug.Log("Distribute");
                }
                break;
            case 3:
                pickTrumpTurn = turn;
                currentTrumpPlayer = photonPlayer;
                int cardIndex = (int)(byte)move;

                //placing the trump card 
                if (!photonPlayer.IsLocal)
                {
                    //bet
                    if (cardIndex != 255)
                    {
                        //update the other players trump card variables 
                        Debug.Log("Player: " + photonPlayer.ID + " update trumpcard");
                        gameManager.PickATrumpCard((int)(byte)move,(photonPlayer.ID-1));
                    }
                    //pass
                    else
                    {
                        gameManager.PassUpdate(photonPlayer.ID - 1); 
                    }
                }
                else
                {
                    if (cardIndex != 255)
                    {
                        isBet = true; 
                    }
                    else
                    {
                        isPass = true;
                    }
                }
                break;
            case 4:
                if (!photonPlayer.IsLocal)
                {
                    //update the other players with the trumpt amount 
                    Debug.Log("Player: " + photonPlayer.ID + " update bet amount");
                    gameManager.CallBet((int)(byte)move);
                    uiController.UpdateBetDataUI(); 
                }
                break;
                //update the number of players bet 
            case 5:
                if (!photonPlayer.IsLocal)
                {
                    numberofplayerbetsorpass = (int)(byte)move;
                    gameManager.RoundNumberPlayerBetorPassed = numberofplayerbetsorpass;
                }
                break;
            case 6:
                //call next person or end turn if it's the last player 
                callNextPlayerTurn = turn; 
                int nextPlayerID = (int)(byte)move;
                //if (numberofplayerbetsorpass>3)
                //{
                //    //begin the game; card play 
                //    uiController.EndTurn(); 
                //}
                //else 
                int normalizedID = uiController.NormalizePlayerID(playerID); 
                if (normalizedID == nextPlayerID)
                {
                    Debug.Log("Next Player: " + playerID + " time to bet"); 
                    //enable the betting for the next player  
                    uiController.EnableBettingPanel();
                }
                break;
            case 8:
                
                break; 
            case 9:
                currentActivePlayer = photonPlayer;
                currentActivePlayerID = currentActivePlayer.ID - 1;
                int cardindex;
                if (currentHandPlayerCount == 1)
                {
                    cardindex = (int)(byte)move;
                    //update current hand suite 
                    gameManager.UpdateCurentHandSuite(cardindex);
                    gameManager.gameData.firstHandPlayer = (photonPlayer.ID - 1);
                }
                if (!photonPlayer.IsLocal)
                {
                    cardindex = (int)(byte)move;
                    int playerID = photonPlayer.ID - 1;
                    gameManager.AddCardtoActiveHand(playerID, cardindex);
                    uiController.UpdateActiveHandUI(playerID, cardindex); 
                }


                break; 
            default:
                break;
        }

        if (PhotonNetwork.isMasterClient)
        {
            this.turnManager.BeginTurn();
        }

    }

    public void OnTurnTimeEnds(int obj)
    {
        if (!IsShowingResults)
        {
            Debug.Log("OnTurnTimeEnds: Not Calling OnTurnCompleted");
            //OnTurnCompleted(-1);
        }
    }


    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2, PlayerTtl = 20000 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.room.Name);
        playerID = PhotonNetwork.player.ID-1;
        gameManager.gameData.LocalPlayerID = playerID; 
        Debug.Log("Player ID: " + PhotonNetwork.player.ID);
           
        uiController.playerIDText.text = playerID.ToString();

        if (PhotonNetwork.room.PlayerCount > 0)
        {
            int numberOfPlayers = PhotonNetwork.room.PlayerCount;
            Debug.Log("Number of players: " + numberOfPlayers);

            PhotonPlayer[] photonPlayers = PhotonNetwork.playerList; 

            for (int i = 0; i < photonPlayers.Length; i++)
            {
                int newPlayerID = photonPlayers[i].ID - 1; 
                gameManager.InitiatePlayerData(photonPlayers[i].NickName, newPlayerID);
            }
        }

        uiController.OnJoinedRoomUpdatePlayerDataUI(playerID);
        uiController.CheckForAllPlayers(); 
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        Debug.Log("Disconnected due to: " + cause);
    }

    public override void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
    {
        Debug.Log("OnPhotonPlayerActivityChanged() for " + otherPlayer.NickName + " IsInactive: " + otherPlayer.IsInactive);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        int newPlayerID = newPlayer.ID - 1;
        Debug.Log("Player Name: " + newPlayer.NickName);
        gameManager.InitiatePlayerData(newPlayer.NickName, newPlayerID);
        uiController.CheckForAllPlayers();

    }

    #endregion

    #region Function Blocks 
    public void CheckEndPlayer(int playerID)
    {
        int teamPlayerID;
        int nextPlayerID;

        int normalizedPlayerID = uiController.NormalizePlayerID(playerID);
        teamPlayerID = gameManager.Players[playerID].PartnerID;

        switch (normalizedPlayerID)
        {
            case 0:
                //call on next player 
                if (isPass)
                {
                    uiController.CallOnNextPlayerTurn(true); 
                }
                else if (isBet)
                {
                    uiController.CallOnNextPlayerTurn(false);

                }
                break;
            case 1:
                //teamPlayerID = 3;
                nextPlayerID = 2;
                nextPlayerID = uiController.NormalizePlayerID(nextPlayerID);

                if (isPass)
                {
                    if (gameManager.Players[teamPlayerID].hasPassed)
                    {
                        //end turn 
                        EndTrumpTurn(); 
                    }
                    else
                    {
                        //call on next player  
                        uiController.CallOnNextPlayerTurn(true);
                    }
                }
                else if (isBet)
                {
                    if (gameManager.Players[nextPlayerID].hasBet)
                    {
                        //end turn 
                        EndTrumpTurn(); 
                    }
                    else
                    {
                        //call on next player 
                        uiController.CallOnNextPlayerTurn(false);

                    }
                }
                break;
            case 2:
                //teamPlayerID = 0;
                nextPlayerID = 3;
                nextPlayerID = uiController.NormalizePlayerID(nextPlayerID);

                if (isPass)
                {
                    //end turn 
                    EndTrumpTurn(); 
                }
                else if (isBet)
                {
                    //call next player 
                    uiController.CallOnNextPlayerTurn(false);

                }
                break;
            case 3:
                //teamPlayerID = 1;
                nextPlayerID = 0;
                nextPlayerID = uiController.NormalizePlayerID(nextPlayerID);

                if (isPass)
                {
                    if (gameManager.Players[teamPlayerID].hasPassed)
                    {
                        //end turn 
                        EndTrumpTurn(); 
                    }
                    else
                    {
                        //call on next player  
                        uiController.CallOnNextPlayerTurn(true);

                    }
                }
                else if (isBet)
                {
                    //end turn 
                    EndTrumpTurn(); 
                }
                break;
            default:
                break;
        }
    }

    public void EndTrumpTurn()
    {
        PhotonNetwork.room.SetTurn(8);
    }

    public void ResetTrumpVariables()
    {
        isPass = false;
        isBet = false;
        isTrumpRevealed = false; 
    }
    #endregion 

}
