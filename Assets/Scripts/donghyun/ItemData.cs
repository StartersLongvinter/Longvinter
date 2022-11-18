using UnityEngine;


[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/ItemData")]

public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Food = 0, Weapon, Equipment, Tool
    }
    
    public enum ItemClassify
    {
        Fish, Feather, Plant
    }

    [SerializeField] private string itemName; //아이템 이름
    [SerializeField] private string itemKorName;
    [SerializeField] private string itemExplain;
    [SerializeField] private ItemType itemType; //아이템 종류
    [SerializeField] private ItemClassify itemClassify; //아이템 분류
    [SerializeField] private GameObject itemPrefab; //아이템 프리펩
    [SerializeField] private Sprite itemImage; //아이템 사진
    

    public string itName => itemName;

    public string itKorName => itemKorName;

    public string itExplan => itemExplain;
    public ItemType itType => itemType;
    public ItemClassify itClassify => itemClassify;

    public GameObject itPrefab => itemPrefab;
    public Sprite itImage => itemImage;
}
