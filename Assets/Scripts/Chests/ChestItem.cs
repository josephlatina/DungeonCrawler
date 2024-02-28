/*
 * ChestItem.cs
 * Author: Joseph Latina
 * Created: February 27, 2024
 * Description: Script to initialize and materialize the chest item contained within the chest object
 */

using System.Collections;
using TMPro;
using UnityEngine;

public class ChestItem : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;
    [HideInInspector] public bool isItemMaterialized = false;

    private void Awake() {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    /// <summary>
    /// Initialize the chest item's sprite and position
    /// </summary>
    public void Initialize(Sprite sprite, Vector3 spawnPosition, Color materializeColor) {
        
        spriteRenderer.sprite = sprite;
        transform.position = spawnPosition;

        StartCoroutine(MaterializeItem(materializeColor));
    }

    /// <summary>
    /// Materialize the chest item
    /// </summary>
    private IEnumerator MaterializeItem(Color materializeColor)
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, 1f, spriteRendererArray, GameResources.Instance.litMaterial));

        isItemMaterialized = true;
    }
}
