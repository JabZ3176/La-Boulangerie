using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // lets other scripts find this easily

    [Header("Items")]
    public List<string> itemsToCollect = new List<string>(); // fill this in the Inspector
    private List<string> collectedItems = new List<string>();

    [Header("UI")]
    public Transform itemListParent;   // the panel that holds the list
    public GameObject itemEntryPrefab; // a Text prefab for each list row

    [Header("Door")]
    public Door door; // drag your Door object here in the Inspector

    void Awake()
    {
        Instance = this; // make this accessible from anywhere
    }

    void Start()
    {
        BuildItemList(); // draw the checklist when the game starts
    }

    void BuildItemList()
    {
        foreach (string itemName in itemsToCollect)
        {
            GameObject entry = Instantiate(itemEntryPrefab, itemListParent);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = "[ ] " + itemName;
            entry.name = itemName; // name it so we can find it later
        }
    }

    public void CollectItem(string itemName)
    {
        if (collectedItems.Contains(itemName)) return; // don't collect twice

        collectedItems.Add(itemName);
        UpdateItemInList(itemName);

        if (collectedItems.Count >= itemsToCollect.Count)
        {
            door.Unlock(); // all items collected — open the door!
        }
    }

    void UpdateItemInList(string itemName)
    {
        // Find the matching row in the UI and tick it off
        Transform entry = itemListParent.Find(itemName);
        if (entry != null)
        {
            entry.GetComponentInChildren<TextMeshProUGUI>().text = "[X] " + itemName;
        }
    }
}