using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public WindController controller;
    public List<Hole> holes;
    public GameUI gameUI;
    public ScoreboardUI scoreboardUI;
    public Button closeTutorialButton;

    private bool newBoundsEntered = false;
    private bool playerInBounds = false;

    public int stroke = 0;
    public int currentHole = 0;

    public float scoreBoardDurationSeconds = 5;
    private bool levelTransitioning = false;
    private float scoreboardTimer;
    private bool levelEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        gameUI.stroke.text = "Stroke: " + stroke;
        gameUI.windDirection.text = "Wind Direction: None";
        gameUI.windDuration.minValue = controller.minWindDuration;
        gameUI.windDuration.maxValue = controller.maxWindDuration;
        gameUI.windDuration.value = controller.windDuration;
        scoreboardUI.Initialize(holes);
        scoreboardUI.scoreLines[currentHole].scoreText.text = "0";
        controller.onStrokeTaken += () =>
        {
            stroke++;
            scoreboardUI.scoreLines[currentHole].scoreText.text = stroke.ToString();
            gameUI.stroke.text = "Stroke: " + stroke;
            gameUI.windDurationFillImage.material.color = gameUI.durationUnadjustableColor;
            float direction = 90 - Mathf.Atan2(controller.windDirection.z, controller.windDirection.x) * Mathf.Rad2Deg;
            if(direction < 0)
                direction += 360;
            gameUI.windDirection.text = "Wind Direction: " + direction;
        };
        controller.onWindStop += () =>
        {
            gameUI.windDirection.text = "Wind Direction: None";
        };
        controller.onWindDurationChanged += (float oldVal, float newVal) =>
        {
            if(!controller.ballMoving)
                gameUI.windDuration.value = newVal;
        };
        controller.onBallStop += () =>
        {
            gameUI.windDuration.value = controller.windDuration;
            newBoundsEntered = false;
            gameUI.windDurationFillImage.material.color = gameUI.durationAdjustableColor;
            if (holes.Any(hole => hole.goal.playerCollided))
            {
                Debug.Log("Goal!");
                if(currentHole + 1 < holes.Count)
                {
                    BeginTransitionLevel();
                }
                else
                {
                    levelEnded = true;
                    controller.enabled = false;
                    scoreboardUI.gameObject.SetActive(true);
                }
            }
        };
        foreach (Hole hole in holes)
        {
            foreach (TriggerCallbacks callback in hole.levelBounds)
            {
                callback.onTriggerEntered += (GameObject obj, Collider collider) =>
                {
                    newBoundsEntered = true;
                    playerInBounds = true;
                };
                callback.onTriggerExit += (GameObject obj, Collider collider) =>
                {
                    if (!newBoundsEntered)
                        playerInBounds = false;
                };
            }
        }
        BeginTransitionLevel();
        levelTransitioning = false;
        currentHole = 0;
        closeTutorialButton.onClick.AddListener(() =>
        {
            EndTransitionLevel();
        });
    }

    private void BeginTransitionLevel()
    {
        scoreboardTimer = 0;
        levelTransitioning = true;
        currentHole++;
        scoreboardUI.gameObject.SetActive(true);
        controller.enabled = false;
        gameUI.windDurationFillImage.material.color = gameUI.durationUnadjustableColor;
    }

    private void EndTransitionLevel()
    {
        scoreboardUI.gameObject.SetActive(false);
        levelTransitioning = false;
        controller.enabled = true;
        stroke = 0;
        Camera.main.transform.position = holes[currentHole].cameraPosition.transform.position;
        controller.playerBody.position = holes[currentHole].teePosition.transform.position;
        controller.lastHitPosition = holes[currentHole].teePosition.transform.position;
        gameUI.windDurationFillImage.material.color = gameUI.durationAdjustableColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInBounds)
        {
            controller.ResetLastPosition();
        }
        if (!levelEnded)
        {
            if (levelTransitioning)
            {
                scoreboardTimer += Time.deltaTime;
                if (scoreboardTimer >= scoreBoardDurationSeconds)
                {
                    EndTransitionLevel();
                }
            }
            else
            {
                scoreboardUI.gameObject.SetActive(Input.GetKey(KeyCode.Tab));
            }
        }
    }
}
