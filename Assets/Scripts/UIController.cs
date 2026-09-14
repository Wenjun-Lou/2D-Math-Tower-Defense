using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField] private Transform waveUI;
    [SerializeField] private Transform livesUI;
    [SerializeField] private Transform resourcesUI;

    [SerializeField] private GameObject towerSelectPanel;
    [SerializeField] private GameObject towerCardPrefab;
    [SerializeField] private Transform cardsContainer;

    [SerializeField] private TowerData[] towers;
    private List<GameObject> activeCards = new List<GameObject>();

    private Platform _currentPlatform;

    [SerializeField] private Button speedButton;
    [SerializeField] private Sprite speed1Sprite;
    [SerializeField] private Sprite speed2Sprite;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button nextLevelButton;

    [SerializeField] private GameObject pausePanel;
    private bool _isGamePaused = false;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text objectiveText;

    [SerializeField] private GameObject missionCompletePanel;
    private bool _missionCompleteSoundPlayed = false;
    [SerializeField] private ParticleSystem missionCompleteParticles;

    [SerializeField] private GameObject towerPanel;
    [SerializeField] private Transform towerInfo;
    private Tower _currentTower;

    [SerializeField] private QuestionUI questionUI;
    [SerializeField] private Button upgradeButton;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLivesText;
        GameManager.OnResourcesChanged += UpdateResourcesText;
        Platform.OnPlatformClicked += HandlePlatformClicked;
        Tower.OnTowerClicked += HandleTowerClicked;
        TowerCard.OnTowerSelected += HandleTowerSelected;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Spawner.OnMissionComplete += ShowMissionComplete;
        QuestionUI.OnAnswerCorrect += UpgradeTower;
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLivesText;
        GameManager.OnResourcesChanged -= UpdateResourcesText;
        Platform.OnPlatformClicked -= HandlePlatformClicked;
        Tower.OnTowerClicked -= HandleTowerClicked;
        TowerCard.OnTowerSelected -= HandleTowerSelected;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Spawner.OnMissionComplete -= ShowMissionComplete;
        QuestionUI.OnAnswerCorrect -= UpgradeTower;
    }

    private void Start()
    {
        speedButton.onClick.AddListener(ToggleGameSpeed);

        UpdateSpeedButtonImage();
    }

    private void ToggleGameSpeed()
    {
        if (_isGamePaused) return;
        if (GameManager.Instance.GameSpeed == 2f)
        {
            SetGameSpeed(4f);
            AudioManager.Instance.PlaySpeedFast();
        }
        else
        {
            SetGameSpeed(2f);
            AudioManager.Instance.PlaySpeedNormal();
        }

        UpdateSpeedButtonImage();
    }

    private void UpdateSpeedButtonImage()
    {
        if (GameManager.Instance.GameSpeed == 2f)
        {
            speedButton.image.sprite = speed1Sprite;
        }
        else if (GameManager.Instance.GameSpeed == 4f)
        {
            speedButton.image.sprite = speed2Sprite;
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "LevelSelect")
        {
            TogglePause();
        }
    }

    private void UpdateWaveText(int currentWave)
    {
        TMP_Text text = waveUI.GetComponentInChildren<TMP_Text>();
        text.text = $"{currentWave + 1}";
    }

    private void UpdateLivesText(int currentLives)
    {
        TMP_Text text = livesUI.GetComponentInChildren<TMP_Text>();
        text.text = $"{currentLives}";

        if (currentLives <= 0)
        {
            ShowGameOver();
        }
    }

    private void UpdateResourcesText(int currentResources)
    {
        TMP_Text text = resourcesUI.GetComponentInChildren<TMP_Text>();
        text.text = $"{currentResources}";
    }

    private void HandlePlatformClicked(Platform platform)
    {
        if (Tower.towerPanelOpen) return;
        _currentPlatform = platform;
        ShowTowerSelectPanel();
    }

    private void ShowTowerSelectPanel()
    {
        towerSelectPanel.SetActive(true);
        Platform.towerSelectPanelOpen = true;
        GameManager.Instance.SetTimeScale(0f);
        PopulateTowerCards();
        AudioManager.Instance.PlayPanelToggle();
    }

    public void HideTowerSelectPanel()
    {
        towerSelectPanel.SetActive(false);
        Platform.towerSelectPanelOpen = false;
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
    }

    private void PopulateTowerCards()
    {
        foreach (var card in activeCards)
        {
            Destroy(card);
        }
        activeCards.Clear();

        foreach (var data in towers)
        {
            GameObject cardGameObject = Instantiate(towerCardPrefab, cardsContainer);
            TowerCard card = cardGameObject.GetComponent<TowerCard>();
            card.Initialize(data);
            activeCards.Add(cardGameObject);
        }
    }

    private void HandleTowerClicked(Tower tower)
    {
        if (Platform.towerSelectPanelOpen) return;
        if (GameManager.Instance.GameSpeed == 0f) return;
        _currentTower = tower;
        ShowTowerPanel();
    }

    private void ShowTowerPanel()
    {
        towerPanel.SetActive(true);
        upgradeButton.interactable = true;
        questionUI.Initialize();
        Tower.towerPanelOpen = true;
        GameManager.Instance.SetTimeScale(0f);
        towerInfo.GetComponent<TowerInfoUI>().UpdateTowerInfo(_currentTower);
        AudioManager.Instance.PlayPanelToggle();
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        Tower.towerPanelOpen = false;
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
    }

    private void HandleTowerSelected(TowerData towerData)
    {
        if (_currentPlatform.transform.childCount > 0)
        {
            HideTowerSelectPanel();
            return;
        }
        if (GameManager.Instance.Resources >= towerData.cost)
        {
            AudioManager.Instance.PlayTowerPlaced();
            GameManager.Instance.SpendResources(towerData.cost);
            _currentPlatform.PlaceTower(towerData);
        }

        HideTowerSelectPanel();
    }

    private void SetGameSpeed(float timeScale)
    {
        GameManager.Instance.SetGameSpeed(timeScale);
    }

    public void TogglePause()
    {
        if (towerSelectPanel.activeSelf)
            return;

        if (_isGamePaused)
        {
            pausePanel.SetActive(false);
            _isGamePaused = false;
            GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
            AudioManager.Instance.PlayUnpause();
        }
        else
        {
            pausePanel.SetActive(true);
            _isGamePaused = true;
            GameManager.Instance.SetTimeScale(0f);
            AudioManager.Instance.PlayPause();
        }
    }

    public void RestartLevel()
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowGameOver()
    {
        GameManager.Instance.SetTimeScale(0f);
        gameOverPanel.SetActive(true);
        AudioManager.Instance.PlayGameOver();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = mainCamera;

        HidePanels();
        _isGamePaused = false;
        _missionCompleteSoundPlayed = false;

        if (scene.name == "MainMenu" || scene.name == "LevelSelect")
        {
            HideUI();
        }
        else
        {
            ShowUI();
            StartCoroutine(ShowObjective());
        }
    }

    private IEnumerator ShowObjective()
    {
        objectiveText.text = $"Survive {LevelManager.Instance.CurrentLevel.wavesToWin} waves!";
        objectiveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        objectiveText.gameObject.SetActive(false);
    }

    private void ShowMissionComplete()
    {
        if (!_missionCompleteSoundPlayed)
        {
            UpdateNextLevelButton();
            missionCompletePanel.SetActive(true);
            GameManager.Instance.SetTimeScale(0f);
            AudioManager.Instance.PlayMissionComplete();
            _missionCompleteSoundPlayed = true;
            missionCompleteParticles.Play();
        }

    }

    private void HideUI()
    {
        HidePanels();
        waveUI.gameObject.SetActive(false);
        livesUI.gameObject.SetActive(false);
        resourcesUI.gameObject.SetActive(false);

        speedButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
    }

    private void ShowUI()
    {
        waveUI.gameObject.SetActive(true);
        livesUI.gameObject.SetActive(true);
        resourcesUI.gameObject.SetActive(true);

        speedButton.gameObject.SetActive(true);
        UpdateSpeedButtonImage();
        pauseButton.gameObject.SetActive(true);
    }

    private void HidePanels()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        missionCompletePanel.SetActive(false);
    }

    public void LoadNextLevel()
    {
        var levelManager = LevelManager.Instance;
        int currentIndex = Array.IndexOf(levelManager.allLevels, levelManager.CurrentLevel);
        int nextIndex = currentIndex + 1;
        if (nextIndex < levelManager.allLevels.Length)
        {
            levelManager.LoadLevel(levelManager.allLevels[nextIndex]);
        }
    }

    private void UpdateNextLevelButton()
    {
        var levelManager = LevelManager.Instance;
        int currentIndex = Array.IndexOf(levelManager.allLevels, levelManager.CurrentLevel);
        nextLevelButton.interactable = currentIndex + 1 < levelManager.allLevels.Length;
    }

    public void ShowQuestionUI()
    {
        if (GameManager.Instance.Resources < _currentTower.UpgradeCost) return;
        GameManager.Instance.SpendResources(_currentTower.UpgradeCost);
        questionUI.ShowNextQuestion();
        upgradeButton.interactable = false;
    }

    public void UpgradeTower()
    {
        _currentTower.Upgrade();
        towerInfo.GetComponent<TowerInfoUI>().UpdateTowerInfo(_currentTower);
    }

    public void DestroyTower()
    {
        _currentTower.DestroyTower();
        HideTowerPanel();
    }
}
