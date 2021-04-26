using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectFourEngine : MonoBehaviour
{
    int[,] Board = new int[8, 8];
    List<GameObject> TokenList = new List<GameObject>();

    public GameObject DeleteTokenButtonsParent;
    public GameObject DeleteTokenButtonPrefab;

    public GameObject TokenButtons;

    public GameObject TokenParent;
    public GameObject Player1Token;
    public GameObject Player2Token;

    private float TurnPauseTimer;
    private int CurrentPlayerTurn;

    public int PlayerPoints;
    public int OpponentPoints;

    public AudioClip CursedAudio;

    private List<int[]> TokenPosDestroyList = new List<int[]>();
    private float DestroyTimer;

    private int PlayerTokenCount;
    private int OpponentTokenCount;

    public GameObject PlayerPointsText;
    public GameObject OpponentPointsText;
    public TextMeshProUGUI Announcements;
    public TextMeshProUGUI RotationText;

    private int TurnsLeft = 100;
    private int RotationTurnsLeft = 8;
    private float RotateTimer;
    private int RotateAmount = 1;
    public GameObject TokenDoubleParent;
    private string RotateDirection = "Counter-Clockwise";

    private int Connect = 5;
    private int ConnectTurnsLeft;
    public GameObject Connect5;
    public GameObject Connect4;

    public GameObject EndScreen;
    public TextMeshProUGUI ScoreText;
    private float EndScreenTimer;

    // Start is called before the first frame update
    void Start()
    {
        EndScreen.SetActive(false);

        if (Random.Range(0, 80) == 0)
        {
            GetComponent<AudioSource>().clip = CursedAudio;
            GetComponent<AudioSource>().Play();
        }

        if(Random.Range(0, 2) == 0)
        {
            RotateDirection = "Clockwise";
        }

        ConnectTurnsLeft = Random.Range(9, 18);

        Connect5.SetActive(true);
        Connect4.SetActive(false);

        StartPlayerTurn();
    }
    public void UpdateScoreText()
    {
        PlayerPointsText.GetComponent<TextMeshProUGUI>().text = PlayerPoints.ToString();
        OpponentPointsText.GetComponent<TextMeshProUGUI>().text = OpponentPoints.ToString();
    }
    void StartPlayerTurn()
    {
        NextTurn();
        CurrentPlayerTurn = 1;
        TokenButtons.SetActive(true);

        for(int i = 0; i < TokenButtons.transform.childCount; i++)
        {
            if(Board[i, 7] == 0)
            {
                TokenButtons.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                TokenButtons.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    void NextTurn()
    {
        Announcements.color = new Color32(100, 100, 100, 255);
        Announcements.text = TurnsLeft + " turns left";
        TurnsLeft -= 1;

        if(RotationTurnsLeft <= 2)
        {
            RotationText.color = new Color32(255, 0, 0, 255);
        }
        else
        {
            RotationText.color = new Color32(100, 100, 100, 255);
        } 

        RotationText.text = "Board rotates in " + RotationTurnsLeft + " turns (x" + RotateAmount + ") " + RotateDirection;
        RotationTurnsLeft -= 1;

        Connect = 5;
        Connect4.SetActive(false);
        Connect5.SetActive(true);

        if (ConnectTurnsLeft <= 4)
        {
            Connect5.GetComponent<TextMeshProUGUI>().text = "Connect Five (" + ConnectTurnsLeft + ")";
        }
        else
        {
            Connect5.GetComponent<TextMeshProUGUI>().text = "Connect Five";
        }
        ConnectTurnsLeft -= 1;
    }
    public void PlayerAction(int Column)
    {
        GameObject Token = Instantiate(Player1Token);
        Token.transform.SetParent(TokenParent.transform);
        Token.transform.localScale = new Vector3(1, 1, 1);
        Token.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        Token.transform.localPosition = new Vector3(Column, 8.2f, 0);
        Token.GetComponent<ConnectFourTokenScript>().Player = 1;
        TokenList.Add(Token);

        Board[Column, 7] = 1;
        Board = GravityCheckBoard(Board);

        EndPlayerTurn(1f);
    }
    void EndPlayerTurn(float TurnPauseTime)
    {
        TokenButtons.SetActive(false);

        EndTurn(TurnPauseTime);
    }
    void AnnounceResults(int[] Results)
    {
        string PlayerPointWord = "point";
        string OpponentPointWord = "point";

        if (Results[0] > 1)
        {
            PlayerPointWord = "points";
        }
        if (Results[1] > 1)
        {
            OpponentPointWord = "points";
        }

        if (Results[0] > 0 && Results [1] > 0)
        {
            Announcements.color = new Color32(100, 100, 100, 255);
            Announcements.text = "You gained " + Results[0] + " " + PlayerPointWord + "\nThe opponent gained " + Results[1] + " " + OpponentPointWord;
        }
        else if (Results[0] > 0)
        {
            Announcements.color = new Color32(255, 136, 100, 255);
            Announcements.text = "You gained " + Results[0] + " " + PlayerPointWord;
        }
        else if (Results[1] > 0)
        {
            Announcements.color = new Color32(0, 120, 255, 255);
            Announcements.text = "The opponent gained " + Results[1] + " " + OpponentPointWord;
        }
    }
    void PlayerChooseToDestroy()
    {
        Announcements.color = new Color32(255, 136, 100, 255);
        Announcements.text = "Choose " + (PlayerTokenCount - OpponentTokenCount) + " spheres to destroy";

        DeleteTokenButtonsParent.SetActive(true);
        foreach(Transform Child in DeleteTokenButtonsParent.transform)
        {
            Destroy(Child.gameObject);
        }

        foreach(GameObject Token in TokenList)
        {
            Token.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            if(Token.GetComponent<ConnectFourTokenScript>().Player == 1)
            {
                int x = Token.GetComponent<ConnectFourTokenScript>().x;
                int y = Token.GetComponent<ConnectFourTokenScript>().y;

                GameObject DeleteTokenButton = Instantiate(DeleteTokenButtonPrefab);
                DeleteTokenButton.transform.SetParent(DeleteTokenButtonsParent.transform);
                DeleteTokenButton.transform.localScale = new Vector3(1, 1, 1);
                DeleteTokenButton.GetComponent<RectTransform>().anchoredPosition = new Vector3((x * 102.5f), (y * 102.5f));
                DeleteTokenButton.transform.localPosition = new Vector3((DeleteTokenButton.transform.localPosition.x), (DeleteTokenButton.transform.localPosition.y), -100);
                DeleteTokenButton.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                DeleteTokenButton.GetComponent<ConnectFourDeleteTokenButtonScript>().x = x;
                DeleteTokenButton.GetComponent<ConnectFourDeleteTokenButtonScript>().y = y;
            }
        }
    }
    public void PlayerChooseToDestroyOnClick(int x, int y, GameObject DeleteTokenButton)
    {
        foreach (GameObject Token in TokenList)
        {
            if(Token.GetComponent<ConnectFourTokenScript>().Player == 1 &&
               Token.GetComponent<ConnectFourTokenScript>().x == x &&
               Token.GetComponent<ConnectFourTokenScript>().y == y)
            {
                print("Destroying: " + x + ", " + y);
                TokenList.Remove(Token);
                Board[x, y] = 0;

                DestroyTokenObject(Token);
                break;
            }
        }

        Destroy(DeleteTokenButton);
        TokenCount();

        if((PlayerTokenCount - OpponentTokenCount) == 1)
        {
            Announcements.text = "Choose " + (PlayerTokenCount - OpponentTokenCount) + " sphere to destroy";
        } 
        else if((PlayerTokenCount - OpponentTokenCount) > 1)
        {
            Announcements.text = "Choose " + (PlayerTokenCount - OpponentTokenCount) + " spheres to destroy";
        }

        if (PlayerTokenCount <= OpponentTokenCount)
        {
            DeleteTokenButtonsParent.SetActive(false);
            Board = GravityCheckBoard(Board);

            foreach (GameObject Token in TokenList)
            {
                Token.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }

            if (CurrentPlayerTurn == 1)
            {
                EndPlayerTurn(1f);
            }
            else if (CurrentPlayerTurn == 2)
            {
                EndOpponentTurn(1f);
            }
        }
    }
    void DestroyTokenObject(GameObject Token)
    {
        Token.GetComponent<ConnectFourTokenScript>().BreakParticles();
        Destroy(Token);
    }
    void StartOpponentTurn()
    {
        NextTurn();
        CurrentPlayerTurn = 2;
        OpponentAction();
    }
    public void OpponentAction()
    {
        int RandomMove()
        {
            int ReturnMove = Random.Range(0, 8);

            while(Board[ReturnMove, 7] != 0)
            {
                ReturnMove = Random.Range(0, 8);
            }

            return ReturnMove;
        }

        int Move = -1;
        for(int x = 0; x < 8; x++)
        {
            if(CheckWinningMove(x, 2, Connect) == true)
            {
                Move = x;
            }
        }

        if(Move == -1)
        {
            for (int x = 0; x < 8; x++)
            {
                if (CheckWinningMove(x, 1, Connect) == true)
                {
                    Move = x;
                }
            }
        }

        /*if (Move == -1)
        {
            for (int x = 0; x < 8; x++)
            {
                if (CheckWinningMove(x, 2, 4) == true)
                {
                    Move = x;
                }
            }
        }

        if (Move == -1)
        {
            for (int x = 0; x < 8; x++)
            {
                if (CheckWinningMove(x, 1, 4) == true)
                {
                    Move = x;
                }
            }
        }*/

        if (Move == -1)
        {
            Move = RandomMove();
            print("No strategy - Random move: " + Move);
        }
        else
        {
            print("Strategy used: " + Move);
        }

        GameObject Token = Instantiate(Player2Token);
        Token.transform.SetParent(TokenParent.transform);
        Token.transform.localScale = new Vector3(1, 1, 1);
        Token.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        Token.transform.localPosition = new Vector3(Move, 8.2f, 0);
        Token.GetComponent<ConnectFourTokenScript>().Player = 2;
        TokenList.Add(Token);

        Board[Move, 7] = 2;
        Board = GravityCheckBoard(Board);

        EndOpponentTurn(1f);
    }
    void EndOpponentTurn(float TurnPauseTime)
    {
        EndTurn(TurnPauseTime);
    }
    void EndTurn(float TurnPauseTime)
    {
        if (TurnsLeft > 0)
        {
            int[] Results = CheckWinningBoard(Board, true, Connect);
            if (Results[0] != 0 || Results[1] != 0)
            {
                PlayerPoints += Results[0];
                OpponentPoints += Results[1];

                AnnounceResults(Results);
            }
            else if (Results[0] == 0 && Results[1] == 0 && RotationTurnsLeft > 0 && ConnectTurnsLeft > 0)
            {
                TurnPauseTimer = TurnPauseTime;
            }
            else if (ConnectTurnsLeft <= 0)
            {
                ConnectChange();
            }
            else if (RotationTurnsLeft <= 0)
            {
                string PluralTimeWord = "Times";

                if (RotateAmount == 1)
                {
                    PluralTimeWord = "Time";
                }

                RotationText.text = "Rotating " + RotateAmount + " " + PluralTimeWord + "\n" + RotateDirection;
                RotateTimer = 3f;
            }
        }
        else
        {
            Announcements.text = "0 turns left";
            EndScreenTimer = 3f;
        }
    }
    void EndScreenCountDown()
    {
        if(EndScreenTimer > 0)
        {
            EndScreenTimer -= Time.deltaTime;
        }
        else if(EndScreenTimer < 0)
        {
            EndScreen.SetActive(true);
            var score = ScoreCalculation();
            var hackeronis = Mathf.RoundToInt(score * 3.1415f);
            ScoreText.text = string.Format("Score: {0}\n\n Hackeronis: {1}", score, hackeronis);
            
            PlayerPrefManager.AddEarnedHackeronis(hackeronis);
            EndScreenTimer = 0;
        }
    }
    int ScoreCalculation()
    {
        int ScoreReturn = 0;
        float ScoreRatio = (float)PlayerPoints / ((float)OpponentPoints + 0.1f);

        if(ScoreRatio >= 3f)
        {
            ScoreReturn = 100;
        }
        else if (ScoreRatio >= 2.5f)
        {
            ScoreReturn = 98;
        }
        else if (ScoreRatio >= 2f)
        {
            ScoreReturn = 95;
        }
        else if (ScoreRatio >= 1.5f)
        {
            ScoreReturn = 92;
        }
        else if (ScoreRatio >= 1f)
        {
            ScoreReturn = 88;
        }
        else
        {
            ScoreReturn = Mathf.RoundToInt(88f * ScoreRatio);
        }

        return ScoreReturn;
    }
    void ConnectChange()
    {
        Connect5.SetActive(false);
        Connect4.SetActive(true);
        Connect = 4;

        if (TurnsLeft <= 1)
        {
            ConnectTurnsLeft = 100;
        }
        else if (TurnsLeft - 32 <= 0)
        {
            ConnectTurnsLeft = TurnsLeft - 1;
        }
        else
        {
            ConnectTurnsLeft = Random.Range(16, 33);
        }

        if (CurrentPlayerTurn == 1)
        {
            EndPlayerTurn(1f);
        }
        else if (CurrentPlayerTurn == 2)
        {
            EndOpponentTurn(1f);
        }
    }
    void RotateTimerMethod()
    {
        if(RotateTimer > 0)
        {
            RotateTimer -= Time.deltaTime;

            if(RotateTimer <= 1f)
            {
                if (TokenDoubleParent.transform.localRotation == Quaternion.Euler(new Vector3(0, 0, 0)))
                {
                    foreach (GameObject Token in TokenList)
                    {
                        Token.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    }
                }

                if (RotateDirection == "Counter-Clockwise")
                {
                    TokenDoubleParent.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (90f * (1f - RotateTimer))));
                }
                else if(RotateDirection == "Clockwise")
                {
                    TokenDoubleParent.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (-90f * (1f - RotateTimer))));
                }
            }
        }
        else
        {
            TokenDoubleParent.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            foreach (GameObject Token in TokenList)
            {
                Token.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }

            RotateTimer = 0;
            RotateAmount -= 1;

            RotateBoard();
            Board = GravityCheckBoard(Board);

            if (RotateAmount <= 0)
            {
                if(TurnsLeft <= 1)
                {
                    RotationTurnsLeft = 100;
                    TurnsLeft = 0;
                }
                else if (TurnsLeft - 32 <= 0)
                {
                    RotationTurnsLeft = TurnsLeft - 1;
                    RotateAmount = 4;
                }
                else
                {
                    RotationTurnsLeft = Random.Range(12, 33);

                    int RotateAmountRandomRange = Random.Range(0, 100);
                    if (RotateAmountRandomRange < 60)
                    {
                        RotateAmount = 1;
                    }
                    else if (RotateAmountRandomRange < 90)
                    {
                        RotateAmount = 2;
                    }
                    else if (RotateAmountRandomRange < 100)
                    {
                        RotateAmount = 3;
                    }
                }

                if (Random.Range(0, 2) == 0)
                {
                    RotateDirection = "Clockwise";
                }
                else
                {
                    RotateDirection = "Counter-Clockwise";
                }
            }

            if (CurrentPlayerTurn == 1)
            {
                EndPlayerTurn(2f);
            }
            else if (CurrentPlayerTurn == 2)
            {
                EndOpponentTurn(2f);
            }
        }
    }
    void OpponentChooseToDestroy()
    {
        Announcements.color = new Color32(0, 120, 255, 255);
        Announcements.text = "Opponent chose " + (OpponentTokenCount - PlayerTokenCount) + " spheres to destroy";

        int DestroyAmount = OpponentTokenCount - PlayerTokenCount;
        List<GameObject> OpponentTokenList = new List<GameObject>();

        foreach (GameObject Token in TokenList)
        {
            if(Token.GetComponent<ConnectFourTokenScript>().Player == 2)
            {
                OpponentTokenList.Add(Token);
            }
        }

        for(int i = 0; i < DestroyAmount; i++)
        {
            if(OpponentTokenList.Count > 0) { 
                GameObject Token = OpponentTokenList[Random.Range(0, OpponentTokenList.Count)];

                int x = Token.GetComponent<ConnectFourTokenScript>().x;
                int y = Token.GetComponent<ConnectFourTokenScript>().y;
                print("Destroying: " + x + ", " + y);
                OpponentTokenList.Remove(Token);
                TokenList.Remove(Token);
                Board[x, y] = 0;
                DestroyTokenObject(Token);
            }
        }

        Board = GravityCheckBoard(Board);

        if (CurrentPlayerTurn == 1)
        {
            EndPlayerTurn(2f);
        }
        else if (CurrentPlayerTurn == 2)
        {
            EndOpponentTurn(2f);
        }
    }
    bool CheckWinningMove(int Column, int Player, int ConnectAmount)
    {
        if(Board[Column, 7] != 0)
        {
            return false;
        }

        int[,] CheckBoard = CloneBoard(Board);
        CheckBoard[Column, 7] = Player;
        CheckBoard = GravityCheckBoard(CheckBoard);
        int[] Result = CheckWinningBoard(CheckBoard, false, ConnectAmount);

        //PrintBoard(CheckBoard);
        //print("Result of Checkboard is P1: " + Result[0] + ", P2: " + Result[1]);

        if((Player == 1 && Result[0] > Result[1]) ||
            Player == 2 && Result[1] > Result[0])
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    int[,] CloneBoard(int[,] SourceBoard)
    {
        int[,] ReturnBoard = new int[8, 8];

        for(int y = 0; y < 8; y++)
        {
            for(int x = 0; x < 8; x++)
            {
                ReturnBoard[x, y] = SourceBoard[x, y];
            }
        }

        return ReturnBoard;
    }
    int[] CheckWinningBoard(int[,] CheckBoard, bool DestroyTokensTrue, int ConnectAmount)
    {
        List<int[]> WinningTokenPositions = new List<int[]>();

        int Player1Wins = 0;
        int Player2Wins = 0;

        //Horisontal Check
        for(int y = 0; y < 8; y++)
        {
            int P1Count = 0;
            int P2Count = 0;
            List<int[]> TempTokenPositions1 = new List<int[]>();
            List<int[]> TempTokenPositions2 = new List<int[]>();
            for (int x = 0; x < 8; x++)
            {
                if(CheckBoard[x, y] == 1)
                {
                    P1Count += 1;
                    P2Count = 0;
                    TempTokenPositions1.Add(new int[2] { x, y });
                    TempTokenPositions2 = new List<int[]>();
                }
                else if (CheckBoard[x, y] == 2)
                {
                    P2Count += 1;
                    P1Count = 0;
                    TempTokenPositions2.Add(new int[2] { x, y });
                    TempTokenPositions1 = new List<int[]>();
                }
                else
                {
                    P1Count = 0;
                    P2Count = 0;
                    TempTokenPositions1 = new List<int[]>();
                    TempTokenPositions2 = new List<int[]>();
                }

                if(P1Count >= ConnectAmount)
                {
                    Player1Wins += 1;
                    WinningTokenPositions.AddRange(TempTokenPositions1);
                }
                else if(P2Count >= ConnectAmount)
                {
                    Player2Wins += 1;
                    WinningTokenPositions.AddRange(TempTokenPositions2);
                }
            }
        }

        //Vertical Check
        for (int x = 0; x < 8; x++)
        {
            int P1Count = 0;
            int P2Count = 0;
            List<int[]> TempTokenPositions1 = new List<int[]>();
            List<int[]> TempTokenPositions2 = new List<int[]>();
            for (int y = 0; y < 8; y++)
            {
                if (CheckBoard[x, y] == 1)
                {
                    P1Count += 1;
                    P2Count = 0;
                    TempTokenPositions1.Add(new int[2] { x, y });
                    TempTokenPositions2 = new List<int[]>();
                }
                else if (CheckBoard[x, y] == 2)
                {
                    P2Count += 1;
                    P1Count = 0;
                    TempTokenPositions2.Add(new int[2] { x, y });
                    TempTokenPositions1 = new List<int[]>();
                }
                else
                {
                    P1Count = 0;
                    P2Count = 0;
                    TempTokenPositions1 = new List<int[]>();
                    TempTokenPositions2 = new List<int[]>();
                }

                if (P1Count >= ConnectAmount)
                {
                    Player1Wins += 1;
                    WinningTokenPositions.AddRange(TempTokenPositions1);
                }
                else if (P2Count >= ConnectAmount)
                {
                    Player2Wins += 1;
                    WinningTokenPositions.AddRange(TempTokenPositions2);
                }
            }
        }

        //Diagonal Check1
        for (int GhostX = -8; GhostX < 8; GhostX++)
        {
            int P1Count = 0;
            int P2Count = 0;
            List<int[]> TempTokenPositions1 = new List<int[]>();
            List<int[]> TempTokenPositions2 = new List<int[]>();

            for (int y = 0; y < 8; y++)
            {
                int x = GhostX + y;
                if (x >= 0 && x <= 7)
                {
                    if (CheckBoard[x, y] == 1)
                    {
                        P1Count += 1;
                        P2Count = 0;
                        TempTokenPositions1.Add(new int[2] { x, y });
                        TempTokenPositions2 = new List<int[]>();
                    }
                    else if (CheckBoard[x, y] == 2)
                    {
                        P2Count += 1;
                        P1Count = 0;
                        TempTokenPositions2.Add(new int[2] { x, y });
                        TempTokenPositions1 = new List<int[]>();
                    }
                    else
                    {
                        P1Count = 0;
                        P2Count = 0;
                        TempTokenPositions1 = new List<int[]>();
                        TempTokenPositions2 = new List<int[]>();
                    }

                    if (P1Count >= ConnectAmount)
                    {
                        Player1Wins += 1;
                        WinningTokenPositions.AddRange(TempTokenPositions1);
                    }
                    else if (P2Count >= ConnectAmount)
                    {
                        Player2Wins += 1;
                        WinningTokenPositions.AddRange(TempTokenPositions2);
                    }
                }
            }
        }

        //Diagonal Check2
        for (int GhostX = 15; GhostX >= 0; GhostX--)
        {
            int P1Count = 0;
            int P2Count = 0;
            List<int[]> TempTokenPositions1 = new List<int[]>();
            List<int[]> TempTokenPositions2 = new List<int[]>();

            for (int y = 0; y < 8; y++)
            {
                int x = GhostX - y;
                if (x >= 0 && x <= 7)
                {
                    if (CheckBoard[x, y] == 1)
                    {
                        P1Count += 1;
                        P2Count = 0;
                        TempTokenPositions1.Add(new int[2] { x, y });
                        TempTokenPositions2 = new List<int[]>();
                    }
                    else if (CheckBoard[x, y] == 2)
                    {
                        P2Count += 1;
                        P1Count = 0;
                        TempTokenPositions2.Add(new int[2] { x, y });
                        TempTokenPositions1 = new List<int[]>();
                    }
                    else
                    {
                        P1Count = 0;
                        P2Count = 0;
                        TempTokenPositions1 = new List<int[]>();
                        TempTokenPositions2 = new List<int[]>();
                    }

                    if (P1Count >= ConnectAmount)
                    {
                        Player1Wins += 1;
                        WinningTokenPositions.AddRange(TempTokenPositions1);
                    }
                    else if (P2Count >= ConnectAmount)
                    {
                        Player2Wins += 1;
                        WinningTokenPositions.AddRange(TempTokenPositions2);
                    }
                }
            }
        }

        if (DestroyTokensTrue == true && WinningTokenPositions.Count > 0)
        {
            print("Found winning tokens");
            WinningTokenPositions = RemoveDuplicates(WinningTokenPositions);
            TokenPosDestroyList = WinningTokenPositions;
            DestroyTimer = 2.5f;
        }

        return new int[2] { Player1Wins, Player2Wins };
    }
    void DestroyTokens()
    {
        List<GameObject> TokensToDestroy = new List<GameObject>();

        string TokenPosListString = "";

        foreach(int[] TokenPos in TokenPosDestroyList)
        {
            TokenPosListString += TokenPos[0].ToString() + ", " + TokenPos[1].ToString() + " | ";
        }

        foreach(GameObject Token in TokenList)
        {
            int x = Token.GetComponent<ConnectFourTokenScript>().x;
            int y = Token.GetComponent<ConnectFourTokenScript>().y;
            int[] TokenPos = new int[2] { x, y };
            string TokenPosString = x.ToString() + ", " + y.ToString();
            if(ContainsIntArray(TokenPosDestroyList, TokenPos))
            {
                TokensToDestroy.Add(Token);
            }
        }
        
        foreach(GameObject Token in TokensToDestroy)
        {
            int x = Token.GetComponent<ConnectFourTokenScript>().x;
            int y = Token.GetComponent<ConnectFourTokenScript>().y;
            TokenList.Remove(Token);
            Board[x, y] = 0;
            Token.GetComponent<ConnectFourTokenScript>().BreakPointParticles();
            Destroy(Token);
        }

        Board = GravityCheckBoard(Board);

        if (CurrentPlayerTurn == 1)
        {
            EndPlayerTurn(2.5f);
        }
        else if (CurrentPlayerTurn == 2)
        {
            EndOpponentTurn(2.5f);
        }
    }
    bool ContainsIntArray(List<int[]> List, int[] SearchObj)
    {
        bool Found = false;
        foreach(int[] IntArray in List)
        {
            bool AllMatches = true;

            for(int i = 0; i < IntArray.Length; i++)
            {
                if(IntArray[i] != SearchObj[i])
                {
                    AllMatches = false;
                }
            }

            if(AllMatches == true)
            {
                Found = true;
            }
        }

        return Found;
    }
    List<int[]> RemoveDuplicates(List<int[]> List)
    {
        List<int[]> ReturnList = new List<int[]>();

        foreach(int[] IntArray in List)
        {
            if(ContainsIntArray(ReturnList, IntArray) == false)
            {
                ReturnList.Add(IntArray);
            }
        }

        return ReturnList;
    }
    void RotateBoard()
    {
        Announcements.color = new Color32(100, 100, 100, 255);
        Announcements.text = "Board has rotated!";

        int[,] NewBoard = new int[8, 8];

        if (RotateDirection == "Counter-Clockwise")
        {
            int NewX = 7;
            for (int y = 0; y < 8; y++)
            {
                int NewY = 0;
                for (int x = 0; x < 8; x++)
                {
                    if (Board[x, y] != 0)
                    {
                        NewBoard[NewX, NewY] = Board[x, y];
                        MoveToken(x, y, NewX, NewY);
                    }
                    NewY += 1;
                }
                NewX -= 1;
            }
        }
        else if(RotateDirection == "Clockwise")
        {
            int NewX = 0;
            for (int y = 0; y < 8; y++)
            {
                int NewY = 7;
                for (int x = 0; x < 8; x++)
                {
                    if (Board[x, y] != 0)
                    {
                        NewBoard[NewX, NewY] = Board[x, y];
                        MoveToken(x, y, NewX, NewY);
                    }
                    NewY -= 1;
                }
                NewX += 1;
            }
        }

        Board = NewBoard;
    }
    void MoveToken(int OldX, int OldY, int NewX, int NewY)
    {
        foreach(GameObject Token in TokenList)
        {
            if(Token.GetComponent<ConnectFourTokenScript>().x == OldX &&
               Token.GetComponent<ConnectFourTokenScript>().y == OldY)
            {
                Token.transform.localPosition = new Vector3(NewX, NewY, 0);
            }
        }
    }
    int[,] GravityCheckBoard(int[,] SourceBoard)
    {
        int[,] NewBoard = new int[8, 8];

        for (int x = 0; x < 8; x++)
        {
            int NewY = 0;
            for (int y = 0; y < 8; y++)
            {
                if (SourceBoard[x, y] != 0)
                {
                    //print(x.ToString() + ", " + NewY.ToString() + "!");
                    NewBoard[x, NewY] = SourceBoard[x, y];
                    NewY += 1;
                }
            }
        }

        return NewBoard;
    }
    void PrintBoard(int[,] Board)
    {
        string PrintString = "";
        for (int y = Board.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < Board.GetLength(0); x++)
            {
                PrintString += Board[x, y].ToString() + " ";
            }
            PrintString += "\n";
        }
        print(PrintString);
    }
    // Update is called once per frame
    void Update()
    {
        if (TurnPauseTimer != 0)
        {
            TurnPauseMethod();
        }

        if (DestroyTimer != 0)
        {
            DestroyTimerMethod();
        }

        if(RotateTimer != 0)
        {
            RotateTimerMethod();
        }

        EndScreenCountDown();
    }
    void DestroyTimerMethod()
    {
        DestroyTimer -= Time.deltaTime;

        if (DestroyTimer > 0)
        {

        }
        else if (DestroyTimer <= 0)
        {
            DestroyTimer = 0;

            DestroyTokens();
        }
    }
    void TurnPauseMethod()
    {
        TurnPauseTimer -= Time.deltaTime;

        if (TurnPauseTimer > 0)
        {
            
        }
        else if(TurnPauseTimer <= 0)
        {
            TurnPauseTimer = 0;

            SwitchPlayer();
        }
    }
    void SwitchPlayer()
    {
        bool TokensUnbalanced = TokenCount();
        if (TokensUnbalanced == false)
        {
            if (CurrentPlayerTurn == 1)
            {
                StartOpponentTurn();
            }
            else if (CurrentPlayerTurn == 2)
            {
                StartPlayerTurn();
            }
        }
        else
        {
            if (OpponentTokenCount > PlayerTokenCount + 1)
            {
                OpponentChooseToDestroy();
            }
            else if (PlayerTokenCount > OpponentTokenCount + 1)
            {
                PlayerChooseToDestroy();
            }
        }
    }
    bool TokenCount()
    {
        PlayerTokenCount = 0;
        OpponentTokenCount = 0;

        foreach(GameObject Token in TokenList)
        {
            if(Token.GetComponent<ConnectFourTokenScript>().Player == 1)
            {
                PlayerTokenCount += 1;
            }
            else if(Token.GetComponent<ConnectFourTokenScript>().Player == 2)
            {
                OpponentTokenCount += 1;
            }
        }

        if(OpponentTokenCount > PlayerTokenCount + 1 || PlayerTokenCount > OpponentTokenCount + 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
