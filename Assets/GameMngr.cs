using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMngr : MonoBehaviour
{
    [Header("Sound parameters")]
    protected AudioSource audioSource;
    public AudioClip[] pop;

    [Header("Game parameters")]
    [Range(2, 4)]
    public int numberOfPlayers;
    public int numberOfRound = 5;
    public Transform[] playersSpawns;
    public IntRange levelsToLoad;
    public GameObject playerPrefab;

    [Header("UI parameters")]
    public Text winText;
    public Text roundText;
    public Text[] scoreBoard;
    public Color[] playerColors =
    {
        Color.red, Color.blue, Color.yellow, Color.green
    };

    public string[] winMessages = { "name is unstoppable." };

    private int actualNumberOfRounds = 0;
    private bool gameStarted = false;

    private int[] scores = { 0, 0, 0, 0 };

    public IEnumerator Init()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        while (operation.isDone == false)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));
        SetupGame();

        audioSource = GetComponent<AudioSource>();
    }

    private void SetupGame()
    {
        UpdateScoreBoard();
        winText.text = System.String.Empty;
        //Spawn all players
        for (int i = 0; i < numberOfPlayers; ++i)
        {
            GameObject player = Instantiate(playerPrefab, playersSpawns[i].position, Quaternion.identity);
            player.GetComponent<Avatar>().Setup((TeamEnum)i);
        }
        StartCoroutine(GameIntro());
    }

    private IEnumerator GameIntro()
    {
        yield return new WaitForSeconds(1.0f);
        gameStarted = true;
        Avatar[] players = FindObjectsOfType<Avatar>();
        foreach (Avatar player in players)
        {
            player.enabled = true;
        }
    }

    void Update()
    {
        if (GetNumberOfPlayersAlive() == 1 && gameStarted)
        {
            gameStarted = false;
            Win();
            actualNumberOfRounds++;
            if (actualNumberOfRounds >= numberOfRound)
            {
                //BackToMenu
                StartCoroutine(EndGame());
            }
            else
            {
                //LoadNextLevel
                StartCoroutine(NextRoundTimer());
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        int savedIndex = SceneManager.GetActiveScene().buildIndex;
        AsyncOperation operation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        while (operation.isDone == false)
        {
            yield return null;
        }
        operation = SceneManager.LoadSceneAsync(savedIndex, LoadSceneMode.Additive);
        while (operation.isDone == false)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(savedIndex));
        SetupGame();
    }

    private void Win()
    {
        Avatar lastManStanding = FindObjectOfType<Avatar>();
        winText.text = GetRandomTauntMessage(lastManStanding.gameObject.name);
        scores[(int)lastManStanding.team]++;
        winText.color = playerColors[(int)lastManStanding.team];
    }

    private void UpdateScoreBoard()
    {
        for (int i = 0; i < scoreBoard.Length; ++i)
        {
            if (i < numberOfPlayers)
            {
                scoreBoard[i].text = scores[i].ToString();
                // scoreBoard[i].color = playerColors[i];
            }
            else
            {
                scoreBoard[i].text = System.String.Empty;
                scoreBoard[i].GetComponentInParent<Image>().enabled = false;
            }
        }

        roundText.text = "Round " + (actualNumberOfRounds + 1) + " out of " + numberOfRound;
    }

    private IEnumerator NextRoundTimer()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        int nextSceneIndex = GetNextSceneIndex();
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Additive);
        while (operation.isDone == false)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(nextSceneIndex));
        SetupGame();
    }

    private IEnumerator EndGame()
    {
        winText.text = (TeamEnum)GetGameWinner() + " win the game.";
        winText.color = playerColors[GetGameWinner()];
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadSceneAsync(0);
    }

    private int GetGameWinner()
    {
        int maxScore = 0;
        int index = 0;
        for (int i = 0; i < scores.Length; ++i)
        {
            if (scores[i] > maxScore)
            {
                maxScore = scores[i];
                index = i;
            }
        }
        return index;
    }

    private int GetNextSceneIndex()
    {
        return (int)Random.Range(levelsToLoad.a, levelsToLoad.b + 1);
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

    public void PlayBomb()
    {
        audioSource.clip = pop[0];
        audioSource.Play();
    }

    public void PlayCut()
    {
        audioSource.clip = pop[1];
        audioSource.Play();
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

