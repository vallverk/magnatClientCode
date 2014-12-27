using UnityEngine;
using System.Collections;

public class CubesController : MonoBehaviour
{
	public static CubesController Instance;

	public Transform Cube1;
	public Transform Cube2;

	public Animator Anim;

	public Vector3[] Rotations;

	void Awake()
	{
		Instance = this;

		Anim.StopPlayback();
		Cube1.gameObject.SetActive(false);
		Cube2.gameObject.SetActive(false);
	}

	private IEnumerator Hide(float offset)
	{
		yield return new WaitForSeconds(offset);
		float start = Time.time;
		for (float delta = 0; delta < 1; delta = Time.time - start)
		{
			Color col = Cube1.renderer.sharedMaterial.GetColor("_Color");
			col.a = 1-delta;
			Cube1.renderer.sharedMaterial.SetColor("_Color",col);
			yield return new WaitForEndOfFrame();
		}
		//Anim.Play("null");
	}

	public void Drop(int a, int b)
	{
		StopAllCoroutines();

		Color col = Cube1.renderer.sharedMaterial.GetColor("_Color");
		col.a = 1;
		Cube1.renderer.sharedMaterial.SetColor("_Color",col);

		Cube1.gameObject.SetActive(true);
		Cube2.gameObject.SetActive(true);

		Cube1.localRotation = Quaternion.Euler(Rotations[a-1]);
		Cube2.localRotation = Quaternion.Euler(Rotations[b-1]);

        //Anim.Play("rebase");
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Take 001"))
            Anim.SetTrigger("Exit");
        Anim.SetTrigger("Throw");

		StartCoroutine(Hide(0.85f));
	}

	public static void DropDice(int a, int b)
	{
		Instance.Drop(a,b);
	}
}
