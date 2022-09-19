using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickController : MonoBehaviour
{

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    Card heldCard;
    bool holding = false;
    Vector3 holdOffset;

    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        List<RaycastResult> results;
        if (Input.GetMouseButtonDown(0) && !holding){
            results = RayCast(Input.mousePosition);
            Card cardidate = results[0].gameObject.GetComponent<Card>();
            if (cardidate != null){
                if (cardidate.Pickup()){
                    heldCard = cardidate;
                    holding = true;
                    holdOffset = cardidate.transform.position - Input.mousePosition;
                }
            }
        }

        if (holding){
            heldCard.transform.position = Input.mousePosition + holdOffset;
        }

        if (Input.GetMouseButtonUp(0) && holding){
            if (AttemptPlace(Input.mousePosition)){

            }else if(AttemptPlace(heldCard.transform.position)){

            }else{
                heldCard.holder.holdParent.ReturnCard(heldCard.holder);
            }
            holding = false;
            heldCard = null;
            holdOffset = Vector3.zero;
        }
    }

    bool AttemptPlace(Vector3 pos){
        List<RaycastResult> results = RayCast(pos);
        int i = 1;
        while (results[i].gameObject.GetComponent<CardHolder>() != null && results[i].gameObject.GetComponent<CardHolder>().StackContains(heldCard.holder)){
            i++;
        }
        CardHolder hhh = results[i].gameObject.GetComponent<CardHolder>();
        if (hhh != null && hhh.CanPlace(heldCard.holder)){
            hhh.Place(heldCard.holder);
            return true;
        }else{
            return false;
        }
    }

    List<RaycastResult> RayCast(Vector2 p){
        //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = p;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            return results;
    }
}
