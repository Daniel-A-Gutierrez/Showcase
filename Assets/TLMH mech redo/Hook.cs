using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public float speed;
    public float maxDistance;
    public float minDistance;
    public bool affixed = false;
    public bool canAffix = true;
    public bool outgoing = true;
    public Spodermun Controller;
    Vector3 affixedOffset;
    Transform affixedTo;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right *speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(affixed)
                transform.position = affixedTo.position - affixedOffset;
        if(outgoing)
        {
            if(  (transform.position - Controller.transform.position).magnitude > maxDistance )
                outgoing = false;    
        }
        else if (!affixed)
        {
            rb.velocity =  ( Controller.transform.position - transform.position ).normalized*speed;
            transform.right = -rb.velocity.normalized;
            if((transform.position - Controller.transform.position).magnitude < minDistance)
            {
                Controller.DestroyHook();
                Destroy(gameObject);
            }
        }
    }

    public void Release()
    {   
        if(affixed)
            canAffix = false;
        affixed = false;
        affixedTo = null;
        rb.constraints = RigidbodyConstraints2D.None;
        outgoing  = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(canAffix)
        {
            affixed = true;
            affixedTo =col.transform;
            affixedOffset = affixedTo.position - transform.position;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
