using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

  
public class SLoadGame : MonoBehaviour
{
    public void ChangeurDeScene(string SceneChange)
    {
        SceneManager.LoadScene(SceneChange);
    }
}
