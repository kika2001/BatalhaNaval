using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class RoomManager : MonoBehaviour
    {
        private static RoomManager instance;
        public static RoomManager Instance => instance;
        [SerializeField] private GameObject roomsParent;

        private void Awake()
        {
            if (instance!=null)
            {
                Destroy(this.gameObject);
            }
            instance = this;
            DontDestroyOnLoad(this);
        }

        public void OpenRoom(GameObject go)
        {
            var count = roomsParent.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject g = roomsParent.transform.GetChild(i).gameObject;
                if (go!=g)
                {
                    g.SetActive(false);
                }
                else
                {
                    g.SetActive(true);
                }
            
            }
        }

        public void OpenGame()
        {
            if (SceneManager.GetActiveScene().name != "Game")
            {
                SceneManager.LoadScene("Game");
            }
            
        }

        public void CloseGame()
        {
            if (SceneManager.GetActiveScene().name != "Menu")
            {
                SceneManager.LoadScene("Menu");
            }
        }

        public void Victory()
        {
            if (SceneManager.GetActiveScene().name != "Victory")
            {
                SceneManager.LoadScene("Victory");
            }
        }
        public void Lose()
        {
            if (SceneManager.GetActiveScene().name != "Lost")
            {
                SceneManager.LoadScene("Lost");
            }
        }
        public void OpenRoom(string go)
        {
            var count = roomsParent.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject g = roomsParent.transform.GetChild(i).gameObject;
                if (go!=g.name)
                {
                    g.SetActive(false);
                }
                else
                {
                    g.SetActive(true);
                }
            
            }
        }
    }
}