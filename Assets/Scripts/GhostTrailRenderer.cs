using System.Collections;
using UnityEngine;

public class GhostTrailRenderer : MonoBehaviour
{
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float spawnInterval = 0.05f;
    [SerializeField] private float trailDuration = 0.3f;

    public void DrawTrail(float duration)
    {
        StartCoroutine(TrailCoroutine(duration));
    }

    private IEnumerator TrailCoroutine(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            SpawnGhost();
            yield return new WaitForSeconds(spawnInterval);
            timer += spawnInterval;
        }
    }

    private void SpawnGhost()
    {
        // Instantiate prefab at player position
        GameObject ghost = Instantiate(ghostPrefab, transform.position, transform.rotation);

        SpriteRenderer sr = ghost.GetComponent<SpriteRenderer>();
        SpriteRenderer playerSR = GetComponent<SpriteRenderer>();

        // Copy sprite and flip
        sr.sprite = playerSR.sprite;
        sr.flipX = playerSR.flipX;

        // Set initial color
        sr.color = new Color(1f, 1f, 1f, 0.5f);

        // Fade out over trailDuration
        ghost.AddComponent<GhostFade>().duration = trailDuration;

        // Destroy after duration
        Destroy(ghost, trailDuration);
    }
}

// Optional fade script
public class GhostFade : MonoBehaviour
{
    public float duration = 0.3f;
    private SpriteRenderer sr;
    private float timer = 0f;

    void Awake() { sr = GetComponent<SpriteRenderer>(); }

    void Update()
    {
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(0.5f, 0f, timer / duration);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
    }
}
