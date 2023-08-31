using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text arrowsText;
    private int points = 0;

    private void Start() {
        Arrow_OnPointsAdded(0);
    }

    private void OnDisable() {
        PullInteraction.OnTotalArrowsUpdated -= PullInteraction_OnTotalArrowsUpdated;
        PullInteraction.OnArrowsFinished -= PullInteraction_OnArrowsFinished;
        Arrow.OnPointsAdded -= Arrow_OnPointsAdded;
    }

    private void OnEnable() {
        PullInteraction.OnTotalArrowsUpdated += PullInteraction_OnTotalArrowsUpdated;
        PullInteraction.OnArrowsFinished += PullInteraction_OnArrowsFinished;
        Arrow.OnPointsAdded += Arrow_OnPointsAdded;
    }

    private void Arrow_OnPointsAdded(int value) {
        points += value;
        pointsText.text = $"{points}";
    }

    private void PullInteraction_OnArrowsFinished() {
        Reload();
    }

    private void Reload() {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private void PullInteraction_OnTotalArrowsUpdated(int value) {
        arrowsText.text = $"{value}";
    }
}
