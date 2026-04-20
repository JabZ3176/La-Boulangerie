using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    [Header("Items")]
    public List<string> itemsToCollect = new List<string>(); 
    private List<string> collectedItems = new List<string>();

    [Header("UI")]
    public Transform itemListParent;   
    public GameObject itemEntryPrefab; 

    [Header("Door")]
    public Door door; 

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        BuildItemList(); 
    }

    void BuildItemList()
    {
        foreach (string itemName in itemsToCollect)
        {
            GameObject entry = Instantiate(itemEntryPrefab, itemListParent);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = "[ ] " + itemName;
            entry.name = itemName; 
    }

    public void CollectItem(string itemName)
    {
        if (collectedItems.Contains(itemName)) return; 

        collectedItems.Add(itemName);
        UpdateItemInList(itemName);

        if (collectedItems.Count >= itemsToCollect.Count)
        {
            door.Unlock(); 
        }
    }

    void UpdateItemInList(string itemName)
    {
        Transform entry = itemListParent.Find(itemName);
        if (entry != null)
        {
            entry.GetComponentInChildren<TextMeshProUGUI>().text = "[X] " + itemName;
        }
    }
}