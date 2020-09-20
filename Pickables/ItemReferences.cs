using UnityEngine;

[CreateAssetMenu(fileName = "ItemReferences", menuName = "ScriptableObjects/ItemReferences", order = 1)]
public class ItemReferences : ScriptableObject
{
    [SerializeField]
    private GameObject[] itemReferences;
    public GameObject[] Items { get { return itemReferences; } }
}
