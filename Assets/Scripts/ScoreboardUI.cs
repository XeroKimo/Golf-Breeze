using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardUI : MonoBehaviour
{
    public ScoreLine scoreLinePrefab;
    public GameObject scoreLineParent;
    public List<ScoreLine> scoreLines = new List<ScoreLine>();
    public Button quitButton;

    private void Start()
    {
        quitButton.onClick.AddListener(() => { Debug.Log("Quitting game"); Application.Quit(); });
    }

    public void Initialize(List<Hole> holes)
    {
        foreach(ScoreLine score in scoreLines)
        {
            Destroy(score.gameObject);
        }
        scoreLines.Clear();

        int i = 0;
        foreach(Hole hole in holes)
        {
            ScoreLine scoreLine = Instantiate(scoreLinePrefab, scoreLineParent.transform);
            scoreLines.Add(scoreLine);
            scoreLine.holeText.text = (i + 1).ToString();
            scoreLine.parText.text = hole.par.ToString();
            scoreLine.scoreText.text = " ";
            i++;
        }
    }
}
