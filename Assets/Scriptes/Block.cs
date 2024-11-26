using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public int blockValue;
    public bool isLocked;


    public void Lock()
    {
        isLocked = true;
        GetComponent<Image>().color = Color.gray;
    }
    public void Unlock() {
        isLocked = false;
        GetComponent<Image>().color = Color.white;
    }



}
