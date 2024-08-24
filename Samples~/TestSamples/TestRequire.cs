using Better.Validation.Runtime.Attributes;
using UnityEngine;

public class TestRequire : MonoBehaviour
{
    [Find(typeof(Test))][SerializeField] private Test te;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
