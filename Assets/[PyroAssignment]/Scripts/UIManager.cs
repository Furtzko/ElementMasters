using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static bool isGamePaused;
    public GameObject pauseMenuUI;
    private GameObject touchToStart;
    private GameObject pauseBtn;
    public GameObject levelFailUI;
    public GameObject levelCompletedUI;

    private void OnEnable()
    {
        EventManager.OnLevelFail += LevelFail;
        EventManager.OnLevelCompleted += LevelCompleted;
    }

    private void OnDisable()
    {
        EventManager.OnLevelFail -= LevelFail;
        EventManager.OnLevelCompleted -= LevelCompleted;
    }

    private void Start()
    {
        Time.timeScale = 0f;
        isGamePaused = true;
        touchToStart = transform.GetChild(0).gameObject;
        pauseBtn = transform.GetChild(1).gameObject;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Time.timeScale = 1f;
            isGamePaused = false;
            touchToStart.SetActive(false);
            pauseBtn.SetActive(true);
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LevelFail()
    {
        levelFailUI.SetActive(true);
    }

    public void LevelCompleted()
    {
        GameObject hadouken = GameObject.FindGameObjectsWithTag("Hadouken").Where(i => i.GetComponent<SwervingController>().isActiveAndEnabled).FirstOrDefault();
        HadoukenController controller = hadouken.GetComponent<HadoukenController>();
        transform.GetChild(4).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = controller.score.ToString();
        transform.GetChild(4).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "x " + controller.multiplierValue.ToString();

        levelCompletedUI.SetActive(true);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) %2);
    }
}
