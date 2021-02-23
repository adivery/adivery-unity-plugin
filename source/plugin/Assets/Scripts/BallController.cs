using System;
using System.Collections.Generic;
using UnityEngine;
using AdiveryUnity;

/// <summary>
/// Controller for the ball GameObject.
/// </summary>
public class BallController : MonoBehaviour
{
    public float Speed;

    private int coinCount;
    private Rigidbody rb;
    private float horizontalMovement;
    private float verticalMovement;
    private Rect gameOverDialogBounds;
    private bool gameOver;
    private int pickUpsRemaining;
    private List<GameObject> pickUps;

    private RewardedAd rewarded;

    public void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
        this.pickUps = new List<GameObject>();

        for(int i = 1; i < 5; i++)
        {
            this.pickUps.Add(GameObject.Find("Pick Up " + i));
        }

        rewarded = new RewardedAd("3f97dc4d-3e09-4024-acaf-931862c03ba8");
        rewarded.OnAdLoaded += OnRewardedAdLoaded;
        rewarded.OnAdLoadFailed += OnRewardedAdLoadFailed;
        rewarded.OnAdShown += OnRewardedAdShown;
        rewarded.OnAdShowFailed += OnRewardedAdShowFailed;
        rewarded.OnAdClicked += OnRewardedAdClicked;
        rewarded.OnAdRewarded += OnRewardedAdRewarded;
        rewarded.OnAdClosed += OnRewardedAdClosed;
        rewarded.LoadAd();

        this.ResetGame();
    }

    public void FixedUpdate()
    {
        Vector3 movement = new Vector3(this.horizontalMovement, 0, this.verticalMovement);
        this.rb.AddForce(movement * this.Speed);

        this.horizontalMovement = 0;
        this.verticalMovement = 0;
    }

    public void OnGUI()
    {
        GUIStyle coinCountLabelStyle = new GUIStyle();
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        coinCountLabelStyle.alignment = TextAnchor.LowerRight;
        coinCountLabelStyle.fontSize = (int)Math.Round(Screen.height * 0.06);
        coinCountLabelStyle.normal.textColor = Color.green;
        string coinCountText = "Coins: " + this.coinCount;
        GUI.Label(rect, coinCountText, coinCountLabelStyle);

        GUIStyle buttonStyle = new GUIStyle("button");
        buttonStyle.fontSize = (int)Math.Round(Screen.height * 0.05);
        buttonStyle.stretchHeight = true;
        buttonStyle.stretchWidth = true;
        buttonStyle.wordWrap = true;
        buttonStyle.margin = new RectOffset(0, 0, 0, 0);

        if (!this.gameOver)
        {
            float size = Mathf.Min(Screen.height, Screen.width) * 0.5f;

            GUILayout.BeginArea(new Rect(0, Screen.height - size - 10, size, size));
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.RepeatButton(
                    "▲",
                    buttonStyle,
                    GUILayout.Width(size / 3),
                    GUILayout.Height(size / 3)))
            {
                this.verticalMovement = 1;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.RepeatButton(
                    "◀",
                    buttonStyle,
                    GUILayout.Width(size / 3),
                    GUILayout.Height(size / 3)))
            {
                this.horizontalMovement = -1;
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.RepeatButton(
                    "▶",
                    buttonStyle,
                    GUILayout.Width(size / 3),
                    GUILayout.Height(size / 3)))
            {
                this.horizontalMovement = 1;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.RepeatButton(
                    "▼",
                    buttonStyle,
                    GUILayout.Width(size / 3),
                    GUILayout.Height(size / 3)))
            {
                this.verticalMovement = -1;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        GUIStyle windowStyle = new GUIStyle("window");
        windowStyle.fontSize = (int)(Screen.height * 0.03);
        windowStyle.wordWrap = true;

        if (this.gameOver)
        {
            Rect windowRect = new Rect(
                    Screen.width * 0.15f,
                    Screen.height * 0.33f,
                    Screen.width * 0.7f,
                    Screen.height * 0.33f);
            this.gameOverDialogBounds = GUI.Window(
                    0,
                    windowRect,
                    this.WindowFunction,
                    "Game Over. Want to play again?",
                    windowStyle);
        }
    }

    /// <summary>
    /// Handles the trigger enter event.
    /// </summary>
    /// <param name="other">Collider of other GameObject in collision.</param>
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Pick Up"))
        {
            other.gameObject.SetActive(false);
            this.pickUpsRemaining--;
            this.coinCount++;
        }

        if (this.pickUpsRemaining == 0)
        {
            this.gameOver = true;
            this.StopMovement();
        }
    }

    /// <summary>
    /// Starts a new game by making pick ups active.
    /// </summary>
    private void ResetGame()
    {
        this.pickUpsRemaining = 4;
        this.gameOver = false;
        transform.position = new Vector3(0f, 0.5f, 0f);

        foreach (GameObject pickUp in this.pickUps)
        {
            pickUp.SetActive(true);
        }
    }

    /// <summary>
    /// Defines layout of game over window.
    /// </summary>
    /// <param name="windowID">WindowID.</param>
    private void WindowFunction(int windowID)
    {
        GUIStyle buttonStyle = new GUIStyle("button");
        buttonStyle.fontSize = (int)(Screen.height * 0.02f);
        buttonStyle.stretchHeight = true;
        buttonStyle.wordWrap = true;

        GUILayout.BeginArea(new Rect(
                this.gameOverDialogBounds.width * 0.15f,
                this.gameOverDialogBounds.height * 0.25f,
                this.gameOverDialogBounds.width * 0.7f,
                this.gameOverDialogBounds.height * 0.55f));

        GUILayout.BeginVertical();

        if (GUILayout.Button("Restart Game", buttonStyle))
        {
            this.ResetGame();
            print("Showing rewarded ad " + rewarded.IsLoaded());
            if (rewarded.IsLoaded())
            {
                rewarded.Show();
            }
        }

        if (GUILayout.Button("Dismiss", buttonStyle))
        {
            this.gameOver = false;
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    /// <summary>
    /// Stops the movement of the ball.
    /// </summary>
    private void StopMovement()
    {
        this.horizontalMovement = 0;
        this.verticalMovement = 0;
        this.rb.velocity = Vector3.zero;
        this.rb.angularVelocity = Vector3.zero;
    }

    // Rewarded Events

    public void OnRewardedAdLoaded(object caller, EventArgs args)
    {
        LogEvent("OnRewardedAdLoaded");
    }

    private void OnRewardedAdLoadFailed(object sender, int e)
    {
        LogEvent("OnRewardedAdLoadFailed");
    }

    private void OnRewardedAdShowFailed(object sender, int e)
    {
        LogEvent("OnRewardedAdShowFailed");
    }

    private void OnRewardedAdShown(object sender, EventArgs e)
    {
        LogEvent("OnRewardedAdShown");
    }

    private void OnRewardedAdClicked(object sender, EventArgs e)
    {
        LogEvent("OnRewardedAdClicked");
    }


    private void OnRewardedAdRewarded(object sender, EventArgs e)
    {
        LogEvent("OnRewardedAdRewarded");
    }

    private void OnRewardedAdClosed(object sender, EventArgs e)
    {
        LogEvent("OnRewardedAdClosed");
        rewarded.LoadAd();
    }

    private void LogEvent(string evt)
    {
        print("Adivery Ad Event: " + evt);
    }

}
