using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamEnum { Red, Blue, Yellow, Green, Neutral, Env, }

public class Shape : MonoBehaviour
{
    protected AudioSource audioSource;
    public AudioClip[] pop;

    public float explosionRadius = 5.0f;
    public float explosionForce = 400.0f;
    public TeamEnum team = TeamEnum.Neutral;

    public bool wait = false;

    protected virtual void Start()
    {
        SwitchState(TeamEnum.Neutral);
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        if (transform.localEulerAngles.x != 0) transform.localEulerAngles.Set(0, 0, 0);
        if (transform.localEulerAngles.y != 0) transform.localEulerAngles.Set(0, 0, 0);
        if (transform.localEulerAngles.z != 0) transform.localEulerAngles.Set(0, 0, 0);
    }

    public void SwitchState(TeamEnum newState)
    {
        switch (newState)
        {
            case TeamEnum.Blue:
                GetComponentInChildren<SpriteRenderer>().material.color = Color.blue;
                break;

            case TeamEnum.Red:
                GetComponentInChildren<SpriteRenderer>().material.color = Color.red;
                break;

            case TeamEnum.Neutral:
                GetComponentInChildren<SpriteRenderer>().material.color = Color.white;
                break;

            case TeamEnum.Yellow:
                GetComponentInChildren<SpriteRenderer>().material.color = Color.yellow;
                break;

            case TeamEnum.Green:
                GetComponentInChildren<SpriteRenderer>().material.color = Color.green;
                break;

            /*case TeamEnum.Env:
                GetComponentInChildren<SpriteRenderer>().material.color = Color.white;
                break;*/
            default:
                break;
        }
        team = newState;
    }

    void OnCollisionEnter(Collision other)
    {

        ContactPoint contact = other.contacts[0];
        if (contact.thisCollider.gameObject.GetComponent<Shape>() == null || contact.otherCollider.gameObject.GetComponent<Shape>() == null || wait == true)
        {
            return;
        }

        TeamEnum thisTeam = contact.thisCollider.gameObject.GetComponent<Shape>().team;
        TeamEnum otherTeam = contact.otherCollider.gameObject.GetComponent<Shape>().team;

        if (contact.otherCollider.gameObject.CompareTag("Player"))
        {
            if (thisTeam == TeamEnum.Neutral)
            {
                CollisionNeutralToPlayer(contact.otherCollider);
                PlayPop();
            }
            else if (thisTeam != otherTeam && contact.thisCollider.gameObject.tag == "Shape")
            {
                PlayDeath();
                DestroyBoth(contact);
            }
        }
        else if (contact.otherCollider.gameObject.CompareTag("Shape") && contact.thisCollider.gameObject.tag != "Player")
        {
            if (thisTeam == TeamEnum.Neutral && otherTeam != TeamEnum.Neutral)
            {
                CollisionNeutralToShape(contact.otherCollider);
                PlayPop();
            }
            else if (contact.thisCollider.gameObject.CompareTag("Cutter"))
            {
                Cutted(contact);
                PlayPop();
            }
            else if (contact.thisCollider.gameObject.CompareTag("Bomb"))
            {
                Debug.Log("Hello " + contact.thisCollider + " | " + contact.otherCollider);
                Boooom(contact);
            }
            else if (thisTeam != TeamEnum.Neutral && otherTeam != thisTeam && otherTeam != TeamEnum.Neutral)
            {
                DestroyBoth(contact);
            }
        }
    }

    protected void FreeChild(Transform other)
    {
        int children = other.childCount;
        for (int i = 0; i < children; ++i)
        {
            if (other.GetChild(i).gameObject.GetComponent<Shape>() != null)
            {
                FreeChild(other.GetChild(i));
                other.GetChild(i).gameObject.GetComponent<Shape>().SwitchState(TeamEnum.Neutral);
                other.GetChild(i).gameObject.AddComponent<Rigidbody>();
                other.GetChild(i).gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                other.GetChild(i).gameObject.GetComponent<Shape>().wait = true;
                other.GetChild(i).gameObject.GetComponent<Shape>().StartCoroutine("IsWaiting");
                other.GetChild(i).SetParent(null, true);
            }
        }
    }

    public IEnumerator IsWaiting()
    {
        yield return new WaitForSeconds(0.8f);
        wait = false;
    }

    void Cutted(ContactPoint contact)
    {
        FreeChild(contact.otherCollider.transform);
        FreeOne(contact.otherCollider);
        contact.thisCollider.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);

        Collider[] cols = Physics.OverlapSphere(contact.otherCollider.transform.position, explosionRadius);
        foreach (Collider col in cols)
        {
            Rigidbody rb = col.GetComponentInParent<Rigidbody>();
            if (rb != null && col.transform.parent == null && col.tag != "Player")
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    void Boooom(ContactPoint contact)
    {
        Collider[] cols = Physics.OverlapSphere(contact.thisCollider.transform.position, contact.thisCollider.gameObject.GetComponent<Bomb>().boomRadius);
        foreach (Collider col in cols)
        {
            if (col.tag != "Player")
            {
                Debug.Log("Boom");
                FreeChild(col.transform);
                Destroy(col.gameObject);
            }
        }
    }

    void FreeOne(Collider other)
    {
        other.gameObject.GetComponent<Shape>().SwitchState(TeamEnum.Neutral);
        other.gameObject.AddComponent<Rigidbody>();
        other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        other.transform.SetParent(null, true);
        other.gameObject.GetComponent<Shape>().wait = true;
        other.gameObject.GetComponent<Shape>().StartCoroutine("IsWaiting");
    }

    void CollisionNeutralToShape(Collider other)
    {
        SwitchState(other.gameObject.GetComponent<Shape>().team);
        this.transform.SetParent(other.gameObject.transform);
        Destroy(this.GetComponent<Rigidbody>());
    }

    void DestroyBoth(ContactPoint contact)
    {
        FreeChild(contact.otherCollider.transform);
        Destroy(contact.otherCollider.gameObject);
        FreeChild(contact.thisCollider.transform);
        Destroy(contact.thisCollider.gameObject);
    }

    void OnDestroy()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider col in cols)
        {
            Rigidbody rb = col.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    void CollisionNeutralToPlayer(Collider other)
    {
        SwitchState(other.gameObject.GetComponent<Avatar>().team);
        this.transform.SetParent(other.gameObject.transform, true);
        Destroy(this.GetComponent<Rigidbody>());
    }

    void PlayPop()
    {
        int index = Random.Range(0, pop.Length - 1);
        audioSource.clip = pop[index];
        audioSource.Play();
    }

    void PlayDeath()
    {
        audioSource.clip = pop[0];
        audioSource.Play();
        //System.Threading.Thread.Sleep(2000);
    }
}