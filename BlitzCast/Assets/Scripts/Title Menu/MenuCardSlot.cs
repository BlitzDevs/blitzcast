using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuCardSlot : CardSlot, IPointerClickHandler
{
    public string sceneName;

    public void OnPointerClick(PointerEventData eventData)
    {
        Activate();
    }

    public void Activate()
    {
        SceneManager.LoadScene(sceneName);
    }

}
