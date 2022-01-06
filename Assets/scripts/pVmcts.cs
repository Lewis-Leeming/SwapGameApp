using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class pVmcts : MCTSbyclicks
{
    private LineRenderer line;
    private Vector2 firstPos;
    private Vector3 mousePos;
    private int currLines = 0;
    private float distance = 0;
    private bool turnTaken = true;
    [SerializeField] GameObject popUp;
    private GameObject go;
    private GameObject tileCount;
    public GameObject boardArea;
    public Button yesButton;
    public Button noButton;
    private int currTiles = 0;
    private bool result;
    private string GOclicked;
    private GameObject tileClicked;
    private int invalidLength = -1;
    //private bool playerOnePass;

    private List<object> BoardState;

    private Vector2 GetSquareClicked()
    {
        Vector2 clickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(clickPos);
        Vector2 gridPos = SnapToGrid(worldPos);
        return gridPos;
    }

    private Vector2 SnapToGrid(Vector2 rawWorldPos)
    {
        float newX = Mathf.Sign(rawWorldPos.x) * (Mathf.Abs((int) rawWorldPos.x) + 0.5f);
        float newY = Mathf.Sign(rawWorldPos.y) * (Mathf.Abs((int) rawWorldPos.y) + 0.5f);
        return new Vector2(newX, newY);
    }

    public GameObject GetGameObjectClicked()
    {
        return tileClicked;
    }

    // Start is called before the first frame update

    public string gameObjectHit(Vector2 coordinates)
    {
        RaycastHit2D hit = Physics2D.Raycast(coordinates, Vector2.zero);
        return hit.collider.gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == gameState.PLAYERTWO)
        {
            enabled = false;
            playerTwoTurn(BoardState);
            state = gameState.PLAYERONE;
            enabled = true;
        }
        //when mouse is clicked 
        if (Input.GetMouseButtonDown(0))
        {
            bool makeLine = true;
            firstPos = GetSquareClicked();
            GOclicked = gameObjectHit(firstPos);
            if (state == gameState.PLAYERONE)
            {
                if (NoMovesAvailable(playerOneTiles))
                {
                    if (GOclicked.Contains("blue"))
                    {
                        Debug.Log("you hit blue");
                        popUp.SetActive(true);
                        //Debug.Log(pop.name);
                        tileClicked = GameObject.Find(GOclicked);
                        enabled = false;
                        makeLine = false;
                    }
                }
            }
            else if (state == gameState.PLAYERTWO)
            {
                if (NoMovesAvailable(playerTwoTiles))
                {
                    if (GOclicked.Contains("red"))
                    {
                        Debug.Log("you hit red");
                        popUp.SetActive(true);
                        tileClicked = GameObject.Find(GOclicked);
                        enabled = false;
                        makeLine = false;
                    }
                }
            }

            if (makeLine && firstPos.x < 3.5 && firstPos.x > -3.5 && firstPos.y < 3.5 && firstPos.y > -3.5)
            {
                if (line == null)
                {
                    createLine();
                }

                mousePos = GetSquareClicked();
                mousePos.z = 4;
                line.SetPosition(0, mousePos);
                line.SetPosition(1, mousePos);
            }
        }
        else if (Input.GetMouseButtonUp(0) && line)
        {
            mousePos = GetSquareClicked();
            mousePos.z = 1;
            validInputs();
            tileCheck(distance);
            line = null;
            line = null;
            currLines++;
            if (turnTaken == true)
            {
                if (state == gameState.PLAYERONE || state == gameState.FROZENBLUE)
                {
                    playerHUD.SetHUD(getRedFour(), getRedThree(), getRedTwo(), getRedOne(), redImage);
                    state = gameState.PLAYERTWO;
                }
                else
                {
                    playerHUD.SetHUD(getBlueFour(), getBlueThree(), getBlueTwo(), getBlueOne(), blueImage);
                    state = gameState.PLAYERONE;
                }
            }
        }
        else if (Input.GetMouseButton(0) && line)
        {
            mousePos = GetSquareClicked();
            mousePos.z = 1;
            validInputs();

        }
    }
    
    public bool NoMovesSwap(Dictionary<int, int> playerTiles, int invalidTile)
    {
        // no moves if tile count == 0 
        // if taking a way a tile will not effect outcome, dont implement yet
        //if smallest tile!!! < largest space
        int largestSpace = 0;
        //get smallest tile
        //check if largest space < smallest tile
        int smallestTile = GetSmallestTileWithInvalid(playerTiles,invalidTile);
        int length1 = getLargestSpaceHorizontal();
        int length2 = getLargestSpaceVertical();
        largestSpace = Math.Max(length1, length2);
        if (largestSpace < smallestTile)
        {
            return true;
        }

        return false;
    }

    public bool NoMovesAvailable(Dictionary<int, int> playerTiles)
    {
        // no moves if tile count == 0 
        // if taking a way a tile will not effect outcome, dont implement yet
        //if smallest tile!!! < largest space
        int largestSpace = 0;
        //get smallest tile
        //check if largest space < smallest tile
        int smallestTile = GetSmallestTile(playerTiles);
        int length1 = getLargestSpaceHorizontal();
        int length2 = getLargestSpaceVertical();
        largestSpace = Math.Max(length1, length2);
        if (largestSpace < smallestTile)
        {
            return true;
        }

        return false;
    }

    public int GetSmallestTileWithInvalid(Dictionary<int,int> playerTiles, int invalidTile)
    {
        for (int i = 1; i <= 4; i++)
        {
            if (playerTiles[i] != 0 && i != invalidTile)
            {
                return i;
            }
        }
        //number that is too big, change so less hardcoded
        return 7;
    }

    public int getLargestSpaceVertical()
    {
        int tmp = 0;
        int largestSpace = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int k = i; k < 36; k++)
            {
                if (BoardPositionsArray[k] == 0)
                {
                    tmp++;
                }
                else
                {
                    tmp = 0;
                }

                if (tmp > largestSpace)
                {
                    largestSpace = tmp;
                }

                k += 5;
            }

            tmp = 0;
        }

        return largestSpace;
    }

    public int getLargestSpaceHorizontal()
    {
        int tmp = 0;
        int index = 0;
        int largestSpace = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                index = j + (i * 6);
                if (BoardPositionsArray[index] == 0)
                {
                    tmp++;
                }
                else
                {
                    tmp = 0;
                }

                if (tmp > largestSpace)
                {
                    largestSpace = tmp;
                }
            }

            tmp = 0;
        }

        return largestSpace;
    }

    void createLine()
    {
        line = new GameObject("Line" + currLines).AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.15f;
        line.endWidth = 0.15f;
        line.useWorldSpace = false;
        line.numCapVertices = 50;
        line.sortingOrder = 1;
        line.material = new Material(Shader.Find("Sprites/Default"));
        float alpha = 1.0f;
        if (state == gameState.PLAYERONE || state == gameState.FROZENBLUE)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.cyan, 1.0f)},
                new GradientAlphaKey[] {new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f)}
            );
            line.colorGradient = gradient;
        }
        else
        {
            Gradient gradient2 = new Gradient();
            gradient2.SetKeys(
                new GradientColorKey[]
                    {new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.yellow, 1.0f)},
                new GradientAlphaKey[] {new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f)}
            );
            line.colorGradient = gradient2;
        }
    }

    //remove
    //more than 4 squares long
    //diagonal lines
    private void validInputs()
    {
        //check if mouse 
        //check if diagonal
        //check if 4 squares
        if ((Math.Abs(mousePos.x - firstPos.x) < 4 && mousePos.y == firstPos.y) ||
            (Math.Abs(mousePos.y - firstPos.y) < 4 && mousePos.x == firstPos.x))
        {
            if (mousePos.x < 3.5 && mousePos.x > -3.5 && mousePos.y < 3.5 && mousePos.y > -3.5)
            {
                line.SetPosition(1, mousePos);
                distance = ((Vector2) mousePos - (Vector2) firstPos).magnitude;
                if (Math.Abs(distance) > 3)
                {
                    distance = 3;
                }
            }
        }

    }

    private void removeTile(int tileNo)
    {
        if (state == gameState.PLAYERONE)
        {
            playerOneTiles[tileNo] -= 1;
        }
        else if (state == gameState.PLAYERTWO)
        {
            playerTwoTiles[tileNo] -= 1;
        }
    }


    private void tileCheck(float distance)
    {
        int length = (int) distance + 1;
        if (length == invalidLength)
        {
            Destroy(line);
            turnTaken = false;
        }
        else if (mousePos.y == firstPos.y || mousePos.x == firstPos.x)
        {
            switch (distance)
            {
                case 0:
                    if ((state == gameState.PLAYERONE && playerOneTiles[1] <= 0) ||
                        (state == gameState.PLAYERTWO && playerTwoTiles[1] <= 0))
                    {
                        Destroy(line);
                        turnTaken = false;
                    }
                    else
                    {

                        TilePosition();
                        removeTile(1);
                        turnTaken = true;

                    }

                    break;
                case 1:
                    if ((state == gameState.PLAYERONE && playerOneTiles[2] <= 0) ||
                        (state == gameState.PLAYERTWO && playerTwoTiles[2] <= 0))
                    {
                        Destroy(line);
                        turnTaken = false;
                    }
                    else
                    {
                        TilePosition();
                        removeTile(2);
                        turnTaken = true;
                    }

                    break;
                case 2:
                    if ((state == gameState.PLAYERONE && playerOneTiles[3] <= 0) ||
                        (state == gameState.PLAYERTWO && playerTwoTiles[3] <= 0))
                    {
                        Destroy(line);
                        turnTaken = false;
                    }
                    else
                    {
                        TilePosition();
                        removeTile(3);
                        turnTaken = true;
                    }

                    break;

                case 3:
                    if ((state == gameState.PLAYERONE && playerOneTiles[4] <= 0) ||
                        (state == gameState.PLAYERTWO && playerTwoTiles[4] <= 0))
                    {
                        Destroy(line);
                        turnTaken = false;
                    }
                    else
                    {
                        removeTile(4);
                        TilePosition();
                        turnTaken = true;
                    }

                    break;

                case 4:
                    Destroy(line);
                    break;

            }
        }
        else
        {
            Destroy(line);
            turnTaken = false;
        }


        /*foreach (KeyValuePair<int, int> kvp in blueTiles)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            Debug.Log(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
        }*/

    }

    /*public bool firstBiggerCheck(float first, float last)
    {
        if (first > last)
        {
            return true;
        }else
        {
            return false;
        }
    }*/

    public GameObject checkState()
    {
        if (state == gameState.PLAYERONE)
        {
            return blueTile;
        }
        else if (state == gameState.PLAYERTWO)
        {
            return redTile;
        }
        else if (state == gameState.FROZENRED)
        {
            return redFrozen;
        }
        else
        {
            return blueFrozen;
        }
    }

    public void createSingleTile()
    {
        int a = 0;
        GameObject tileColour = checkState();
        GameObject newTile = Instantiate(tileColour, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
        newTile.transform.localScale = new Vector3(1, 1, 1);
        newTile.name = tileColour.name + currTiles + ":" + distance;
        newTile.layer = 1;
        Destroy(line);
        int xnumero = (int) (mousePos.x + 2.5);
        int ynumero = (int) (mousePos.y + 2.5) * 6;
        BoardPositionsArray[xnumero + ynumero] = 1;
        printPosition();
    }

    public void onYesOrNo(int check)
    {
        //1 is yes 
        if (check == 1)
        {
            Debug.Log("you said yes");
            string[] newno = tileClicked.name.Split(':');
            int length = int.Parse(newno[1]) + 1;
            invalidLength = length;

            if (state == gameState.PLAYERONE)
            {
                state = gameState.FROZENBLUE;
                playerOneTiles[length] += 1;
                playerHUD.SetHUD(getBlueFour(), getBlueThree(), getBlueTwo(), getBlueOne(), blueImage);
            }
            else
            {
                state = gameState.FROZENRED;
                playerTwoTiles[length] += 1;
                playerHUD.SetHUD(getRedFour(), getRedThree(), getRedTwo(), getRedOne(), redImage);
            }

            Vector2 tileCoords = tileClicked.transform.position;
            Vector2 tileScale = tileClicked.transform.localScale;
            if (tileScale.y == 1 && tileScale.x > 2)
            {
                tileCoords = new Vector2(tileCoords.x - 1, tileCoords.y);
                addToHorizontal(tileCoords, (int) tileScale.x, true);
            }
            else if (tileScale.x == 1 && tileScale.y > 2)
            {
                tileCoords = new Vector2(tileCoords.x, tileCoords.y - 1);
                addToVertical(tileCoords, (int) tileScale.y, true);
            }
            else if (tileScale.y == 1)
            {
                addToHorizontal(tileCoords, (int) tileScale.x, true);
            }
            else
            {
                addToVertical(tileCoords, (int) tileScale.y, true);
            }

            printPosition();
            Destroy(tileClicked);
            removeFromAllBoardPositions(allBoardPositions,move);
            
            if (NoMovesSwap(playerOneTiles,length))
            {
                Debug.Log("gameoverplayer");
                playerOnePass = true;
                playerTwoTurn(BoardState);
            }
        }
        else
        {
            Debug.Log("You said no");
        }

        popUp.SetActive(false);
        enabled = true;
    }
    
    public bool checkOverlapHorizontal(Vector2 first, int distance)
    {
        Debug.Log(first);
        int xnumero = (int) (first.x + 2.5);
        int ynumero = (int) (first.y + 2.5) * 6;
        //Debug.Log(xcoord);
        //Debug.Log(ycoordinates);
        for (int i = 0; i < distance; i++)
        {
            if (BoardPositionsArray[i + xnumero + ynumero] == 1)
            {
                return false;
            }
        }

        return true;
        //position[4] = 1;
    }

    public bool checkOverlapVertical(Vector2 first, int distance)
    {
        int xnumero = (int) (first.x + 2.5);
        int ynumero = (int) (first.y + 2.5) * 6;
        Debug.Log("x coord is " + xnumero);
        for (int i = 0; i < distance; i++)
        {
            if (BoardPositionsArray[i + xnumero + ynumero] == 1)
            {
                return false;
            }

            ynumero = ynumero + 5;
        }

        return true;
    }

    public void TilePosition()
    {
        float middleOfTile;
        GameObject tileColour = checkState();
        invalidLength = -1;
        //if vertical and first is bigger
        if (mousePos.x == firstPos.x && mousePos.y > firstPos.y)
        {
            if (checkOverlapVertical(firstPos, (int) distance + 1))
            {
                middleOfTile = firstPos.y + (distance / 2);
                CreateTile(tileColour, middleOfTile, true);
                addToVertical(firstPos, (int) distance + 1, false);
                setBoardState();
            }
            else
            {
                if (state == gameState.PLAYERONE)
                {
                    state = gameState.PLAYERTWO;
                }
                else
                {
                    state = gameState.PLAYERONE;
                }
            }

            printPosition();
        } //if vertical and second is bigger
        else if (mousePos.x == firstPos.x && mousePos.y < firstPos.y)
        {
            if (checkOverlapVertical(mousePos, (int) distance + 1))
            {
                middleOfTile = firstPos.y - (distance / 2);
                CreateTile(tileColour, middleOfTile, true);
                addToVertical(mousePos, (int) distance + 1, false);
                setBoardState();
            }
            else
            {
                if (state == gameState.PLAYERONE)
                {
                    state = gameState.PLAYERTWO;
                }
                else
                {
                    state = gameState.PLAYERONE;
                }
            }
            printPosition();
        }
        else if (mousePos.x != firstPos.x && mousePos.x > firstPos.x) //if horizontal and first is bigger
        {
            if (checkOverlapHorizontal(firstPos, (int) distance + 1))
            {
                middleOfTile = firstPos.x + (distance / 2);
                CreateTile(tileColour, middleOfTile, false);
                addToHorizontal(firstPos, (int) distance + 1, false);
                setBoardState();
            }
            else
            {
                if (state == gameState.PLAYERONE)
                {
                    state = gameState.PLAYERTWO;
                }
                else
                {
                    state = gameState.PLAYERONE;
                }
            }
            printPosition();
        }
        else
        {
            if (checkOverlapHorizontal(mousePos, (int) distance + 1))
            {
                middleOfTile = firstPos.x - (distance / 2);
                CreateTile(tileColour, middleOfTile, false);
                addToHorizontal(mousePos, (int) distance + 1, false);
                setBoardState();
            }
            else
            {
                if(state == gameState.PLAYERONE)
                {
                    state = gameState.PLAYERTWO;
                }
                else
                {
                    state = gameState.PLAYERONE;
                }
            }

            printPosition();
        }

        Destroy(line);
        currTiles++;
        //if horizontal and second is bigger
    }
    
    public void setBoardState()
    {
        //BoardPositionsArray = (int[])b[0];
        //allBoardPositions = (List<int>)b[1];
        //playerOneTiles = (Dictionary<int,int>)b[2];
        //playerTwoTiles = (Dictionary<int,int>)b[3];
        //playerOneMovesTaken = (List<int[]>)b[4];
        //playerTwoMovesTaken = (List<int[]>)b[5];
        //state = (gameState)b[6];
        //move = (int[]) b[7];
        //removeMove = (int[]) b[8];
        BoardState = new List<object>();
        BoardState.Add(BoardPositionsArray);
        allBoardPositions.AddRange(move);
        BoardState.Add(allBoardPositions);
        BoardState.Add(playerOneTiles);
        BoardState.Add(playerTwoTiles);
        playerOneMovesTaken.Add(move);
        BoardState.Add(playerOneMovesTaken);
        BoardState.Add(playerTwoMovesTaken);
        BoardState.Add(gameState.PLAYERTWO);
        BoardState.Add(move);
        BoardState.Add(null);
    }

    public void CreateTile(GameObject tileColour, float middleOfTile, bool vertical)
    {
        if (vertical)
        {
            GameObject newTile = Instantiate(tileColour, new Vector3(mousePos.x, middleOfTile, 0), Quaternion.identity);
            newTile.transform.localScale =
                new Vector3(transform.localScale.x, transform.localScale.y + Math.Abs(distance), 1);
            newTile.name = tileColour.name + currTiles + ":" + distance;
            newTile.layer = 1;
            Destroy(line);
        }
        else
        {
            GameObject newTile = Instantiate(tileColour, new Vector3(middleOfTile, mousePos.y, 0), Quaternion.identity);
            newTile.transform.localScale =
                new Vector3(transform.localScale.x + Math.Abs(distance), transform.localScale.y, 1);
            newTile.name = tileColour.name + currTiles + ":" + distance;
            newTile.layer = 1;
            Destroy(line);
        }
    }

    public void addToHorizontal(Vector2 first, int distance, bool remove)
    {
        Debug.Log(first);
        int xnumero = (int) (first.x + 2.5);
        int ynumero = (int) (first.y + 2.5) * 6;
        int a = 1;
        int index;
        move = new int[distance];
        //Debug.Log(xcoord);
        //Debug.Log(ycoordinates);
        if (remove)
        {
            a = 0;
        }

        for (int i = 0; i < distance; i++)
        {
            index = i + xnumero + ynumero;
            BoardPositionsArray[index] = a;
            move[i] = index;
        }

        //position[4] = 1;
    }
    
    

    public void addToVertical(Vector2 first, int distance, bool remove)
    {
        int xnumero = (int) (first.x + 2.5);
        int ynumero = (int) (first.y + 2.5) * 6;
        int a = 1;
        int index;
        move = new int[distance];
        if (remove)
        {
            a = 0;
        }

        for (int i = 0; i < distance; i++)
        {
            index = i + xnumero + ynumero;
            BoardPositionsArray[index] = a;
            move[i] = index;
            ynumero = ynumero + 5;
        }
    }

    public void printPosition()
    {
        string str = "";
        for (int i = 0; i < BoardPositionsArray.Length; i++)
        {
            str += BoardPositionsArray[i];
        }

        Debug.Log(str);
    }
}
