using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManagerGame : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] Image healthBar;
    [SerializeField] ushort maxHealth;
    [SerializeField] SpriteRenderer background;
    [SerializeField] GameObject endGamePopUp;
    [SerializeField] TMP_Text nameLabel;
    [SerializeField] TMP_Text scoreLabel;
    [SerializeField] GameObject stick;
    [SerializeField] GameObject fireButton;
    private void Awake()
    {
        endGamePopUp.SetActive(false);
    }
    private void OnEnable()
    {
        PlayerNetwork.ChangedScoreEvent += ChangeTextEvent;
        PlayerNetwork.ChangedHealthEvent += ChangeHealthEvent;
    }
    private void OnDisable()
    {
        PlayerNetwork.ChangedScoreEvent -= ChangeTextEvent;
        PlayerNetwork.ChangedHealthEvent -= ChangeHealthEvent;
    }
    private void ChangeTextEvent(ushort score)
    {
        label.text = "Score: " + score.ToString();
    }

    private void ChangeHealthEvent(ushort health)
    {
        healthBar.fillAmount = (float)health / (float)maxHealth;
    }

    private void Start()
    {
        // Растягиваем фон на весь экран
        if (background != null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                float height = 2f * mainCamera.orthographicSize;
                float width = height * mainCamera.aspect;
                float size = Mathf.Max(width, height) / 10.0f;
                background.transform.localScale = new Vector3(size, size, 1f);
            }
        }
    }

    public void ShowEndPopUp(string name, int score)
    {
        fireButton.SetActive(false);
        stick.SetActive(false);
        endGamePopUp.SetActive(true);
        nameLabel.text = name + " wins!!!";
        scoreLabel.text = "His score " + score.ToString();

    }
}
