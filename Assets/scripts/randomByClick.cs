using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class randomByClick : randomScript
{
    private bool playerOnePass;
    private bool playerTwoPass;
    public void random()
    {
        printPositions();
        int randomNo = 0;
        bool isFinished = false;
        int length = 0;
        if (state == gameState.PLAYERONE)
        {
            playerOnePass = playerOneTurn();
        }
        else
        {
            playerTwoPass = playerTwoTurn();
            isFinished = checkFinished(playerOnePass, playerTwoPass);
        }

        if (isFinished)
        {
            getWinner(playerOnePass,playerTwoPass);
        }
    }
}