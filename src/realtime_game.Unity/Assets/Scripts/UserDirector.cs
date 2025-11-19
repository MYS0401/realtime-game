using UnityEngine;

public class UserDirector : MonoBehaviour
{
    Rigidbody rb;
    float speed = 0.2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            rb.transform.position += new Vector3(0, 0, 0.2f) * speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.transform.position -= new Vector3(0.2f, 0, 0) * speed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            rb.transform.position -= new Vector3(0, 0, 0.2f) * speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.transform.position += new Vector3(0.2f, 0, 0) * speed;
        }
    }
}
