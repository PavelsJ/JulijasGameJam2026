using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private PlayerPreview playerPreview;
    private PlayerBase playerBase;
    
    public void Init(PlayerBase player)
    {
        playerBase = player;
    }

    public void TickUpdate()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        playerPreview.UpdatePreview(mouseWorld);
    }
    
    public void UpdateAim(Vector3 player, Vector3 start, Vector3 current, float maxDistance)
    {
        playerPreview.UpdateAim(player,start, current, maxDistance);
    }

    public void ShowAction()
    {
        playerPreview.ShowPreview(1);
    }

    public void HideAction()
    {
        playerPreview.ClearPreview();
    }
    
    public void WrongAction()
    {
        playerPreview.ShakePreview(0.2f);
    }
}
