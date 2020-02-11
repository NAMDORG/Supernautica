using UnityEngine;

public class SpawningBehavior : MonoBehaviour
{
    Vector3 playerPosition;
    Consumable generateItemID;
    GameObject generatedItem;

    float rotateX, rotateY, rotateZ;
    
    void Start()
    {
        playerPosition = GameObject.Find("Player").transform.position;
        InvokeRepeating("CreateResource", 0.0f, 10.0f);

        rotateX = Random.Range(-1.0f, 1.0f) * 60 * Time.deltaTime;
        rotateY = Random.Range(-1.0f, 1.0f) * 60 * Time.deltaTime;
        rotateZ = Random.Range(-1.0f, 1.0f) * 60 * Time.deltaTime;
    }
    
    void Update()
    {
        this.transform.position = playerPosition;

        foreach (Transform child in transform)
        {
            child.Rotate(rotateX, rotateY, rotateZ);
        }
    }
    
    private void CreateResource()
    {
        generateItemID = ConsumableDatabase.consumables[Random.Range(0, 2)];
        string generateItemPath = "Prefabs/Consumables/" + generateItemID.title;
        GameObject objectToGenerate = Resources.Load(generateItemPath) as GameObject;

        Vector3 generateLocation = (GameObject.Find("Player").transform.position + (Random.insideUnitSphere * 10.0f));

        generatedItem = Instantiate(objectToGenerate, generateLocation, Quaternion.identity, this.transform);

        GeneratedItemBehavior();
    }

    private void GeneratedItemBehavior()
    {
        generatedItem.name = generateItemID.title;



        generatedItem.transform.Rotate(rotateX, rotateY, rotateZ);
    }
}
