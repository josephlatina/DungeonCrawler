using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthDisplay : MonoBehaviour
{
    public GameObject heartPrefab;
    public PlayerHealth playerHealth;
    List<HealthHeart> hearts = new List<HealthHeart>();

    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += DrawHearts;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= DrawHearts;
    }

    void Start()
    {
        playerHealth = playerHealth == null
            ? GameObject.FindWithTag("Player").GetComponent<PlayerHealth>()
            : playerHealth;
        
        DrawHearts();
    }

    public void DrawHearts()
    {
        ClearHearts();

        // float maxHealthRemainder = (playerHealth.maxHealthPoints) % 2;
        int heartsToMake = (int)(playerHealth.maxHealthPoints);
        for (int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(playerHealth.currentHealthPoints * 2 - (i * 2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);

        // Get the RectTransform component of the instantiated heart prefab
        RectTransform heartRectTransform = newHeart.GetComponent<RectTransform>();

        // Set the scale to (1, 1, 1)
        const float scale = 0.8f;
        heartRectTransform.localScale =  new Vector3(scale,scale, scale);

        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }

    public void ClearHearts()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }

        hearts = new List<HealthHeart>();
    }
}