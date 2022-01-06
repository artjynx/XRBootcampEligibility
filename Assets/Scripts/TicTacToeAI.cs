using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public enum TicTacToeState{none, cross, circle}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
	 
}

public class TicTacToeAI : MonoBehaviour
{
	EndMessage _ai;
	int _aiLevel;

	TicTacToeState[,] boardState;

	public string[,] gameStateArray; 
	[SerializeField] private bool _isPlayerTurn;
	[SerializeField] private int _gridSize = 3;

	[SerializeField] private TicTacToeState playerState = TicTacToeState.cross;
	private TicTacToeState aiState = TicTacToeState.circle;

	[SerializeField] private GameObject _xPrefab; 
	[SerializeField] private GameObject _oPrefab;
	
	public UnityEvent onGameStarted;
	private int _turnCount = 0;
	private string side;
	public int winnerState;

	

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	ClickTrigger[,] _triggers;

	public TicTacToeAI()
	{
		this.gameStateArray= new string[3,3]{
			{null, null, null},
			{null, null, null},
			{null, null, null}
		};
	}

	
	
	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}
		
	}

	public void StartAI(int AiLevel){
		_aiLevel = AiLevel;
		StartGame();
	}

	
	
	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_triggers[myCoordX, myCoordY] = clickTrigger;
		
	}

	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3];
		onGameStarted.Invoke();
	}


	// Gets the current side
	public string GetSide()
	{
		return side;
	}

	// Tracks the current turn of who is playing
	void changeSide()
	{
		if (side == "X")
			side = "0";
		else
		{
			side = "X";
		}
	}

	//This is the check to see if you win code.... I did not get to the trying to win phase though so it always starts at the top left
	public void EndTurn()
	{
		_turnCount++;
		if (gameStateArray[0,2] == side && gameStateArray[1,2] == side && gameStateArray[2,2] == side && gameStateArray[2,2]!= null)
		GameOver();
		else if (gameStateArray[0,1] == side && gameStateArray[1,1] == side && gameStateArray[2,1] == side && gameStateArray[2,1]!= null)
			GameOver();
		else if (gameStateArray[0,0] == side && gameStateArray[1,0] == side && gameStateArray[2,0] == side && gameStateArray[2,0]!= null)
			GameOver();
		else if (gameStateArray[0,2] == side && gameStateArray[0,1] == side && gameStateArray[0,0] == side && gameStateArray[0,2]!= null)
			GameOver();
		else if (gameStateArray[1,2] == side && gameStateArray[1,1] == side && gameStateArray[1,0] == side && gameStateArray[1,2]!= null)
			GameOver();
		else if (gameStateArray[2,2] == side && gameStateArray[2,1] == side && gameStateArray[2,0] == side && gameStateArray[2,2]!= null)
			GameOver();
		else if (gameStateArray[0,2] == side && gameStateArray[1,1]== side && gameStateArray[2,0] == side && gameStateArray[0,2]!= null)
			GameOver();
		else if (gameStateArray[2,2] == side && gameStateArray[1,1] == side && gameStateArray[0,0] == side && gameStateArray[2,2]!= null)
			GameOver();
		//tracks the number of turns - it's 10 because the initial click to play gets counted right now
		if (_turnCount >= 10)
		{
			//declares it a tie and sets the winnerState to -1
			Debug.Log($"Game Over It's a Tie!");
			winnerState = -1;
		}
		//Once the check to see if the turn is a winner or a tie is not found it calls the change side for the other player
		changeSide();
		 
	}

	//I realize I needed to use the OnPlayerWin method but I ran out of time
	//This calls to find the current side to declare it a winner
	public void GameOver()
	{
		Debug.Log($"{side} wins!!");
		winnerState = 1;
	}

	//place an x for the player
	public void PlayerSelects(int coordX, int coordY)
	{
			//Initializes side here, otherwise the first click to play is null and messes everything up	
			side = "X";
			
			//call to place the player gamepiece
			SetVisual(coordX, coordY, playerState);
			
			//record the player game piece location and type
			gameStateArray[coordX, coordY] = side;
			Debug.Log($"I am the player {side}");
			
			//Check to see if it's a winner or a tie
			EndTurn();
			
			//if it's not a winner or a tie, then next turn for AI
			if(winnerState != 1 && winnerState != -1){
			NextMove();
			}
	}

//Place an O for the AI
	public void AiSelects(int coordX, int coordY){
		//call to create game piece
		SetVisual(coordX, coordY, aiState);
		Debug.Log($"I am the AI {side}");
		//record the player game piece location and type
		gameStateArray[coordX, coordY] = side;
		
		//Check to see if it's a winner or a tie
		EndTurn();
	}
	
	
	//This is the logic for the AI to take a turn. I loops through the game cells to look for an empty one. This is not currently strategic.
	public void NextMove()
	{
		
		
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				//if it finds an empty cell, push through the AI Circle to 
				if (gameStateArray[i,j] == null)
				{
					//Make an O
					AiSelects(i, j);
					
					//Change sides for the player's turn
					changeSide();
					
					return;

				}
			}
		}
	}

	//This checks for the player to see if a space has been taken by the AI. So you don't get X's on top of 'Os
	public bool placeAvailable(int x, int y)
	{
		if (gameStateArray[x, y] == null)
		{
			return true;
		}
		return false;
	}

	
	//Logic to make a gamePiece on the board
	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		
		Instantiate(
			targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
			_triggers[coordX, coordY].transform.position, 
			Quaternion.identity
		);
		Debug.Log($"{coordX} {coordY} {targetState}");
	}
}


