﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour {

    public GameObject player;
    public GameObject scoreText; // Text for current score
    public GameObject recordText; // Text for best score
    public Transform bestScoreLine; // Visual line for best score

    public bool newHighScore = false;
    public int score;

    private PlayerScript playerData;
    private float maxHeight;
    private int currentBestScore;

	// Use this for initialization
	void Start () {
		playerData = player.GetComponent<PlayerScript>();
        currentBestScore = LoadScore();
        recordText.GetComponent<Text>().text = "Best: " + currentBestScore;
	}
	
	// Update is called once per frame
	void Update () {
        bestScoreLine.position = new Vector3(Camera.main.transform.position.x, Mathf.Max(currentBestScore, maxHeight), 0);

		if(!playerData.gameStarted) return;

        recordText.SetActive(false);
        
        if(player.transform.position.y > maxHeight)
            maxHeight = player.transform.position.y;

        score = (int) maxHeight;
        scoreText.GetComponent<Text>().text = score.ToString();

        if(playerData.isDead) {
            int best = LoadScore();
            if(score > best) {
                newHighScore = true;
                SaveScore();
            }
            Destroy(gameObject);
        }
	}

    void SaveScore() {
        Debug.Log("Saving score " + score + " to " + Application.persistentDataPath);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Create(Application.persistentDataPath + "/best.score");
        bf.Serialize(fs, score);
        fs.Close();
    }

    public static int LoadScore() {
        if(File.Exists(Application.persistentDataPath + "/best.score")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + "/best.score", FileMode.Open);
            int score = (int) bf.Deserialize(fs);
            fs.Close();
            Debug.Log("Loaded score " + score);
            return score;
        }
        return 0;
    }
}
