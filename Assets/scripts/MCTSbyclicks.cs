using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class MCTSbyclicks : MCTSscript
{
    public int[] move;
    public int[] removeMove;
    
    public bool playerOnePass;

    private List<object> BoardState;

    public void ApplyMove(List<object> b)
    {
        BoardPositionsArray = (int[])b[0];
        allBoardPositions = (List<int>)b[1];
        playerOneTiles = (Dictionary<int,int>)b[2];
        playerTwoTiles = (Dictionary<int,int>)b[3];
        playerOneMovesTaken = (List<int[]>)b[4];
        playerTwoMovesTaken = (List<int[]>)b[5];
        state = (gameState)b[6];
        move = (int[]) b[7];
        removeMove = (int[]) b[8];
    }

    public void MCTSbutton()
    {
        int randomNo = 0;
        bool isFinished = false;
        int length = 0;
        if (state == gameState.PLAYERONE)
        {
            playerOneTurn();
        }
        else
        {
            playerTwoTurn();
        }
    }

    public void playerOneTurn()
    {
        
        var playerOne = computerMove(BoardState);
        BoardState = playerOne.move;
        playerOnePass = playerOne.GameOver;
        ApplyMove(BoardState);
        if (!playerOnePass)
        {
            SwapGame.printPositions(BoardPositionsArray);
            SwapGame.printTiles(playerOneTiles);
            if ((int[]) BoardState[8]!= null)
            {
                int[] move = (int[]) BoardState[8];
                Destroy(GameObject.Find("randomScript" + move[0]));
                PutMoveOnBoard((int[])BoardState[7], blueFrozen);
            }
            else
            {
                PutMoveOnBoard((int[])BoardState[7], blueTile);
            }
            //playerone.gameover is true when no moves can be made
        }
        else
        {
            Debug.Log("player one passes");
        }
        playerHUD.SetHUD(getRedFour(), getRedThree(), getRedTwo(), getRedOne(), redImage);
    }

    public void playerTwoTurn(List<object> BoardState)
    {
        var playerTwo = computerMove(BoardState);
        if (!checkFinishedMain(playerOnePass, playerTwo.GameOver))
        {
            BoardState = playerTwo.move;
            if ((int[]) BoardState[8] != null)
            {
                int[] move = (int[]) BoardState[8];
                Destroy(GameObject.Find("randomScript" + move[0]));
                PutMoveOnBoard((int[]) BoardState[7], redFrozen);
            }
            else
            {
                PutMoveOnBoard((int[]) BoardState[7], redTile);
            }

            ApplyMove(BoardState);
            printBoardState(BoardState);
            playerHUD.SetHUD(getBlueFour(), getBlueThree(), getBlueTwo(), getBlueOne(), blueImage);
            //playerone.gameover is true when no moves can be made
        }
    }

}