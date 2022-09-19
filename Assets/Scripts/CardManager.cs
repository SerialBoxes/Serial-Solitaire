using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public enum Suit{Spades,Hearts,Clubs,Diamonds};
    public enum Rank{Ace,Two,Three,Four,Five,Six,Seven,Eight,Nine,Ten,Jack,Queen};

    public static CardManager singleton;

    Card[] cards;

    public RectTransform cardParent;
    public RectTransform cardParentHeld;
    public RectTransform animShield;
    public RectTransform winBanner;
    public RectTransform instructions;
    public GameObject cardPrefab;

    public CardHolder[] slots;
    public CardHolder stack;

    public AnimationCurve animCurve;
    public float flightTime;
    public float sepTime;

    public string[] abbr = new string[] {"A","2","3","4","5","6","7","8","9","10","J","Q","Q"};

    void Start()
    {
        if (singleton != null){
            Debug.LogError("MULTIPLE MANAGERS");
        }
        singleton = this;

        InitializeCards();
        ShuffleCards();
        StartCoroutine(DelayOneFrame());
    }

    void InitializeCards(){
        cards = new Card[52];
        for (int i = 0; i < cards.Length;i++){
            cards[i] = Instantiate(cardPrefab,new Vector3(-540f,234.5f,0f),Quaternion.identity).GetComponent<Card>();//horizonal layout hasn't worked its thing yet oi
            cards[i].transform.SetParent(cardParent,false);
            cards[i].Setup(i);
        }
    }

    IEnumerator DelayOneFrame(){
        yield return null;
        DealCards();
    }

    public void DealCards(){//THis is so hacky don't worry about it 
        int slotIndex = 0;
        for (int i = 0; i < cards.Length; i++){
            if (cards[i] != null){
                StartCoroutine(slots[slotIndex].PlaceAnim(cards[i].holder,sepTime * i));
                slotIndex++;
                if (slotIndex >= slots.Length){
                    slotIndex = 0;
                }
            }
        }
        StartCoroutine(DealHelper());

    }

    IEnumerator DealHelper(){
        cardParent.gameObject.SetActive(false);
        animShield.gameObject.SetActive(true);
        yield return null;
        cardParent.gameObject.SetActive(true);
        yield return null;
        cards[cards.Length-1].transform.SetParent(cardParent);
        float time = 0f;
        float end = sepTime*52 + flightTime;
        while (time < end){
            time += Time.deltaTime;
            yield return null;
        }
        cards[cards.Length-1].transform.SetParent(cards[cards.Length-1].holder.holdParent.transform);
        animShield.gameObject.SetActive(false);
    }

    public void ShuffleCards ()
    {
        int n = cards.Length;
        while (n > 1) 
        {
            int k = Random.Range(0,n--);
            Card temp = cards[n];
            cards[n] = cards[k];
            cards[k] = temp;
        }
        int i = cards.Length-1;
        while (cards[i] == null){
            i--;
        }
        cards[i].transform.SetSiblingIndex(51);
    }

    public void ResetEverything(){
        for (int i = 0; i < cards.Length;i++){
            cards[i].Reset();
            cards[i].transform.SetParent(cardParent,false);
            cards[i].transform.position = stack.transform.position;
        }
        for (int i = 0; i < slots.Length;i++){
            slots[i].Reset();
        }
        stack.Reset();

        winBanner.gameObject.SetActive(false);
    }

    public void VibeCheck(CardHolder placed){
        /*CardHolder rootSlot = placed.GetRootParent();
        if (rootSlot.holdType != CardHolder.HoldType.Slot || rootSlot.cards[0] == null){
            return;
        }
        CardHolder root = rootSlot.cards[0];
        if (root.IsValidStack() && root.GetBottomChild().cardComp.rank == Rank.Ace && root.cardComp.rank == Rank.Queen && root.cards[0].cardComp.rank == Rank.Queen && root.cards[0].cards[0].cardComp.rank == Rank.Jack){
            root.Collapse(root.transform.position);
        }*/
        bool won = true;
        for (int i = 0; i < slots.Length; i++){
            if (slots[i].cards[0] == null || (slots[i].cards[0].IsValidStack() && slots[i].cards[0].GetBottomChild().cardComp.rank == Rank.Ace && slots[i].cards[0].cardComp.rank == Rank.Queen && slots[i].cards[0].cards[0].cardComp.rank == Rank.Queen && slots[i].cards[0].cards[0].cards[0].cardComp.rank == Rank.Jack)){
                //this is win condition just didnt wanna invert the condition pepega
            }else{
                won = false;
            }
        }
        if (won){
            for (int i = 0; i < slots.Length; i++){
                if (slots[i].cards[0] != null){
                    slots[i].cards[0].Collapse(slots[i].cards[0].transform.position);
                }
            }
            winBanner.gameObject.SetActive(true);
        }
    }

    public void Restart(){
        ResetEverything();
        ShuffleCards();
        DealCards();
    }

    public void ShowInstructions(){
        instructions.gameObject.SetActive(true);
    }

    public void HideInstructions(){
        instructions.gameObject.SetActive(false);
    }
}
