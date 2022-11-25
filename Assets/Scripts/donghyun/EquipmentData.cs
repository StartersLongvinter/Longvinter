using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "ScriptableObject/EquipmentData")]

public class EquipmentData : ScriptableObject
{
    public enum EquipmentClassify
    {
        Melee, Range, Default, Ammo
    }
    
    public enum EquipmentPosition
    {
        Head, Hand, Default
    }

    public enum EquipmentAnim
    {
        Back, Forward, Default
    }
    
    
    [SerializeField] private EquipmentClassify equipmentClassify;
    [SerializeField] private EquipmentPosition equipmentArea;
    [SerializeField] private EquipmentAnim equipmentAnim;
    [SerializeField] private GameObject equipmentCanvasPrefab; //아이템 프리펩
    [SerializeField] private GameObject equipmentPrefab;
    //[SerializeField] private Sprite equipmentImage; //아이템 사진
    [SerializeField] private float weaponDamage;
    [SerializeField] private float weaponAttackRate;
    [SerializeField] private int ammoCount;
    [SerializeField] private string equipmentKorName;
    [SerializeField] private bool isAuto;

    

    public EquipmentClassify emClassify => equipmentClassify;
    public EquipmentPosition emArea => equipmentArea;
    public EquipmentAnim emAnim => equipmentAnim;
    public GameObject itPrefab => equipmentCanvasPrefab;

    public GameObject emPrefab => equipmentPrefab;
    //public Sprite itImage => equipmentImage;
    public float wpDamage => weaponDamage;
    public float wpAttackRate => weaponAttackRate;

    public int amCount => ammoCount;
    
    public string emKorName => equipmentKorName;
    public bool auto => isAuto;
}
