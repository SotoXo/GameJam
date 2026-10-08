using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ControladorMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelInstrucciones;
    [SerializeField] private GameObject botonInstrucciones;
    [SerializeField] private GameObject botonVolver;

    public void Jugar()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    public void MostrarInstrucciones()
    {
        panelMenu.SetActive(false);
        panelInstrucciones.SetActive(true);
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(botonVolver);
    }

    public void OcultarInstrucciones()
    {
        panelInstrucciones.SetActive(false);
        panelMenu.SetActive(true);
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(botonInstrucciones);
    }

    public void Salir()
    {
        Application.Quit();
    }
}
