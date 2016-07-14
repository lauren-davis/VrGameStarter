using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    public Transform PositionA;
    public Transform PositionB;
    public Transform CurrentTarget;
    public float MoveSpeed=2;

	// Use this for initialization
	void Start ()
	{
	    CurrentTarget = PositionA;


	}
	
	// Update is called once per frame
	void Update ()
	{
	    var Dif = CurrentTarget.position - transform.position;
	    if (Dif.magnitude < 1)
	    {
	        if (CurrentTarget == PositionA)
	        {
	            CurrentTarget = PositionB;
	        }
	        else
	        {
	            CurrentTarget = PositionA;
	        }
	    }
	    transform.position = transform.position + Dif.normalized*MoveSpeed*Time.deltaTime;
	}
}
