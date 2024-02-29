/*
 * ChestItem.cs
 * Author: Joseph Latina
 * Created: February 27, 2024
 * Description: Script to handle logic for materializing the chest and chest item inside and going between different chest states
 */

using System.Collections;
using TMPro;
using UnityEngine;

// need to implement the use method
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Chest : MonoBehaviour, IUseable
{
    // color we want to use when materializing the chest
    #region Tooltip
    [Tooltip("Set this to the colour to be used for the materialization effect")]
    #endregion Tooltip
    [ColorUsage(false, true)]
    [SerializeField] private Color materializeColor;

    // time it takes to materialize the chest
    #region Tooltip
    [Tooltip("Set this to the time is will take to materialize the chest")]
    #endregion Tooltip
    [SerializeField] private float materializeTime = 3f;

    // spawn position of chest
    #region Tooltip
    [Tooltip("Populate with ItemSpawnPoint transform")]
    #endregion
    [SerializeField] private Transform itemSpawnPoint;

    // hold reference to the weapon item SO, animator, sprite renderer and materialize effect components
    private WeaponItem weaponItem;
    private ConsumableItem healthPotionitem;
    private ConsumableItem pillItem;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;

    // not enabled when chest is still materializing
    private bool isEnabled = false;
    
    // set the starting chest state to be closed
    private ChestState chestState = ChestState.closed;
    // holds reference to the chest item game object within scene
    private GameObject chestItemGameObject;
    // holds reference to the chest item component
    private ChestItem chestItem;

    private void Awake() {

        // Cache components
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();

    }

    /// <summary>
    /// Initialize chest and decide whether to make it visible immediately or materialize
    /// </summary>
    public void Initialize(bool shouldMaterialize, WeaponItem weaponItem) {
        this.weaponItem = weaponItem;

        // if chest should be materialized, proceed to materialize it
        if (shouldMaterialize) {
            StartCoroutine(MaterializeChest());
        } 
        // otherwise, just enable it(make visible) immediately
        else {
            EnableChest();
        }
    }

    /// <summary>
    /// Materialize the chest
    /// </summary>
    private IEnumerator MaterializeChest() {

        // Initialize sprite renderer array (needed for materializing the chest)
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        // start materializing chest but wait for it to be fully materialized (until end of materializeTime) before we proceed
        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, materializeTime, spriteRendererArray, GameResources.Instance.litMaterial));

        // once fully materialized, enable it (make visible)
        EnableChest();
    }

    /// <summary>
    /// Enable the chest
    /// </summary>
    private void EnableChest() {

        // Set variable to enabled
        isEnabled = true;
    }

    /// <summary>
    /// Check state of the chest and perform appropriate action using a state machine
    /// </summary>
    public void UseItem() {

        // check if chest is enabled
        if (!isEnabled) return;

        // implement state machine
        switch (chestState) {

            case ChestState.closed:
                OpenChest();
                break;

            default:
                return;
        }
    }

    /// <summary>
    /// Open the chest
    /// </summary>
    private void OpenChest() {

        // use the animator to play the 'chest open' animation by setting the use parameter to true
        animator.SetBool(Settings.use, true);

        // materialize the random chest item inside and update chest state
        UpdateChestState();
    }

    /// <summary>
    /// Materialize items randomly and update the chest state accordingly
    /// </summary>
    private void UpdateChestState() {

        int randomState = Random.Range(0, 3);

        switch (randomState) {

            case 0:
                chestState = ChestState.healthPotionItem;
                InstantiateHealthPotionItem();
                break;
            
            case 1:
                chestState = ChestState.weaponItem;
                InstantiateWeaponItem();
                break;

            case 2:
                chestState = ChestState.pillItem;
                InstantiatePillItem();
                break;
            
            default:
                return;
        }

        GetComponent<BoxCollider2D>().enabled = false;
    }

    /// <summary>
    /// Instantiate chest item
    /// </summary>
    private void InstantiateItem() {

        // Instantiate the chest item prefab as a game object within the scene
        chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform);

        // Get the Chest Item component embedded on the instantiated chest item prefab
        chestItem = chestItemGameObject.GetComponent<ChestItem>();
    }

    /// <summary>
    /// Instantiate and materialize the health potion item for player to collect
    /// </summary>
    private void InstantiateHealthPotionItem() {

        // Instantiate the chest item
        InstantiateItem();

        // From the Chest Item component pulled from the instantiation, initialize the health potion item by rendering the sprite and materializing it
        chestItem.Initialize(GameResources.Instance.healthPotionIcon, itemSpawnPoint.position, materializeColor);
    }

    /// <summary>
    /// Instantiate and materialize the weapon item for player to collect (only item that's not using default icon from GameResources)
    /// </summary>
    private void InstantiateWeaponItem() {

        // Instantiate the chest item
        InstantiateItem();

        // From the Chest Item component pulled from the instantiation, initialize the weapon item by rendering the sprite and materializing it
        chestItem.Initialize(weaponItem.itemSprite, itemSpawnPoint.position, materializeColor);
    }

    /// <summary>
    /// Instantiate and materialize the pill item for player to collect
    /// </summary>
    private void InstantiatePillItem() {

        // Instantiate the chest item
        InstantiateItem();

        // From the Chest Item component pulled from the instantiation, initialize the pill item by rendering the sprite and materializing it
        chestItem.Initialize(GameResources.Instance.pillIcon, itemSpawnPoint.position, materializeColor);
    }
}
