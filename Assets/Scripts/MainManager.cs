using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text Ranking;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private static string bestPlayer;
    private static int bestScore;

    
    private void Awake() {  // new added
            LoadGame();     // new added
    }                       // new added

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        SetBestPlayer();  // new added
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        PlayerDataTransfer.Instance.Score = m_Points; // new added 
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        UpdateBestPlayer(); // needs to be added
        GameOverText.SetActive(true);
    }

    /*
    New functionalities from here:
        - Load and Save to json file 
        - UpdateBestPlayer() fnction when game over
        - SetBestPlayer() when awake (into Ranking text) 
    */
    private void UpdateBestPlayer(){
        int currentScore = PlayerDataTransfer.Instance.Score;   

        if(currentScore > bestScore){
            bestPlayer = PlayerDataTransfer.Instance.PlayerName;
            bestScore = currentScore;

            Ranking.text = $"Best Score: {bestPlayer}: {bestScore}";
            SaveGame(bestPlayer,bestScore);
        }
    }

    public void SetBestPlayer(){
        if(bestPlayer == null && bestScore == 0){
            ScoreText.text = "";
        } else {
            Ranking.text = $"Best Score: {bestPlayer}: {bestScore}";
        }
    }

    public void SaveGame(string bestPlayerName, int bestScore){
        SaveData data = new SaveData();
        data.TheBestPlayer = bestPlayerName;
        data.HighiestScore = bestScore;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "savefil.json", json);
    }

    public void LoadGame(){
        string path = Application.persistentDataPath + "savefil.json";

        if (File.Exists(path)){        
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            bestPlayer = data.TheBestPlayer;
            bestScore = data.HighiestScore;
        } 
    }

     [System.Serializable]
    class SaveData
    {
        public int HighiestScore;
        public string TheBestPlayer;
    }

}