using UnityEngine;

// Put on cursor object. 
public class Cursor : MonoBehaviour
{
    [SerializeField]
    private GameObject _inkDotPrefab;

    private GameObject _copy;

    private void OnEnable()
    {
        GameManager.OnInstantiatedCopy += GetCopyReference;
    }

    private void OnDisable()
    {
        GameManager.OnInstantiatedCopy -= GetCopyReference;
    }

    private void Update()
    {
        // Put cursor at mouse position. 
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Clamp cursor within bounds of copy. 
        float x = Mathf.Clamp(mousePosition.x, 0f, 10f);
        float y = Mathf.Clamp(mousePosition.y, -5f, 5f);
        transform.position = new Vector3(x, y, 0f);

        // If left click, draw circle sprite at mouse position. 
        if (Input.GetMouseButton(0))
        {
            GameObject dot = Instantiate(_inkDotPrefab, transform.position, Quaternion.identity);
            dot.transform.parent = _copy.transform;
            dot.transform.localScale = Vector3.one * 0.125f * 0.4f;
        }
    }

    private void GetCopyReference(GameObject copy)
    {
        Debug.Log("Get Copy Reference called");
        _copy = copy;
    }
}