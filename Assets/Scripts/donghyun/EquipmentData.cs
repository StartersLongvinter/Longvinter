using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "ScriptableObject/EquipmentData")]
public class EquipmentData : ScriptableObject
{
    public enum EquipmentClassify
    {
        Default, Melee, Range, Ammo
    }
    
    public enum EquipmentPosition
    {
        Default, Head, OneHand, TwoHand
    }
    
    [SerializeField] private EquipmentClassify equipmentClassify;
    [SerializeField] private EquipmentPosition equipmentArea;
    [SerializeField] private GameObject equipmentCanvasPrefab; //아이템 프리펩
    [SerializeField] private GameObject equipmentPrefab;
    //[SerializeField] private Sprite equipmentImage; //아이템 사진
    [SerializeField] private float weaponDamage;
    [SerializeField] private float weaponAttackRate;
    [SerializeField] private int ammoCount;
    [SerializeField] private string equipmentKorName;
    [SerializeField] private bool isAuto;
    [SerializeField] private int equipmentIndex;
    
    public EquipmentClassify emClassify => equipmentClassify;
    public EquipmentPosition emArea => equipmentArea;
    public GameObject itPrefab => equipmentCanvasPrefab;
    public GameObject emPrefab => equipmentPrefab;
    //public Sprite itImage => equipmentImage;
    public float wpDamage => weaponDamage;
    public float wpAttackRate => weaponAttackRate;
    public int amCount => ammoCount;
    public string emKorName => equipmentKorName;
    public bool auto => isAuto;
    public int emIndex => equipmentIndex;
}
