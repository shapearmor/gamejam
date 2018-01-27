using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMngr : MonoBehaviour
{
    [Header("Game parameters")]
    [Range(2, 4)]
    public int numberOfPlayers;
    public int numberOfRound = 5;
    public Transform[] playersSpawns;
    public IntRange levelsToLoad;
    public GameObject playerPrefab;

    [Header("UI parameters")]
    public Text winText;
    public Color[] playerColors =
    {
        Color.red, Color.blue, Color.yellow, Color.green
    };

    public string[] winMessages = { "name is unstoppable." };

    private int actualNumberOfRounds = 0;
	private bool gameStarted = false;

	void Start()
	{
		SetupGame();
	}

    private void SetupGame()
    {
        //Spawn all players
        for (int i = 0; i < numberOfPlayers; ++i)
        {
            GameObject player = Instantiate(playerPrefab, playersSpawns[i].position, Quaternion.identity);
            player.GetComponent<Avatar>().Setup((TeamEnum)i);
        }
    }

	private IEnumerator GameIntro()
	{
		yield return new WaitForSeconds(5.0f);
		gameStarted = true;
		Avatar[] players = FindObjectsOfType<Avatar>();
		foreach(Avatar player in players)
		{
			player.enabled = true;
		}
	}

    void Update()
    {
        if (GetNumberOfPlayersAlive() == 1 && gameStarted)
        {
            Win();
            actualNumberOfRounds++;
			if (actualNumberOfRounds >= numberOfRound)
			{
				//BackToMenu
				SceneManager.LoadSceneAsync(0);
			}
			else
			{
				//LoadNextLevel
				StartCoroutine(NextRoundTimer());
			}
        }
    }

    private void Win()
    {
        Avatar lastManStanding = FindObjectOfType<Avatar>();
        winText.text = GetRandomTauntMessage(lastManStanding.gameObject.name);
        winText.color = playerColors[(int)lastManStanding.team];
    }

    private IEnumerator NextRoundTimer()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        AsyncOperation operation = SceneManager.LoadSceneAsync(GetNextSceneIndex(), LoadSceneMode.Additive);
		while (operation.isDone == false)
		{
			yield return null;
		}
		Debug.Log("Active scene is now : " + SceneManager.GetActiveScene().name);
    }

    private int GetNextSceneIndex()
    {
        return Random.Range(levelsToLoad.a, levelsToLoad.b);
    }

    private string GetRandomTauntMessage(string playerName)
    {
        string originalString = winMessages[Random.Range(0, winMessages.Length)];
        return originalString.Replace("name", playerName);
    }

    private int GetNumberOfPlayersAlive()
    {
        return FindObjectsOfType<Avatar>().Length;
    }

}

[System.Serializable]
public struct IntRange
{
    public int a;
    public int b;

    public IntRange(int a, int b)
    {
        this.a = a;
        this.b = b;
    }
}
