using System.Collections;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public enum CharacterState
    {
        Idle = 0,
        Run,
        Attack,
        Jump,
        CutOff
    }

    Animator animator;
    public float skillTime;
    public GameObject esPadaSword;
    public float maelstromDistance;
    public float charSpeed;
    public float jumpPower;
    public Rigidbody rigdbody;
    public bool mealstromState;

    AnimatorStateInfo runState;

    public InputManager inputmanager;



    public GameObject[] enermy;


    private CharacterStatus stat;
    private int potionCount = 3;
    public int[] skillCoolTime;


    public Animator Animator { get { return animator; } }

    CharacterState state;
    public CharacterState State { get { return state; } }


    void Start()
    {
        stat = GetComponent<CharacterStatus>();
        animator = GetComponent<Animator>();
        state = CharacterState.Idle;
        enermy = null;
        rigdbody = GetComponent<Rigidbody>();
        inputmanager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
        mealstromState = false;
    }

    void Update()
    {
        enermy = GameObject.FindGameObjectsWithTag("Enermy");

        if (mealstromState)
        {
            Maelstrom();
        }

    }

    public void Move(float ver, float hor)
    {
        runState = this.animator.GetCurrentAnimatorStateInfo(0);

        if (!animator.GetBool("Attack"))
        {
            if (ver != 0 || hor != 0)
            {
                animator.SetFloat("Ver", ver);
                animator.SetFloat("Hor", hor);

                if (ver < 0)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 180.0f, 0));
                }
                else if (ver > 0)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0.0f, 0));
                }
                CharState("Run");

                transform.Translate((Vector3.forward * ver - Vector3.right * hor) * Time.deltaTime * charSpeed, Space.World);
            }
            else if (ver == 0 && hor == 0)
            {
                animator.SetBool("Run", false);
                CharState("Idle");
            }
        }

    }

    void SetStateDefault()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Run", false);
    }

    public void CharState(string Inputstate)
    {
        SetStateDefault();
        //idle=0,run=1,attack=2
        switch (Inputstate)
        {
            case "Idle":
                state = CharacterState.Idle;
                animator.SetBool("Idle", true);
                break;

            case "Run":
                state = CharacterState.Run;
                animator.SetBool("Run", true);
                break;

            case "Attack":
                state = CharacterState.Attack;
                animator.SetTrigger("Attack");
                break;
            case "Jump":
                state = CharacterState.Jump;
                animator.SetTrigger("Jump");
                break;
            case "CutOff":
                state = CharacterState.CutOff;
                animator.SetTrigger("CutOff");
                break;

        }
    }

    public void NormalAttack()
    {
        CharState("Attack");
    }

    public void Jump()
    {
        if (!inputmanager.IsJumping)
        {
            inputmanager.isJumping = true;
            CharState("Jump");
            rigdbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            return;
        }
    }

    //swordmaster Skill
    public void Espada()
    {
        Debug.Log("espada");
        GameObject EspadaTemp = (GameObject)Instantiate(esPadaSword, transform.position + new Vector3(0.0f, 10.0f, 10f), transform.rotation);
        Destroy(EspadaTemp, 1.0f);
    }

    public void Maelstrom()
    {
        float maelstromSpeed = 3f;

        skillTime += Time.deltaTime;

        if (maelstromDistance <= 10.0f)
        {
            for (int i = 0; i < enermy.Length; i++)
            {
                //Vector3.Lerp (enermy[i].transform.position, this.transform.position, Time.deltaTime *maelstromSpeed);
                enermy[i].transform.Translate((this.transform.position - enermy[i].transform.position) * maelstromSpeed * Time.deltaTime, Space.World);
            }
        }
        if (skillTime >= 2)
        {
            mealstromState = false;
            skillTime = 0;
        }
    }

    public void CutOff()
    {
        CharState("CutOff");
    }

    public void CutOffMove()
    {
        Instantiate(Resources.Load<GameObject>("Effect/SwordShadow"), new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), Quaternion.identity);
        transform.Translate(0, 0, 5);

        //animation stop and keyboardinput Lock
    }

    public void UsingPotion()
    {   //Potion Effect create
        GameObject potionEffect = Instantiate(Resources.Load<GameObject>("Effect/Potion"), transform.position, Quaternion.identity) as GameObject;
        potionEffect.transform.parent = gameObject.transform;
        potionEffect.transform.position += Vector3.up;
        StartCoroutine(Potion());
    }

    IEnumerator Potion()
    {
        for (int i = 0; i < potionCount; i++)
        {
            stat.healthPoint += (int)(stat.healthPoint * 0.3);
            yield return new WaitForSeconds(1f);
        }
    }

    public CharacterState GetCharacterState(int state)
    {
        switch (state)
        {
            case 0:
                return CharacterState.Idle;

            case 1:
                return CharacterState.Run;

            case 2:
                return CharacterState.Attack;

            default:
                return CharacterState.Idle;
        }
    }

    public void SetState(CharacterStateData newStateData)
    {
        Debug.Log("상태 설정");
        animator.SetBool("Direction", newStateData.direction);
        animator.SetFloat("Ver", newStateData.ver);
        animator.SetFloat("Hor", newStateData.hor);
        transform.position = new Vector3(newStateData.posX, newStateData.posY, newStateData.posZ);

        //animator.SetBool(GetCharacterState(newStateData.state).ToString(), true);
    }
}