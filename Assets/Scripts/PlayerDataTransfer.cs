using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataTransfer : MonoBehaviour
{
    //Static Class for save the current player data
    //Singleton pattern
    public static PlayerDataTransfer Instance;

    public string PlayerName;

    public int Score;

    private void Awake() {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
