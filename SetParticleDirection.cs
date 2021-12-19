using System;
using UnityEngine;

[ExecuteInEditMode]
public class SetParticleDirection : MonoBehaviour
{
	private ParticleSystem PS;

	public bool trackParent;

	public bool invertRotation;

	public float offset;

	private void Start()
	{
		PS = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		ParticleSystem.MainModule main = PS.main;
		if (trackParent)
		{
			main.startRotationY = (invertRotation ? (base.transform.parent.transform.eulerAngles.y * (MathF.PI / 180f) + offset) : ((0f - base.transform.parent.transform.eulerAngles.y) * (MathF.PI / 180f) + offset));
		}
		else
		{
			main.startRotationY = (invertRotation ? (base.transform.eulerAngles.y * (MathF.PI / 180f) + offset) : ((0f - base.transform.eulerAngles.y) * (MathF.PI / 180f) + offset));
		}
	}
}
