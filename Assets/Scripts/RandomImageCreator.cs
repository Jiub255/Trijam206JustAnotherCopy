using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomImageCreator : MonoBehaviour
{
    [SerializeField]
	private GameObject _blankCanvasPrefab;
    private GameObject _blankCanvas;

    [SerializeField]
    private List<GameObject> _primitives = new List<GameObject>();

    private List<Vector2Int> _usedInts = new List<Vector2Int>();

    // Instantiate primitive as child of blank canvas. 
    // Choose two random ints in (-4, 4), divide them by 10. 
    // Make sure it doesn't match previous random x,y. 
    // Move child to that position in parent's local space. 
	public GameObject CreateRandomImage(int numberOfPrimitives)
    {
        _blankCanvas = Instantiate(_blankCanvasPrefab);

        for (int i = 0; i < numberOfPrimitives; i++)
        {
            // Random Index 
            int randomIndex = Random.Range(0, _primitives.Count - 1);

            // Random Position 
            Vector2Int randomInts = GetRandomInts();
            Vector2 randomPosition = (Vector2)randomInts / 10f;
            Debug.Log("Random Position: " + randomPosition.ToString());

            // Random Rotation 
            Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 360));

            GameObject primitive = Instantiate(_primitives[randomIndex], _blankCanvas.transform);
            primitive.transform.localPosition = randomPosition;
            primitive.transform.rotation = randomRotation;
            primitive.transform.localScale = Vector3.one * 0.125f;
        }

        return _blankCanvas;
    }

    private Vector2Int GetRandomInts()
    {
        Vector2Int randomInts = new Vector2Int(Random.Range(-4, 5), Random.Range(-4, 5));
        // Check if already used here. 
        if (_usedInts.Contains(randomInts))
        {
            return GetRandomInts();
        }
        else
        {
            return randomInts;
        }
    }
}