using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public int blockValue;
    public bool isLocked;
    public Color activeColor;
    public Color lockedColor;
    private Image blockImage;

    private void Start()
    {
        blockImage = GetComponent<Image>();
        UpdateBlockAppearance();
    }

    public void ToggleLock()
    {
        isLocked = !isLocked;
        UpdateBlockAppearance();
    }

    public void UpdateBlockAppearance()
    {
        blockImage.color = isLocked ? lockedColor : activeColor;
    }

    public void OnBlockClicked()
    {
        if (!isLocked)
        {
            // Logique pour déplacer le bloc ou effectuer une action
            
        }
    }
}
