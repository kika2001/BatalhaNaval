using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutroScreen : MonoBehaviour
{

    public void ReturnMainMenu()
    {
        RoomManager.Instance.CloseGame();
    }
}
