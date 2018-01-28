using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamEnum { Red, Blue, Yellow, Green, Neutral, Env, }

public class Shape : MonoBehaviour
{
    protected AudioSource audioSource;
    public AudioClip[] pop;

    protected SpriteRenderer spriteRenderer;
    public Sprite[] skin;

    public float explosionRadius = 5.0f;
    public float explosionForce = 400.0f;
    public TeamEnum team = TeamEnum.Neutral;

    public bool wait = false;

    protected virtual void Start()
    {
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        SwitchState(TeamEnum.Neutral);
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        if (transform.rotation.eulerAngles.x != 0.0f || transform.rotation.eulerAngles.z != 0.0f)
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.x = 0.0f;
            euler.z = 0.0f;
            transform.rotation = Quaternion.Euler(euler);
        }
        if (transform.position.y != 0.0f)
        {
            Vector3 position = transform.position;
            position.y = 0.0f;
            transform.position = position;
        }
    }

    protected virtual void SwitchState(TeamEnum newState)
    {
        switch (newState)
        {
            case TeamEnum.Blue:
                spriteRenderer.sprite = skin[1];
                break;

            case TeamEnum.Red:
                spriteRenderer.sprite = skin[0];
                break;

            case TeamEnum.Neutral:
                spriteRenderer.sprite = skin[4];
                break;

            case TeamEnum.Yellow:
                spriteRenderer.sprite = skin[2];
                break;

            case TeamEnum.Green:
                spriteRenderer.sprite = skin[3];
                break;
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
                PopEffect(contact);
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
            else if (contact.thisCollider.gameObject.CompareTag("Cutter") && otherTeam != TeamEnum.Neutral )
            {
                Cutted(contact);
                FindObjectOfType<GameMngr>().PlayCut();
            }
            else if (contact.thisCollider.gameObject.CompareTag("Bomb"))
            {
                PopEffect(contact);
                Boooom(contact);
                FindObjectOfType<GameMngr>().PlayBomb();
            }
            else if (thisTeam != TeamEnum.Neutral && otherTeam != thisTeam && otherTeam != TeamEnum.Neutral)
            {
                DestroyBoth(contact);
                PlayImpact();
            }
        }
    }

    protected virtual void FreeChild(Transform other)
    {
        int children = other.childCount;
        for (int i = 0; i < children; ++i)
        {
            if (other.GetChild(i).gameObject.GetComponent<Shape>() != null)
            {
                FreeChild(other.GetChild(i));
                if (other.gameObject.GetComponentInParent<Avatar>() != null)
                {
                    if (other.gameObject.GetComponentInParent<Avatar>().pitchLevel > 0.2)
                        other.gameObject.GetComponentInParent<Avatar>().pitchLevel -= 0.1f;
                }
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
                FreeChild(col.transform);
                Destroy(col.gameObject);
            }
        }
    }

    void FreeOne(Collider other)
    {
        if(other.gameObject.GetComponentInParent<Avatar>() != null)
        {
            if(other.gameObject.GetComponentInParent<Avatar>().pitchLevel > 0.2)
                other.gameObject.GetComponentInParent<Avatar>().pitchLevel -= 0.1f;
        }
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

    protected virtual void OnDestroy()
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

    protected virtual void PopEffect(ContactPoint contact)
    {
        return;
    }

    void CollisionNeutralToPlayer(Collider other)
    {
        SwitchState(other.gameObject.GetComponent<Avatar>().team);
        this.transform.SetParent(other.gameObject.transform, true);
        Destroy(this.GetComponent<Rigidbody>());
    }

    void PlayPop()
    {
        if(tag == "Shape")
        {
            audioSource.clip = pop[0];
            audioSource.pitch = gameObject.GetComponentInParent<Avatar>().pitchLevel;
            if (gameObject.GetComponentInParent<Avatar>() != null)
            {
                if (gameObject.GetComponentInParent<Avatar>().pitchLevel <= 2.8)
                    gameObject.GetComponentInParent<Avatar>().pitchLevel += 0.1f;
            }
            audioSource.Play();
        }
    }

    void PlayDeath()
    {
        audioSource.clip = pop[0];
        audioSource.Play();
        //System.Threading.Thread.Sleep(2000);
    }

    void PlayImpact()
    {
        audioSource.clip = pop[1];
        audioSource.Play();
    }
}