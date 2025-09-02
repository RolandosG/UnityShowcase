using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class HydraNeckController : MonoBehaviour
{

    private HydraHeadController headController;
    public GameObject explosionPrefab;
    private PlayerController m_Pc;
    public float frequency = 1f;
    public float amplitude = 1f;
    public float speed = 1f;
    public int neckCubeHealth = 5;
   // public UnityEvent<GameObject> OnNeckCubeDestroyed;

    private List<Vector3> initialPositions;
    public List<int> neckCubesHealth;

    void Start()
    {
        GameObject playerObject = GameObject.Find("Player");
        if(playerObject != null )
        {
            m_Pc = playerObject.GetComponent<PlayerController>();
        }
        initialPositions = new List<Vector3>(transform.childCount);
        neckCubesHealth = new List<int>(transform.childCount);

        for (int i = 0; i < transform.childCount - 1; i++) // Exclude the head from the loop
        {
            initialPositions.Add(transform.GetChild(i).localPosition);
            neckCubesHealth.Add(neckCubeHealth);
        }

        // Get the HydraHeadController from the head
        headController = transform.GetChild(transform.childCount - 1).GetComponent<HydraHeadController>();
    }

    void Update()
    {
        for (int i = 0; i < transform.childCount - 1; i++) // Exclude the head from the loop
        {
            GameObject segment = transform.GetChild(i).gameObject;
            float sineWaveValue = Mathf.Sin((Time.time * speed) + (i * frequency)) * amplitude;
            Vector3 newPosition = initialPositions[i] + new Vector3(sineWaveValue, 0, 0);
            segment.transform.localPosition = newPosition;
        }
    }

    public void TakeDamage(int cubeIndex, int damage)
    {
        neckCubesHealth[cubeIndex] -= damage;

        if (neckCubesHealth[cubeIndex] <= 0)
        {
            DestroyCube(cubeIndex);
        }
    }

    private void DestroyCube(int cubeIndex)
    {
        Transform cubeToDestroy = transform.GetChild(cubeIndex);
        NeckCubeController cubeExplosion = cubeToDestroy.GetComponent<NeckCubeController>();

        if (cubeExplosion != null)
        {
            cubeExplosion.Explode();
        }
        else
        {
            Destroy(cubeToDestroy.gameObject);
        }

        neckCubesHealth.RemoveAt(cubeIndex);
        initialPositions.RemoveAt(cubeIndex);
    }


    public void HandleCubeDestroyed(GameObject cube)
    {
        // Get the index of the destroyed cube
        int destroyedCubeIndex = -1;
        for (int i = 0; i < transform.childCount - 1; i++) // Exclude the head from the loop
        {
            if (transform.GetChild(i).gameObject == cube)
            {
                destroyedCubeIndex = i;
                break;
            }
        }

        // Shift the cubes above the destroyed cube downwards
        if (destroyedCubeIndex >= 0)
        {
            for (int i = destroyedCubeIndex + 1; i < transform.childCount - 1; i++) // Exclude the head from the loop
            {
                GameObject cubeToShift = transform.GetChild(i).gameObject;
                cubeToShift.transform.position -= new Vector3(0, 1, 0); // Shift the cube downwards
            }

            // If the last cube is destroyed, update the lastNeckSegment variable in the HydraHeadController
            if (destroyedCubeIndex == transform.childCount - 2)
            {
                headController.transform.position -= new Vector3(0, 1, 0);
            }
        }
    }
    public void OnHeadDestroyed(GameObject destroyedHead)
    {
        // TODO: Implement what should happen when a head is destroyed.
        // You can use the destroyedHead object to get information about the head that was destroyed, such as its position, rotation, etc.
    }
    private void Explode(Transform cubeTransform)
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, cubeTransform.position, Quaternion.identity);
        }

        if (m_Pc != null)
        {
            m_Pc.RecieveScore();
        }
    }

}
