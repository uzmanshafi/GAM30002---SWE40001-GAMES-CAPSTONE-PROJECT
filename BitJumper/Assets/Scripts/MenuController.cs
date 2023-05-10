using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
   [Header("Levels To Load")]
   public string _nameGameLevel;
   private string levelToLoad;
   [SerializeField] private GameObject noSavedGameDialog = null;

   public void NewGameDialogYes()
   {
        SceneManager.LoadScene(_nameGameLevel);
   }

   public void LoadGameDialogYes()
   {
        if(PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
   }

   public void ExitButton()
   {
        Application.Quit();
   }
}
