using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Inventory Inventory;

    public GameObject MousePanel;
    public GameObject ControllerPanel;
    public GameObject TimeRemainingLabel;
    public GameObject RespawnMap;
    public GameObject GameOverPanel;
    public GameObject ScoreLabel;
    public GameObject ScienceGeeksFinalScore;
    public GameObject BookWormsFinalScore;
    public GameObject JocksFinalScore;
    public GameObject PausePanel;
    public GameObject VendingMachinePanel;
    public GameObject RemainingBuysLabel;
    private Transform inventoryPanel;

    // Use this for initialization
    void Start()
    {
        Inventory.ItemAdded += InventoryScript_ItemAdded;
        Inventory.ItemRemoved += Inventory_ItemRemoved;
        inventoryPanel = transform.Find("InventoryPanel");
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
        }
    }

    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
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
        }
    }

    public void inventoryRemoveAll()
    {
        int index = -1;

        foreach (Transform slot in inventoryPanel)
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

            txtCount.text = "";
            image.enabled = false;
            image.sprite = null;
        }
    }

    public void SetTimeRemainingText(int minutes, int seconds)
    {
        string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        TimeRemainingLabel.GetComponent<Text>().text = "Time Remaining: " + formattedTime;
    }

    public void SetScoreText()
    {
        ScoreLabel.GetComponent<Text>().text = "Science Geeks: " + GameManager.scienceGeeksScore + "/250     " + "Book Worms: " + GameManager.bookWormsScore + "/250     " + "Jocks: " + GameManager.jocksScore + "/250";
    }

    public void SetFinalScoreText()
    {
        ScienceGeeksFinalScore.GetComponent<Text>().text = "Science Geeks: " + GameManager.scienceGeeksScore;
        BookWormsFinalScore.GetComponent<Text>().text = "Book Worms: " + GameManager.bookWormsScore;
        JocksFinalScore.GetComponent<Text>().text = "Jocks: " + GameManager.jocksScore;
    }

    public void OpenVendingMachinePanel()
    {
        VendingMachinePanel.SetActive(true);
        Cursor.visible = true;
    }

    public void UpdateVendingMachinePanel(int purchases)
    {
        RemainingBuysLabel.GetComponent<Text>().text = "Purchases Remaining: " + purchases;
    }

    public void CloseVendingMachinePanel()
    {
        VendingMachinePanel.SetActive(false);
        Cursor.visible = false;
    }

    public void OpenRespawnMap()
    {
        RespawnMap.SetActive(true);
    }

    public void CloseRespawnMap()
    {
        RespawnMap.SetActive(false);
    }

    public void OpenPausePanel()
    {
        PausePanel.SetActive(true);
        Cursor.visible = true;
        Time.timeScale = 0f;
    }
    
    public void ClosePausePanel()
    {
        PausePanel.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
    public void OpenGameOverPanel()
    {
        GameOverPanel.SetActive(true);
    }

    public void CloseGameOverPanel()
    {
        GameOverPanel.SetActive(false);
    }

    public void OpenMessagePanel(bool controller)
    {
        if (controller)
            ControllerPanel.SetActive(true);
        else
            MousePanel.SetActive(true);

    }

    public void CloseMessagePanel(bool controller)
    {
        if (controller)
            ControllerPanel.SetActive(false);
        else
            MousePanel.SetActive(false);
    }
}
