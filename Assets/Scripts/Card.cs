using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Card : MonoBehaviour
{
    public CardManager.Suit suit;
    public CardManager.Rank rank;

    public bool faceUp;

    Sprite faceImage;
    Sprite backImage;
    Image imageComp;
    Image imageCompChild;

    public CardHolder holder;

    public void Setup(int i){
        int ri = i%13;
        if (ri == 12){
            ri = 11;
        }
        rank = (CardManager.Rank)(ri);
        suit = (CardManager.Suit)(int)(i/13);

        holder = GetComponent<CardHolder>();
        imageComp = GetComponent<Image>();
        imageCompChild = transform.GetChild(0).GetComponent<Image>();
        imageCompChild.sprite = CardManager.singleton.faceTextures[(int)rank];
        setFace(false);
    }

    public bool getFace(){
        return faceUp;
    }

    public void setFace(bool f){
        faceUp = f;
        if (faceUp){
            imageComp.color = Color.white;
            imageCompChild.gameObject.SetActive(true);
        }else{
            imageComp.color = Color.black;
            imageCompChild.gameObject.SetActive(false);
        }
    }

    public bool Pickup(){
        bool valid = false;
        if (faceUp){
            if (holder.IsValidStack()){
                valid = true;
                holder.Picked();
            }
        }
        return valid;
    }

    public void Reset(){
        GetComponent<Image>().raycastTarget = true;
        setFace(false);
        holder.Reset();
    }
}
