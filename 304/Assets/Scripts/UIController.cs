using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class UIController : MonoBehaviour
{
    public GameManager gameManager;
    public PhotonManager photonManager;

    public InputField playerName;
    public Button joinRoomButton;

    public Button BetButton;
    public Button PassButton;
    public InputField BetAmountInputText;

    public Text BetAmountText; 
    public Text TrumpTeamText; 
    public Text TrumpSuiteText;
    public Image TrumpCardImage; 

    public Text playerIDText; 
    public Text playerNameText;
    public Text playerTeamText;

    public Text AScoreText; 
    public Text BScoreText; 

    public GameObject ShuffleButton;  
    public GameObject DistributeButton;

    public List<Image> teamARound; 
    public List<Image> teamBRound;
    public int aRoundCount; 
    public int bRoundCount;

    public Text endText; 

    //Panels
    public GameObject LobbyPanel;
    public GameObject JoinRoomPanel; 
    public GameObject WaitForAllPlayers; 
    public GameObject ShuffleandDistributePanel;
    public GameObject WaitForShufflePanel; 

    public GameObject HUDPlayerDataPanel; 

    public GameObject GameRoomPanel;
    public GameObject BettingPanel;
    public GameObject WaitingPanel;
    public GameObject TrumpPanel;

    public GameObject EndGamePanel;


    public Sprite defaultCardSprite; 
    public Sprite UIMaskSprite; 

    public List<Image> hand;
    public List<Button> handButton;

    public List<Image> activeHand;
    public List<Text> playerTags; 


    private int BetAmount; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnValueChange()
    {
        if (playerName.text.Length >= 3)
        {
            joinRoomButton.interactable = true;
        }
        else
        {
            joinRoomButton.interactable = false;
        }

    }


    //Testing Core Game Functions 
    public void AddPlayer()
    {

        if (gameManager.gameData.NumberOfPlayers < 4)
        {
            gameManager.InitiatePlayerData(playerName.text,0);
            playerName.text = "";
        }
    } 

    public void JoinRoom()
    {
        //join a room with a username 
        //initiate player data from game manager 
        photonManager.JoinARoom(playerName.text);
        playerName.text = "";

    }

    public void EnableShuffleandDistributeUI()
    {
        int playerID = gameManager.gameData.LocalPlayerID;
        int firstPlayerID = gameManager.RoundFirstPlayer; 
        if (playerID==firstPlayerID)
        {
            PanelActivation(JoinRoomPanel, false);
            SwitchPanels(WaitForAllPlayers, ShuffleandDistributePanel);
        }
        else
        {
            PanelActivation(JoinRoomPanel, false);
            SwitchPanels(WaitForAllPlayers, WaitForShufflePanel);
        }
    }

    public void CheckForAllPlayers()
    {
        if (gameManager.Players.Count>3)
        {
            photonManager.StartTurn();
            EnableShuffleandDistributeUI();
            UpdatePlayerTagsUI();
        }
        else
        {
            SwitchPanels(JoinRoomPanel, WaitForAllPlayers);
        }
    }



    public void RestartRoundtoShuffle()
    {
        //reset HUD 
        ResetHUDData(); 

        //switch from game room to lobby 
        SwitchPanels(GameRoomPanel, LobbyPanel);

        //switch from distribute to shuffle 
        SwitchPanels(DistributeButton, ShuffleButton);

        //deactivate all panels 
        PanelActivation(WaitForAllPlayers, false);
        PanelActivation(WaitForShufflePanel, false);
        PanelActivation(ShuffleandDistributePanel, false);
        EnableShuffleandDistributeUI();
    }

    public void ResetHUDData()
    {
        BetAmountText.text = "n/a";
        TrumpTeamText.text = "n/a";
        TrumpSuiteText.text = "not revealed";

        TrumpCardImage.sprite = UIMaskSprite;
    }

    public void ShuffleCards()
    {
        gameManager.ShuffleCards();
    }

    public void EnableDistributeUI()
    {
        SwitchPanels(ShuffleButton, DistributeButton); 
    }

    public void Distribute()
    {
        photonManager.DistributeCards(); 
        gameManager.DistributeCards();
        SwitchPanels(LobbyPanel, GameRoomPanel);
        AssignSpritesToHand();
        PanelActivation(BettingPanel,true);
    } 

    public void OnDistributeOtherPlayers()
    {
        SwitchPanels(LobbyPanel, GameRoomPanel);
        PanelActivation(WaitingPanel, true);
        AssignSpritesToHand(); 
    }

    public void SwitchPanels(GameObject PanelFrom, GameObject PanelTo)
    {
        PanelFrom.SetActive(false);
        PanelTo.SetActive(true); 
    }

    public void PanelActivation(GameObject panel, bool isActive)
    {
        panel.SetActive(isActive); 
    }

    public void EnableBettingPanel()
    {
        SwitchPanels(WaitingPanel, BettingPanel); 
    }

    public void AssignSpritesToHand()
    {
        int localPlayerID = gameManager.gameData.LocalPlayerID;
        PlayerData player = gameManager.GetLocalPlayerData(localPlayerID);
        bool trumpCalled = gameManager.isTrumpCalled(); 

        int count = hand.Count / 2;
        int startIndex = 0; 
        if (gameManager.gameData.trumpCalled)
        {
            Debug.Log("second hand dealt");
            startIndex = hand.Count / 2;
        }
       

        for (int i = 0; i < count; i++)
        {
            int cardindex = player.Hand[i + startIndex];
            CardData card = gameManager.Cards[cardindex];
            Sprite cardSprite = card.cardSprite;
            hand[i + startIndex].sprite = cardSprite; 
        }
    }

    public void ClearPlayerHandSprites()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].sprite = UIMaskSprite;
        }
    }

    public void BetCheck()
    {
        BetAmount = int.Parse(BetAmountInputText.text);
        int minBetAmount = gameManager.gameData.minBetAount; 
        if (BetAmount > minBetAmount && BetAmount<305)
        {
            HandInteractable(true); 

        }

    }

    public void ActivateBettingPanel()
    {
        PanelActivation(TrumpPanel, true);

        int localPlayerID = gameManager.gameData.LocalPlayerID;
        int roundFirstPlayerID = gameManager.RoundFirstPlayer;
        if (localPlayerID == roundFirstPlayerID)
        {
            //PanelActivation(BettingPanel, true);
            SwitchPanels(WaitingPanel, BettingPanel);
        }
        else
        {
            //PanelActivation(WaitingPanel, true);
            SwitchPanels(BettingPanel, WaitingPanel);

        }
    }

    public void HandInteractable(bool isInteractable)
    {
        int count = handButton.Count;
        if (!gameManager.RoundStarted)
        {
            count = handButton.Count / 2; 
        }

        for (int i = 0; i < count; i++)
        {
            int cardIndex = gameManager.Players[gameManager.gameData.LocalPlayerID].Hand[i];
            if (cardIndex < 0)
            {
                handButton[i].interactable = false;
            }
            else
            {
                handButton[i].interactable = isInteractable;
            }
        }
    }

    public void HandFunctions(int handIndex)
    {
        int localPlayerID = gameManager.gameData.LocalPlayerID;

        if (!gameManager.RoundStarted)
        {
            PickTrumpCard(handIndex); 
        }
        else
        {
            //PickPlayCard(handIndex);
            CheckForSuiteinHand(handIndex);
        }
    }

    public void PickTrumpCard(int handIndex)
    {
        int cardIndex = gameManager.Players[gameManager.gameData.LocalPlayerID].Hand[handIndex];

        gameManager.PickATrumpCard(cardIndex, gameManager.gameData.LocalPlayerID);
        BetButton.interactable = true; 
    }


    public void UpdateBetDataUI()
    {
        int trumplayer = gameManager.RoundTrumpPlayer; 
        BetAmountText.text = gameManager.gameData.trumpBet.ToString();
        TrumpTeamText.text = gameManager.Players[trumplayer].PlayerName;

        if (gameManager.RoundTrumpRevealed)
        {
            TrumpSuiteText.text = gameManager.gameData.trumpSuite.ToString();
        }
        else
        {
            TrumpSuiteText.text = "not revealed";
        }

    }

    public void PlaceBet()
    {
        HandInteractable(false);

        //gamemngr callbet 
        //send bet amount 
        gameManager.CallBet(BetAmount);
        photonManager.PlacedCard(gameManager.gameData.trumpCard);
        UpdateBetDataUI(); 

    }

    public void CallOnNextPlayerTurn(bool isPass)
    {
        //check if you're the trump caller (player 0), then the chance goes to your opponent (player 1)  

        //if you're player 1 (trump callers oppponent) then the chance goes to your opponent (player 2) 

        //if you're player 2 (trump callers partner) then the chance goes to your opponent (player 3) 

        //if you're player 3 (trump callers oppponent), then this is taken as the final starting bet for the round 

        int localPlayerID = gameManager.gameData.LocalPlayerID;

        //normalize the ID 
        int normalizedPlayerID = NormalizePlayerID(localPlayerID);

        int nextPlayerID = normalizedPlayerID + 1;


        if (isPass)
        {
            switch (normalizedPlayerID)
            {
                case 0:
                    nextPlayerID = 2;

                    SwitchPanels(BettingPanel, WaitingPanel);
                    photonManager.CallOnTheNextPlayer(nextPlayerID);
                    break;
                case 1:
                    nextPlayerID = 3;
                    SwitchPanels(BettingPanel, WaitingPanel);
                    photonManager.CallOnTheNextPlayer(nextPlayerID);
                    break;
                //case 2:

                //    photonManager.CallOnTheNextPlayer(nextPlayerID);
                //    break;
                case 3:
                    nextPlayerID = 1;
                    SwitchPanels(BettingPanel, WaitingPanel);
                    photonManager.CallOnTheNextPlayer(nextPlayerID);
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (nextPlayerID > 3)
            {
                nextPlayerID = 0;
            }
            

            SwitchPanels(BettingPanel, WaitingPanel);
            photonManager.CallOnTheNextPlayer(nextPlayerID);
        }

    }


    public void PassBet()
    {
        //check if you're the trump caller (player 0), then the chance goes to your partner (player 2)  

        //if you're player 1 (trump callers oppponent) then you can pass to your partner (player 3) if they have never gotten a chance
        //if your partner (player 3) has already gotten a chance then the round starts with the last trump called 

        //if you're the trump callers partner (player 2), you cannot pass you must bet, then the chance to bet goes to oppponent player 3 

        //if you're player 3 (trump callers oppponent), you can pass to your partner (player 1) if they have never gotten a chance
        //if your partner (player 1) has already gotten a chance then the round starts with the last trump called  
        photonManager.PlacedCard(-1);
        HandInteractable(false);
        gameManager.PassUpdate(gameManager.gameData.LocalPlayerID);
    }

    public void EnablePassButton()
    {
        int localPlayerID = gameManager.gameData.LocalPlayerID;
        int normalizedPlayerID = NormalizePlayerID(localPlayerID);

        int teamPlayerID = 0;
        teamPlayerID = gameManager.Players[localPlayerID].PartnerID;

        switch (normalizedPlayerID)
        {
            case 0:
                PassButton.interactable = true; 
                break;
            case 1:
                //teamPlayerID = 3;
                //if (gameManager.Players[teamPlayerID].hasBet || gameManager.Players[teamPlayerID].hasPassed)
                //{
                //    PassButton.interactable = false;
                //}
                //else
                //    PassButton.interactable = true;

                PassButton.interactable = true;
                break;
            case 2:
                //teamPlayerID = 0;
                if (gameManager.Players[teamPlayerID].hasPassed)
                {
                    PassButton.interactable = false;
                }
                else if (gameManager.Players[teamPlayerID].hasBet)
                {
                    PassButton.interactable = true;
                }

                break;
            case 3:
                //teamPlayerID = 1;
                //if (gameManager.Players[teamPlayerID].hasBet || gameManager.Players[teamPlayerID].hasPassed)
                //{
                //    PassButton.interactable = false;
                //}
                //else
                //    PassButton.interactable = true; 

                PassButton.interactable = true;
                break;
            default:
                break;
        }
    }

    public void StartTurn()
    {
        photonManager.StartTurn(); 
    }  

    public void EndTurn()
    {
        PanelActivation(TrumpPanel, false);
    }



    public void OnJoinedRoomUpdatePlayerDataUI(int playerID)
    {
        playerIDText.text = playerID.ToString();
        playerNameText.text = gameManager.Players[playerID].PlayerName;
        playerTeamText.text = gameManager.Players[playerID].team.ToString(); 

        PanelActivation(HUDPlayerDataPanel, true); 
    } 

    public void ActivateCurrentPlayerHand()
    {
        int localPlayerID = gameManager.gameData.LocalPlayerID;
        int currentActivePlayerID = gameManager.gameData.currentActivePlayerID; 

        if (localPlayerID==currentActivePlayerID)
        {
            HandInteractable(true); 
        }
        else
        {
            HandInteractable(false); 
        }
    }

    public void CheckForSuiteinHand(int handIndex)
    {
        int localPlayerID = gameManager.gameData.LocalPlayerID;
        int currentActivePlayerID = gameManager.gameData.currentActivePlayerID;
        List<int> hand = gameManager.Players[localPlayerID].Hand;
        int cardIndex = hand[handIndex];
        bool isCurrentSuiteinHand = false;
        Suite currentHandSuite = gameManager.gameData.currentHandSuite;

        Debug.Log("First hand player: " + gameManager.gameData.firstHandPlayer.ToString()); 
        Debug.Log("Local player ID: " + localPlayerID.ToString()); 
        Debug.Log("Round Trump player ID: " + gameManager.RoundTrumpPlayer.ToString()); 

        //check if its the first card of the active hand
        if (localPlayerID == gameManager.gameData.firstHandPlayer)
        {
            //if first player is trump player 
            //if card playing is same suite as trump suite, then it has to be the trump card 
            //else if different suite any card can be played 
            if (localPlayerID == gameManager.RoundTrumpPlayer)
            {
                if (!gameManager.RoundTrumpRevealed)
                {
                    if (gameManager.Cards[cardIndex].suite == gameManager.gameData.trumpSuite)
                    {
                        if (cardIndex == gameManager.gameData.trumpCard)
                        {
                            //trump revealed 
                            photonManager.TrumpRevealed();
                            PickPlayCard(localPlayerID, cardIndex, handIndex);
                        }
                    }
                    else
                    {
                        PickPlayCard(localPlayerID, cardIndex, handIndex);
                    }
                }
                else
                {
                    PickPlayCard(localPlayerID, cardIndex, handIndex);
                }
            }
            else
            {
                PickPlayCard(localPlayerID, cardIndex, handIndex);

            }
        }
        else
        {
            isCurrentSuiteinHand = CheckHand(currentHandSuite);

            //if player is trump player
            if (localPlayerID == gameManager.RoundTrumpPlayer)
            {
                //if suite is in hand then play that 
                if (!isCurrentSuiteinHand)
                {
                    if (!gameManager.RoundTrumpRevealed)
                    {
                        if (gameManager.Cards[cardIndex].suite == gameManager.gameData.trumpSuite)
                        {
                            if (cardIndex == gameManager.gameData.trumpCard)
                            {
                                //trump revealed 
                                photonManager.TrumpRevealed();
                                PickPlayCard(localPlayerID, cardIndex, handIndex);
                            }
                        }
                        else
                        {
                            PickPlayCard(localPlayerID, cardIndex, handIndex);
                        }
                    }
                    else
                    {
                        PickPlayCard(localPlayerID, cardIndex, handIndex);
                    }

                }
                else
                {
                    //if the current suite is the trump suite
                    if (gameManager.Cards[cardIndex].suite == gameManager.gameData.trumpSuite)
                    {
                        //if trump is not revealed, the card has to be the trump card
                        //else 
                        if (!gameManager.RoundTrumpRevealed)
                        {
                            if (cardIndex == gameManager.gameData.trumpCard)
                            {
                                //trump revealed 
                                photonManager.TrumpRevealed();
                                PickPlayCard(localPlayerID, cardIndex, handIndex);
                            }
                        }
                        else
                        {
                            //if (cardIndex != gameManager.gameData.trumpCard)
                            //{
                            //    //trump revealed 
                            //    //photonManager.TrumpRevealed();
                                PickPlayCard(localPlayerID, cardIndex, handIndex);
                            //}
                        }
                        
                    }
                    else if (gameManager.Cards[cardIndex].suite == currentHandSuite)
                    {
                        PickPlayCard(localPlayerID, cardIndex, handIndex);
                    }
                }
                //else play the trump card 
            }
            //else if not the trump player 
            else
            {
                //if there are then the card has to be of the same suite 
                //else any suite of card would do 
                if (!isCurrentSuiteinHand)
                {
                    //check if the player cut the hand 
                    if (gameManager.Cards[cardIndex].suite == gameManager.gameData.trumpSuite)
                    {
                        photonManager.TrumpRevealed();
                    }
                    PickPlayCard(localPlayerID, cardIndex, handIndex);
                }
                else
                {
                    if (gameManager.Cards[cardIndex].suite == currentHandSuite)
                    {
                        PickPlayCard(localPlayerID, cardIndex, handIndex);
                    }
                }
            }

        }
    }

    public bool CheckHand(Suite cardSuite)
    {
        bool isCardSuiteinHand = false;
        int localPlayerID = gameManager.gameData.LocalPlayerID;
        List<int> hand = gameManager.Players[localPlayerID].Hand;

        //check if there are cards of the same suite as the first card of the hand

        for (int i = 0; i < hand.Count; i++)
        {
            int tempCardIndex = hand[i];
            if (tempCardIndex>0)
            {
                if (gameManager.Cards[tempCardIndex].suite == cardSuite)
                {
                    isCardSuiteinHand = true;
                }
            }
        }
        return isCardSuiteinHand;
    }

    public void OnTrumpRevealed()
    {
        int trumpCard = gameManager.gameData.trumpCard; 
        TrumpSuiteText.text = gameManager.gameData.trumpSuite.ToString() + " " + gameManager.Cards[trumpCard].number.ToString();
        TrumpCardImage.sprite = gameManager.Cards[trumpCard].cardSprite;
    }

    public void PickPlayCard(int localPlayerID, int cardIndex, int handIndex)
    {
        //int localPlayerID = gameManager.gameData.LocalPlayerID;
        //int cardIndex = gameManager.Players[gameManager.gameData.LocalPlayerID].Hand[handIndex];

        //check if its the first card of the
        gameManager.AddCardtoActiveHand(localPlayerID, cardIndex);

        //else 
        //check if there are cards of the same suite as the first card of the hand
        //if there are then the card has to be of the same suite 
        //else any suite of card would do 

        //remove card from players hand by assigning '-1' 
        gameManager.Players[gameManager.gameData.LocalPlayerID].Hand[handIndex] = -1;

        hand[handIndex].sprite = null;
        handButton[handIndex].interactable = false;

        UpdateActiveHandUI(localPlayerID, cardIndex); 

        photonManager.PlayCard(cardIndex);
    }

    public void UpdatePlayerTagsUI()
    {
        int localPlayerID = gameManager.gameData.LocalPlayerID;
        int playerTagIndex;
        int playerID = 0; 

        for (int i = 0; i < gameManager.Players.Count; i++)
        {
            playerID = i;
            if (playerID >= localPlayerID)
            {
                playerTagIndex = playerID - localPlayerID;
            }
            else
            {
                playerTagIndex = activeHand.Count - (localPlayerID - playerID);
            }

            playerTags[playerTagIndex].text = gameManager.Players[i].PlayerName; 
        }


    }
    public void UpdateActiveHandUI(int playerID, int cardIndex)
    {
        int localPlayerID = gameManager.gameData.LocalPlayerID;
        int activeHandIndex;
        int offset = activeHand.Count - localPlayerID;

        if (playerID>= localPlayerID)
        {
            activeHandIndex = playerID - localPlayerID; 
        }
        else
        {
            activeHandIndex = activeHand.Count - (localPlayerID - playerID); 
        }

        //if localplayer is trumpcaller all cards are revealed 
        if (localPlayerID==gameManager.RoundTrumpPlayer)
        {
            activeHand[activeHandIndex].sprite = gameManager.Cards[cardIndex].cardSprite;

        }
        else if (localPlayerID==playerID)
        {
            activeHand[activeHandIndex].sprite = gameManager.Cards[cardIndex].cardSprite;
        }
        else
        {
            //if trump revealed all cards are revealed 
            if (gameManager.RoundTrumpRevealed)
            {
                activeHand[activeHandIndex].sprite = gameManager.Cards[cardIndex].cardSprite;
            }
            else
            {
                //else only cards of the same suite are revealed 
                //assign revealed card to active hand slot 
                Suite currentHandSuite = gameManager.gameData.currentHandSuite;
                if (gameManager.Cards[cardIndex].suite == currentHandSuite)
                {
                    activeHand[activeHandIndex].sprite = gameManager.Cards[cardIndex].cardSprite;
                }
                else
                {
                    activeHand[activeHandIndex].sprite = defaultCardSprite; 
                }
            }
        }
    }

    public int NormalizePlayerID(int playerID)
    {
        int normalizedID;

        int firstPlayerID = gameManager.RoundFirstPlayer; 

        if (playerID >= firstPlayerID)
        {
            normalizedID = playerID - firstPlayerID;
        }
        else
        {
            normalizedID = activeHand.Count - (firstPlayerID - playerID);
        }
        return normalizedID; 
    }

    public void UpdateScoreUI()
    {
        AScoreText.text = gameManager.Teams[0].Score.ToString(); 
        BScoreText.text = gameManager.Teams[1].Score.ToString(); 
    }

    public void ClearActiveHandUI()
    {
        for (int i = 0; i < activeHand.Count; i++)
        {
            activeHand[i].sprite = UIMaskSprite; 
        }
    }

    public void UpdateRoundWonUI()
    {
        Team leadingTeam = gameManager.GetLeadingTeam(); 

        if (leadingTeam == Team.A)
        {
            teamARound[aRoundCount].color = Color.red;
            if (aRoundCount < 5)
            {
                aRoundCount++;
            }
            else
                aRoundCount = 0;
        }
        else if (leadingTeam == Team.B)
        {
            teamBRound[bRoundCount].color = Color.red;
            if (bRoundCount < 5)
            {
                bRoundCount++;
            }
            else
                bRoundCount = 0;
        }

    }

    public void GameEndUI()
    {
        endText.text = "Team " + gameManager.gameData.teamWon.ToString() + " Won!"; 
        PanelActivation(EndGamePanel, true); 

    }


}
