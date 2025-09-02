using System.Collections;
using UnityEngine;

public class Summoner : Enemy
{
    public GameObject splitterPrefab;  // The Splitter enemy prefab
    public int health = 200;  // Summoner's health
    public GameObject summonSpawnAnimationPrefab;  // The animation for when the Summoner spawns
    public GameObject summonPostAnimationPrefab;  // The second animation for the summoning attack
    public GameObject SpawnAnimationPrefab;
    private bool isCasting = false;  // Flag to determine if summoning is in progress
    private Coroutine summonCoroutine;

    public AudioClip spawnSound; // Sound when enemy spawns an object
    protected AudioSource summonerAudioSource;

    [Range(0f, 1f)]
    public float spawnSoundVolume = 1f;

    void Start()
    {
        // Instantiate the SpawnAnimationPrefab at the Summoner's current position
        Instantiate(SpawnAnimationPrefab, transform.position, Quaternion.identity);
        summonCoroutine = StartCoroutine(Summon());

        summonerAudioSource = gameObject.AddComponent<AudioSource>();
        summonerAudioSource.volume = spawnSoundVolume;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;

        // Check if the Summoner needs to teleport and summon immediately
        if (health <= 0)
        {
            // The Summoner has been defeated
            base.Defeat();
        }
        else
        {
            if (summonCoroutine != null)
            {
                StopCoroutine(summonCoroutine);
            }
            summonCoroutine = StartCoroutine(InterruptedSummon());
        }
    }

    private void Teleport()
    {
        int x;
        do
        {
            x = Random.Range(-1, 2) * 6;  // -6, 0, or 6
        } while (Mathf.Abs(x - transform.position.x) < 3);

        int z = Random.Range(-50, 50);  // A random z-position between -50 and 50
        transform.position = new Vector3(x, 1, z);
        Instantiate(SpawnAnimationPrefab, transform.position, Quaternion.identity);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(10);  // assuming each projectile causes 10 damage
        }
    }

    private IEnumerator InterruptedSummon()
    {
        Teleport();
        yield return new WaitForSeconds(1f); // Wait for 1 second after teleporting
        PerformSummon();
        yield return new WaitForSeconds(1f); // Wait for 1 second after summoning
        Teleport();
        yield return new WaitForSeconds(1f); // Wait for 4 seconds before returning to normal cycle
        summonCoroutine = StartCoroutine(Summon());
    }

    private IEnumerator Summon()
    {
        while (true)
        {
            // Wait for 2 seconds before starting the summoning process
            yield return new WaitForSeconds(2f);
            isCasting = true;

            // Wait for 4 seconds (the summoning process)
            yield return new WaitForSeconds(2f);

            if (isCasting)
            {
                PerformSummon();
            }

            isCasting = false;

            // Wait for 2 seconds before teleporting
            yield return new WaitForSeconds(2f);

            Teleport();

            // Wait for 2 seconds before starting the next cycle
            yield return new WaitForSeconds(2f);
        }
    }

    private void PerformSummon()
    {
        Instantiate(summonPostAnimationPrefab, transform.position, Quaternion.identity);

        Vector3 spawnPosition = transform.position + new Vector3(0, -1, 0);
        GameObject newSplitterObject = Instantiate(splitterPrefab, spawnPosition, Quaternion.identity);

        Splitter newSplitter = newSplitterObject.GetComponent<Splitter>();
        if (newSplitter != null)
        {
            Vector3 center = transform.position + new Vector3(0, 0, Random.Range(10, 20));  // Random distance between 10 and 20 in front of the Summoner
            newSplitter.SetCenter(center);
        }
        if (spawnSound != null && summonerAudioSource != null)
        {
            summonerAudioSource.PlayOneShot(spawnSound);
        }
    }
}
