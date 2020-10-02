using System.Collections.Generic;
using UnityEngine;

public class ResultsList : MonoBehaviour
{
    [SerializeField] ResultListItem itemPrefab;
    [SerializeField] RectTransform itemContainer;

    public void DisplayItems(List<string> items)
    {
        Clear();
        foreach (var item in items)
        {
            CreateNewListItem(item);
        }
       }

    public void Clear()
    {
        foreach (Transform child in itemContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateNewListItem(string item)
    {
        var newItem = Instantiate(itemPrefab);
        newItem.enabled = true;
        newItem.transform.SetParent(itemContainer, worldPositionStays: false);
        newItem.Text = item;
    }
}
