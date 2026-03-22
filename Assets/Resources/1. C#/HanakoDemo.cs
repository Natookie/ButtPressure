using UnityEngine;

public class HanakoDemo : MonoBehaviour
{
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject wonGameUI;
    [SerializeField] private Door door;

    public void OpenDemo(){
        endGameUI.SetActive(false);
        wonGameUI.SetActive(false);
        GameManager.Instance.isEnded = false;
    }
}