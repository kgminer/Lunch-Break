using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Inventory Inventory;

    public GameObject MessagePanel;
    public GameObject TimeRemainingLabel;
    public GameObject RespawnMap;
    public GameObject GameOverPanel;
    public GameObject ScoreLabel;
    public GameObject ScienceGeeksFinalScore;
    public GameObject BookWormsFinalScore;
    public GameObject JocksFinalScore;

    // Use this for initialization
    void Start()
    {
        Inventory.ItemAdded += InventoryScript_ItemAdded;
        Inventory.ItemRemoved += Inventory_ItemRemoved;
    }

    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");
        int index = -1;

        foreach(Transform slot in inventoryPanel)
        {
            index++;

            // Border... Image
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform textTransform = slot.GetChild(0).GetChild(1);
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();
            Text txtCount = textTransform.GetComponent<Text>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

            if (index == e.Item.Slot.Id)
            {
                image.enabled = true;
                image.sprite = e.Item.Image;

                int itemCount = e.Item.Slot.Count;
                if (itemCount > 1)
                    txtCount.text = itemCount.ToString();

                else
                    txtCount.text = "";

                // Store a reference to the item
                itemDragHandler.Item = e.Item;

                break;
            }

            /*
            // We found the empty slot
            if (!image.enabled)
            {
                image.enabled = true;
                image.sprite = e.Item.Image;

                // Store a reference to the item
                itemDragHandler.Item = e.Item;

                break;
            }
            */
        }
    }

    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");

        int index = -1;

        foreach(Transform slot in inventoryPanel)
        {
            index++;

            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform textTransform = slot.GetChild(0).GetChild(1);
            Image image = imageTransform.GetComponent<Image>();
            Text txtCount = textTransform.GetComponent<Text>();
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();

            // We found the item in the UI
            if (itemDragHandler.Item == null)
                continue;

            if (e.Item.Slot.Id == index)
            {
                int itemCount = e.Item.Slot.Count;
                itemDragHandler.Item = e.Item.Slot.FirstItem;

                if (itemCount < 2)
                {
                    txtCount.text = "";
                }

                else
                {
                    txtCount.text = itemCount.ToString();
                }

                if (itemCount == 0)
                {
                    image.enabled = false;
                    image.sprite = null;
                }
                break;
            }
            /*
            if(itemDragHandler.Item.Equals(e.Item))
            {
                image.enabled = false;
                image.sprite = null;
                itemDragHandler.Item = null;
                break;
            }
            */
        }
    }

    public void SetTimeRemainingText(int minutes, int seconds)
    {
        string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        TimeRemainingLabel.GetComponent<Text>().text = "Time Remaining: " + formattedTime;
    }

    public void SetScoreText(int score1, int score2, int score3)
    {
        ScoreLabel.GetComponent<Text>().text = "Science Geeks: " + score1 + "/100 " + "Book Worms: " + score2 + "/100 " + "Jocks: " + score3 + "/100";
    }

    public void SetFinalScoreText(int score1, int score2, int score3)
    {
        ScienceGeeksFinalScore.GetComponent<Text>().text = "Science Geeks: " + score1;
        BookWormsFinalScore.GetComponent<Text>().text = "Book Worms: " + score2;
        JocksFinalScore.GetComponent<Text>().text = "Jocks: " + score3;
    }

    public void OpenRespawnMap()
    {
        RespawnMap.SetActive(true);
    }

    public void CloseRespawnMap()
    {
        RespawnMap.SetActive(false);
    }

    public void OpenGameOverPanel()
    {
        GameOverPanel.SetActive(true);
    }

    public void CloseGameOverPanel()
    {
        GameOverPanel.SetActive(false);
    }

    public void OpenMessagePanel(string text)
    {
        MessagePanel.SetActive(true);

        // TODO: set text when we will use this for other messages as well
    }

    public void CloseMessagePanel()
    {
        MessagePanel.SetActive(false);
    }
}
