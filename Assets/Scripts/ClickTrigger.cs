using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;



public class ClickTrigger : MonoBehaviour
{
    TicTacToeAI _ai;

    [SerializeField] private int _myCoordX = 0;
    [SerializeField] private int _myCoordY = 0;
    [SerializeField] private bool canClick;


    private void Awake()
    {
        _ai = FindObjectOfType<TicTacToeAI>();
    }

    private void Start()
    {
        
        _ai.onGameStarted.AddListener(AddReference);
        _ai.onGameStarted.AddListener(() => SetInputEndabled(true));
        _ai.onPlayerWin.AddListener((win) => SetInputEndabled(false));
        
    }

    private void SetInputEndabled(bool val)
    {
        canClick = val;


    }

    private void AddReference()
    {
		
        _ai.RegisterTransform(_myCoordX, _myCoordY, this);
        canClick = true;
		
    }

//Logic to determine if a cell can be clicked or not
    private void OnMouseDown()
    {

        //checks to see if the game has a winner or a tie. If so no more clicking.
        if (_ai.winnerState == 1 || _ai.winnerState == -1)
        {
            canClick = false;
            Debug.Log($"Yay! The game is over now!");
        }
        
        //if the space is available and it's the player's turn call the player game piece method and record the side
        if (canClick && _ai.placeAvailable(_myCoordX, _myCoordY))
        {
            _ai.GetSide();
            _ai.PlayerSelects(_myCoordX, _myCoordY);
            


        }
        else
        {
            //checks to see if there is a winner or a tie so clicking no longer allowed even on a previous space
            
            if (_ai.winnerState == 1 || _ai.winnerState == -1)
            {
                canClick = false;
                Debug.Log($"Yay! The game is over now!");
            }

            //Need to test for AI created pieces and not allow for cloning
            //Checks to see if the cell is a space occupied by an AI game piece. 
            else
            {
                Debug.Log($"no clicking on a previous played space!!!");
            }
            
        }
        
        
    }

	
}