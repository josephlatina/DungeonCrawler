using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    public float currentHealthPoints;
    private ConsumableItemController consumableItemController;

    void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        currentHealthPoints = enemyController.GetHealthPoints();
        consumableItemController = GameResources.Instance.consumablePrefab.GetComponent<ConsumableItemController>();
    }

    public void ChangeHealth(float healthChange)
    {
        currentHealthPoints += healthChange;

        if (currentHealthPoints <= 0)
        {
            Transform childTransform = transform.Find("Body");
            Vector3 enemyPosition = childTransform.position;
            Destroy(gameObject);
            GameManager.Instance.SpawnItem(enemyPosition);
            // // Instantiate the item
            // GameObject lootItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, enemyPosition, Quaternion.identity);
            // lootItemGameObject.tag = "interactableObject";
            // ChestItem lootItem = lootItemGameObject.GetComponent<ChestItem>();
            // Debug.Log(childTransform);
            // Debug.Log(enemyPosition);
            // // Instantiate the health potion
            // ConsumableItemController itemController = lootItemGameObject.AddComponent<ConsumableItemController>();
            // itemController.priceView = consumableItemController.priceView;
            // itemController.priceText = consumableItemController.priceText;
            // itemController.item = GameResources.Instance.healthPotionSO;
            // itemController.sprite = consumableItemController.sprite;
            // lootItemGameObject.AddComponent<CircleCollider2D>();

            // Debug.Log("hello3");
            // lootItem.Initialize(GameResources.Instance.healthPotionIcon, enemyPosition, Color.yellow);
        }
    }

    /// <summary>
    /// Materialize the chest item
    /// </summary>
    private IEnumerator MaterializeItem(Color materializeColor, SpriteRenderer spriteRenderer, MaterializeEffect materializeEffect)
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };
        Debug.Log("hello4");
        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, 1f, spriteRendererArray, GameResources.Instance.litMaterial));
    }
}
