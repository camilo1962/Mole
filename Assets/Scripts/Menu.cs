using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
   public void Salir()
    {
        Application.Quit();
    }

    public void CambiarEscena(string nombre)
    {
        SceneManager.LoadScene(nombre);
    }
}
