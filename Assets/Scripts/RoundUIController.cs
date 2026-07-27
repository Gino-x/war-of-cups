using System.Collections;
using UnityEngine;
using TMPro;

public class RoundUIController : MonoBehaviour
{
    [Header("Persistent Round Display (left side)")]
    [SerializeField] private TextMeshProUGUI roundText;

    [Header("Round Announcement Popup")]
    [SerializeField] private CanvasGroup announcementGroup;
    [SerializeField] private TextMeshProUGUI announcementText;
    [SerializeField] private float fadeInTime = 0.3f;
    [SerializeField] private float holdTime = 1.5f;
    [SerializeField] private float fadeOutTime = 0.5f;

    private Coroutine announceRoutine;

    public void UpdateRoundText(int round)
    {
        if (roundText != null)
            roundText.text = "Round " + round;
    }

    public void ShowRoundAnnouncement(int round)
    {
        if (announcementGroup == null)
            return;

        if (announceRoutine != null)
            StopCoroutine(announceRoutine);

        announceRoutine = StartCoroutine(AnnounceRoutine(round));
    }

    private IEnumerator AnnounceRoutine(int round)
    {
        if (announcementText != null)
            announcementText.text = "ROUND " + round;

        announcementGroup.gameObject.SetActive(true);

        yield return Fade(0f, 1f, fadeInTime);
        yield return new WaitForSeconds(holdTime);
        yield return Fade(1f, 0f, fadeOutTime);

        announcementGroup.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            announcementGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        announcementGroup.alpha = to;
    }
}