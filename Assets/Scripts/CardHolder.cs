using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHolder : MonoBehaviour
{
    public enum HoldType {Card,Slot,Stack};

    public HoldType holdType;

    public CardHolder[] cards;
    public CardHolder holdParent;
    public Card cardComp;
    int cardIndex = 0;//only for stack

    Vector3 cardOffset = Vector3.down * 30;
    Vector3 slotOffset = Vector3.up * 225f/2f;
    Vector3 stackOffset = Vector3.left*40;
    Vector3 stackSideOffset = Vector3.right * 20;

    void Awake(){
        if (holdType == HoldType.Card){
            cardComp = GetComponent<Card>();
            cards = new CardHolder[1];
        }else if (holdType == HoldType.Slot){
            cards = new CardHolder[1];
        }else{
            cards = new CardHolder[4];
            cardIndex = -1;
        }
    }

    public bool IsValidStack(){
        if (cards[0] == null){
            return true;
        }
        return ValidEval(cards[0].cardComp,cardComp) && cards[0].IsValidStack();
    }

    public void Picked(){
        transform.SetParent(CardManager.singleton.cardParentHeld);
    }

    public void ChildPicked(){
        cards[cardIndex] = null;
        if (holdType == HoldType.Stack){
            cardIndex--;
            StackSetCastTargets();
        }
    }

    public CardHolder GetBottomChild(){
        if (holdType == HoldType.Stack){
            return null;
        }
        if (cards[0] == null){
            return this;
        }
        return cards[0].GetBottomChild();
    }

    public CardHolder GetRootParent(){
        if (holdParent == null){
            return this;
        }
        return holdParent.GetRootParent();
    }

    public bool StackContains(CardHolder h){
        if (h == this){
            return true;
        }
        if (holdParent == null){
            return false;
        }
        return false || holdParent.StackContains(h);
    }

    public void Place(CardHolder h){
        if (holdType == HoldType.Card && StackContains(CardManager.singleton.stack)){
            CardManager.singleton.stack.Place(h);
            return;
        }

        if (h.holdParent != null){
            h.holdParent.ChildPicked();
        }
        if (holdType == HoldType.Stack){
            cardIndex++;
            cards[cardIndex] = h;
            cards[cardIndex].transform.SetParent(transform,false);
            cards[cardIndex].holdParent = this;
            cards[cardIndex].transform.localPosition = stackOffset + stackSideOffset*cardIndex;
            StackSetCastTargets();
        }else{
            if (cards[0] != null){
                cards[0].Place(h);
            }else{
                cards[0] = h;
                cards[0].transform.SetParent(transform,false);
                cards[0].holdParent = this;
                if (holdType == HoldType.Card){
                    cards[0].transform.localPosition = cardOffset;
                }else{
                    cards[0].transform.localPosition = slotOffset;
                }
            }
        }
        CardManager.singleton.VibeCheck(h);
    }

    public IEnumerator PlaceAnim(CardHolder h, float delay)
    {
        Vector3 start = h.transform.position;
        Place(h);
        Vector3 end = h.transform.position;
        h.transform.SetParent(CardManager.singleton.cardParent);

        yield return null;
        h.transform.SetParent(h.holdParent.transform);
        CardManager man = CardManager.singleton;
        float time = -delay;
        bool flipped = false;
        while (time < man.flightTime)
        {
            if (time > 0f && !flipped){
                flipped = true;
                h.cardComp.setFace(true);
            }
            time += Time.deltaTime;
            h.transform.position = Vector3.Lerp(start,end,man.animCurve.Evaluate(time/man.flightTime));
            yield return null;
        }
    }

    public void ReturnCard(CardHolder h){
        h.transform.SetParent(transform,false);
        if (holdType == HoldType.Card){
            h.transform.localPosition = cardOffset;
        }else if (holdType == HoldType.Slot){
            h.transform.localPosition = slotOffset;
        }else{
            h.transform.localPosition = stackOffset + stackSideOffset*cardIndex;
        }
    }

    public bool CanPlace(CardHolder h){
        if (holdType == HoldType.Card && StackContains(CardManager.singleton.stack)){
            return CardManager.singleton.stack.CanPlace(h);
        }
        if (holdType == HoldType.Stack){
            if (h.cards[0] != null){
                return false;
            }else if (cardIndex+1 >= cards.Length){
                return false;
            }else{
                return true;
            }
        }else{
            if (cards[0] != null){
                return false;
            }else if (holdType == HoldType.Card){
                return ValidEval(h.cardComp,cardComp);
            }else{
                return true;
            }
        }
    }

    public void StackSetCastTargets(){
        if (holdType == HoldType.Stack){
            for(int i = 0; i <cardIndex; i++){
                cards[i].GetComponent<Image>().raycastTarget = false;
            }
            if (cardIndex >=0){
                cards[cardIndex].GetComponent<Image>().raycastTarget = true;
            }
        }
    }

    static bool ValidEval(Card child, Card parent){
        if ((int)child.rank == (int)parent.rank - 1 || (parent.rank == CardManager.Rank.Queen && child.rank == CardManager.Rank.Queen)){
            //if ((int)child.suit % 2 != (int)parent.suit % 2){
                return true;
            //}
        }
        return false;
    }

    public void Collapse(Vector3 pos){
        transform.position = pos;
        cardComp.setFace(false);
        GetComponent<Image>().raycastTarget = false;
        if (cards[0] != null){
            cards[0].Collapse(pos);
        }
    }

    public void Reset(){
        for (int i = 0; i < cards.Length; i++){
            cards[i] = null;
        }
        holdParent = null;
        if (holdType == HoldType.Stack){
            cardIndex = -1;
        }
    }
}
