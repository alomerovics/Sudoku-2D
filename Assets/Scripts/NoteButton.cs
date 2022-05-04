using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoteButton : Selectable,IPointerClickHandler
{
    public Sprite on_Image;
    public Sprite off_Image;
    private bool active;
    void Start()
    {
        active = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        active = !active;
        if (active)
            GetComponent<Image>().sprite = on_Image;
        else
            GetComponent<Image>().sprite = off_Image;
        GameEvents.OnNotesActiveMethod(active);
    }
}
