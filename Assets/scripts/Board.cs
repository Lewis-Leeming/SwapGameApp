using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class Board : MCTSscript
{
    public int[] move;
    public int[] removedMove;
    public bool GameOver;
    public string tileColour;
    public List<List<object>> availableMoves;

    public Board(List<object> b)
    {
        BoardPositionsArray = (int[])b[0];
        allBoardPositions = (List<int>)b[1];
        playerOneTiles = (Dictionary<int,int>)b[2];
        playerTwoTiles = (Dictionary<int,int>)b[3];
        playerOneMovesTaken = (List<int[]>)b[4];
        playerTwoMovesTaken = (List<int[]>)b[5];
        state = (gameState)b[6];
        move = (int[])b[7];
        removedMove = (int[])b[8];
        availableMoves = new List<List<object>>();
        availableMoves = GenerateAvailableMoves();
    }

    public void applyMove(List<object> b)
    {
        BoardPositionsArray = (int[])b[0];
        allBoardPositions = (List<int>)b[1];
        playerOneTiles = (Dictionary<int,int>)b[2];
        playerTwoTiles = (Dictionary<int,int>)b[3];
        playerOneMovesTaken = (List<int[]>)b[4];
        playerTwoMovesTaken = (List<int[]>)b[5];
        state = (gameState)b[6];
        move = (int[]) b[7];
        removedMove = (int[])b[8];
    }
    
    public List<List<object>> GenerateAvailableMoves()
    {
        availableMoves.Clear();
        List<List<object>> tempValidMoves = new List<List<object>>();
        List<List<object>> tempMovesForSwap = new List<List<object>>();
        Dictionary<int, int> playerTiles;
        List<int[]> playerMovesTaken;
        if (state == gameState.PLAYERONE)
        {
            playerTiles = new Dictionary<int, int>(playerOneTiles);
            playerMovesTaken = new List<int[]>(playerOneMovesTaken);
        }
        else
        {
            playerTiles = new Dictionary<int, int>(playerTwoTiles);
            playerMovesTaken = new List<int[]>(playerTwoMovesTaken);
        }
        if (isSwap2(playerTiles, BoardPositionsArray))
        {
            for (int i = 0; i < playerMovesTaken.Count; i++)
            {
                tempMovesForSwap = makeMoveSwap2(playerMovesTaken[i]);
                if (tempMovesForSwap != null)
                {
                    tempValidMoves.AddRange(tempMovesForSwap);
                }
            }
        }
        else
        {
            tempValidMoves = makeMove2();
        }
        return tempValidMoves;
    }
    
    public  List<List<object>> makeMove2()
    {
        int[] TempBoardPositionsArray;
        List<int> TempAllBoardPositions;
        
        Dictionary<int, int> TempPlayerOneTiles;
        Dictionary<int, int> TempPlayerTwoTiles;
        
        List<int[]> TempPlayerOneMovesTaken;
        List<int[]> TempPlayerTwoMovesTaken;
        gameState TempState;
        
        List<List<object>> allMoves = new List<List<object>>();
        List<object> tempMove;
        bool[] invalidTiles;
        
        if (state == gameState.PLAYERONE)
        {
            invalidTiles = SwapGame.Get0Tiles(playerOneTiles);
            TempState = gameState.PLAYERTWO;
        }
        else
        {
            invalidTiles = SwapGame.Get0Tiles(playerTwoTiles);
            TempState = gameState.PLAYERONE;
        }

        //checks all possible moves
        for (int i = 0; i < ValidMoves.Count; i++)
        {
            if (!ValidMoves[i].Intersect(allBoardPositions).Any())
            {
                if (invalidTiles[ValidMoves[i].Length - 1])
                {
                    tempMove = new List<object>();
                    //boardPositionsarray to list
                    TempBoardPositionsArray = BoardPositionsArray.ToArray();
                    tempMove.Add(SwapGame.addMoveToboard2(ValidMoves[i], TempBoardPositionsArray));
                    
                    TempAllBoardPositions = new List<int>(allBoardPositions);
                    TempAllBoardPositions.AddRange(ValidMoves[i]);
                    tempMove.Add(TempAllBoardPositions);
                    
                    if (state == gameState.PLAYERONE)
                    {
                        TempPlayerOneTiles = new Dictionary<int, int>(playerOneTiles);
                        TempPlayerTwoTiles = new Dictionary<int, int>(playerTwoTiles);
                        TempPlayerOneTiles[ValidMoves[i].Length] -= 1;
                        TempPlayerOneMovesTaken = new List<int[]>(playerOneMovesTaken) {ValidMoves[i]};
                        TempPlayerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken);
                    }
                    else
                    {
                        TempPlayerOneTiles = new Dictionary<int, int>(playerOneTiles);
                        TempPlayerTwoTiles = new Dictionary<int, int>(playerTwoTiles);
                        TempPlayerTwoTiles[ValidMoves[i].Length] -= 1;
                        TempPlayerOneMovesTaken = new List<int[]>(playerOneMovesTaken);
                        TempPlayerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken) {ValidMoves[i]};
                    }
                    tempMove.Add(TempPlayerOneTiles);
                    tempMove.Add(TempPlayerTwoTiles);
                    tempMove.Add(TempPlayerOneMovesTaken);
                    tempMove.Add(TempPlayerTwoMovesTaken);

                    tempMove.Add(TempState);
                    tempMove.Add(ValidMoves[i]);
                    tempMove.Add(null);
                    //Finally add tempMove to list allMoves
                    allMoves.Add(tempMove);
                }
            }
        }
        if (allMoves.Count == 0)
        {
            return null;
        }
        return allMoves;
    }
    
    public List<List<object>> makeMoveSwap2 (int[] SwapPiece)
    {
        int[] NewBoardPositionsArray = BoardPositionsArray.ToArray();
        NewBoardPositionsArray = SwapGame.removeFromBoard2(SwapPiece, NewBoardPositionsArray);
        int[] TempBoardPositionsArray;
        
        List<int> NewAllBoardPositions = new List<int>(allBoardPositions);
        NewAllBoardPositions = removeFromAllBoardPositions(NewAllBoardPositions, SwapPiece);
        List<int> TempAllBoardPositions;
        
        Dictionary<int, int> NewPlayerOneTiles = new Dictionary<int, int>(playerOneTiles);;
        Dictionary<int, int> NewPlayerTwoTiles = new Dictionary<int, int>(playerTwoTiles);
        Dictionary<int, int> TempPlayerOneTiles;
        Dictionary<int, int> TempPlayerTwoTiles;
        
        List<int[]> TempPlayerOneMovesTaken = new List<int[]>(playerOneMovesTaken);
        List<int[]> TempPlayerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken);
        gameState TempState;

        //List of moves
        List<List<object>> allMoves = new List<List<object>>();
        List<object> tempMove; //construct move
        bool[] invalidTiles;


        if (state == gameState.PLAYERONE)
        {
            invalidTiles = SwapGame.Get0TilesSwap(playerOneTiles, SwapPiece.Length);
            TempState = gameState.PLAYERTWO;
            NewPlayerOneTiles[SwapPiece.Length] += 1;
            TempPlayerOneMovesTaken.Remove(SwapPiece);
        }
        else
        {
            invalidTiles = SwapGame.Get0TilesSwap(playerTwoTiles, SwapPiece.Length);
            TempState = gameState.PLAYERONE;
            NewPlayerTwoTiles[SwapPiece.Length] += 1;
            TempPlayerTwoMovesTaken.Remove(SwapPiece);
        }

        int length;
        //checks all possible moves
        for (int i = 0; i < ValidMoves.Count; i++)
        {
            length = ValidMoves[i].Length - 1;
            if (invalidTiles[length])
            {
                if (!ValidMoves[i].Intersect(NewAllBoardPositions).Any())
                {
                    tempMove = new List<object>();
                    //boardPositionsarray to list
                    TempBoardPositionsArray = NewBoardPositionsArray.ToArray();
                    tempMove.Add(SwapGame.addMoveToboard2(ValidMoves[i], TempBoardPositionsArray));
                    //boardPositions
                    TempAllBoardPositions = new List<int>(NewAllBoardPositions);
                    TempAllBoardPositions.AddRange(ValidMoves[i]);
                    tempMove.Add(TempAllBoardPositions);


                    if (state == gameState.PLAYERONE)
                    {
                        TempPlayerOneTiles = new Dictionary<int, int>(NewPlayerOneTiles);
                        TempPlayerTwoTiles = new Dictionary<int, int>(NewPlayerTwoTiles);
                        TempPlayerOneTiles[ValidMoves[i].Length] -= 1;
                    }
                    else
                    {
                        TempPlayerOneTiles = new Dictionary<int, int>(NewPlayerOneTiles);
                        TempPlayerTwoTiles = new Dictionary<int, int>(NewPlayerTwoTiles);
                        TempPlayerTwoTiles[ValidMoves[i].Length] -= 1;
                    }
                    tempMove.Add(TempPlayerOneTiles);
                    tempMove.Add(TempPlayerTwoTiles);
                    
                    tempMove.Add(TempPlayerOneMovesTaken);
                    tempMove.Add(TempPlayerTwoMovesTaken);

                    tempMove.Add(TempState);
                    tempMove.Add(ValidMoves[i]);
                    tempMove.Add(SwapPiece);
                    //Finally add tempMove to list allMoves
                    allMoves.Add(tempMove);
                }
            }
        }
        if (allMoves.Count == 0)
        {
            return null;
        }
        return allMoves;
    }
    
    public bool isSwap2(Dictionary<int,int> playerTiles, int[] BoardPositionsArray)
    {
        int largestSpace = 0;
        //get smallest tile
        //check if largest space < smallest tile
        int smallestTile = GetSmallestTile(playerTiles);
        int length1 = getLargestSpaceHorizontal(BoardPositionsArray);
        int length2 = getLargestSpaceVertical(BoardPositionsArray);
        largestSpace = Math.Max(length1, length2);
        if (largestSpace < smallestTile)
        {
            return true;
        }
        return false;
    }





}