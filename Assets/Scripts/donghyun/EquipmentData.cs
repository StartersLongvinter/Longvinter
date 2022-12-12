using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "ScriptableObject/EquipmentData")]
public class EquipmentData : ScriptableObject
{
    public enum EquipmentClassify
    {
        Default, MeleeWeapon, RangeWeapon, Ammo
    }
    public enum EquipmentPosition
    {
        Default, OneHand, TwoHand, Head, Back
    }

    [SerializeField] private string equipmentKorName;
    [SerializeField][TextArea] private string equipmentExplain;
    [SerializeField] private EquipmentClassify equipmentClassify;
    [SerializeField] private EquipmentPosition equipmentPosition;
    [SerializeField] private GameObject equipmentCanvasPrefab;
    [SerializeField] private GameObject equipmentPrefab;
    [SerializeField] private int equipmentIndex;
    [SerializeField] private float weaponDamage;
    [SerializeField] private float weaponAttackRate;
    [SerializeField] private bool isAuto;
    [SerializeField] private int ammoCount; // Player 이동 예정

    public string eqKorName => equipmentKorName;
    public string eqExplain => equipmentExplain;
    public EquipmentClassify eqClassify => equipmentClassify;
    public EquipmentPosition eqPosition => equipmentPosition;
    public GameObject eqCanvasPrefab => equipmentCanvasPrefab;
    public GameObject eqPrefab => equipmentPrefab;
    public int eqIndex => equipmentIndex;
    //public Vector2 wpMinMaxDamage => weaponMinMaxDamage;
    public float wpDamage => weaponDamage;
    public float wpAttackRate => weaponAttackRate;
    public bool auto => isAuto;
    public int amCount => ammoCount;
}
