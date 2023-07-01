using System;
using System.Collections;
using System.Collections.Generic;
using Dungeon;
using Enemies;
using Health;
using Misc;
using Player;
using StaticEvents;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace GameManager
{
    [DisallowMultipleComponent]
    public class GameManager : SingletonMonobehaviour<GameManager>
    {
        #region Header GAMEOBJECT REFERENCES

        [Space(10)]
        [Header("GAMEOBJECT REFERENCES")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with pause menu gameobject in hierarchy")]

        #endregion

        [SerializeField]
        private GameObject pauseMenu;

        #region Tooltip

        [Tooltip("Populate with the MessageText textmeshpro component in the FadeScreenUI")]

        #endregion

        [SerializeField]
        private TextMeshProUGUI messageTextTMP;

        #region Tooltip

        [Tooltip("Populate with the FadeImage canvasgroup component in the FadeScreenUI")]

        #endregion

        [SerializeField]
        private CanvasGroup canvasGroup;
        
        #region Header DUNGEON LEVELS

        [Space(10)]
        [Header("DUNGEON LEVELS")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with the dungeon level scriptable objects")]

        #endregion

        [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

        #region Tooltip

        [Tooltip("Populate with the starting dungeon level for testing, first level = 0")]

        #endregion

        [SerializeField] private int currentDungeonLevelListIndex = 0;

        private Room currentRoom;
        private Room previousRoom;
        private PlayerDetailsSO playerDetails;
        private Player.Player player;
        
        [HideInInspector] public GameState gameState;
        [HideInInspector] public GameState previousGameState;

        private long gameScore;
        private int scoreMultiplier;
        private InstantiatedRoom bossRoom;
        private bool isFading = false;

        protected override void Awake()
        {
            base.Awake();
            playerDetails = GameResources.Instance.currentPlayer.playerDetails;

            InstantiatePlayer();
        }

        private void InstantiatePlayer()
        {
            GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);
            player = playerGameObject.GetComponent<Player.Player>();
            player.Initialize(playerDetails);
        }

        private void OnEnable()
        {
            StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
            StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
            StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;
            StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;
            player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
        }
        
        private void OnDisable()
        {
            StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
            StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
            StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;
            StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;
            player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;
        }
        
        private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
        {
            SetCurrentRoom(roomChangedEventArgs.room);
        }
        
        private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
        {
            RoomEnemiesDefeated();
        }
        
        private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
        {
            gameScore += pointsScoredArgs.points * scoreMultiplier;
            StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
        }
        
        private void StaticEventHandler_OnMultiplier(MultiplierArgs multiplierArgs)
        {
            if (multiplierArgs.multiplier)
            {
                scoreMultiplier++;
            }
            else
            {
                scoreMultiplier--;
            }

            scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);
            
            StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
        }

        private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
        {
            previousGameState = gameState;
            gameState = GameState.gameLost;
        }
        
        private void Start()
        {
            previousGameState = GameState.gameStarted;
            gameState = GameState.gameStarted;
            gameScore = 0;
            scoreMultiplier = 1;

            StartCoroutine(Fade(0f, 1f, 0f, Color.black));
        }

        private void Update()
        {
            HandleGameState();
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     gameState = GameState.gameStarted;
            // }
        }

        private void HandleGameState()
        {
            switch (gameState)
            {
                case GameState.gameStarted:
                    PlayDungeonLevel(currentDungeonLevelListIndex);
                    gameState = GameState.playingLevel;
                    RoomEnemiesDefeated();
                    break;
                case GameState.playingLevel:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        PauseGameMenu();
                    }
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        DisplayDungeonOverviewMap();
                    }
                    break;
                case GameState.engagingEnemies:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        PauseGameMenu();
                    }
                    break;
                case GameState.dungeonOverviewMap:
                    if (Input.GetKeyUp(KeyCode.Tab))
                    {
                        DungeonMap.DungeonMap.Instance.ClearDungeonOverviewMap();
                    }
                    break;
                case GameState.bossStage:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        PauseGameMenu();
                    }
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        DisplayDungeonOverviewMap();
                    }
                    break;
                case GameState.engagingBoss:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        PauseGameMenu();
                    }

                    break;
                case GameState.levelCompleted:
                    StartCoroutine(LevelCompleted());
                    break;
                case GameState.gameWon:
                    if (previousGameState != GameState.gameWon)
                    {
                        StartCoroutine(GameWon());
                    }
                    break;
                case GameState.gameLost:
                    if (previousGameState != GameState.gameLost)
                    {
                        StopAllCoroutines();
                        StartCoroutine(GameLost());
                    }
                    break;
                case GameState.restartGame:
                    RestartGame();
                    break;
                case GameState.gamePaused:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        PauseGameMenu();
                    }
                    break;
            }
        }
        
        public void SetCurrentRoom(Room room)
        {
            previousRoom = currentRoom;
            currentRoom = room;
        }
        
        private void RoomEnemiesDefeated()
        {
            bool isDungeonClearOfRegularEnemies = true;
            bossRoom = null;

            foreach (var keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
            {
                if (keyValuePair.Value.roomNodeType.isBossRoom)
                {
                    bossRoom = keyValuePair.Value.instantiatedRoom;
                    continue;
                }

                if (!keyValuePair.Value.isClearedOfEnemies)
                {
                    isDungeonClearOfRegularEnemies = false;
                    break;
                }
            }

            if ((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
            {
                if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
                {
                    gameState = GameState.levelCompleted;
                }
                else
                {
                    gameState = GameState.gameWon;
                }
            }
            else if (isDungeonClearOfRegularEnemies)
            {
                gameState = GameState.bossStage;
                StartCoroutine(BossStage());
            }
        }

        public void PauseGameMenu()
        {
            if (gameState != GameState.gamePaused)
            {
                pauseMenu.SetActive(true);
                GetPlayer().playerControl.DisablePlayer();
                previousGameState = gameState;
                gameState = GameState.gamePaused;
            }
            else if (gameState == GameState.gamePaused)
            {
                pauseMenu.SetActive(false);
                GetPlayer().playerControl.EnablePlayer();
                gameState = previousGameState;
                previousGameState = GameState.gamePaused;
            }
        }
        
        private void DisplayDungeonOverviewMap()
        {
            if (isFading)
            {
                return;
            }   
            DungeonMap.DungeonMap.Instance.DisplayDungeonOverviewMap();
        }

        private void PlayDungeonLevel(int dungeonLevelIndex)
        {
            bool dungeonBuiltSuccessfully =
                DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelIndex]);
            if (!dungeonBuiltSuccessfully)
            {
                Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
            }
            
            StaticEventHandler.CallRoomChangedEvent(currentRoom);
            
            player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
                (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0);

            player.gameObject.transform.position =
                HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);

            StartCoroutine(DisplayDungeonLevelText());
        }

        private IEnumerator DisplayDungeonLevelText()
        {
            StartCoroutine(Fade(0f, 1f, 0f, Color.black));
            
            GetPlayer().playerControl.DisablePlayer();

            string textMessage =
                $"LEVEL {currentDungeonLevelListIndex + 1}\n\n{dungeonLevelList[currentDungeonLevelListIndex].levelName.ToUpper()}";
            yield return StartCoroutine(DisplayMessageRoutine(textMessage, Color.white, 2f));
            GetPlayer().playerControl.EnablePlayer();

            yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
        }

        private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
        {
            messageTextTMP.SetText(text);
            messageTextTMP.color = textColor;

            if (displaySeconds > 0f)
            {
                float timer = displaySeconds;
                while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (!Input.GetKeyDown(KeyCode.Return))
                {
                    yield return null;
                }
            }

            yield return null;
            
            messageTextTMP.SetText("");
        }

        private IEnumerator BossStage()
        {
            bossRoom.gameObject.SetActive(true);
            bossRoom.UnlockDoors(0f);

            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));
            yield return StartCoroutine(DisplayMessageRoutine(
                $"WELL DONE  {GameResources.Instance.currentPlayer.playerName}! YOU'VE SURVIVED .... SO FAR\n\nNOW FIND AND DEFEAT THE BOSS....GOOD LUCK!",
                Color.white, 5f));
            yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
        }
        
        private IEnumerator LevelCompleted()
        {
            gameState = GameState.playingLevel;
            yield return new WaitForSeconds(2f);
            
            yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));
            yield return StartCoroutine(DisplayMessageRoutine(
                $"WELL DONE  {GameResources.Instance.currentPlayer.playerName}! YOU'VE SURVIVED THIS DUNGEON LEVEL",
                Color.white, 5f));
            yield return StartCoroutine(DisplayMessageRoutine(
                $"COLLECT ANY LOOT ....THEN PRESS RETURN\n\nTO DESCEND FURTHER INTO THE DUNGEON",
                Color.white, 5f));
            yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }

            yield return null;
            currentDungeonLevelListIndex++;
            PlayDungeonLevel(currentDungeonLevelListIndex);
        }

        public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
        {
            isFading = false;
            Image image = canvasGroup.GetComponent<Image>();
            image.color = backgroundColor;
            
            float time = 0f;
            while (time <= fadeSeconds)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
                yield return null;
            }

            isFading = false;
        }
        
        private IEnumerator GameWon()
        {
            previousGameState = GameState.gameWon;
            
            GetPlayer().playerControl.DisablePlayer();

            int rank = HighScoreManager.Instance.GetRank(gameScore);
            string rankText;
            if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
            {
                rankText = $"YOUR SCORE IS RANKED {rank} IN THE TOP {Settings.numberOfHighScoresToSave:#0}";
                string name = GameResources.Instance.currentPlayer.playerName;
                if (name == "")
                {
                    name = playerDetails.playerCharacterName.ToUpper();
                }
                HighScoreManager.Instance.AddScore(new Score()
                {
                    playerName = name,
                    levelDescription = $"LEVEL {currentDungeonLevelListIndex + 1} - {GetCurrentDungeonLevel().levelName.ToUpper()}",
                    playerScore = gameScore
                }, rank);
            }
            else
            {
                rankText = $"YOUR SCORE INS'T RANKED IN THE TOP {Settings.numberOfHighScoresToSave:#0}";
            }
            yield return new WaitForSeconds(1f);
            
            
            yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));
            yield return StartCoroutine(DisplayMessageRoutine(
                $"WELL DONE  {GameResources.Instance.currentPlayer.playerName}! YOU'VE DEFEATED THE DUNGEON",
                Color.white, 3f));
            yield return StartCoroutine(DisplayMessageRoutine(
                $"YOU SCORED {gameScore:###,###0} \n\n{rankText}",
                Color.white, 4f));
            yield return StartCoroutine(DisplayMessageRoutine(
                $"PRESS RETURN TO RESTART GAME",
                Color.white, 0f));

            gameState = GameState.restartGame; 
        }

        private IEnumerator GameLost()
        {
            previousGameState = GameState.gameLost;
            
            GetPlayer().playerControl.DisablePlayer();
            
            int rank = HighScoreManager.Instance.GetRank(gameScore);
            string rankText;
            if (rank > 0 && rank <= Settings.numberOfHighScoresToSave)
            {
                rankText = $"YOUR SCORE IS RANKED {rank} IN THE TOP {Settings.numberOfHighScoresToSave:#0}";
                string name = GameResources.Instance.currentPlayer.playerName;
                if (name == "")
                {
                    name = playerDetails.playerCharacterName.ToUpper();
                }
                HighScoreManager.Instance.AddScore(new Score()
                {
                    playerName = name,
                    levelDescription = $"LEVEL {currentDungeonLevelListIndex + 1} - {GetCurrentDungeonLevel().levelName.ToUpper()}",
                    playerScore = gameScore
                }, rank);
            }
            else
            {
                rankText = $"YOUR SCORE INS'T RANKED IN THE TOP {Settings.numberOfHighScoresToSave:#0}";
            }
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

            Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();
            foreach (var enemy in enemyArray)
            {
                enemy.gameObject.SetActive(false);
            }
            
            yield return StartCoroutine(DisplayMessageRoutine(
                $"BAD LUCK  {GameResources.Instance.currentPlayer.playerName}! YOU'VE SUCCUMBED TO THE DUNGEON",
                Color.white, 2f));
            yield return StartCoroutine(DisplayMessageRoutine(
                $"YOU SCORED {gameScore:###,###0}\n\n{rankText}",
                Color.white, 4f));
            yield return StartCoroutine(DisplayMessageRoutine(
                $"PRESS RETURN TO RESTART THE GAME",
                Color.white, 0f));
            
            gameState = GameState.restartGame;
        }

        private void RestartGame()
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        public Player.Player GetPlayer()
        {
            return player;
        }

        public Sprite GetPlayerMiniMapIcon()
        {
            return playerDetails.playerMiniMapIcon;
        }
        
        public Room GetCurrentRoom()
        {
            return currentRoom;
        }

        public DungeonLevelSO GetCurrentDungeonLevel()
        {
            return dungeonLevelList[currentDungeonLevelListIndex];
        }

        #region Editor

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
            HelperUtilities.ValidateCheckNullValues(this, nameof(canvasGroup), canvasGroup);
            HelperUtilities.ValidateCheckNullValues(this, nameof(pauseMenu), pauseMenu);
            HelperUtilities.ValidateCheckNullValues(this, nameof(messageTextTMP), messageTextTMP);
        }

#endif

        #endregion
    }
}