using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlayer : MonoBehaviour
{
    bool jump = false;
    Rigidbody rigidbody;
    public float jumpForce = 10;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        if (jump)
        {
            rigidbody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            jump = false;
        }
    }
}
