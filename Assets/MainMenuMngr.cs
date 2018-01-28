using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuMngr : MonoBehaviour
{
    public Animator animator;
    public Text showPlayerNumber;
    public Text showRoundNumber;

    private int playerNumber = 2;
    private int roundNumber = 5;

    public void PreBattle()
    {
        animator.SetTrigger("switch");
    }

    public void Play()
    {
		StartCoroutine(Launch());
    }

	private IEnumerator Launch()
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
		while (operation.isDone != true)
		{
			yield return null;
		}
		GameMngr mngr = FindObjectOfType<GameMngr>();
		mngr.numberOfPlayers = playerNumber;
		mngr.numberOfRound = roundNumber;
		mngr.StartCoroutine(mngr.Init());
		SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(0));
	}

    public void UpdatePlayerNumber(Slider number)
    {
        playerNumber = (int)number.value;
        showPlayerNumber.text = playerNumber.ToString();
    }

    public void UpdateRoundNumber(Slider number)
    {
        roundNumber = (int)number.value;
        showRoundNumber.text = roundNumber.ToString();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
