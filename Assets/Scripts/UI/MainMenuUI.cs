using System;
using GameManager;
using Sounds;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        #region Header OBJECT REFERENCES

        [Space(10)]
        [Header("OBJECT REFERENCES")]

        #endregion

        #region Tooltip

        [Tooltip("Populate with the enter the dungeon play button gameobject")]

        #endregion

        [SerializeField]
        private GameObject playButton;
        
        #region Tooltip

        [Tooltip("Populate with the quit button gameobject")]

        #endregion

        [SerializeField]
        private GameObject quitButton;
        
        #region Tooltip

        [Tooltip("Populate with the high scores button gameobject")]

        #endregion

        [SerializeField]
        private GameObject highScoresButton;
        
        #region Tooltip

        [Tooltip("Populate with the instructions button gameobject")]

        #endregion

        [SerializeField]
        private GameObject instructionsButton;
        
        #region Tooltip

        [Tooltip("Populate with the return to main menu button gameobject")]

        #endregion

        [SerializeField]
        private GameObject returnToMainMenuButton;

        private bool isHighScoresSceneLoaded = false;
        private bool isInstructionsSceneLoaded = false;
        private void Start()
        {
            MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);
            SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
            returnToMainMenuButton.SetActive(false);
        }

        public void PlayGame()
        {
            SceneManager.LoadScene("MainGameScene");
        }
        
        public void LoadHighScores()
        {
            playButton.SetActive(false);
            quitButton.SetActive(false);
            highScoresButton.SetActive(false);
            instructionsButton.SetActive(false);
            isHighScoresSceneLoaded = true;
            SceneManager.UnloadSceneAsync("CharacterSelectorScene");
            returnToMainMenuButton.SetActive(true);

            SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
        }

        public void LoadCharacterSelector()
        {
            returnToMainMenuButton.SetActive(false);
            if (isHighScoresSceneLoaded)
            {
                SceneManager.UnloadSceneAsync("HighScoreScene");
                isHighScoresSceneLoaded = false;
            }
            else if (isInstructionsSceneLoaded)
            {
                SceneManager.UnloadSceneAsync("InstructionScene");
                isInstructionsSceneLoaded = false;
            }

            playButton.SetActive(true);
            quitButton.SetActive(true);
            highScoresButton.SetActive(true);
            instructionsButton.SetActive(true);

            SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
        }

        public void LoadInstructions()
        {
            playButton.SetActive(false);
            quitButton.SetActive(false);
            highScoresButton.SetActive(false);
            instructionsButton.SetActive(false);
            isInstructionsSceneLoaded = true;
            SceneManager.UnloadSceneAsync("CharacterSelectorScene");
            returnToMainMenuButton.SetActive(true);

            SceneManager.LoadScene("InstructionScene", LoadSceneMode.Additive);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        

        #region Validation

#if UNITY_EDITOR

        private void OnValidate()
        {
            HelperUtilities.ValidateCheckNullValues(this, nameof(playButton), playButton);
            HelperUtilities.ValidateCheckNullValues(this, nameof(quitButton), quitButton);
            HelperUtilities.ValidateCheckNullValues(this, nameof(highScoresButton), highScoresButton);
            HelperUtilities.ValidateCheckNullValues(this, nameof(instructionsButton), instructionsButton);
            HelperUtilities.ValidateCheckNullValues(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
        }

#endif

        #endregion
    }
}